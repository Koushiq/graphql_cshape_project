using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Data;
using graphql_proj_Csharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace graphql_proj_Csharp.Controllers;

[ApiController]
[Route("api/controllers/orders")]
public sealed class OrdersController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<OrderResponse>>> GetOrders(
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize)
    {
        var normalizedPage = Math.Max(page, Pagination.DefaultPage);
        var normalizedPageSize = Math.Clamp(pageSize, 1, Pagination.MaxPageSize);

        var query = dbContext.Orders
            .Include(order => order.Customer)
            .Include(order => order.Items).ThenInclude(item => item.Book)
            .OrderByDescending(order => order.OrderedAtUtc)
            .ThenByDescending(order => order.Id);

        var totalCount = await query.CountAsync();
        var orders = await query
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .ToListAsync();

        var response = new PagedResponse<OrderResponse>(
            orders.Select(ToResponse).ToList(),
            normalizedPage,
            normalizedPageSize,
            totalCount,
            (int)Math.Ceiling(totalCount / (double)normalizedPageSize));

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> CreateOrder(CreateOrderRequest request)
    {
        if (!await dbContext.Customers.AnyAsync(customer => customer.Id == request.CustomerId))
        {
            return BadRequest("Customer not found.");
        }

        if (request.Items.Count == 0)
        {
            return BadRequest("An order must contain at least one item.");
        }

        var requestedBookIds = request.Items.Select(item => item.BookId).Distinct().ToArray();
        var books = await dbContext.Books
            .Where(book => requestedBookIds.Contains(book.Id))
            .ToDictionaryAsync(book => book.Id);

        if (books.Count != requestedBookIds.Length)
        {
            return BadRequest("One or more book ids do not exist.");
        }

        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0)
            {
                return BadRequest("Order item quantity must be greater than zero.");
            }

            if (books[item.BookId].Stock < item.Quantity)
            {
                return BadRequest($"Not enough stock for book id {item.BookId}.");
            }
        }

        var order = new Order
        {
            CustomerId = request.CustomerId,
            OrderedAtUtc = DateTime.UtcNow,
            Status = "Paid",
            Items = request.Items
                .Select(item =>
                {
                    var book = books[item.BookId];
                    book.Stock -= item.Quantity;

                    return new OrderItem
                    {
                        BookId = item.BookId,
                        Quantity = item.Quantity,
                        UnitPrice = book.Price
                    };
                })
                .ToList()
        };

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();

        var savedOrder = await dbContext.Orders
            .Include(saved => saved.Customer)
            .Include(saved => saved.Items).ThenInclude(item => item.Book)
            .FirstAsync(saved => saved.Id == order.Id);

        return Created($"/api/controllers/orders/{order.Id}", ToResponse(savedOrder));
    }

    private static OrderResponse ToResponse(Order order)
    {
        var customer = order.Customer is null
            ? new CustomerResponse(order.CustomerId, string.Empty, string.Empty)
            : new CustomerResponse(order.Customer.Id, order.Customer.FullName, order.Customer.Email);

        var items = order.Items
            .Select(item => new OrderItemResponse(
                item.BookId,
                item.Book?.Title ?? string.Empty,
                item.Quantity,
                item.UnitPrice))
            .ToList();

        return new OrderResponse(
            order.Id,
            order.OrderedAtUtc,
            order.Status,
            customer,
            items,
            items.Sum(item => item.UnitPrice * item.Quantity));
    }
}
