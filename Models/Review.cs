namespace graphql_proj_Csharp.Models;

public sealed class Review
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public required string Comment { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public int BookId { get; set; }
    public Book? Book { get; set; }

    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
}
