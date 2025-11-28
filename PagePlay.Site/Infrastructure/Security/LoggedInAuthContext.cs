namespace PagePlay.Site.Infrastructure.Security;

public interface ICurrentUserContext
{
    long? UserId { get; }
}

public class CurrentUserContext : ICurrentUserContext
{
    public long? UserId { get; set; }
}
