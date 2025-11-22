using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Accounts.Login;

public class LoginWorkflow(
    IJwtTokenService _jwtTokenService
) : IWorkflow<LoginRequest, LoginResponse>
{
    public Task<IApplicationResult<LoginResponse>> Perform(LoginRequest request)
    {
        var token = generateToken(request.Email);
        return Task.FromResult(response(token));
    }

    private IApplicationResult<LoginResponse> response(string token) =>
        ApplicationResult<LoginResponse>.Succeed(new LoginResponse { Token = token });

    private string generateToken(string email) =>
        _jwtTokenService.GenerateToken(email);
}