using graphql_proj_Csharp.Contracts;

namespace graphql_proj_Csharp.Services;

public interface ICategoryService
{
    Task<PagedResponse<CategoryResponse>> GetCategoriesAsync(int page, int pageSize);
}
