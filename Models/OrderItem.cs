namespace graphql_proj_Csharp.Models;

public sealed class OrderItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public int OrderId { get; set; }
    public Order? Order { get; set; }

    public int BookId { get; set; }
    public Book? Book { get; set; }
}
