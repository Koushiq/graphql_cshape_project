using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Models;

namespace graphql_proj_Csharp.Auth;

public interface ITokenService
{
    Task<AuthResponse> CreateAuthResponseAsync(ApplicationUser user);
    string HashRefreshToken(string refreshToken);
}
