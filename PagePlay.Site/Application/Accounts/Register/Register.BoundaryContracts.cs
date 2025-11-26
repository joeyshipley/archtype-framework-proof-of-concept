using FluentValidation;
using PagePlay.Site.Infrastructure.Core.Application;

namespace PagePlay.Site.Application.Accounts.Register;

public class RegisterWorkflowResponse : IWorkflowResponse
{
    public long UserId { get; set; }
}

public class RegisterWorkflowRequest : IWorkflowRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

public class RegisterRequestValidator : AbstractValidator<RegisterWorkflowRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Password confirmation is required.")
            .Equal(x => x.Password).WithMessage("Passwords do not match.");
    }
}