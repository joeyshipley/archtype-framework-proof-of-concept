using FluentValidation;
using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Application.Accounts.Login;

public class LoginResponse : IResponse
{
    public long UserId { get; set; }
    public string Token { get; set; }
}

public class LoginRequest : IRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}