# SQL Injection Protection Considerations

## Current State

**Primary Defense:**
- ✅ EF Core with parameterized queries (excellent baseline)
- ✅ LINQ expressions compiled to parameterized SQL
- ✅ No raw SQL concatenation in codebase

**What This Prevents:**
- Classic SQL injection: `admin' OR '1'='1`
- Union-based attacks
- Blind SQL injection
- Second-order SQL injection (stored then executed)

## Defense Layers

**Layer 1: ORM Parameterization (Current)**
```csharp
// ✅ SAFE - EF Core parameterizes automatically
var user = await dbContext.Users
    .Where(u => u.Email == userEmail)
    .FirstOrDefaultAsync();

// ❌ DANGEROUS - Never do this
var query = $"SELECT * FROM Users WHERE Email = '{userEmail}'";
```

**Layer 2: Input Validation**
- Validate data types, lengths, formats
- Reject inputs with SQL keywords if accepting raw strings
- Current: Endpoint-level validation only

**Layer 3: Database Permissions (Missing)**
- Application DB user should have minimal permissions
- No DROP, CREATE, ALTER permissions needed
- Read-only user for queries, write user for mutations
- Current: Single postgres user with full permissions

**Layer 4: WAF (Missing)**
- Detect SQL injection patterns in requests
- Block common attack signatures
- Cloudflare WAF or Azure Application Gateway
- Current: No WAF layer

## Scenarios Where We're Protected

**Standard CRUD Operations:**
```csharp
// All safe - EF Core handles parameterization
await repository.CreateAsync(entity);
await repository.UpdateAsync(entity);
await repository.GetByIdAsync(id);
```

**Dynamic Queries:**
```csharp
// Safe - LINQ builds parameterized query
var results = dbContext.Users
    .Where(u => searchTerm == null || u.Name.Contains(searchTerm))
    .ToListAsync();
```

## Scenarios Requiring Caution

**Raw SQL (if ever needed):**
```csharp
// ✅ SAFE - parameterized
await dbContext.Database.ExecuteSqlRawAsync(
    "SELECT * FROM Users WHERE Email = {0}", userEmail);

// ❌ DANGEROUS - interpolated
await dbContext.Database.ExecuteSqlRawAsync(
    $"SELECT * FROM Users WHERE Email = '{userEmail}'");
```

**Dynamic Column/Table Names:**
```csharp
// ❌ Cannot parameterize table/column names
// Must use whitelisting:
var allowedColumns = new[] { "Name", "Email", "CreatedAt" };
if (!allowedColumns.Contains(sortColumn))
    throw new ValidationException("Invalid sort column");
```

**Stored Procedures (if added):**
- Still parameterize: `EXEC GetUser @Email = {0}`
- Avoid dynamic SQL inside stored procedures

## Database Security Hardening

**Connection String Security:**
```json
// Current (appsettings.json):
"ConnectionString": "Host=localhost;Port=5432;Database=pageplay;Username=postgres;Password=postgres"
```

**Recommendations:**
1. Create application-specific DB user (not `postgres` superuser)
2. Grant only needed permissions: SELECT, INSERT, UPDATE, DELETE
3. Deny: CREATE, DROP, ALTER, TRUNCATE
4. Use separate read-only user for reporting queries

**Example:**
```sql
CREATE USER pageplay_app WITH PASSWORD 'strong_password';
GRANT CONNECT ON DATABASE pageplay TO pageplay_app;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO pageplay_app;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO pageplay_app;
REVOKE CREATE ON SCHEMA public FROM pageplay_app;
```

## Monitoring & Detection

**What to Log:**
- Failed queries with unusual patterns
- Queries exceeding expected parameter counts
- Database errors (potential injection probes)
- Currently: No specific SQL injection monitoring

**Red Flags in Logs:**
- Multiple single quotes in input
- SQL keywords (UNION, SELECT, DROP) in user input
- Unexpected error messages with SQL syntax

**Future Enhancement:**
- Add `IDbCommandInterceptor` to log/analyze all SQL before execution
- Alert on suspicious patterns
- Log parameter values for security audit

## EF Core Best Practices

**Always Safe:**
- LINQ queries
- `FindAsync()`, `FirstOrDefaultAsync()`, etc.
- `Add()`, `Update()`, `Remove()`
- Navigation property access

**Use With Caution:**
- `FromSqlRaw()` - always parameterize
- `ExecuteSqlRaw()` - always parameterize
- Dynamic queries - whitelist columns/tables

**Never Safe:**
- String interpolation in SQL: `$"SELECT * FROM {tableName}"`
- Raw concatenation: `"SELECT * FROM Users WHERE Id = " + userId`

## Current Architecture Integration

**Repository Pattern Benefits:**
- Centralized data access
- Consistent use of EF Core
- No ad-hoc SQL scattered in codebase
- Easy to audit and test

**Performer Pattern Benefits:**
- Input validation at boundary
- Type-safe requests
- Prevents raw user input reaching database layer

## Open Questions

- Should we create a dedicated DB user with minimal permissions?
- Do we need `IDbCommandInterceptor` for SQL logging/monitoring?
- Should we add WAF when deploying (Cloudflare recommended)?
- How do we handle dynamic sorting/filtering safely?
