using System;
using GSEvent.DTOs.Auth;

namespace GSEvent.Services.Interfaces;

public interface IAuthService
{
    Task<UserReadDto?> RegisterAsync(RegisterDto register);
    Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto?> VerifyAndGenerateTokenAsync(TokenResetDto tokenReset);
}
