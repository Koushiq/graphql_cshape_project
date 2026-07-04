using graphql_proj_Csharp.Contracts;
using graphql_proj_Csharp.Repositories;

namespace graphql_proj_Csharp.Services;

public static class ServiceMappingExtensions
{
    public static PagedResponse<TOut> MapItems<TIn, TOut>(
        this PagedResult<TIn> result,
        Func<TIn, TOut> mapper) =>
        new(
            result.Items.Select(mapper).ToList(),
            result.Page,
            result.PageSize,
            result.TotalCount,
            result.TotalPages);
}
