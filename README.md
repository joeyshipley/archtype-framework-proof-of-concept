# PagePlay

A server-rendered web framework applying game architecture patterns to web development.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop)

## Development Setup

### 1. Start PostgreSQL

```bash
docker-compose up -d
```

This starts a PostgreSQL 16 container with:
- **Host:** localhost
- **Port:** 5432
- **Database:** pageplay
- **Username:** postgres
- **Password:** postgres

### 2. Apply Database Migrations

```bash
cd PagePlay.Site
dotnet ef database update
```

### 3. Run the Application

```bash
cd PagePlay.Site
dotnet run
```

The app will be available at `https://localhost:5001` (or the port shown in console output).

## Common Tasks

### Reset Database

If you need to start fresh:

```bash
# Stop and remove the container and volume
docker-compose down -v

# Start fresh
docker-compose up -d

# Re-apply migrations
cd PagePlay.Site && dotnet ef database update
```

### Add a New Migration

```bash
cd PagePlay.Site
dotnet ef migrations add MigrationName
```

### Run Tests

```bash
dotnet test
```

## Project Structure

```
PagePlay/
├── PagePlay.Site/          # Main web application
│   ├── Application/        # Feature vertical slices
│   ├── Pages/              # Page views and interactions
│   └── Infrastructure/     # Framework, data, UI rendering
├── PagePlay.Tests/         # Test project
└── docker-compose.yml      # PostgreSQL container config
```

See `.claude/docs/` for detailed architecture documentation.
