using graphql_proj_Csharp.Data;
using graphql_proj_Csharp.Models;
using Microsoft.EntityFrameworkCore;

namespace graphql_proj_Csharp.Repositories;

public sealed class GraphQLMutationRepository(AppDbContext dbContext) : IGraphQLMutationRepository
{
    public async Task<Author> AddAuthorAsync(Author author)
    {
        dbContext.Authors.Add(author);
        await dbContext.SaveChangesAsync();

        return author;
    }

    public async Task<Category> AddCategoryAsync(Category category)
    {
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        return category;
    }

    public Task<bool> AuthorExistsAsync(int authorId) =>
        dbContext.Authors.AnyAsync(author => author.Id == authorId);

    public Task<bool> PublisherExistsAsync(int publisherId) =>
        dbContext.Publishers.AnyAsync(publisher => publisher.Id == publisherId);

    public async Task<IReadOnlyList<int>> GetExistingCategoryIdsAsync(IEnumerable<int> categoryIds)
    {
        var idArray = categoryIds.Distinct().ToArray();

        return await dbContext.Categories
            .Where(category => idArray.Contains(category.Id))
            .Select(category => category.Id)
            .ToListAsync();
    }

    public async Task<Book> AddBookAsync(Book book)
    {
        dbContext.Books.Add(book);
        await dbContext.SaveChangesAsync();

        return await dbContext.Books
            .Include(savedBook => savedBook.Author)
            .Include(savedBook => savedBook.Publisher)
            .Include(savedBook => savedBook.BookCategories).ThenInclude(bookCategory => bookCategory.Category)
            .FirstAsync(savedBook => savedBook.Id == book.Id);
    }

    public Task<Book?> GetBookForStockUpdateAsync(int bookId) =>
        dbContext.Books.FirstOrDefaultAsync(book => book.Id == bookId);

    public Task SaveChangesAsync() =>
        dbContext.SaveChangesAsync();

    public Task<bool> BookExistsAsync(int bookId) =>
        dbContext.Books.AnyAsync(book => book.Id == bookId);

    public Task<bool> CustomerExistsAsync(int customerId) =>
        dbContext.Customers.AnyAsync(customer => customer.Id == customerId);

    public void AddReview(Review review) =>
        dbContext.Reviews.Add(review);

    public async Task<IReadOnlyDictionary<int, Book>> GetBooksForOrderAsync(IEnumerable<int> bookIds)
    {
        var idArray = bookIds.Distinct().ToArray();

        return await dbContext.Books
            .Where(book => idArray.Contains(book.Id))
            .ToDictionaryAsync(book => book.Id);
    }

    public async Task<Order> AddOrderAsync(Order order)
    {
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();

        return await dbContext.Orders
            .Include(savedOrder => savedOrder.Customer)
            .Include(savedOrder => savedOrder.Items).ThenInclude(item => item.Book)
            .FirstAsync(savedOrder => savedOrder.Id == order.Id);
    }
}
