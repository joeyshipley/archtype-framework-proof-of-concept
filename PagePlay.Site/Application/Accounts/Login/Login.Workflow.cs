using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Application.Accounts.Domain.Repository;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Accounts.Login;

public class LoginWorkflow(
    IUserRepository _userRepository,
    IPasswordHasher _passwordHasher,
    IJwtTokenService _jwtTokenService,
    IValidator<LoginRequest> _validator
) : WorkflowBase<LoginRequest, LoginResponse>, IWorkflow<LoginRequest, LoginResponse>
{
    public async Task<IApplicationResult<LoginResponse>> Perform(LoginRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return Fail(validationResult);

        var user = await getUserByEmail(request.Email);
        if (user == null)
            return Fail("Invalid email or password.");

        var passwordValid = verifyPassword(request.Password, user.PasswordHash);
        if (!passwordValid)
            return Fail("Invalid email or password.");

        var token = generateToken(user.Id);
        return Succeed(buildResponse(user.Id, token));
    }

    private async Task<ValidationResult> validate(LoginRequest request) =>
        await _validator.ValidateAsync(request);

    private async Task<Domain.Models.User> getUserByEmail(string email) =>
        await _userRepository.Get(UserSpecifications.ByEmail(email));

    private bool verifyPassword(string password, string passwordHash) =>
        _passwordHasher.VerifyPassword(password, passwordHash);

    private string generateToken(long userId) =>
        _jwtTokenService.GenerateToken(new TokenClaims { UserId = userId });

    private LoginResponse buildResponse(long userId, string token) =>
        new LoginResponse { UserId = userId, Token = token };
}