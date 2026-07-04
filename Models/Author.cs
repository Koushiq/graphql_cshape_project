namespace graphql_proj_Csharp.Models;

public sealed class Author
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Bio { get; set; }
    public DateOnly DateOfBirth { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
