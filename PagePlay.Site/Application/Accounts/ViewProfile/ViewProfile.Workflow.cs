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
) : IWorkflow<ViewProfileRequest, ViewProfileResponse>
{
    public async Task<IApplicationResult<ViewProfileResponse>> Perform(ViewProfileRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return response(validationResult);

        var user = await getUserById(_authContext.UserId);
        if (user == null)
            return response("User not found.");

        return response(user.Email, user.CreatedAt);
    }

    private async Task<ValidationResult> validate(ViewProfileRequest request) =>
        await _validator.ValidateAsync(request);

    private async Task<Domain.Models.User> getUserById(long userId) =>
        await _userRepository.GetById(userId);

    private IApplicationResult<ViewProfileResponse> response(ValidationResult validationResult) =>
        ApplicationResult<ViewProfileResponse>.Fail(validationResult);

    private IApplicationResult<ViewProfileResponse> response(string errorMessage) =>
        ApplicationResult<ViewProfileResponse>.Fail(errorMessage);

    private IApplicationResult<ViewProfileResponse> response(string email, DateTime createdAt) =>
        ApplicationResult<ViewProfileResponse>.Succeed(
            new ViewProfileResponse
            {
                Email = email,
                CreatedAt = createdAt
            }
        );
}
