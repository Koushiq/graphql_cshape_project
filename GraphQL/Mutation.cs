using graphql_proj_Csharp.Data;
using graphql_proj_Csharp.Models;
using Microsoft.EntityFrameworkCore;

namespace graphql_proj_Csharp.GraphQL;

public sealed class Mutation
{
    public async Task<Author> AddAuthor(AddAuthorInput input, AppDbContext dbContext)
    {
        var author = new Author
        {
            Name = input.Name.Trim(),
            Bio = input.Bio?.Trim(),
            DateOfBirth = input.DateOfBirth
        };

        dbContext.Authors.Add(author);
        await dbContext.SaveChangesAsync();

        return author;
    }

    public async Task<Category> AddCategory(AddCategoryInput input, AppDbContext dbContext)
    {
        var category = new Category
        {
            Name = input.Name.Trim(),
            Description = input.Description?.Trim()
        };

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        return category;
    }

    public async Task<Book> AddBook(AddBookInput input, AppDbContext dbContext)
    {
        await EnsureAuthorExists(input.AuthorId, dbContext);
        await EnsurePublisherExists(input.PublisherId, dbContext);

        var categories = await dbContext.Categories
            .Where(category => input.CategoryIds.Contains(category.Id))
            .Select(category => category.Id)
            .ToListAsync();

        if (categories.Count != input.CategoryIds.Distinct().Count())
        {
            throw new GraphQLException("One or more category ids do not exist.");
        }

        var book = new Book
        {
            Title = input.Title.Trim(),
            Isbn = input.Isbn.Trim(),
            Description = input.Description?.Trim(),
            Price = input.Price,
            Stock = input.Stock,
            PublishedOn = input.PublishedOn,
            AuthorId = input.AuthorId,
            PublisherId = input.PublisherId,
            BookCategories = categories
                .Select(categoryId => new BookCategory { CategoryId = categoryId })
                .ToList()
        };

        dbContext.Books.Add(book);
        await dbContext.SaveChangesAsync();

        return await dbContext.Books
            .Include(savedBook => savedBook.Author)
            .Include(savedBook => savedBook.Publisher)
            .Include(savedBook => savedBook.BookCategories).ThenInclude(bookCategory => bookCategory.Category)
            .FirstAsync(savedBook => savedBook.Id == book.Id);
    }

    public async Task<Book> UpdateBookStock(UpdateBookStockInput input, AppDbContext dbContext)
    {
        var book = await dbContext.Books.FirstOrDefaultAsync(book => book.Id == input.BookId);

        if (book is null)
        {
            throw new GraphQLException("Book not found.");
        }

        book.Stock = input.Stock;
        await dbContext.SaveChangesAsync();

        return book;
    }

    public async Task<Review> AddReview(AddReviewInput input, AppDbContext dbContext)
    {
        if (input.Rating is < 1 or > 5)
        {
            throw new GraphQLException("Rating must be between 1 and 5.");
        }

        await EnsureBookExists(input.BookId, dbContext);
        await EnsureCustomerExists(input.CustomerId, dbContext);

        var review = new Review
        {
            BookId = input.BookId,
            CustomerId = input.CustomerId,
            Rating = input.Rating,
            Comment = input.Comment.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Reviews.Add(review);
        await dbContext.SaveChangesAsync();

        return review;
    }

    public async Task<Order> PlaceOrder(PlaceOrderInput input, AppDbContext dbContext)
    {
        await EnsureCustomerExists(input.CustomerId, dbContext);

        if (input.Items.Count == 0)
        {
            throw new GraphQLException("An order must contain at least one item.");
        }

        var requestedBookIds = input.Items.Select(item => item.BookId).Distinct().ToArray();
        var books = await dbContext.Books
            .Where(book => requestedBookIds.Contains(book.Id))
            .ToDictionaryAsync(book => book.Id);

        if (books.Count != requestedBookIds.Length)
        {
            throw new GraphQLException("One or more book ids do not exist.");
        }

        foreach (var item in input.Items)
        {
            if (item.Quantity <= 0)
            {
                throw new GraphQLException("Order item quantity must be greater than zero.");
            }

            if (books[item.BookId].Stock < item.Quantity)
            {
                throw new GraphQLException($"Not enough stock for book id {item.BookId}.");
            }
        }

        var order = new Order
        {
            CustomerId = input.CustomerId,
            OrderedAtUtc = DateTime.UtcNow,
            Status = "Paid",
            Items = input.Items
                .Select(item =>
                {
                    var book = books[item.BookId];
                    book.Stock -= item.Quantity;

                    return new OrderItem
                    {
                        BookId = item.BookId,
                        Quantity = item.Quantity,
                        UnitPrice = book.Price
                    };
                })
                .ToList()
        };

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();

        return await dbContext.Orders
            .Include(savedOrder => savedOrder.Customer)
            .Include(savedOrder => savedOrder.Items).ThenInclude(item => item.Book)
            .FirstAsync(savedOrder => savedOrder.Id == order.Id);
    }

    private static async Task EnsureAuthorExists(int authorId, AppDbContext dbContext)
    {
        if (!await dbContext.Authors.AnyAsync(author => author.Id == authorId))
        {
            throw new GraphQLException("Author not found.");
        }
    }

    private static async Task EnsurePublisherExists(int publisherId, AppDbContext dbContext)
    {
        if (!await dbContext.Publishers.AnyAsync(publisher => publisher.Id == publisherId))
        {
            throw new GraphQLException("Publisher not found.");
        }
    }

    private static async Task EnsureBookExists(int bookId, AppDbContext dbContext)
    {
        if (!await dbContext.Books.AnyAsync(book => book.Id == bookId))
        {
            throw new GraphQLException("Book not found.");
        }
    }

    private static async Task EnsureCustomerExists(int customerId, AppDbContext dbContext)
    {
        if (!await dbContext.Customers.AnyAsync(customer => customer.Id == customerId))
        {
            throw new GraphQLException("Customer not found.");
        }
    }
}

public sealed record AddAuthorInput(string Name, string? Bio, DateOnly DateOfBirth);

public sealed record AddCategoryInput(string Name, string? Description);

public sealed record AddBookInput(
    string Title,
    string Isbn,
    string? Description,
    decimal Price,
    int Stock,
    DateOnly PublishedOn,
    int AuthorId,
    int PublisherId,
    IReadOnlyList<int> CategoryIds);

public sealed record UpdateBookStockInput(int BookId, int Stock);

public sealed record AddReviewInput(int BookId, int CustomerId, int Rating, string Comment);

public sealed record PlaceOrderInput(int CustomerId, IReadOnlyList<PlaceOrderItemInput> Items);

public sealed record PlaceOrderItemInput(int BookId, int Quantity);
