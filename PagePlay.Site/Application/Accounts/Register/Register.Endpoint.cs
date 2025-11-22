using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Routing;

namespace PagePlay.Site.Application.Accounts.Register;

public class RegisterEndpoint(IWorkflow<RegisterRequest, RegisterResponse> _workflow) : IAccountEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints) =>
        endpoints.Register<RegisterResponse>("/register", handle);

    private async Task<IResult> handle(RegisterRequest request) =>
        Respond.With(await _workflow.Perform(request));
}

