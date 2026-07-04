using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace graphql_proj_Csharp.Controllers;

[ApiController]
[Route("api/controllers/reviews")]
public sealed class ReviewsController(IReviewService reviewService) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ReviewResponse>> CreateReview(CreateReviewRequest request)
    {
        var result = await reviewService.CreateReviewAsync(request);

        return result.Succeeded && result.Value is not null
            ? Created($"/api/controllers/reviews/{result.Value.Id}", result.Value)
            : this.ToActionResult(result);
    }
}
