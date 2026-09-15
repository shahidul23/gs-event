using System;
using System.Security.Cryptography;
using GSEvent.Exceptions;
using GSEvent.Models;
using GSEvent.Repositories.Interfaces;
using GSEvent.Services.Interfaces;

namespace GSEvent.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IConfiguration _configuration;
    public RefreshTokenService(
        IRefreshTokenRepository refreshTokenRepository,
        IConfiguration configuration
    )
    {
        _refreshTokenRepository = refreshTokenRepository;
        _configuration = configuration;
    }
    public async Task<RefreshToken> CreateAsync(ApplicationUser user, string jwtId, string? existingRefreshToken)
    {
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = GenerateRefreshToken(),
            JwtId = jwtId,
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.AddDays(
                GetRefreshTokenExpirationDays()
            )
        };
        if (!string.IsNullOrWhiteSpace(existingRefreshToken))
        {
            var oldToken = await _refreshTokenRepository
                .GetByTokenAsync(existingRefreshToken);
            if (oldToken is not null)
            {
                oldToken.IsRevoked = true;
                await _refreshTokenRepository.UpdateAsync(oldToken);
            }
        }
        await _refreshTokenRepository.CreateAsync(refreshToken);
        return refreshToken;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }
        return await _refreshTokenRepository.GetByTokenAsync(token);
    }
    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
    private int GetRefreshTokenExpirationDays()
    {
        var tokenExpiretionDays = int.TryParse(_configuration["Jwt:RefreshTokenExpirationDays"], out var days) 
            ? days
            : throw new UnauthorizedException("Refresh Token expiration minutes is not configured or invalid.");
        return tokenExpiretionDays;
    }

    public Task<bool> ValidateAsync(RefreshToken refreshToken)
    {
        var IsValide = !refreshToken.IsRevoked && refreshToken.ExpiredAt > DateTime.UtcNow;
        return Task.FromResult(IsValide);
    }
}
