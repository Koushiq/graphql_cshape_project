using graphql_proj_Csharp.Data;
using graphql_proj_Csharp.Models;

namespace graphql_proj_Csharp.Repositories;

public sealed class ReviewRepository(AppDbContext dbContext) : IReviewRepository
{
    public void Add(Review review) =>
        dbContext.Reviews.Add(review);

    public Task SaveChangesAsync() =>
        dbContext.SaveChangesAsync();
}
