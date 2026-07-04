using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Models;
using graphql_proj_Csharp.Repositories;

namespace graphql_proj_Csharp.Services;

public sealed class ReviewService(
    IBookRepository bookRepository,
    ICustomerRepository customerRepository,
    IReviewRepository reviewRepository) : IReviewService
{
    public async Task<ServiceResult<ReviewResponse>> CreateReviewAsync(CreateReviewRequest request)
    {
        if (request.Rating is < 1 or > 5)
        {
            return ServiceResult<ReviewResponse>.Validation("Rating must be between 1 and 5.");
        }

        if (!await bookRepository.ExistsAsync(request.BookId))
        {
            return ServiceResult<ReviewResponse>.Validation("Book not found.");
        }

        if (!await customerRepository.ExistsAsync(request.CustomerId))
        {
            return ServiceResult<ReviewResponse>.Validation("Customer not found.");
        }

        var review = new Review
        {
            BookId = request.BookId,
            CustomerId = request.CustomerId,
            Rating = request.Rating,
            Comment = request.Comment.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        reviewRepository.Add(review);
        await reviewRepository.SaveChangesAsync();

        return ServiceResult<ReviewResponse>.Success(new ReviewResponse(
            review.Id,
            review.Rating,
            review.Comment,
            review.CreatedAtUtc));
    }
}
