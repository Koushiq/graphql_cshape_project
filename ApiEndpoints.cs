using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Services;

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

        api.MapGet("/books", async (IBookService bookService, int page = Pagination.DefaultPage, int pageSize = Pagination.DefaultPageSize) =>
            await bookService.GetBooksAsync(page, pageSize));

        api.MapGet("/books/{id:int}", async (int id, IBookService bookService) =>
        {
            var book = await bookService.GetBookByIdAsync(id);

            return book is null ? Results.NotFound() : Results.Ok(book);
        });

        api.MapGet("/authors", async (IAuthorService authorService, int page = Pagination.DefaultPage, int pageSize = Pagination.DefaultPageSize) =>
            await authorService.GetAuthorsAsync(page, pageSize));

        api.MapGet("/categories", async (ICategoryService categoryService, int page = Pagination.DefaultPage, int pageSize = Pagination.DefaultPageSize) =>
            await categoryService.GetCategoriesAsync(page, pageSize));

        api.MapPost("/customers", async (CreateCustomerRequest request, ICustomerService customerService) =>
        {
            var result = await customerService.CreateCustomerAsync(request);

            return result.ErrorType switch
            {
                ServiceErrorType.None when result.Value is not null => Results.Created($"/api/customers/{result.Value.Id}", result.Value),
                ServiceErrorType.Conflict => Results.Conflict(result.ErrorMessage),
                ServiceErrorType.Validation => Results.BadRequest(result.ErrorMessage),
                ServiceErrorType.NotFound => Results.NotFound(result.ErrorMessage),
                _ => Results.BadRequest(result.ErrorMessage)
            };
        });

        return app;
    }
}
