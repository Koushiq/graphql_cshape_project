using System.Security.Claims;
using graphql_proj_Csharp.Auth;
using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace graphql_proj_Csharp.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request) =>
        this.ToActionResult(await authService.RegisterAsync(request));

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request) =>
        this.ToActionResult(await authService.LoginAsync(request));

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshTokenRequest request) =>
        this.ToActionResult(await authService.RefreshAsync(request));

    [Authorize]
    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke(RevokeRefreshTokenRequest request)
    {
        var result = await authService.RevokeAsync(request);

        return result.ErrorType switch
        {
            ServiceErrorType.None => NoContent(),
            ServiceErrorType.NotFound => NotFound(result.ErrorMessage),
            ServiceErrorType.Unauthorized => Unauthorized(result.ErrorMessage),
            ServiceErrorType.Conflict => Conflict(result.ErrorMessage),
            _ => BadRequest(result.ErrorMessage)
        };
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileResponse>> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return userId is null
            ? Unauthorized()
            : this.ToActionResult(await authService.GetProfileAsync(userId));
    }

    [Authorize(Roles = AuthRoles.Admin)]
    [HttpGet("admin-check")]
    public IActionResult AdminCheck() =>
        Ok(new { message = "You are authenticated as an Admin." });
}
