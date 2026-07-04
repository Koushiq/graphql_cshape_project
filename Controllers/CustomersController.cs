using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Data;
using graphql_proj_Csharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace graphql_proj_Csharp.Controllers;

[ApiController]
[Route("api/controllers/customers")]
public sealed class CustomersController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<CustomerResponse>>> GetCustomers(
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize)
    {
        var customers = await dbContext.Customers
            .OrderBy(customer => customer.FullName)
            .Select(customer => new CustomerResponse(customer.Id, customer.FullName, customer.Email))
            .ToPagedResponseAsync(page, pageSize);

        return Ok(customers);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> CreateCustomer(CreateCustomerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) || string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest("Full name and email are required.");
        }

        var email = request.Email.Trim().ToLower();

        if (await dbContext.Customers.AnyAsync(customer => customer.Email == email))
        {
            return Conflict("A customer with this email already exists.");
        }

        var customer = new Customer
        {
            FullName = request.FullName.Trim(),
            Email = email
        };

        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync();

        var response = new CustomerResponse(customer.Id, customer.FullName, customer.Email);
        return CreatedAtAction(nameof(GetCustomers), new { id = customer.Id }, response);
    }
}
