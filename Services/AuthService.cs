using graphql_proj_Csharp.Auth;
using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Models;
using graphql_proj_Csharp.Repositories;
using Microsoft.AspNetCore.Identity;

namespace graphql_proj_Csharp.Services;

public sealed class AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ITokenService tokenService,
    IRefreshTokenRepository refreshTokenRepository) : IAuthService
{
    public async Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            FullName = request.FullName.Trim(),
            UserName = request.Email.Trim().ToLower(),
            Email = request.Email.Trim().ToLower(),
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return ServiceResult<AuthResponse>.Validation(string.Join("; ", result.Errors.Select(error => error.Description)));
        }

        await userManager.AddToRoleAsync(user, AuthRoles.Customer);

        var response = await tokenService.CreateAuthResponseAsync(user);
        await StoreRefreshTokenAsync(user.Id, response.RefreshToken, response.RefreshTokenExpiresAtUtc);

        return ServiceResult<AuthResponse>.Success(response);
    }

    public async Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email.Trim().ToLower());

        if (user is null)
        {
            return ServiceResult<AuthResponse>.Unauthorized("Invalid email or password.");
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            return ServiceResult<AuthResponse>.Unauthorized("Invalid email or password.");
        }

        var response = await tokenService.CreateAuthResponseAsync(user);
        await StoreRefreshTokenAsync(user.Id, response.RefreshToken, response.RefreshTokenExpiresAtUtc);

        return ServiceResult<AuthResponse>.Success(response);
    }

    public async Task<ServiceResult<AuthResponse>> RefreshAsync(RefreshTokenRequest request)
    {
        var tokenHash = tokenService.HashRefreshToken(request.RefreshToken);
        var existingToken = await refreshTokenRepository.GetByHashWithUserAsync(tokenHash);

        if (existingToken?.User is null || !existingToken.IsActive)
        {
            return ServiceResult<AuthResponse>.Unauthorized("Invalid refresh token.");
        }

        var response = await tokenService.CreateAuthResponseAsync(existingToken.User);

        existingToken.RevokedAtUtc = DateTime.UtcNow;
        existingToken.ReplacedByTokenHash = tokenService.HashRefreshToken(response.RefreshToken);

        refreshTokenRepository.Add(new RefreshToken
        {
            UserId = existingToken.UserId,
            TokenHash = tokenService.HashRefreshToken(response.RefreshToken),
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = response.RefreshTokenExpiresAtUtc
        });

        await refreshTokenRepository.SaveChangesAsync();

        return ServiceResult<AuthResponse>.Success(response);
    }

    public async Task<ServiceResult<bool>> RevokeAsync(RevokeRefreshTokenRequest request)
    {
        var tokenHash = tokenService.HashRefreshToken(request.RefreshToken);
        var existingToken = await refreshTokenRepository.GetByHashAsync(tokenHash);

        if (existingToken is null)
        {
            return ServiceResult<bool>.NotFound("Refresh token not found.");
        }

        existingToken.RevokedAtUtc ??= DateTime.UtcNow;
        await refreshTokenRepository.SaveChangesAsync();

        return ServiceResult<bool>.Success(true);
    }

    public async Task<ServiceResult<UserProfileResponse>> GetProfileAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return ServiceResult<UserProfileResponse>.Unauthorized("User not found.");
        }

        var roles = await userManager.GetRolesAsync(user);

        return ServiceResult<UserProfileResponse>.Success(
            new UserProfileResponse(user.Id, user.FullName, user.Email ?? string.Empty, roles.ToList()));
    }

    private async Task StoreRefreshTokenAsync(string userId, string refreshToken, DateTime expiresAtUtc)
    {
        refreshTokenRepository.Add(new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenService.HashRefreshToken(refreshToken),
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = expiresAtUtc
        });

        await refreshTokenRepository.SaveChangesAsync();
    }
}
