using graphql_proj_Csharp.Models;

namespace graphql_proj_Csharp.Repositories;

public interface IReviewRepository
{
    void Add(Review review);
    Task SaveChangesAsync();
}
