using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace graphql_proj_Csharp.Controllers;

[ApiController]
[Route("api/controllers/books")]
public sealed class BooksController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<BookSummaryResponse>>> GetBooks(
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize)
    {
        var books = await dbContext.Books
            .Include(book => book.Author)
            .Include(book => book.Publisher)
            .OrderBy(book => book.Title)
            .Select(book => new BookSummaryResponse(
                book.Id,
                book.Title,
                book.Isbn,
                book.Price,
                book.Stock,
                book.Author == null ? string.Empty : book.Author.Name,
                book.Publisher == null ? string.Empty : book.Publisher.Name))
            .ToPagedResponseAsync(page, pageSize);

        return Ok(books);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookDetailsResponse>> GetBookById(int id)
    {
        var book = await dbContext.Books
            .Include(book => book.Author)
            .Include(book => book.Publisher)
            .Include(book => book.BookCategories).ThenInclude(bookCategory => bookCategory.Category)
            .Include(book => book.Reviews)
            .FirstOrDefaultAsync(book => book.Id == id);

        if (book is null)
        {
            return NotFound();
        }

        return Ok(new BookDetailsResponse(
            book.Id,
            book.Title,
            book.Isbn,
            book.Description,
            book.Price,
            book.Stock,
            book.PublishedOn,
            book.Author?.Name ?? string.Empty,
            book.Publisher?.Name ?? string.Empty,
            book.BookCategories
                .Where(bookCategory => bookCategory.Category is not null)
                .Select(bookCategory => bookCategory.Category!.Name)
                .Order()
                .ToList(),
            book.Reviews
                .OrderByDescending(review => review.CreatedAtUtc)
                .Select(review => new ReviewResponse(review.Id, review.Rating, review.Comment, review.CreatedAtUtc))
                .ToList()));
    }

    [HttpGet("search")]
    public async Task<ActionResult<PagedResponse<BookSummaryResponse>>> SearchBooks(
        [FromQuery] string term,
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize)
    {
        var normalizedTerm = term.Trim().ToLower();

        var books = await dbContext.Books
            .Include(book => book.Author)
            .Include(book => book.Publisher)
            .Where(book =>
                book.Title.ToLower().Contains(normalizedTerm) ||
                book.Isbn.ToLower().Contains(normalizedTerm) ||
                (book.Author != null && book.Author.Name.ToLower().Contains(normalizedTerm)))
            .OrderBy(book => book.Title)
            .Select(book => new BookSummaryResponse(
                book.Id,
                book.Title,
                book.Isbn,
                book.Price,
                book.Stock,
                book.Author == null ? string.Empty : book.Author.Name,
                book.Publisher == null ? string.Empty : book.Publisher.Name))
            .ToPagedResponseAsync(page, pageSize);

        return Ok(books);
    }

    [HttpPatch("{id:int}/stock")]
    public async Task<ActionResult<BookSummaryResponse>> UpdateStock(int id, UpdateBookStockRequest request)
    {
        if (request.Stock < 0)
        {
            return BadRequest("Stock cannot be negative.");
        }

        var book = await dbContext.Books
            .Include(book => book.Author)
            .Include(book => book.Publisher)
            .FirstOrDefaultAsync(book => book.Id == id);

        if (book is null)
        {
            return NotFound();
        }

        book.Stock = request.Stock;
        await dbContext.SaveChangesAsync();

        return Ok(new BookSummaryResponse(
            book.Id,
            book.Title,
            book.Isbn,
            book.Price,
            book.Stock,
            book.Author?.Name ?? string.Empty,
            book.Publisher?.Name ?? string.Empty));
    }
}
