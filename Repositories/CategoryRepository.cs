using graphql_proj_Csharp.Data;
using graphql_proj_Csharp.Models;

namespace graphql_proj_Csharp.Repositories;

public sealed class CategoryRepository(AppDbContext dbContext) : ICategoryRepository
{
    public Task<PagedResult<Category>> GetPagedAsync(int page, int pageSize) =>
        dbContext.Categories
            .OrderBy(category => category.Name)
            .ToPagedResultAsync(page, pageSize);
}
