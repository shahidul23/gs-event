using System;
using GSEvent.Models;

namespace GSEvent.Services.Interfaces;

public interface IRefreshTokenService
{
    Task<RefreshToken> CreateAsync(
        ApplicationUser user,
        string jwtId,
        string existingRefreshToken
    );
    Task<RefreshToken?> GetByTokenAsync(
        string token
    );
    Task<bool> ValidateAsync(RefreshToken refreshToken);
}
