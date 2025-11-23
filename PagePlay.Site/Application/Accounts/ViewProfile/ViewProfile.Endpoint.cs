using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Routing;

namespace PagePlay.Site.Application.Accounts.ViewProfile;

public class ViewProfileEndpoint : IAccountEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints) =>
        endpoints
            .Register<ViewProfileResponse>("/accounts/viewprofile", handle)
            .RequireAuthenticatedUser();

    private async Task<IResult> handle(
        ViewProfileRequest request,
        IWorkflow<ViewProfileRequest, ViewProfileResponse> workflow
    ) => Respond.With(await workflow.Perform(request));
}
