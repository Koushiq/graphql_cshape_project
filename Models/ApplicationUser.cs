using Microsoft.AspNetCore.Identity;

namespace graphql_proj_Csharp.Models;

public sealed class ApplicationUser : IdentityUser
{
    public required string FullName { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
