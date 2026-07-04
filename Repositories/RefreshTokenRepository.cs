using graphql_proj_Csharp.Data;
using graphql_proj_Csharp.Models;
using Microsoft.EntityFrameworkCore;

namespace graphql_proj_Csharp.Repositories;

public sealed class RefreshTokenRepository(AppDbContext dbContext) : IRefreshTokenRepository
{
    public Task<RefreshToken?> GetByHashWithUserAsync(string tokenHash) =>
        dbContext.RefreshTokens
            .Include(refreshToken => refreshToken.User)
            .FirstOrDefaultAsync(refreshToken => refreshToken.TokenHash == tokenHash);

    public Task<RefreshToken?> GetByHashAsync(string tokenHash) =>
        dbContext.RefreshTokens.FirstOrDefaultAsync(refreshToken => refreshToken.TokenHash == tokenHash);

    public void Add(RefreshToken refreshToken) =>
        dbContext.RefreshTokens.Add(refreshToken);

    public Task SaveChangesAsync() =>
        dbContext.SaveChangesAsync();
}
