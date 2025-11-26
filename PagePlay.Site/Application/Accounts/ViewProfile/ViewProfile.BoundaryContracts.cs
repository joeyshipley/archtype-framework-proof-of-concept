using FluentValidation;
using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Application.Accounts.ViewProfile;

public class ViewProfileWorkflowResponse : IWorkflowResponse
{
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ViewProfileWorkflowRequest : IWorkflowRequest
{
    // No request properties needed - user ID comes from JWT token
}

public class ViewProfileRequestValidator : AbstractValidator<ViewProfileWorkflowRequest>
{
    public ViewProfileRequestValidator()
    {
        // No validation rules needed for empty request
    }
}
