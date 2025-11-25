using PagePlay.Site.Infrastructure.Domain;

namespace PagePlay.Site.Application.Accounts.Domain.Models;

public partial class User : IEntity, INormalizeValues
{
    public long Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public void Normalize()
    {
        Email = Email.ToLowerInvariant();
    }

    public static User Create(string email, string passwordHash)
    {
        var user = new User
        {
            Email = email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        user.Normalize();
        return user;
    }
}
