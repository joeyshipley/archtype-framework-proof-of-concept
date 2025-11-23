using System.IdentityModel.Tokens.Jwt;
using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Application.Accounts.Domain.Repository;
using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Application.Accounts.ViewProfile;

public class ViewProfileWorkflow(
    IUserRepository _userRepository,
    IHttpContextAccessor _httpContextAccessor,
    IValidator<ViewProfileRequest> _validator
) : IWorkflow<ViewProfileRequest, ViewProfileResponse>
{
    public async Task<IApplicationResult<ViewProfileResponse>> Perform(ViewProfileRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return response(validationResult);

        var userId = getCurrentUserId();
        if (userId == null)
            return response("User not authenticated.");

        var user = await getUserById(userId.Value);
        if (user == null)
            return response("User not found.");

        return response(user.Email, user.CreatedAt);
    }

    private async Task<ValidationResult> validate(ViewProfileRequest request) =>
        await _validator.ValidateAsync(request);

    private long? getCurrentUserId()
    {
        var userClaim = _httpContextAccessor.HttpContext?.User
            .FindFirst(JwtRegisteredClaimNames.Sub);

        if (userClaim == null || !long.TryParse(userClaim.Value, out var userId))
            return null;

        return userId;
    }

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
