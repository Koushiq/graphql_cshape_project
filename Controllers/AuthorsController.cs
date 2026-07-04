using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Services;
using Microsoft.AspNetCore.Mvc;

namespace graphql_proj_Csharp.Controllers;

[ApiController]
[Route("api/controllers/authors")]
public sealed class AuthorsController(IAuthorService authorService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<AuthorResponse>>> GetAuthors(
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize)
    {
        return Ok(await authorService.GetAuthorsAsync(page, pageSize));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AuthorResponse>> GetAuthorById(int id)
    {
        var author = await authorService.GetAuthorByIdAsync(id);

        return author is null ? NotFound() : Ok(author);
    }
}
