using graphql_proj_Csharp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace graphql_proj_Csharp.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<BookCategory> BookCategories => Set<BookCategory>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Publisher> Publishers => Set<Publisher>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(user => user.FullName).HasMaxLength(160).IsRequired();
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.Property(refreshToken => refreshToken.TokenHash).HasMaxLength(128).IsRequired();
            entity.Property(refreshToken => refreshToken.ReplacedByTokenHash).HasMaxLength(128);
            entity.HasIndex(refreshToken => refreshToken.TokenHash).IsUnique();
            entity.HasOne(refreshToken => refreshToken.User)
                .WithMany(user => user.RefreshTokens)
                .HasForeignKey(refreshToken => refreshToken.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.Property(author => author.Name).HasMaxLength(160).IsRequired();
            entity.Property(author => author.Bio).HasMaxLength(1_000);
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.Property(publisher => publisher.Name).HasMaxLength(160).IsRequired();
            entity.Property(publisher => publisher.Country).HasMaxLength(80).IsRequired();
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(category => category.Name).HasMaxLength(80).IsRequired();
            entity.HasIndex(category => category.Name).IsUnique();
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.Property(book => book.Title).HasMaxLength(220).IsRequired();
            entity.Property(book => book.Isbn).HasMaxLength(20).IsRequired();
            entity.Property(book => book.Description).HasMaxLength(1_500);
            entity.Property(book => book.Price).HasPrecision(10, 2);
            entity.HasIndex(book => book.Isbn).IsUnique();
        });

        modelBuilder.Entity<BookCategory>(entity =>
        {
            entity.HasKey(bookCategory => new { bookCategory.BookId, bookCategory.CategoryId });
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(customer => customer.FullName).HasMaxLength(160).IsRequired();
            entity.Property(customer => customer.Email).HasMaxLength(220).IsRequired();
            entity.HasIndex(customer => customer.Email).IsUnique();
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.Property(review => review.Comment).HasMaxLength(1_000).IsRequired();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(order => order.Status).HasMaxLength(40).IsRequired();
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.Property(orderItem => orderItem.UnitPrice).HasPrecision(10, 2);
        });

        SeedData.Apply(modelBuilder);
    }
}
