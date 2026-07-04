using graphql_proj_Csharp.Contracts;

namespace graphql_proj_Csharp.Services;

public interface IReviewService
{
    Task<ServiceResult<ReviewResponse>> CreateReviewAsync(CreateReviewRequest request);
}
