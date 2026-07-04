using Microsoft.EntityFrameworkCore;

namespace graphql_proj_Csharp.Contracts;

public static class Pagination
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 50;

    public static async Task<PagedResponse<T>> ToPagedResponseAsync<T>(
        this IQueryable<T> query,
        int page = DefaultPage,
        int pageSize = DefaultPageSize)
    {
        var normalizedPage = Math.Max(page, DefaultPage);
        var normalizedPageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .ToListAsync();

        return new PagedResponse<T>(
            items,
            normalizedPage,
            normalizedPageSize,
            totalCount,
            (int)Math.Ceiling(totalCount / (double)normalizedPageSize));
    }
}
