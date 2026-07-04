using graphql_proj_Csharp.Data;
using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Models;
using Microsoft.EntityFrameworkCore;

namespace graphql_proj_Csharp;

public static class ApiEndpoints
{
    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        var api = app.MapGroup("/api");

        api.MapGet("/health", () => Results.Ok(new { status = "ok", service = "GraphQL Bookstore API" }));

        api.MapGet("/info", (IConfiguration configuration) => Results.Ok(new
        {
            name = "GraphQL Bookstore API",
            graphql = "/graphql",
            database = "PostgreSQL",
            applyMigrationsOnStartup = configuration.GetValue("Database:ApplyMigrationsOnStartup", false)
        }));

        api.MapGet("/books", async (AppDbContext dbContext, int page = Pagination.DefaultPage, int pageSize = Pagination.DefaultPageSize) =>
            await dbContext.Books
                .Include(book => book.Author)
                .Include(book => book.Publisher)
                .OrderBy(book => book.Title)
                .Select(book => new BookSummaryResponse(
                    book.Id,
                    book.Title,
                    book.Isbn,
                    book.Price,
                    book.Stock,
                    book.Author == null ? string.Empty : book.Author.Name,
                    book.Publisher == null ? string.Empty : book.Publisher.Name))
                .ToPagedResponseAsync(page, pageSize));

        api.MapGet("/books/{id:int}", async (int id, AppDbContext dbContext) =>
        {
            var book = await dbContext.Books
                .Include(book => book.Author)
                .Include(book => book.Publisher)
                .Include(book => book.BookCategories).ThenInclude(bookCategory => bookCategory.Category)
                .Include(book => book.Reviews)
                .FirstOrDefaultAsync(book => book.Id == id);

            return book is null ? Results.NotFound() : Results.Ok(book);
        });

        api.MapGet("/authors", async (AppDbContext dbContext, int page = Pagination.DefaultPage, int pageSize = Pagination.DefaultPageSize) =>
            await dbContext.Authors
                .Include(author => author.Books)
                .OrderBy(author => author.Name)
                .ToPagedResponseAsync(page, pageSize));

        api.MapGet("/categories", async (AppDbContext dbContext, int page = Pagination.DefaultPage, int pageSize = Pagination.DefaultPageSize) =>
            await dbContext.Categories
                .OrderBy(category => category.Name)
                .ToPagedResponseAsync(page, pageSize));

        api.MapPost("/customers", async (CreateCustomerRequest request, AppDbContext dbContext) =>
        {
            var customer = new Customer
            {
                FullName = request.FullName.Trim(),
                Email = request.Email.Trim().ToLower()
            };

            dbContext.Customers.Add(customer);
            await dbContext.SaveChangesAsync();

            return Results.Created($"/api/customers/{customer.Id}", customer);
        });

        return app;
    }
}
