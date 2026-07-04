using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Data;
using graphql_proj_Csharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace graphql_proj_Csharp.Controllers;

[ApiController]
[Route("api/controllers/reviews")]
public sealed class ReviewsController(AppDbContext dbContext) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ReviewResponse>> CreateReview(CreateReviewRequest request)
    {
        if (request.Rating is < 1 or > 5)
        {
            return BadRequest("Rating must be between 1 and 5.");
        }

        if (!await dbContext.Books.AnyAsync(book => book.Id == request.BookId))
        {
            return BadRequest("Book not found.");
        }

        if (!await dbContext.Customers.AnyAsync(customer => customer.Id == request.CustomerId))
        {
            return BadRequest("Customer not found.");
        }

        var review = new Review
        {
            BookId = request.BookId,
            CustomerId = request.CustomerId,
            Rating = request.Rating,
            Comment = request.Comment.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Reviews.Add(review);
        await dbContext.SaveChangesAsync();

        return Created(
            $"/api/controllers/reviews/{review.Id}",
            new ReviewResponse(review.Id, review.Rating, review.Comment, review.CreatedAtUtc));
    }
}
