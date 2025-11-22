using PagePlay.Site.Infrastructure.Domain;

namespace PagePlay.Site.Application.Accounts.Domain.Models;

public class User : IEntity
{
    public long Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
