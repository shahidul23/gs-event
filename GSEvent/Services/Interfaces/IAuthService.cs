using System;
using GSEvent.DTOs.Auth;

namespace GSEvent.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterDto register);
    Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
}
