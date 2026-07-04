using graphql_proj_Csharp.Models;

namespace graphql_proj_Csharp.Repositories;

public interface IGraphQLMutationRepository
{
    Task<Author> AddAuthorAsync(Author author);
    Task<Category> AddCategoryAsync(Category category);
    Task<bool> AuthorExistsAsync(int authorId);
    Task<bool> PublisherExistsAsync(int publisherId);
    Task<IReadOnlyList<int>> GetExistingCategoryIdsAsync(IEnumerable<int> categoryIds);
    Task<Book> AddBookAsync(Book book);
    Task<Book?> GetBookForStockUpdateAsync(int bookId);
    Task SaveChangesAsync();
    Task<bool> BookExistsAsync(int bookId);
    Task<bool> CustomerExistsAsync(int customerId);
    void AddReview(Review review);
    Task<IReadOnlyDictionary<int, Book>> GetBooksForOrderAsync(IEnumerable<int> bookIds);
    Task<Order> AddOrderAsync(Order order);
}
