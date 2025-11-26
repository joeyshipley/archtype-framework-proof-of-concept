using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Database.Repositories;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Accounts.Login;

public class LoginWorkflow(
    IRepository _repository,
    IPasswordHasher _passwordHasher,
    IJwtTokenService _jwtTokenService,
    IValidator<LoginWorkflowRequest> _validator
) : WorkflowBase<LoginWorkflowRequest, LoginWorkflowResponse>, IWorkflow<LoginWorkflowRequest, LoginWorkflowResponse>
{
    public async Task<IApplicationResult<LoginWorkflowResponse>> Perform(LoginWorkflowRequest workflowRequest)
    {
        var validationResult = await validate(workflowRequest);
        if (!validationResult.IsValid)
            return Fail(validationResult);

        var user = await getUserByEmail(workflowRequest.Email);
        if (user == null)
            return Fail("Invalid email or password.");

        var passwordValid = verifyPassword(workflowRequest.Password, user.PasswordHash);
        if (!passwordValid)
            return Fail("Invalid email or password.");

        var token = generateToken(user.Id);
        return Succeed(buildResponse(user.Id, token));
    }

    private async Task<ValidationResult> validate(LoginWorkflowRequest workflowRequest) =>
        await _validator.ValidateAsync(workflowRequest);

    private async Task<User> getUserByEmail(string email) =>
        await _repository.Get(User.ByEmail(email));

    private bool verifyPassword(string password, string passwordHash) =>
        _passwordHasher.VerifyPassword(password, passwordHash);

    private string generateToken(long userId) =>
        _jwtTokenService.GenerateToken(new TokenClaims { UserId = userId });

    private LoginWorkflowResponse buildResponse(long userId, string token) =>
        new LoginWorkflowResponse { UserId = userId, Token = token };
}