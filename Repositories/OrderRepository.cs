using graphql_proj_Csharp.Data;
using graphql_proj_Csharp.Models;
using Microsoft.EntityFrameworkCore;

namespace graphql_proj_Csharp.Repositories;

public sealed class OrderRepository(AppDbContext dbContext) : IOrderRepository
{
    public async Task<PagedResult<Order>> GetPagedAsync(int page, int pageSize)
    {
        var normalizedPage = Math.Max(page, RepositoryPagination.DefaultPage);
        var normalizedPageSize = Math.Clamp(pageSize, 1, RepositoryPagination.MaxPageSize);

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

        return new PagedResult<Order>(
            orders,
            normalizedPage,
            normalizedPageSize,
            totalCount,
            (int)Math.Ceiling(totalCount / (double)normalizedPageSize));
    }

    public async Task<Order> AddAsync(Order order)
    {
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();

        return await dbContext.Orders
            .Include(saved => saved.Customer)
            .Include(saved => saved.Items).ThenInclude(item => item.Book)
            .FirstAsync(saved => saved.Id == order.Id);
    }
}
