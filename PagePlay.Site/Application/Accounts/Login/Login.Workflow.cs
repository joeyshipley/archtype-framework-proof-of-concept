using FluentValidation;
using PagePlay.Site.Application.Accounts.Domain.Repository;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Accounts.Login;

public class LoginWorkflow(
    IUserRepository _userRepository,
    IPasswordHasher _passwordHasher,
    IJwtTokenService _jwtTokenService,
    IValidator<LoginRequest> _validator
) : IWorkflow<LoginRequest, LoginResponse>
{
    public async Task<IApplicationResult<LoginResponse>> Perform(LoginRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return response(validationResult);

        var user = await getUserByEmail(request.Email);
        if (user == null)
            return response("Invalid email or password.");

        var passwordValid = verifyPassword(request.Password, user.PasswordHash);
        if (!passwordValid)
            return response("Invalid email or password.");

        var token = generateToken(user.Id);
        return response(user.Id, token);
    }

    private IApplicationResult<LoginResponse> response(FluentValidation.Results.ValidationResult validationResult) =>
        ApplicationResult<LoginResponse>.Fail(validationResult);

    private IApplicationResult<LoginResponse> response(string errorMessage) =>
        ApplicationResult<LoginResponse>.Fail(errorMessage);

    private IApplicationResult<LoginResponse> response(long userId, string token) =>
        ApplicationResult<LoginResponse>.Succeed(
            new LoginResponse { UserId = userId, Token = token }
        );

    private async Task<FluentValidation.Results.ValidationResult> validate(LoginRequest request) =>
        await _validator.ValidateAsync(request);

    private async Task<PagePlay.Site.Application.Accounts.Domain.Models.User> getUserByEmail(string email) =>
        await _userRepository.GetByEmail(email);

    private bool verifyPassword(string password, string passwordHash) =>
        _passwordHasher.VerifyPassword(password, passwordHash);

    private string generateToken(long userId) =>
        _jwtTokenService.GenerateToken(new TokenClaims { UserId = userId });
}