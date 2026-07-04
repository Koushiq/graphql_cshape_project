using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Services;
using Microsoft.AspNetCore.Mvc;

namespace graphql_proj_Csharp.Controllers;

[ApiController]
[Route("api/controllers/categories")]
public sealed class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<CategoryResponse>>> GetCategories(
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize)
    {
        return Ok(await categoryService.GetCategoriesAsync(page, pageSize));
    }
}
