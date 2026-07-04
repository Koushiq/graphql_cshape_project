using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace graphql_proj_Csharp.Controllers;

[ApiController]
[Route("api/controllers/authors")]
public sealed class AuthorsController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<AuthorResponse>>> GetAuthors(
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize)
    {
        var authors = await dbContext.Authors
            .Include(author => author.Books)
            .OrderBy(author => author.Name)
            .Select(author => new AuthorResponse(
                author.Id,
                author.Name,
                author.Bio,
                author.DateOfBirth,
                author.Books.Count))
            .ToPagedResponseAsync(page, pageSize);

        return Ok(authors);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AuthorResponse>> GetAuthorById(int id)
    {
        var author = await dbContext.Authors
            .Include(author => author.Books)
            .Where(author => author.Id == id)
            .Select(author => new AuthorResponse(
                author.Id,
                author.Name,
                author.Bio,
                author.DateOfBirth,
                author.Books.Count))
            .FirstOrDefaultAsync();

        return author is null ? NotFound() : Ok(author);
    }
}
