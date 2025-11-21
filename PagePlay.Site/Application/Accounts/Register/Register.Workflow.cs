using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Accounts.Register;

public interface IRegisterWorkflow
{
    Task<IApplicationResult<RegisterResponse>> Register(RegisterRequest request);
}

public class RegisterWorkflow(
    IPasswordHasher _passwordHasher
) : IRegisterWorkflow
{
    public Task<IApplicationResult<RegisterResponse>> Register(RegisterRequest request)
    {
        var response = new RegisterResponse { Message = "Register API Workflow is GO!" };
        return Task.FromResult(ApplicationResult<RegisterResponse>.Succeed(response));
    }
}

