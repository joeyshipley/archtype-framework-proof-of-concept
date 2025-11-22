using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Application.Accounts.ViewProfile;

public class ViewProfileResponse : IResponse
{
    public string Message { get; set; }
}

public class ViewProfileRequest : IRequest {}