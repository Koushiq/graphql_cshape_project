using graphql_proj_Csharp.Data;
using graphql_proj_Csharp.Models;
using Microsoft.EntityFrameworkCore;

namespace graphql_proj_Csharp.Repositories;

public sealed class BookRepository(AppDbContext dbContext) : IBookRepository
{
    public Task<PagedResult<Book>> GetPagedAsync(int page, int pageSize) =>
        dbContext.Books
            .Include(book => book.Author)
            .Include(book => book.Publisher)
            .OrderBy(book => book.Title)
            .ToPagedResultAsync(page, pageSize);

    public Task<Book?> GetDetailsByIdAsync(int id) =>
        dbContext.Books
            .Include(book => book.Author)
            .Include(book => book.Publisher)
            .Include(book => book.BookCategories).ThenInclude(bookCategory => bookCategory.Category)
            .Include(book => book.Reviews)
            .FirstOrDefaultAsync(book => book.Id == id);

    public Task<PagedResult<Book>> SearchAsync(string term, int page, int pageSize)
    {
        var normalizedTerm = term.Trim().ToLower();

        return dbContext.Books
            .Include(book => book.Author)
            .Include(book => book.Publisher)
            .Where(book =>
                book.Title.ToLower().Contains(normalizedTerm) ||
                book.Isbn.ToLower().Contains(normalizedTerm) ||
                (book.Author != null && book.Author.Name.ToLower().Contains(normalizedTerm)))
            .OrderBy(book => book.Title)
            .ToPagedResultAsync(page, pageSize);
    }

    public Task<Book?> GetSummaryByIdAsync(int id) =>
        dbContext.Books
            .Include(book => book.Author)
            .Include(book => book.Publisher)
            .FirstOrDefaultAsync(book => book.Id == id);

    public async Task<IReadOnlyDictionary<int, Book>> GetByIdsAsync(IEnumerable<int> ids)
    {
        var idArray = ids.Distinct().ToArray();

        return await dbContext.Books
            .Where(book => idArray.Contains(book.Id))
            .ToDictionaryAsync(book => book.Id);
    }

    public Task<bool> ExistsAsync(int id) =>
        dbContext.Books.AnyAsync(book => book.Id == id);

    public Task SaveChangesAsync() =>
        dbContext.SaveChangesAsync();
}
