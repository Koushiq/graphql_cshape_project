using graphql_proj_Csharp.Contracts;

namespace graphql_proj_Csharp.Services;

public interface ICustomerService
{
    Task<PagedResponse<CustomerResponse>> GetCustomersAsync(int page, int pageSize);
    Task<ServiceResult<CustomerResponse>> CreateCustomerAsync(CreateCustomerRequest request);
}
