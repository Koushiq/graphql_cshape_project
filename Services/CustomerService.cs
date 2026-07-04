using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Models;
using graphql_proj_Csharp.Repositories;

namespace graphql_proj_Csharp.Services;

public sealed class CustomerService(ICustomerRepository customerRepository) : ICustomerService
{
    public async Task<PagedResponse<CustomerResponse>> GetCustomersAsync(int page, int pageSize) =>
        (await customerRepository.GetPagedAsync(page, pageSize)).MapItems(ToResponse);

    public async Task<ServiceResult<CustomerResponse>> CreateCustomerAsync(CreateCustomerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) || string.IsNullOrWhiteSpace(request.Email))
        {
            return ServiceResult<CustomerResponse>.Validation("Full name and email are required.");
        }

        var email = request.Email.Trim().ToLower();

        if (await customerRepository.EmailExistsAsync(email))
        {
            return ServiceResult<CustomerResponse>.Conflict("A customer with this email already exists.");
        }

        var customer = new Customer
        {
            FullName = request.FullName.Trim(),
            Email = email
        };

        customerRepository.Add(customer);
        await customerRepository.SaveChangesAsync();

        return ServiceResult<CustomerResponse>.Success(ToResponse(customer));
    }

    private static CustomerResponse ToResponse(Customer customer) =>
        new(customer.Id, customer.FullName, customer.Email);
}
