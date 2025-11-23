using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Routing;

namespace PagePlay.Site.Application.Accounts.Register;

public class RegisterEndpoint : IAccountEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints) =>
        endpoints.Register<RegisterResponse>("/register", handle);

    private async Task<IResult> handle(
        RegisterRequest request,
        IWorkflow<RegisterRequest, RegisterResponse> workflow
    ) => Respond.With(await workflow.Perform(request));
}

