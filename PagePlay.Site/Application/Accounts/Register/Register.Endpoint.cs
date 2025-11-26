using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Web.Routing;

namespace PagePlay.Site.Application.Accounts.Register;

public class RegisterEndpoint : IAccountEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints) =>
        endpoints.Register<RegisterWorkflowResponse>("/register", handle);

    private async Task<IResult> handle(
        RegisterWorkflowRequest workflowRequest,
        IWorkflow<RegisterWorkflowRequest, RegisterWorkflowResponse> workflow
    ) => Respond.With(await workflow.Perform(workflowRequest));
}

