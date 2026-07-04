using graphql_proj_Csharp.Models;

namespace graphql_proj_Csharp.Repositories;

public interface IAuthorRepository
{
    Task<PagedResult<Author>> GetPagedAsync(int page, int pageSize);
    Task<Author?> GetByIdAsync(int id);
}
