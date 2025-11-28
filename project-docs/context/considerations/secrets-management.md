# Secrets Management Considerations

## Current State

**Storage Location:**
- `appsettings.json` and `appsettings.Development.json`
- Plain text configuration files
- Checked into git (via .gitignore for appsettings.Production.json)

**Current Secrets:**
- Database connection string (password in plain text)
- JWT secret key
- Password pepper
- (Future: API keys, OAuth client secrets, encryption keys)

**What's Wrong:**
- ❌ No encryption at rest
- ❌ No access audit trail
- ❌ No secret rotation
- ❌ No separation between dev/staging/prod secrets
- ❌ Secrets in memory (process dump = exposed)

## Secrets Hierarchy

**Level 1: Local Development (Current)**
- `appsettings.Development.json`
- Committed to git with dummy/local values
- ✅ Fine for dev (`postgres:postgres`, dummy JWT key)

**Level 2: Environment Variables (Easy Win)**
- Read from env vars at runtime
- Not committed to git
- ⚠️ Still plain text, visible in process list
- Good enough for small deployments

**Level 3: Secrets Manager (Production)**
- Azure Key Vault, AWS Secrets Manager, HashiCorp Vault
- Encrypted at rest
- Access audit logs
- Automatic rotation
- ✅ Production-ready

**Level 4: HSM (High Security)**
- Hardware Security Module
- Secrets never leave hardware
- FIPS 140-2 Level 2/3 compliance
- Only for: Payment processing, healthcare, government

## Implementation Phases

**Phase 1 (Current - Development Only):**
- `appsettings.json` for local dev (acceptable)
- Dummy secrets: `postgres:postgres`, test JWT key

**Phase 2 (Staging/Production):**
- Environment variables for secrets
- `.env` file (not committed) or Docker secrets
- Azure App Configuration or AWS Parameter Store

**Phase 3 (Production Hardening):**
- Azure Key Vault or AWS Secrets Manager
- Secrets never touch disk
- Automatic rotation (90 days)
- Access audit logs

**Phase 4 (Compliance - If Needed):**
- HSM for encryption keys
- SOC 2, PCI-DSS, HIPAA requirements

## Environment Variables Approach

**Setup:**
```bash
# .env file (add to .gitignore)
DATABASE_CONNECTION_STRING="Host=prod-db;Database=pageplay;Username=app;Password=secure123"
JWT_SECRET_KEY="production-secret-key-with-32-chars-minimum"
PASSWORD_PEPPER="production-pepper-value"
```

**appsettings.json:**
```json
{
  "Database": {
    "ConnectionString": "" // Override from env var
  },
  "Security": {
    "PasswordPepper": "",
    "Jwt": {
      "SecretKey": ""
    }
  }
}
```

**Program.cs:**
```csharp
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{env}.json", optional: true)
    .AddEnvironmentVariables(); // Overrides JSON values
```

**Benefits:**
- ✅ Secrets not in git
- ✅ Different per environment
- ✅ Works with Docker, K8s
- ⚠️ Still plain text in memory

## Azure Key Vault Integration

**Setup:**
```csharp
// Program.cs
if (!builder.Environment.IsDevelopment())
{
    var keyVaultUrl = new Uri(builder.Configuration["KeyVault:Url"]);
    builder.Configuration.AddAzureKeyVault(
        keyVaultUrl,
        new DefaultAzureCredential());
}
```

**Key Vault Secrets:**
- `Database--ConnectionString`
- `Security--PasswordPepper`
- `Security--Jwt--SecretKey`

**Benefits:**
- ✅ Encrypted at rest
- ✅ Access audit logs
- ✅ Automatic rotation
- ✅ Role-based access control
- ✅ Secrets never on disk
- 💰 Cost: ~$0.03 per 10k operations

**Drawbacks:**
- Complexity (Azure account, credentials)
- Latency (network call for secrets)
- Vendor lock-in (Azure-specific)

## Secret Rotation Strategy

**Current:** No rotation (secrets static)

**Recommended:**
1. **JWT Secret Key:** Rotate every 90 days
   - Use dual keys during transition (old + new)
   - Validate with both, sign with new
   - Retire old after 24 hours

2. **Database Password:** Rotate every 90 days
   - Use connection pooling with reconnect
   - Brief downtime during rotation (< 1 sec)

3. **Password Pepper:** Never rotate (breaks existing passwords)
   - If compromised: re-hash all passwords with new pepper

4. **API Keys:** Rotate on access or 90 days

**Automation:**
- Key Vault: Built-in rotation policies
- Manual: Calendar reminder, runbook

## Secrets in Code

**❌ Never Do This:**
```csharp
var apiKey = "sk-1234567890abcdef"; // Hardcoded secret
var connStr = "Server=prod;Password=secret123"; // In source code
```

**✅ Always Do This:**
```csharp
var apiKey = _configuration["ExternalService:ApiKey"];
var connStr = _settingsProvider.Database.ConnectionString;
```

**Configuration, Not Secrets:**
- ✅ Feature flags: `"EnableNewUI": true`
- ✅ URLs: `"ApiBaseUrl": "https://api.example.com"`
- ✅ Timeouts: `"RequestTimeoutSeconds": 30`
- ❌ Passwords, API keys, tokens

## Developer Experience

**Local Development:**
```json
// appsettings.Development.json (committed)
{
  "Database": {
    "ConnectionString": "Host=localhost;Database=pageplay;Username=postgres;Password=postgres"
  },
  "Security": {
    "PasswordPepper": "dev-pepper-not-for-production",
    "Jwt": {
      "SecretKey": "dev-secret-key-min-32-characters-long",
      "Issuer": "PagePlay.Dev",
      "Audience": "PagePlay.Dev",
      "ExpirationMinutes": 60
    }
  }
}
```

**CI/CD Pipeline:**
- Inject secrets via GitHub Secrets, Azure DevOps Variables
- Never log secrets (mask in CI output)
- Separate secrets per environment (dev, staging, prod)

**Production:**
- Environment variables or Key Vault
- No secrets in deployment artifacts
- Rotate on team member departure

## Secrets Detection

**Tools to Prevent Leaks:**
- `git-secrets` - Scans commits for secrets
- `truffleHog` - Finds secrets in git history
- GitHub Secret Scanning (automatic)
- Pre-commit hooks

**Example Pre-Commit Hook:**
```bash
#!/bin/bash
# Reject commits with "password=" or "secret="
if git diff --cached | grep -iE "(password=|secret=|api_key=)"; then
    echo "ERROR: Possible secret detected in commit"
    exit 1
fi
```

## Current Architecture Integration

**SettingsProvider Pattern:**
- Already abstracts configuration access
- Easy to swap JSON → Key Vault
- Change one line in `Program.cs`

**No Code Changes Needed:**
```csharp
// This works with JSON, env vars, or Key Vault
var jwtKey = _settingsProvider.Security.Jwt.SecretKey;
```

**Migration Path:**
1. Add environment variable support (1 line)
2. Deploy with env vars (testing)
3. Add Key Vault (production)
4. Enable secret rotation

## Open Questions

- Which secrets manager for production? (Azure Key Vault vs AWS Secrets Manager)
- How to handle local development secrets? (Environment variables vs appsettings.Development.json)
- Secret rotation frequency? (90 days standard, more aggressive?)
- Should we use Docker secrets for container deployments?
- When to move from env vars to Key Vault? (Before public launch)

## Compliance Requirements

**SOC 2:**
- Secrets encrypted at rest ✅ (Key Vault)
- Access audit logs ✅ (Key Vault)
- Regular rotation ✅ (Policies)

**PCI-DSS:**
- Encryption key management ✅ (HSM required)
- No secrets in source code ✅
- Quarterly rotation ✅

**HIPAA:**
- Encrypted secrets ✅
- Access controls ✅
- Audit trails ✅

**Current Status:** None - not needed until handling sensitive data or customer deployments
