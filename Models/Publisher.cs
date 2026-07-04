namespace graphql_proj_Csharp.Models;

public sealed class Publisher
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Country { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
