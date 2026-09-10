using System;
using GSEvent.DTOs.RefreshToken;
using GSEvent.Models;

namespace GSEvent.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> CreateAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task UpdateAsync(RefreshToken refreshToken);
}
