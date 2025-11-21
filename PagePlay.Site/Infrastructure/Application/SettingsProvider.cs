namespace PagePlay.Site.Infrastructure.Application;

public interface ISettingsProvider
{
    JwtSettings Jwt { get; }
}

public class SettingsProvider(IConfiguration _configuration) 
    : ISettingsProvider
{
    public JwtSettings Jwt { get; } = _configuration
        .GetSection("Jwt")
        .Get<JwtSettings>() 
        ?? new JwtSettings();
}

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; }
}