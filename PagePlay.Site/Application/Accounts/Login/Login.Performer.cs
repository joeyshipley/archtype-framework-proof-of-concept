using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Data.Repositories;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Accounts.Login;

public class LoginPerformer(
    IRepository _repository,
    IPasswordHasher _passwordHasher,
    IJwtTokenService _jwtTokenService,
    IValidator<LoginRequest> _validator
) : PerformerBase<LoginRequest, LoginResponse>, IPerformer<LoginRequest, LoginResponse>
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

    private async Task<User> getUserByEmail(string email) =>
        await _repository.Get(User.ByEmail(email));

    private bool verifyPassword(string password, string passwordHash) =>
        _passwordHasher.VerifyPassword(password, passwordHash);

    private string generateToken(long userId) =>
        _jwtTokenService.GenerateToken(new TokenClaims { UserId = userId });

    private LoginResponse buildResponse(long userId, string token) =>
        new LoginResponse { UserId = userId, Token = token };
}
