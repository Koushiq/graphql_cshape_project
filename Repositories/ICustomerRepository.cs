using graphql_proj_Csharp.Models;

namespace graphql_proj_Csharp.Repositories;

public interface ICustomerRepository
{
    Task<PagedResult<Customer>> GetPagedAsync(int page, int pageSize);
    Task<bool> ExistsAsync(int id);
    Task<bool> EmailExistsAsync(string email);
    void Add(Customer customer);
    Task SaveChangesAsync();
}
