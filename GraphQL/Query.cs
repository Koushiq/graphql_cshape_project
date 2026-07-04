using graphql_proj_Csharp.Data;
using graphql_proj_Csharp.Models;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace graphql_proj_Csharp.GraphQL;

public sealed class Query
{
    [UsePaging(IncludeTotalCount = true, DefaultPageSize = 10, MaxPageSize = 50)]
    public IQueryable<Book> GetBooks(AppDbContext dbContext) =>
        dbContext.Books
            .Include(book => book.Author)
            .Include(book => book.Publisher)
            .Include(book => book.BookCategories).ThenInclude(bookCategory => bookCategory.Category)
            .Include(book => book.Reviews)
            .OrderBy(book => book.Id);

    public Task<Book?> GetBookById(int id, AppDbContext dbContext) =>
        GetBooks(dbContext).FirstOrDefaultAsync(book => book.Id == id);

    [UsePaging(IncludeTotalCount = true, DefaultPageSize = 10, MaxPageSize = 50)]
    public IQueryable<Book> SearchBooks(string term, AppDbContext dbContext)
    {
        var normalizedTerm = term.Trim().ToLower();

        return GetBooks(dbContext)
            .Where(book =>
                book.Title.ToLower().Contains(normalizedTerm) ||
                book.Isbn.ToLower().Contains(normalizedTerm) ||
                (book.Author != null && book.Author.Name.ToLower().Contains(normalizedTerm)))
            .OrderBy(book => book.Id);
    }

    [UsePaging(IncludeTotalCount = true, DefaultPageSize = 10, MaxPageSize = 50)]
    public IQueryable<Book> GetBooksByCategory(string categoryName, AppDbContext dbContext)
    {
        var normalizedCategoryName = categoryName.Trim().ToLower();

        return GetBooks(dbContext)
            .Where(book => book.BookCategories.Any(bookCategory =>
                bookCategory.Category != null &&
                bookCategory.Category.Name.ToLower() == normalizedCategoryName))
            .OrderBy(book => book.Id);
    }

    [UsePaging(IncludeTotalCount = true, DefaultPageSize = 10, MaxPageSize = 50)]
    public IQueryable<Author> GetAuthors(AppDbContext dbContext) =>
        dbContext.Authors
            .Include(author => author.Books)
            .OrderBy(author => author.Id);

    public Task<Author?> GetAuthorById(int id, AppDbContext dbContext) =>
        GetAuthors(dbContext).FirstOrDefaultAsync(author => author.Id == id);

    [UsePaging(IncludeTotalCount = true, DefaultPageSize = 10, MaxPageSize = 50)]
    public IQueryable<Category> GetCategories(AppDbContext dbContext) =>
        dbContext.Categories
            .Include(category => category.BookCategories)
            .OrderBy(category => category.Id);

    [UsePaging(IncludeTotalCount = true, DefaultPageSize = 10, MaxPageSize = 50)]
    public IQueryable<Publisher> GetPublishers(AppDbContext dbContext) =>
        dbContext.Publishers
            .Include(publisher => publisher.Books)
            .OrderBy(publisher => publisher.Id);

    [UsePaging(IncludeTotalCount = true, DefaultPageSize = 10, MaxPageSize = 50)]
    public IQueryable<Customer> GetCustomers(AppDbContext dbContext) =>
        dbContext.Customers
            .Include(customer => customer.Orders)
            .OrderBy(customer => customer.Id);

    [UsePaging(IncludeTotalCount = true, DefaultPageSize = 10, MaxPageSize = 50)]
    public IQueryable<Order> GetOrders(AppDbContext dbContext) =>
        dbContext.Orders
            .Include(order => order.Customer)
            .Include(order => order.Items).ThenInclude(item => item.Book)
            .OrderByDescending(order => order.OrderedAtUtc)
            .ThenByDescending(order => order.Id);

    public async Task<IReadOnlyList<BookRatingSummary>> GetTopRatedBooks(AppDbContext dbContext, int take = 5) =>
        await dbContext.Books
            .Where(book => book.Reviews.Any())
            .Select(book => new BookRatingSummary(
                book.Id,
                book.Title,
                book.Reviews.Average(review => review.Rating),
                book.Reviews.Count))
            .OrderByDescending(summary => summary.AverageRating)
            .ThenByDescending(summary => summary.ReviewCount)
            .Take(take)
            .ToListAsync();

    public Task<StoreStats> GetStoreStats(AppDbContext dbContext) =>
        CreateStoreStatsAsync(dbContext);

    private static async Task<StoreStats> CreateStoreStatsAsync(AppDbContext dbContext)
    {
        var bookCount = await dbContext.Books.CountAsync();
        var authorCount = await dbContext.Authors.CountAsync();
        var customerCount = await dbContext.Customers.CountAsync();
        var orderCount = await dbContext.Orders.CountAsync();
        var inventoryValue = await dbContext.Books.SumAsync(book => book.Price * book.Stock);

        return new StoreStats(bookCount, authorCount, customerCount, orderCount, inventoryValue);
    }
}

public sealed record BookRatingSummary(int BookId, string Title, double AverageRating, int ReviewCount);

public sealed record StoreStats(int BookCount, int AuthorCount, int CustomerCount, int OrderCount, decimal InventoryValue);
