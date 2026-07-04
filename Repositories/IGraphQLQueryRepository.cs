using graphql_proj_Csharp.GraphQL;
using graphql_proj_Csharp.Models;

namespace graphql_proj_Csharp.Repositories;

public interface IGraphQLQueryRepository
{
    IQueryable<Book> GetBooks();
    IQueryable<Author> GetAuthors();
    IQueryable<Category> GetCategories();
    IQueryable<Publisher> GetPublishers();
    IQueryable<Customer> GetCustomers();
    IQueryable<Order> GetOrders();
    Task<IReadOnlyList<BookRatingSummary>> GetTopRatedBooksAsync(int take);
    Task<StoreStats> GetStoreStatsAsync();
}
