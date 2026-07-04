using graphql_proj_Csharp.Contracts;

namespace graphql_proj_Csharp.Services;

public interface IBookService
{
    Task<PagedResponse<BookSummaryResponse>> GetBooksAsync(int page, int pageSize);
    Task<BookDetailsResponse?> GetBookByIdAsync(int id);
    Task<PagedResponse<BookSummaryResponse>> SearchBooksAsync(string term, int page, int pageSize);
    Task<ServiceResult<BookSummaryResponse>> UpdateStockAsync(int id, UpdateBookStockRequest request);
}
