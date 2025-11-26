using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Web.Routing;

namespace PagePlay.Site.Application.Accounts.Login;

public class LoginEndpoint : IAccountEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints) =>
        endpoints.Register<LoginWorkflowResponse>("/login", handle);

    private async Task<IResult> handle(
        LoginWorkflowRequest workflowRequest,
        IWorkflow<LoginWorkflowRequest, LoginWorkflowResponse> workflow
    ) => Respond.With(await workflow.Perform(workflowRequest));
}