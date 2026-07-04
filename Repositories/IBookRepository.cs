using graphql_proj_Csharp.Models;

namespace graphql_proj_Csharp.Repositories;

public interface IBookRepository
{
    Task<PagedResult<Book>> GetPagedAsync(int page, int pageSize);
    Task<Book?> GetDetailsByIdAsync(int id);
    Task<PagedResult<Book>> SearchAsync(string term, int page, int pageSize);
    Task<Book?> GetSummaryByIdAsync(int id);
    Task<IReadOnlyDictionary<int, Book>> GetByIdsAsync(IEnumerable<int> ids);
    Task<bool> ExistsAsync(int id);
    Task SaveChangesAsync();
}
