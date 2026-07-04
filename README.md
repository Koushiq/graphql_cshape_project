# GraphQL C# Bookstore API

ASP.NET Core 9 GraphQL API using Hot Chocolate, EF Core, PostgreSQL, and seed data.

## Run locally

1. Start the full stack:

   ```bash
   docker compose up --build
   ```

The API will be available at `http://localhost:5000`.

To run only PostgreSQL and start the API from your IDE:

```bash
docker compose up -d postgres
dotnet run
```

Confirm/update the connection string in `appsettings.Development.json` if your local Postgres credentials differ.

To run only the API from .NET:

   ```bash
   dotnet run
   ```

In development, migrations are applied on startup and the seed data from `Data/SeedData.cs` is inserted through EF migrations. If you prefer applying migrations manually:

   ```bash
   dotnet ef database update
   ```

GraphQL Banana Cake Pop IDE:

```text
http://localhost:5000/graphql
```

Health endpoint:

```text
http://localhost:5000/api/health
```

REST helper endpoints:

```text
GET  /api/info
GET  /api/books?page=1&pageSize=10
GET  /api/books/{id}
GET  /api/authors?page=1&pageSize=10
GET  /api/categories?page=1&pageSize=10
POST /api/customers
```

Controller endpoints:

```text
GET   /api/controllers/health
GET   /api/controllers/info
GET   /api/controllers/books?page=1&pageSize=10
GET   /api/controllers/books/{id}
GET   /api/controllers/books/search?term=earth&page=1&pageSize=10
PATCH /api/controllers/books/{id}/stock
GET   /api/controllers/authors?page=1&pageSize=10
GET   /api/controllers/authors/{id}
GET   /api/controllers/categories?page=1&pageSize=10
GET   /api/controllers/customers?page=1&pageSize=10
POST  /api/controllers/customers
GET   /api/controllers/orders?page=1&pageSize=10
POST  /api/controllers/orders
POST  /api/controllers/reviews
```

## Example queries

```graphql
query Books {
  books(first: 10) {
    totalCount
    pageInfo {
      hasNextPage
      endCursor
    }
    nodes {
      id
      title
      price
      stock
      author {
        name
      }
      publisher {
        name
      }
      reviews {
        rating
        comment
      }
    }
  }
}
```

```graphql
query SearchBooks {
  searchBooks(term: "earth", first: 5) {
    totalCount
    pageInfo {
      hasNextPage
      endCursor
    }
    nodes {
      id
      title
      isbn
      author {
        name
      }
    }
  }
}
```

```graphql
query NextBooksPage($cursor: String) {
  books(first: 10, after: $cursor) {
    pageInfo {
      hasNextPage
      endCursor
    }
    nodes {
      id
      title
    }
  }
}
```

```graphql
query StoreStats {
  storeStats {
    bookCount
    authorCount
    customerCount
    orderCount
    inventoryValue
  }
}
```

```graphql
mutation AddReview {
  addReview(input: {
    bookId: 1
    customerId: 1
    rating: 5
    comment: "Excellent read"
  }) {
    id
    rating
    comment
  }
}
```

```graphql
mutation PlaceOrder {
  placeOrder(input: {
    customerId: 1
    items: [
      { bookId: 1, quantity: 1 }
      { bookId: 3, quantity: 2 }
    ]
  }) {
    id
    status
    items {
      quantity
      unitPrice
      book {
        title
      }
    }
  }
}
```
