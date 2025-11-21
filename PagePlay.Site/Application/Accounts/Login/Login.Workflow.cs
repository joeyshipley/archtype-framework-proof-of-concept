using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Application.Accounts.Login;

public interface ILoginWorkflow
{
    Task<IApplicationResult<LoginResponse>> Login(LoginRequest request);
}

public class LoginWorkflow() : ILoginWorkflow
{
    public Task<IApplicationResult<LoginResponse>> Login(LoginRequest request)
    {
        var response = new LoginResponse { Message = "Login API Workflow is GO!" };
        return Task.FromResult(ApplicationResult<LoginResponse>.Succeed(response));
    }
}