namespace graphql_proj_Csharp.Contracts;

public sealed record BookSummaryResponse(
    int Id,
    string Title,
    string Isbn,
    decimal Price,
    int Stock,
    string Author,
    string Publisher);

public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);

public sealed record BookDetailsResponse(
    int Id,
    string Title,
    string Isbn,
    string? Description,
    decimal Price,
    int Stock,
    DateOnly PublishedOn,
    string Author,
    string Publisher,
    IReadOnlyList<string> Categories,
    IReadOnlyList<ReviewResponse> Reviews);

public sealed record AuthorResponse(int Id, string Name, string? Bio, DateOnly DateOfBirth, int BookCount);

public sealed record CategoryResponse(int Id, string Name, string? Description);

public sealed record CustomerResponse(int Id, string FullName, string Email);

public sealed record OrderResponse(
    int Id,
    DateTime OrderedAtUtc,
    string Status,
    CustomerResponse Customer,
    IReadOnlyList<OrderItemResponse> Items,
    decimal Total);

public sealed record OrderItemResponse(int BookId, string BookTitle, int Quantity, decimal UnitPrice);

public sealed record ReviewResponse(int Id, int Rating, string Comment, DateTime CreatedAtUtc);

public sealed record CreateCustomerRequest(string FullName, string Email);

public sealed record CreateReviewRequest(int BookId, int CustomerId, int Rating, string Comment);

public sealed record UpdateBookStockRequest(int Stock);

public sealed record CreateOrderRequest(int CustomerId, IReadOnlyList<CreateOrderItemRequest> Items);

public sealed record CreateOrderItemRequest(int BookId, int Quantity);
