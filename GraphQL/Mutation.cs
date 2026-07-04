using graphql_proj_Csharp.Models;
using graphql_proj_Csharp.Repositories;

namespace graphql_proj_Csharp.GraphQL;

public sealed class Mutation(IGraphQLMutationRepository mutationRepository)
{
    public Task<Author> AddAuthor(AddAuthorInput input)
    {
        var author = new Author
        {
            Name = input.Name.Trim(),
            Bio = input.Bio?.Trim(),
            DateOfBirth = input.DateOfBirth
        };

        return mutationRepository.AddAuthorAsync(author);
    }

    public Task<Category> AddCategory(AddCategoryInput input)
    {
        var category = new Category
        {
            Name = input.Name.Trim(),
            Description = input.Description?.Trim()
        };

        return mutationRepository.AddCategoryAsync(category);
    }

    public async Task<Book> AddBook(AddBookInput input)
    {
        await EnsureAuthorExists(input.AuthorId);
        await EnsurePublisherExists(input.PublisherId);

        var categories = await mutationRepository.GetExistingCategoryIdsAsync(input.CategoryIds);

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

        return await mutationRepository.AddBookAsync(book);
    }

    public async Task<Book> UpdateBookStock(UpdateBookStockInput input)
    {
        var book = await mutationRepository.GetBookForStockUpdateAsync(input.BookId);

        if (book is null)
        {
            throw new GraphQLException("Book not found.");
        }

        book.Stock = input.Stock;
        await mutationRepository.SaveChangesAsync();

        return book;
    }

    public async Task<Review> AddReview(AddReviewInput input)
    {
        if (input.Rating is < 1 or > 5)
        {
            throw new GraphQLException("Rating must be between 1 and 5.");
        }

        await EnsureBookExists(input.BookId);
        await EnsureCustomerExists(input.CustomerId);

        var review = new Review
        {
            BookId = input.BookId,
            CustomerId = input.CustomerId,
            Rating = input.Rating,
            Comment = input.Comment.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        mutationRepository.AddReview(review);
        await mutationRepository.SaveChangesAsync();

        return review;
    }

    public async Task<Order> PlaceOrder(PlaceOrderInput input)
    {
        await EnsureCustomerExists(input.CustomerId);

        if (input.Items.Count == 0)
        {
            throw new GraphQLException("An order must contain at least one item.");
        }

        var requestedBookIds = input.Items.Select(item => item.BookId).Distinct().ToArray();
        var books = await mutationRepository.GetBooksForOrderAsync(requestedBookIds);

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

        return await mutationRepository.AddOrderAsync(order);
    }

    private async Task EnsureAuthorExists(int authorId)
    {
        if (!await mutationRepository.AuthorExistsAsync(authorId))
        {
            throw new GraphQLException("Author not found.");
        }
    }

    private async Task EnsurePublisherExists(int publisherId)
    {
        if (!await mutationRepository.PublisherExistsAsync(publisherId))
        {
            throw new GraphQLException("Publisher not found.");
        }
    }

    private async Task EnsureBookExists(int bookId)
    {
        if (!await mutationRepository.BookExistsAsync(bookId))
        {
            throw new GraphQLException("Book not found.");
        }
    }

    private async Task EnsureCustomerExists(int customerId)
    {
        if (!await mutationRepository.CustomerExistsAsync(customerId))
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
