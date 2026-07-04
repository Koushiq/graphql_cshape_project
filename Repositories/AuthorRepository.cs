using graphql_proj_Csharp.Data;
using graphql_proj_Csharp.Models;
using Microsoft.EntityFrameworkCore;

namespace graphql_proj_Csharp.Repositories;

public sealed class AuthorRepository(AppDbContext dbContext) : IAuthorRepository
{
    public Task<PagedResult<Author>> GetPagedAsync(int page, int pageSize) =>
        dbContext.Authors
            .Include(author => author.Books)
            .OrderBy(author => author.Name)
            .ToPagedResultAsync(page, pageSize);

    public Task<Author?> GetByIdAsync(int id) =>
        dbContext.Authors
            .Include(author => author.Books)
            .FirstOrDefaultAsync(author => author.Id == id);
}
