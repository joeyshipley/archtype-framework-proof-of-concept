using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Web.Routing;

namespace PagePlay.Site.Application.Accounts.ViewProfile;

public class ViewProfileEndpoint : IAccountEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints) =>
        endpoints
            .Register<ViewProfileWorkflowResponse>("/viewprofile", handle)
            .RequireAuthenticatedUser();

    private async Task<IResult> handle(
        ViewProfileWorkflowRequest workflowRequest,
        IWorkflow<ViewProfileWorkflowRequest, ViewProfileWorkflowResponse> workflow
    ) => Respond.With(await workflow.Perform(workflowRequest));
}
