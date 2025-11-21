using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Accounts.Login;

public interface ILoginWorkflow
{
    Task<IApplicationResult<LoginResponse>> Login(LoginRequest request);
}

public class LoginWorkflow(
    IJwtTokenService _jwtTokenService
) : ILoginWorkflow
{
    public Task<IApplicationResult<LoginResponse>> Login(LoginRequest request)
    {
        var token = _jwtTokenService.GenerateToken(request.Email);
        var response = new LoginResponse { Token = token };
        return Task.FromResult(ApplicationResult<LoginResponse>.Succeed(response));
    }
}