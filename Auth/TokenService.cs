using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Models;
using graphql_proj_Csharp.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace graphql_proj_Csharp.Auth;

public sealed class TokenService(
    UserManager<ApplicationUser> userManager,
    IOptions<JwtOptions> jwtOptions) : ITokenService
{
    private readonly JwtOptions jwt = jwtOptions.Value;

    public async Task<AuthResponse> CreateAuthResponseAsync(ApplicationUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var accessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(jwt.AccessTokenMinutes);
        var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(jwt.RefreshTokenDays);
        var accessToken = CreateAccessToken(user, roles, accessTokenExpiresAtUtc);
        var refreshToken = CreateRefreshToken();

        return new AuthResponse(
            accessToken,
            refreshToken,
            accessTokenExpiresAtUtc,
            refreshTokenExpiresAtUtc,
            new UserProfileResponse(user.Id, user.FullName, user.Email ?? string.Empty, roles.ToList()));
    }

    public string HashRefreshToken(string refreshToken)
    {
        var tokenBytes = Encoding.UTF8.GetBytes(refreshToken);
        var hashBytes = SHA256.HashData(tokenBytes);

        return Convert.ToBase64String(hashBytes);
    }

    private string CreateAccessToken(ApplicationUser user, IEnumerable<string> roles, DateTime expiresAtUtc)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? string.Empty)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            jwt.Issuer,
            jwt.Audience,
            claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string CreateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(bytes);
    }
}
