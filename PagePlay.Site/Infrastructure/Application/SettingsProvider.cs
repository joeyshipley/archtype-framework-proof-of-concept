namespace PagePlay.Site.Infrastructure.Application;

public interface ISettingsProvider
{
    SecuritySettings Security { get; }
}

public class SettingsProvider(IConfiguration _configuration) 
    : ISettingsProvider
{
    public SecuritySettings Security { get; } = _configuration
        .GetSection("Security")
        .Get<SecuritySettings>()
        ?? new SecuritySettings();
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