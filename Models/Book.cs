namespace graphql_proj_Csharp.Models;

public sealed class Book
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Isbn { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public DateOnly PublishedOn { get; set; }

    public int AuthorId { get; set; }
    public Author? Author { get; set; }

    public int PublisherId { get; set; }
    public Publisher? Publisher { get; set; }

    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
