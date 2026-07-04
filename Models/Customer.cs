namespace graphql_proj_Csharp.Models;

public sealed class Customer
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
