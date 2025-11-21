using Microsoft.VisualBasic;
using PagePlay.Site.Infrastructure.Routing;

namespace PagePlay.Site.Application.Accounts.Login;

public class LoginEndpoint(ILoginWorkflow _workflow) : IAccountEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/", Handle)
            .WithName("Login")
            .WithSummary("Login to an account")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .WithApplicationErrorResponses();
    }

    private async Task<IResult> Handle(LoginRequest request)
        => Respond.Ok(await _workflow.Login(request));
}