using graphql_proj_Csharp.Models;
using graphql_proj_Csharp.Repositories;
using HotChocolate.Types;

namespace graphql_proj_Csharp.GraphQL;

public sealed class Query(IGraphQLQueryRepository queryRepository)
{
    [UsePaging(IncludeTotalCount = true, DefaultPageSize = 10, MaxPageSize = 50)]
    public IQueryable<Book> GetBooks() =>
        queryRepository.GetBooks();

    public Book? GetBookById(int id) =>
        queryRepository.GetBooks().FirstOrDefault(book => book.Id == id);

    [UsePaging(IncludeTotalCount = true, DefaultPageSize = 10, MaxPageSize = 50)]
    public IQueryable<Book> SearchBooks(string term)
    {
        var normalizedTerm = term.Trim().ToLower();

        return queryRepository.GetBooks()
            .Where(book =>
                book.Title.ToLower().Contains(normalizedTerm) ||
                book.Isbn.ToLower().Contains(normalizedTerm) ||
                (book.Author != null && book.Author.Name.ToLower().Contains(normalizedTerm)))
            .OrderBy(book => book.Id);
    }

    [UsePaging(IncludeTotalCount = true, DefaultPageSize = 10, MaxPageSize = 50)]
    public IQueryable<Book> GetBooksByCategory(string categoryName)
    {
        var normalizedCategoryName = categoryName.Trim().ToLower();

        return queryRepository.GetBooks()
            .Where(book => book.BookCategories.Any(bookCategory =>
                bookCategory.Category != null &&
                bookCategory.Category.Name.ToLower() == normalizedCategoryName))
            .OrderBy(book => book.Id);
    }

    [UsePaging(IncludeTotalCount = true, DefaultPageSize = 10, MaxPageSize = 50)]
    public IQueryable<Author> GetAuthors() =>
        queryRepository.GetAuthors();

    public Author? GetAuthorById(int id) =>
        queryRepository.GetAuthors().FirstOrDefault(author => author.Id == id);

    [UsePaging(IncludeTotalCount = true, DefaultPageSize = 10, MaxPageSize = 50)]
    public IQueryable<Category> GetCategories() =>
        queryRepository.GetCategories();

    [UsePaging(IncludeTotalCount = true, DefaultPageSize = 10, MaxPageSize = 50)]
    public IQueryable<Publisher> GetPublishers() =>
        queryRepository.GetPublishers();

    [UsePaging(IncludeTotalCount = true, DefaultPageSize = 10, MaxPageSize = 50)]
    public IQueryable<Customer> GetCustomers() =>
        queryRepository.GetCustomers();

    [UsePaging(IncludeTotalCount = true, DefaultPageSize = 10, MaxPageSize = 50)]
    public IQueryable<Order> GetOrders() =>
        queryRepository.GetOrders();

    public Task<IReadOnlyList<BookRatingSummary>> GetTopRatedBooks(int take = 5) =>
        queryRepository.GetTopRatedBooksAsync(take);

    public Task<StoreStats> GetStoreStats() =>
        queryRepository.GetStoreStatsAsync();
}

public sealed record BookRatingSummary(int BookId, string Title, double AverageRating, int ReviewCount);

public sealed record StoreStats(int BookCount, int AuthorCount, int CustomerCount, int OrderCount, decimal InventoryValue);
