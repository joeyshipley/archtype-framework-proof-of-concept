using FluentValidation;
using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Application.Accounts.Domain.Repository;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Accounts.Register;

public class RegisterWorkflow(
    IPasswordHasher _passwordHasher,
    IUserRepository _userRepository,
    IValidator<RegisterRequest> _validator
) : IWorkflow<RegisterRequest, RegisterResponse>
{
    public async Task<IApplicationResult<RegisterResponse>> Execute(RegisterRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return response(validationResult);

        var user = createUser(request);

        var emailExists = await checkEmailExists(user.Email);
        if (emailExists)
            return response("An account with this email already exists.");

        await saveUser(user);

        return response();
    }

    private IApplicationResult<RegisterResponse> response(FluentValidation.Results.ValidationResult validationResult) =>
        ApplicationResult<RegisterResponse>.Fail(validationResult);

    private IApplicationResult<RegisterResponse> response(string errorMessage) =>
        ApplicationResult<RegisterResponse>.Fail(errorMessage);

    private IApplicationResult<RegisterResponse> response() =>
        ApplicationResult<RegisterResponse>.Succeed(
            new RegisterResponse { Message = "Account created successfully. You can now log in." }
        );

    private async Task<FluentValidation.Results.ValidationResult> validate(RegisterRequest request) =>
        await _validator.ValidateAsync(request);

    private async Task<bool> checkEmailExists(string email) =>
        await _userRepository.EmailExistsAsync(email);

    private User createUser(RegisterRequest request) =>
        User.Create(request.Email, _passwordHasher.HashPassword(request.Password));

    private async Task saveUser(User user)
    {
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
    }
}

