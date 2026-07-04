using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace graphql_proj_Csharp.Controllers;

[ApiController]
[Route("api/controllers/orders")]
public sealed class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<OrderResponse>>> GetOrders(
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize)
    {
        return Ok(await orderService.GetOrdersAsync(page, pageSize));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> CreateOrder(CreateOrderRequest request)
    {
        var result = await orderService.CreateOrderAsync(request);

        return result.Succeeded && result.Value is not null
            ? Created($"/api/controllers/orders/{result.Value.Id}", result.Value)
            : this.ToActionResult(result);
    }
}
