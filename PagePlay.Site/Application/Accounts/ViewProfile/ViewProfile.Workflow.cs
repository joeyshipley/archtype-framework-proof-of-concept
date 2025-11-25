using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Database.Repositories;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Accounts.ViewProfile;

public class ViewProfileWorkflow(
    IRepository _userRepository,
    LoggedInAuthContext _authContext,
    IValidator<ViewProfileRequest> _validator
) : WorkflowBase<ViewProfileRequest, ViewProfileResponse>, IWorkflow<ViewProfileRequest, ViewProfileResponse>
{
    public async Task<IApplicationResult<ViewProfileResponse>> Perform(ViewProfileRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return Fail(validationResult);

        var user = await getUserById(_authContext.UserId);
        if (user == null)
            return Fail("User not found.");

        return Succeed(buildResponse(user));
    }

    private async Task<ValidationResult> validate(ViewProfileRequest request) =>
        await _validator.ValidateAsync(request);

    private async Task<User> getUserById(long userId) =>
        await _userRepository.Get<User>(User.ById(userId));

    private ViewProfileResponse buildResponse(User user) =>
        new ViewProfileResponse
        {
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
}
