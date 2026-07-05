# GraphQL C# Bookstore API

ASP.NET Core 10 GraphQL API using Hot Chocolate, EF Core, PostgreSQL, Identity, JWT bearer authentication, refresh tokens, role-based authorization, and seed data.

Public API surfaces depend on services or GraphQL repositories instead of using `AppDbContext` directly. EF Core access is isolated in the `Repositories/` layer and registered through dependency injection.

## Run locally

Prerequisites:

```text
.NET SDK 10
Docker Desktop or PostgreSQL 16+
```

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

Check the installed SDK:

```bash
dotnet --version
```

The project targets:

```text
net10.0
```

In development, migrations are applied on startup and the seed data from `Data/SeedData.cs` is inserted through EF migrations. If you prefer applying migrations manually:

   ```bash
   dotnet ef database update
   ```

Default local admin seeded at startup:

```text
Email:    admin@example.com
Password: Admin123!
Role:     Admin
```

Change these values in `appsettings.Development.json` before using this beyond local development.

GraphQL Banana Cake Pop IDE:

```text
http://localhost:5000/graphql
```

Swagger UI:

```text
http://localhost:5000/swagger
```

Use `POST /api/auth/login`, copy the returned access token, click **Authorize** in Swagger, and paste:

```text
Bearer <access-token>
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

Auth endpoints:

```text
POST /api/auth/register
POST /api/auth/login
POST /api/auth/refresh
POST /api/auth/revoke
GET  /api/auth/me
GET  /api/auth/admin-check
```

JWT/role protection:

```text
PATCH /api/controllers/books/{id}/stock  requires Admin or Manager
POST  /api/controllers/orders            requires any authenticated user
POST  /api/controllers/reviews           requires any authenticated user
GET   /api/controllers/customers         requires Admin
GET   /api/auth/admin-check              requires Admin
```

Login response returns an access token and refresh token. Use the access token as:

```text
Authorization: Bearer <access-token>
```

Refresh token flow:

```json
{
  "refreshToken": "<refresh-token>"
}
```

The refresh endpoint rotates refresh tokens: the old token is revoked and replaced by a new one.

## Example queries

Full read and mutation examples are available in:

```text
GraphQL/examples.graphql
```

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
