using graphql_proj_Csharp.Models;

namespace graphql_proj_Csharp.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByHashWithUserAsync(string tokenHash);
    Task<RefreshToken?> GetByHashAsync(string tokenHash);
    void Add(RefreshToken refreshToken);
    Task SaveChangesAsync();
}
