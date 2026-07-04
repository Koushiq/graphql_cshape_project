using graphql_proj_Csharp.Auth;
using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace graphql_proj_Csharp.Controllers;

[ApiController]
[Route("api/controllers/customers")]
public sealed class CustomersController(ICustomerService customerService) : ControllerBase
{
    [Authorize(Roles = AuthRoles.Admin)]
    [HttpGet]
    public async Task<ActionResult<PagedResponse<CustomerResponse>>> GetCustomers(
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize)
    {
        return Ok(await customerService.GetCustomersAsync(page, pageSize));
    }

    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> CreateCustomer(CreateCustomerRequest request)
    {
        var result = await customerService.CreateCustomerAsync(request);

        return result.Succeeded && result.Value is not null
            ? CreatedAtAction(nameof(GetCustomers), new { id = result.Value.Id }, result.Value)
            : this.ToActionResult(result);
    }
}
