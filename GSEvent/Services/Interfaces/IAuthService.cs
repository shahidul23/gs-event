using System;
using GSEvent.Common.Pagination;
using GSEvent.DTOs.Auth;
using GSEvent.Models;

namespace GSEvent.Services.Interfaces;

public interface IAuthService
{
    Task<UserReadDto?> RegisterAsync(RegisterDto register);
    Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto?> VerifyAndGenerateTokenAsync(TokenResetDto tokenReset);
    Task<bool> LogoutAsync(LogoutDto logout);
    Task<bool> VerifyEmailAsync(EmailVerificationDto emailVerification);
    Task<List<ReadRoleDto>> GetAllRolesAsync();
    Task<PaginationResponse<UserReadDto>> GetAllUsersAsync(PaginationRequest request);
    Task<UserReadDto> GetUserAsync(Guid id);
    Task<bool> UserUpdateAsync(UserUpdateDto dto);
}
