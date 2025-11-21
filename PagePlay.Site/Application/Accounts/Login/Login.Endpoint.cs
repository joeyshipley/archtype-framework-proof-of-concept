using PagePlay.Site.Infrastructure.Routing;

namespace PagePlay.Site.Application.Accounts.Login;

public class LoginEndpoint(ILoginWorkflow _workflow) : IAccountEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints) => 
        endpoints.Register<LoginResponse>("/login", handle);

    private async Task<IResult> handle(LoginRequest request) =>
        Respond.With(await _workflow.Login(request));
}