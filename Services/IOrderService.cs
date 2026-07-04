using graphql_proj_Csharp.Contracts;

namespace graphql_proj_Csharp.Services;

public interface IOrderService
{
    Task<PagedResponse<OrderResponse>> GetOrdersAsync(int page, int pageSize);
    Task<ServiceResult<OrderResponse>> CreateOrderAsync(CreateOrderRequest request);
}
