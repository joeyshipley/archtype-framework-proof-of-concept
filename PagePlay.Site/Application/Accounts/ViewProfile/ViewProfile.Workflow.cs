using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Database.Repositories;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Accounts.ViewProfile;

public class ViewProfileWorkflow(
    IRepository _repository,
    LoggedInAuthContext _authContext,
    IValidator<ViewProfileWorkflowRequest> _validator
) : WorkflowBase<ViewProfileWorkflowRequest, ViewProfileWorkflowResponse>, IWorkflow<ViewProfileWorkflowRequest, ViewProfileWorkflowResponse>
{
    public async Task<IApplicationResult<ViewProfileWorkflowResponse>> Perform(ViewProfileWorkflowRequest workflowRequest)
    {
        var validationResult = await validate(workflowRequest);
        if (!validationResult.IsValid)
            return Fail(validationResult);

        var user = await getUserById(_authContext.UserId);
        if (user == null)
            return Fail("User not found.");

        return Succeed(buildResponse(user));
    }

    private async Task<ValidationResult> validate(ViewProfileWorkflowRequest workflowRequest) =>
        await _validator.ValidateAsync(workflowRequest);

    private async Task<User> getUserById(long userId) =>
        await _repository.Get(User.ById(userId));

    private ViewProfileWorkflowResponse buildResponse(User user) =>
        new ViewProfileWorkflowResponse
        {
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
}
