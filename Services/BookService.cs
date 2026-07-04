using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Models;
using graphql_proj_Csharp.Repositories;

namespace graphql_proj_Csharp.Services;

public sealed class BookService(IBookRepository bookRepository) : IBookService
{
    public async Task<PagedResponse<BookSummaryResponse>> GetBooksAsync(int page, int pageSize) =>
        (await bookRepository.GetPagedAsync(page, pageSize)).MapItems(ToSummaryResponse);

    public async Task<BookDetailsResponse?> GetBookByIdAsync(int id)
    {
        var book = await bookRepository.GetDetailsByIdAsync(id);

        if (book is null)
        {
            return null;
        }

        return new BookDetailsResponse(
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
                .ToList());
    }

    public async Task<PagedResponse<BookSummaryResponse>> SearchBooksAsync(string term, int page, int pageSize) =>
        (await bookRepository.SearchAsync(term, page, pageSize)).MapItems(ToSummaryResponse);

    public async Task<ServiceResult<BookSummaryResponse>> UpdateStockAsync(int id, UpdateBookStockRequest request)
    {
        if (request.Stock < 0)
        {
            return ServiceResult<BookSummaryResponse>.Validation("Stock cannot be negative.");
        }

        var book = await bookRepository.GetSummaryByIdAsync(id);

        if (book is null)
        {
            return ServiceResult<BookSummaryResponse>.NotFound("Book not found.");
        }

        book.Stock = request.Stock;
        await bookRepository.SaveChangesAsync();

        return ServiceResult<BookSummaryResponse>.Success(ToSummaryResponse(book));
    }

    private static BookSummaryResponse ToSummaryResponse(Book book) =>
        new(
            book.Id,
            book.Title,
            book.Isbn,
            book.Price,
            book.Stock,
            book.Author?.Name ?? string.Empty,
            book.Publisher?.Name ?? string.Empty);
}
