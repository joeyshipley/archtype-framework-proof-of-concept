using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Application.Accounts.Domain.Repository;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Accounts.ViewProfile;

public class ViewProfileWorkflow(
    IUserRepository _userRepository,
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

    private async Task<Domain.Models.User> getUserById(long userId) =>
        await _userRepository.GetById(userId);

    private ViewProfileResponse buildResponse(Domain.Models.User user) =>
        new ViewProfileResponse
        {
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
}
