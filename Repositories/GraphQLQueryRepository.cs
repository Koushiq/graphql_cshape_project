using graphql_proj_Csharp.Data;
using graphql_proj_Csharp.GraphQL;
using graphql_proj_Csharp.Models;
using Microsoft.EntityFrameworkCore;

namespace graphql_proj_Csharp.Repositories;

public sealed class GraphQLQueryRepository(AppDbContext dbContext) : IGraphQLQueryRepository
{
    public IQueryable<Book> GetBooks() =>
        dbContext.Books
            .Include(book => book.Author)
            .Include(book => book.Publisher)
            .Include(book => book.BookCategories).ThenInclude(bookCategory => bookCategory.Category)
            .Include(book => book.Reviews)
            .OrderBy(book => book.Id);

    public IQueryable<Author> GetAuthors() =>
        dbContext.Authors
            .Include(author => author.Books)
            .OrderBy(author => author.Id);

    public IQueryable<Category> GetCategories() =>
        dbContext.Categories
            .Include(category => category.BookCategories)
            .OrderBy(category => category.Id);

    public IQueryable<Publisher> GetPublishers() =>
        dbContext.Publishers
            .Include(publisher => publisher.Books)
            .OrderBy(publisher => publisher.Id);

    public IQueryable<Customer> GetCustomers() =>
        dbContext.Customers
            .Include(customer => customer.Orders)
            .OrderBy(customer => customer.Id);

    public IQueryable<Order> GetOrders() =>
        dbContext.Orders
            .Include(order => order.Customer)
            .Include(order => order.Items).ThenInclude(item => item.Book)
            .OrderByDescending(order => order.OrderedAtUtc)
            .ThenByDescending(order => order.Id);

    public async Task<IReadOnlyList<BookRatingSummary>> GetTopRatedBooksAsync(int take) =>
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

    public async Task<StoreStats> GetStoreStatsAsync()
    {
        var bookCount = await dbContext.Books.CountAsync();
        var authorCount = await dbContext.Authors.CountAsync();
        var customerCount = await dbContext.Customers.CountAsync();
        var orderCount = await dbContext.Orders.CountAsync();
        var inventoryValue = await dbContext.Books.SumAsync(book => book.Price * book.Stock);

        return new StoreStats(bookCount, authorCount, customerCount, orderCount, inventoryValue);
    }
}
