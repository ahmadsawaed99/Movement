# Movement API

A .NET 8 REST API with a multi-layer caching strategy: Redis (L1) → in-process LRU (L2) → PostgreSQL.

## Prerequisites

| Tool | Version |
|------|---------|
| .NET SDK | 8.0+ |
| PostgreSQL | 14+ |
| Redis | 7+ |

## Configuration

Update `Movement.API/appsettings.json` with your environment values:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=movement_db;Username=<user>;Password=<password>",
    "Redis": "localhost:6379"
  },
  "RedisSettings": {
    "TimeoutMs": 500
  }
}
```

| Setting | Description | Default |
|---------|-------------|---------|
| `ConnectionStrings:DefaultConnection` | PostgreSQL connection string | — |
| `ConnectionStrings:Redis` | Redis connection string | — |
| `RedisSettings:TimeoutMs` | Max wait per Redis operation before falling back | `500` |

## Setup

**1. Apply database migrations**

```bash
dotnet ef database update --project Movement.API
```

**2. Run the API**

```bash
dotnet run --project Movement.API
```

**3. Open Swagger UI**

Navigate to `http://localhost:<port>/swagger` in your browser.

## API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/items/{id}` | Get an item by ID |
| `POST` | `/api/items` | Create a new item |

## Caching Behavior

Reads follow a three-tier fallback:

1. **Redis** — distributed cache, shared across instances (5-minute TTL)
2. **LRU in-process cache** — bounded to 50 entries, per-instance
3. **PostgreSQL** — source of truth; result is written back to both caches

If Redis is unavailable, `RedisService` catches the error, logs a warning, and the request continues through the LRU cache and database transparently — no downtime.
