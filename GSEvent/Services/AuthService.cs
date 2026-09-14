using System;
using AutoMapper;
using GSEvent.DTOs.Auth;
using GSEvent.Models;
using GSEvent.Repositories.Interfaces;
using GSEvent.Services.Interfaces;

namespace GSEvent.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IMapper _mapper;
    public AuthService(
        IUserRepository userRepository,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService,
        IMapper mapper
    )
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
        _mapper = mapper;
    }
    public Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
    {
        throw new NotImplementedException();
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto register)
    {
        var existingUser = await _userRepository.ExistsByEmailAsync(register.Email);
        if (!existingUser)
        {
            return null;
        }
        var existingUsername = await _userRepository
            .ExistsByUsernameAsync(register.UserName);

        if (existingUsername)
        {
            return null;
        }
        var newUser = _mapper.Map<ApplicationUser>(register); 

        var createUser = await _userRepository.CreateAsync(newUser, register.Password);

        var UserDto = _mapper.Map<UserReadDto>(createUser);


        return new AuthResponseDto
        {
            Token = "",
            RefreshToken = "",
            ExpiresAt = DateTime.UtcNow,
            User = UserDto
        };
    }
}
