using System;
using GSEvent.Data;
using GSEvent.Models;
using GSEvent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GSEvent.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _appDbContext;
    public RefreshTokenRepository(AppDbContext context)
    {
        _appDbContext = context;
    }
    public async Task<RefreshToken> CreateAsync(RefreshToken refreshToken)
    {
        await _appDbContext.RefreshTokens.AddAsync(refreshToken);
        await _appDbContext.SaveChangesAsync();

        return refreshToken;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _appDbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == token);
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        _appDbContext.RefreshTokens.Update(refreshToken);
        await _appDbContext.SaveChangesAsync();
    }
}
