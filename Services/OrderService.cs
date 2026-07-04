using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Models;
using graphql_proj_Csharp.Repositories;

namespace graphql_proj_Csharp.Services;

public sealed class OrderService(
    IBookRepository bookRepository,
    ICustomerRepository customerRepository,
    IOrderRepository orderRepository) : IOrderService
{
    public async Task<PagedResponse<OrderResponse>> GetOrdersAsync(int page, int pageSize) =>
        (await orderRepository.GetPagedAsync(page, pageSize)).MapItems(ToResponse);

    public async Task<ServiceResult<OrderResponse>> CreateOrderAsync(CreateOrderRequest request)
    {
        if (!await customerRepository.ExistsAsync(request.CustomerId))
        {
            return ServiceResult<OrderResponse>.Validation("Customer not found.");
        }

        if (request.Items.Count == 0)
        {
            return ServiceResult<OrderResponse>.Validation("An order must contain at least one item.");
        }

        var requestedBookIds = request.Items.Select(item => item.BookId).Distinct().ToArray();
        var books = await bookRepository.GetByIdsAsync(requestedBookIds);

        if (books.Count != requestedBookIds.Length)
        {
            return ServiceResult<OrderResponse>.Validation("One or more book ids do not exist.");
        }

        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0)
            {
                return ServiceResult<OrderResponse>.Validation("Order item quantity must be greater than zero.");
            }

            if (books[item.BookId].Stock < item.Quantity)
            {
                return ServiceResult<OrderResponse>.Validation($"Not enough stock for book id {item.BookId}.");
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

        var savedOrder = await orderRepository.AddAsync(order);

        return ServiceResult<OrderResponse>.Success(ToResponse(savedOrder));
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
