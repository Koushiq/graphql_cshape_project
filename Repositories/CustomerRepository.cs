using graphql_proj_Csharp.Data;
using graphql_proj_Csharp.Models;
using Microsoft.EntityFrameworkCore;

namespace graphql_proj_Csharp.Repositories;

public sealed class CustomerRepository(AppDbContext dbContext) : ICustomerRepository
{
    public Task<PagedResult<Customer>> GetPagedAsync(int page, int pageSize) =>
        dbContext.Customers
            .OrderBy(customer => customer.FullName)
            .ToPagedResultAsync(page, pageSize);

    public Task<bool> ExistsAsync(int id) =>
        dbContext.Customers.AnyAsync(customer => customer.Id == id);

    public Task<bool> EmailExistsAsync(string email) =>
        dbContext.Customers.AnyAsync(customer => customer.Email == email);

    public void Add(Customer customer) =>
        dbContext.Customers.Add(customer);

    public Task SaveChangesAsync() =>
        dbContext.SaveChangesAsync();
}
