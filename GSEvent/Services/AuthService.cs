using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using GSEvent.DTOs.Auth;
using GSEvent.Enums;
using GSEvent.Exceptions;
using GSEvent.Messaging;
using GSEvent.Models;
using GSEvent.RabbitMQ.Service.Interface;
using GSEvent.Repositories.Interfaces;
using GSEvent.Services.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;

namespace GSEvent.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly IRoleRepository _roleRepository;
    private readonly IRabbitMqPublisher _rabbitMqPublisher;
    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService,
        TokenValidationParameters tokenValidationParameters,
        IRoleRepository roleRepository,
        IRabbitMqPublisher rabbitMqPublisher
    )
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
        _tokenValidationParameters = tokenValidationParameters;
        _refreshTokenRepository = refreshTokenRepository;
        _roleRepository = roleRepository;
        _rabbitMqPublisher = rabbitMqPublisher;
    }
    public async Task<AuthResponseDto?> LoginAsync(LoginDto login)
    {
        ApplicationUser? existingUser;
        if (login.UsernameOrEmailOrPhone.Contains("@"))
        {
            existingUser = await _userRepository
                .GetByEmailAsync(login.UsernameOrEmailOrPhone);
        }
        else if (login.UsernameOrEmailOrPhone.All(char.IsDigit))
        {
            existingUser = await _userRepository
                .GetByPhoneAsync(login.UsernameOrEmailOrPhone);
        }
        else
        {
            existingUser = await _userRepository
                .GetByUsernameAsync(login.UsernameOrEmailOrPhone);
        }

        if (existingUser == null)
        {
            throw new BadRequestException(
                "Invalid username, email, or phone number."
            );
        }
        var passwordValide = await _userRepository.CheckPasswordAsync(existingUser, login.Password);
        if (!passwordValide)
        {
            throw new BadRequestException("Invalid username, email, or phone number.");
        }
        var role = await _userRepository.GetRoleAsync(existingUser);

        var jwtResult = _jwtService.GenerateJwtTokenAsync(existingUser, role);
        var refreshToken = await _refreshTokenService.CreateAsync(existingUser, jwtResult.JwtId, "");

        return new AuthResponseDto
        {
            Token = jwtResult.Token,
            RefreshToken = refreshToken.Token,
            ExpiresAt = jwtResult.ExpiresAt,
            User = new UserReadDto
            {
                Id = existingUser.Id,
                FullName = existingUser.FullName,
                UserName = existingUser.UserName ?? string.Empty,
                Email = existingUser.Email ?? string.Empty
            }
        };
    }

    public async Task<bool> LogoutAsync(LogoutDto logout)
    {
        if (string.IsNullOrWhiteSpace(logout.RefreshToken))
        {
            return false;
        }
        return await _refreshTokenRepository.RevokeAsync(logout.RefreshToken);
    }

    public async Task<UserReadDto?> RegisterAsync(RegisterDto register)
    {
        var existingUser = await _userRepository.ExistsByEmailAsync(register.Email);
        if (existingUser)
        {
            throw new ConflictException($"{register.Email} Mail Already Exist");
        }
        var existingUsername = await _userRepository
            .ExistsByUsernameAsync(register.UserName);

        if (existingUsername)
        {
            throw new ConflictException($"{register.UserName} Username Already Exist");
        }
        var existingByPhone = await _userRepository.ExistsByPhoneAsync(register.Phone);
        if (existingByPhone)
        {
            throw new ConflictException($"{register.Phone} Phone Already Exist");
        }

        var newUser = new ApplicationUser()
        {
            FullName = register.FullName,
            UserName = register.UserName,
            Email = register.Email,
            PhoneNumber = register.Phone,
            Address = register.Address,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        Console.WriteLine(newUser.UserName);

        var createUser = await _userRepository.CreateAsync(newUser, register.Password);

        if (createUser is null)
        {
            return null;
        }
        var role = register.Role.ToString();
        var roleExist = await _roleRepository.RoleExistsAsync(role);
        if (!roleExist)
        {
            throw new NotFoundException("This Role Not Found");
        }
        await _userRepository.AddRoleAsync(createUser, role);
        var token = await _userRepository.UserConfirmationAsync(createUser);
        var encodedToken = WebEncoders.Base64UrlEncode(
            Encoding.UTF8.GetBytes(token)
        );
        var message = new EmailVerificationMessage
        {
            UserName = newUser.UserName,
            Email = newUser.Email,
            VerificationToken = encodedToken
        };
        await _rabbitMqPublisher.PublishAsync(
            RabbitMqQueue.EmailVerification,
            message
        );
        return new UserReadDto
        {
            Id = createUser.Id,
            FullName = createUser.FullName,
            UserName = createUser.UserName ?? string.Empty,
            Email = createUser.Email ?? string.Empty
        };
    }

    public async Task<AuthResponseDto?> VerifyAndGenerateTokenAsync(
    TokenResetDto tokenReset)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken;

        try
        {
            jwtTokenHandler.ValidateToken(
                tokenReset.Token,
                _tokenValidationParameters,
                out var validatedToken
            );

            jwtToken = validatedToken as JwtSecurityToken
                ?? throw new BadRequestException("Invalid token format.");
        }
        catch (SecurityTokenExpiredException)
        {
            jwtToken = jwtTokenHandler.ReadJwtToken(tokenReset.Token);
        }
        catch (SecurityTokenException)
        {
            throw new BadRequestException("Invalid access token.");
        }

        if (!jwtToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new BadRequestException("Invalid token algorithm.");
        }

        var jti = jwtToken.Claims
            .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)
            ?.Value;

        if (string.IsNullOrWhiteSpace(jti))
        {
            throw new BadRequestException("Token JTI is missing.");
        }

        var refreshToken = await _refreshTokenRepository
            .GetByTokenAsync(tokenReset.RefreshToken);

        if (refreshToken == null)
        {
            throw new BadRequestException(
                "Refresh token does not exist in our database."
            );
        }

        if (refreshToken.JwtId != jti)
        {
            throw new BadRequestException(
                "Refresh token does not match the JWT."
            );
        }

        if (refreshToken.IsRevoked)
        {
            throw new BadRequestException(
                "Refresh token has been revoked."
            );
        }

        if (refreshToken.ExpiredAt <= DateTime.UtcNow)
        {
            throw new BadRequestException(
                "Your refresh token has expired. Please authenticate again."
            );
        }
        var user = await _userRepository
            .GetByIdAsync(refreshToken.UserId);

        if (user == null)
        {
            throw new BadRequestException(
                "User associated with token was not found."
            );
        }
        var role = await _userRepository.GetRoleAsync(user);
        var jwtResult = _jwtService.GenerateJwtTokenAsync(user, role);
        var newRefreshToken = await _refreshTokenService.CreateAsync(
            user,
            jwtResult.JwtId,
            tokenReset.RefreshToken
        );
        return new AuthResponseDto
        {
            Token = jwtResult.Token,
            RefreshToken = newRefreshToken.Token,
            ExpiresAt = jwtResult.ExpiresAt,
            User = new UserReadDto
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty
            }
        };
    }

    private DateTime UnixTimeStampToDateTimeInUTC(long utcExpireyDate)
    {
        var dateTimeVal = new DateTime(1970,1,1,0,0,0,0, DateTimeKind.Utc);
        dateTimeVal = dateTimeVal.AddSeconds(utcExpireyDate);
        return dateTimeVal;
    }
}
