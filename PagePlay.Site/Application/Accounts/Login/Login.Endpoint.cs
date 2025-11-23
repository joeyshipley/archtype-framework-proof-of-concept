using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Routing;

namespace PagePlay.Site.Application.Accounts.Login;

public class LoginEndpoint : IAccountEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints) =>
        endpoints.Register<LoginResponse>("/login", handle);

    private async Task<IResult> handle(
        LoginRequest request,
        IWorkflow<LoginRequest, LoginResponse> workflow
    ) => Respond.With(await workflow.Perform(request));
}