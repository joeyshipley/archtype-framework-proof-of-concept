namespace PagePlay.Site.Infrastructure.Security;

public interface IAuthContext
{
    long? UserId { get; }
}

public class LoggedInAuthContext : IAuthContext
{
    public long? UserId { get; set; }
}
