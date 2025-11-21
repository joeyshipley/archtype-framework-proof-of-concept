using System.Security.Cryptography;
using System.Text;
using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Infrastructure.Security;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}

public class PasswordHasher(ISettingsProvider _settingsProvider) : IPasswordHasher
{
    public string HashPassword(string password)
    {
        var pepperedPassword = HmacSha256(password, _settingsProvider.Security.PasswordPepper);
        return BCrypt.Net.BCrypt.HashPassword(pepperedPassword, workFactor: 12);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        var pepperedPassword = HmacSha256(password, _settingsProvider.Security.PasswordPepper);
        return BCrypt.Net.BCrypt.Verify(pepperedPassword, hashedPassword);
    }

    private string HmacSha256(string password, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hash);
    }
}