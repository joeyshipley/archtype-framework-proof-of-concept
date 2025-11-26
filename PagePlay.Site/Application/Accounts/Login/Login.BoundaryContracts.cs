using FluentValidation;
using PagePlay.Site.Infrastructure.Core.Application;

namespace PagePlay.Site.Application.Accounts.Login;

public class LoginWorkflowResponse : IWorkflowResponse
{
    public long UserId { get; set; }
    public string Token { get; set; }
}

public class LoginWorkflowRequest : IWorkflowRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginRequestValidator : AbstractValidator<LoginWorkflowRequest>
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