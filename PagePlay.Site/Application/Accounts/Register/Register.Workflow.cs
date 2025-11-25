using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Application.Accounts.Domain.Repository;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Accounts.Register;

public class RegisterWorkflow(
    IPasswordHasher _passwordHasher,
    IUserRepository _userRepository,
    IValidator<RegisterRequest> _validator
) : WorkflowBase<RegisterRequest, RegisterResponse>, IWorkflow<RegisterRequest, RegisterResponse>
{
    public async Task<IApplicationResult<RegisterResponse>> Perform(RegisterRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return Fail(validationResult);

        var user = createUser(request);

        var emailExists = await checkEmailExists(user.Email);
        if (emailExists)
            return Fail("An account with this email already exists.");

        await saveUser(user);

        return Succeed(buildResponse(user));
    }

    private async Task<ValidationResult> validate(RegisterRequest request) =>
        await _validator.ValidateAsync(request);

    private async Task<bool> checkEmailExists(string email) =>
        await _userRepository.Exists(UserSpecifications.ByEmail(email));

    private User createUser(RegisterRequest request) =>
        User.Create(request.Email, _passwordHasher.HashPassword(request.Password));

    private async Task saveUser(User user)
    {
        await _userRepository.Add(user);
        await _userRepository.SaveChanges();
    }

    private RegisterResponse buildResponse(User user) =>
        new RegisterResponse { UserId = user.Id };
}

