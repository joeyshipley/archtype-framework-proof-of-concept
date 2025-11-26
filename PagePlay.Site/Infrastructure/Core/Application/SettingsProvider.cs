namespace PagePlay.Site.Infrastructure.Core.Application;

public interface ISettingsProvider
{
    SecuritySettings Security { get; }
    DatabaseSettings Database { get; }
}

public class SettingsProvider(IConfiguration _configuration)
    : ISettingsProvider
{
    public SecuritySettings Security { get; } = _configuration
        .GetSection("Security")
        .Get<SecuritySettings>()
        ?? new SecuritySettings();

    public DatabaseSettings Database { get; } = _configuration
        .GetSection("Database")
        .Get<DatabaseSettings>()
        ?? new DatabaseSettings();
}

public class SecuritySettings
{
    public string PasswordPepper { get; set; }
    public JwtSettings Jwt { get; set; }
}

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; }
}

public class DatabaseSettings
{
    public string ConnectionString { get; set; } = string.Empty;
}