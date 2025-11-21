using PagePlay.Site.Infrastructure.Routing;

namespace PagePlay.Site.Application.Accounts.ViewProfile;

public class ViewProfileEndpoint(IViewProfileWorkflow _workflow) : IAccountEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints) => 
        endpoints
            .Register<ViewProfileResponse>("/profile", handle)
            .RequireAuthorization();

    private async Task<IResult> handle(ViewProfileRequest request) =>
        Respond.With(await _workflow.ViewProfile(request));
}