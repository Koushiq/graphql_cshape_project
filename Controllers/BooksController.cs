using graphql_proj_Csharp.Auth;
using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace graphql_proj_Csharp.Controllers;

[ApiController]
[Route("api/controllers/books")]
public sealed class BooksController(IBookService bookService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<BookSummaryResponse>>> GetBooks(
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize)
    {
        return Ok(await bookService.GetBooksAsync(page, pageSize));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookDetailsResponse>> GetBookById(int id)
    {
        var book = await bookService.GetBookByIdAsync(id);

        return book is null ? NotFound() : Ok(book);
    }

    [HttpGet("search")]
    public async Task<ActionResult<PagedResponse<BookSummaryResponse>>> SearchBooks(
        [FromQuery] string term,
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize)
    {
        return Ok(await bookService.SearchBooksAsync(term, page, pageSize));
    }

    [Authorize(Roles = AuthRoles.AdminOrManager)]
    [HttpPatch("{id:int}/stock")]
    public async Task<ActionResult<BookSummaryResponse>> UpdateStock(int id, UpdateBookStockRequest request)
    {
        return this.ToActionResult(await bookService.UpdateStockAsync(id, request));
    }
}
