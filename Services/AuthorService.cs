using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Models;
using graphql_proj_Csharp.Repositories;

namespace graphql_proj_Csharp.Services;

public sealed class AuthorService(IAuthorRepository authorRepository) : IAuthorService
{
    public async Task<PagedResponse<AuthorResponse>> GetAuthorsAsync(int page, int pageSize) =>
        (await authorRepository.GetPagedAsync(page, pageSize)).MapItems(ToResponse);

    public async Task<AuthorResponse?> GetAuthorByIdAsync(int id)
    {
        var author = await authorRepository.GetByIdAsync(id);

        return author is null ? null : ToResponse(author);
    }

    private static AuthorResponse ToResponse(Author author) =>
        new(author.Id, author.Name, author.Bio, author.DateOfBirth, author.Books.Count);
}
