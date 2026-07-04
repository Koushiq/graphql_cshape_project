namespace graphql_proj_Csharp.Models;

public sealed class Order
{
    public int Id { get; set; }
    public DateTime OrderedAtUtc { get; set; }
    public required string Status { get; set; }

    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
