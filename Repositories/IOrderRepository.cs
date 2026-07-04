using graphql_proj_Csharp.Models;

namespace graphql_proj_Csharp.Repositories;

public interface IOrderRepository
{
    Task<PagedResult<Order>> GetPagedAsync(int page, int pageSize);
    Task<Order> AddAsync(Order order);
}
