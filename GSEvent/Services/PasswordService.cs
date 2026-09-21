using System;
using System.Security.Claims;
using GSEvent.DTOs.Auth;
using GSEvent.Exceptions;
using GSEvent.Repositories.Interfaces;
using GSEvent.Services.Interfaces;

namespace GSEvent.Services;

public class PasswordService : IPasswordService
{
    private readonly IPasswordRepository _passwordRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    public PasswordService(
        IPasswordRepository passwordRepository, 
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository
    )
    {
        _passwordRepository = passwordRepository;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
    }
    public async Task<string> ChangePasswordAsync(
        ChangePasswordDto request)
    {
        var userId = _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedException("User id not found.");
        }
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new UnauthorizedException("User not found.");
        }
        var result = await _passwordRepository.ChangePasswordAsync(
            user,
            request
        );
        if (!result.Succeeded)
        {
            var errors = string.Join(
                ", ",
                result.Errors.Select(x =>
                    $"{x.Code}: {x.Description}"
                )
            );
            throw new BadRequestException(errors);
        }
        return string.Empty;
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
    {
        var result = await _passwordRepository.GeneratePasswordResetTokenAsync(forgotPasswordDto.Email);
        if (!result)
        {
            throw new BadRequestException(
                "Failed to generate password reset token."
            );
        }
        return true;
    }

    public async Task<string> ResetPasswordAsync(
        ResetPasswordDto request
    )
    {
        var result = await _passwordRepository.ResetPasswordAsync(
            request
        );

        if (!result)
        {
            throw new BadRequestException(
                "Failed to reset password."
            );
        }
        return string.Empty;
    }
}
