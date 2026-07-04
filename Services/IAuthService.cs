using graphql_proj_Csharp.Contracts;

namespace graphql_proj_Csharp.Services;

public interface IAuthService
{
    Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request);
    Task<ServiceResult<AuthResponse>> RefreshAsync(RefreshTokenRequest request);
    Task<ServiceResult<bool>> RevokeAsync(RevokeRefreshTokenRequest request);
    Task<ServiceResult<UserProfileResponse>> GetProfileAsync(string userId);
}
