using graphql_proj_Csharp.Contracts;

namespace graphql_proj_Csharp.Services;

public interface IAuthorService
{
    Task<PagedResponse<AuthorResponse>> GetAuthorsAsync(int page, int pageSize);
    Task<AuthorResponse?> GetAuthorByIdAsync(int id);
}
