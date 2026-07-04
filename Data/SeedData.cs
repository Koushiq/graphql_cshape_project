using graphql_proj_Csharp.Models;
using Microsoft.EntityFrameworkCore;

namespace graphql_proj_Csharp.Data;

public static class SeedData
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>().HasData(
            new Author { Id = 1, Name = "Octavia Butler", Bio = "Speculative fiction author.", DateOfBirth = new DateOnly(1947, 6, 22) },
            new Author { Id = 2, Name = "Ursula K. Le Guin", Bio = "Author of literary science fiction and fantasy.", DateOfBirth = new DateOnly(1929, 10, 21) },
            new Author { Id = 3, Name = "N. K. Jemisin", Bio = "Award-winning speculative fiction author.", DateOfBirth = new DateOnly(1972, 9, 19) });

        modelBuilder.Entity<Publisher>().HasData(
            new Publisher { Id = 1, Name = "Beacon Press", Country = "USA" },
            new Publisher { Id = 2, Name = "Ace Books", Country = "USA" },
            new Publisher { Id = 3, Name = "Orbit", Country = "USA" });

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Science Fiction", Description = "Future-facing speculative stories." },
            new Category { Id = 2, Name = "Fantasy", Description = "Magic, myth, and secondary worlds." },
            new Category { Id = 3, Name = "Classics", Description = "Enduring books with long-lived influence." });

        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, Title = "Kindred", Isbn = "9780807083697", Description = "A modern classic about time travel and history.", Price = 16.99m, Stock = 24, PublishedOn = new DateOnly(1979, 6, 1), AuthorId = 1, PublisherId = 1 },
            new Book { Id = 2, Title = "A Wizard of Earthsea", Isbn = "9780547773742", Description = "The beginning of the Earthsea cycle.", Price = 10.99m, Stock = 18, PublishedOn = new DateOnly(1968, 11, 1), AuthorId = 2, PublisherId = 2 },
            new Book { Id = 3, Title = "The Fifth Season", Isbn = "9780316229296", Description = "The first book in The Broken Earth trilogy.", Price = 18.99m, Stock = 32, PublishedOn = new DateOnly(2015, 8, 4), AuthorId = 3, PublisherId = 3 },
            new Book { Id = 4, Title = "Parable of the Sower", Isbn = "9781538732182", Description = "A prescient dystopian novel.", Price = 17.99m, Stock = 15, PublishedOn = new DateOnly(1993, 10, 1), AuthorId = 1, PublisherId = 1 });

        modelBuilder.Entity<BookCategory>().HasData(
            new BookCategory { BookId = 1, CategoryId = 1 },
            new BookCategory { BookId = 1, CategoryId = 3 },
            new BookCategory { BookId = 2, CategoryId = 2 },
            new BookCategory { BookId = 2, CategoryId = 3 },
            new BookCategory { BookId = 3, CategoryId = 1 },
            new BookCategory { BookId = 3, CategoryId = 2 },
            new BookCategory { BookId = 4, CategoryId = 1 });

        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, FullName = "Amina Rahman", Email = "amina@example.com" },
            new Customer { Id = 2, FullName = "Jon Rivera", Email = "jon@example.com" },
            new Customer { Id = 3, FullName = "Maya Chen", Email = "maya@example.com" });

        modelBuilder.Entity<Review>().HasData(
            new Review { Id = 1, BookId = 1, CustomerId = 1, Rating = 5, Comment = "Powerful and unforgettable.", CreatedAtUtc = new DateTime(2026, 1, 12, 10, 0, 0, DateTimeKind.Utc) },
            new Review { Id = 2, BookId = 2, CustomerId = 2, Rating = 4, Comment = "Lean, mythic, and beautifully written.", CreatedAtUtc = new DateTime(2026, 2, 7, 14, 30, 0, DateTimeKind.Utc) },
            new Review { Id = 3, BookId = 3, CustomerId = 3, Rating = 5, Comment = "Inventive worldbuilding with real force.", CreatedAtUtc = new DateTime(2026, 3, 19, 9, 15, 0, DateTimeKind.Utc) },
            new Review { Id = 4, BookId = 4, CustomerId = 1, Rating = 5, Comment = "Still feels startlingly current.", CreatedAtUtc = new DateTime(2026, 4, 4, 12, 45, 0, DateTimeKind.Utc) });

        modelBuilder.Entity<Order>().HasData(
            new Order { Id = 1, CustomerId = 1, OrderedAtUtc = new DateTime(2026, 4, 20, 11, 0, 0, DateTimeKind.Utc), Status = "Paid" },
            new Order { Id = 2, CustomerId = 2, OrderedAtUtc = new DateTime(2026, 5, 2, 16, 20, 0, DateTimeKind.Utc), Status = "Shipped" });

        modelBuilder.Entity<OrderItem>().HasData(
            new OrderItem { Id = 1, OrderId = 1, BookId = 1, Quantity = 1, UnitPrice = 16.99m },
            new OrderItem { Id = 2, OrderId = 1, BookId = 3, Quantity = 2, UnitPrice = 18.99m },
            new OrderItem { Id = 3, OrderId = 2, BookId = 2, Quantity = 1, UnitPrice = 10.99m });
    }
}
