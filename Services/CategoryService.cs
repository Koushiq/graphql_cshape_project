using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Models;
using graphql_proj_Csharp.Repositories;

namespace graphql_proj_Csharp.Services;

public sealed class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public async Task<PagedResponse<CategoryResponse>> GetCategoriesAsync(int page, int pageSize) =>
        (await categoryRepository.GetPagedAsync(page, pageSize)).MapItems(ToResponse);

    private static CategoryResponse ToResponse(Category category) =>
        new(category.Id, category.Name, category.Description);
}
