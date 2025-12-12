using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Data.Repositories;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Accounts.ViewProfile;

public class ViewProfilePerformer(
    IRepository _repository,
    ICurrentUserContext currentUserContext,
    IValidator<ViewProfileRequest> _validator
) : PerformerBase<ViewProfileRequest, ViewProfileResponse>, IPerformer<ViewProfileRequest, ViewProfileResponse>
{
    public async Task<IApplicationResult<ViewProfileResponse>> Perform(ViewProfileRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return Fail(validationResult);

        var user = await getUserById(currentUserContext.UserId.Value);
        if (user == null)
            return Fail("User not found.");

        return Succeed(buildResponse(user));
    }

    private async Task<ValidationResult> validate(ViewProfileRequest request) =>
        await _validator.ValidateAsync(request);

    private async Task<User> getUserById(long userId) =>
        await _repository.Get(User.ById(userId));

    private ViewProfileResponse buildResponse(User user) =>
        new ViewProfileResponse
        {
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
}
