using graphql_proj_Csharp.Models;

namespace graphql_proj_Csharp.Repositories;

public interface ICategoryRepository
{
    Task<PagedResult<Category>> GetPagedAsync(int page, int pageSize);
}
