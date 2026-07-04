using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace graphql_proj_Csharp.Controllers;

[ApiController]
[Route("api/controllers/categories")]
public sealed class CategoriesController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<CategoryResponse>>> GetCategories(
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize)
    {
        var categories = await dbContext.Categories
            .OrderBy(category => category.Name)
            .Select(category => new CategoryResponse(category.Id, category.Name, category.Description))
            .ToPagedResponseAsync(page, pageSize);

        return Ok(categories);
    }
}
