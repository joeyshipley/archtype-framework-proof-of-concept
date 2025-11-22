using FluentValidation;
using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Application.Accounts.Domain.Repository;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Accounts.Register;

public interface IRegisterWorkflow
{
    Task<IApplicationResult<RegisterResponse>> Register(RegisterRequest request);
}

public class RegisterWorkflow(
    IPasswordHasher _passwordHasher,
    IUserRepository _userRepository,
    IValidator<RegisterRequest> _validator
) : IRegisterWorkflow
{
    public async Task<IApplicationResult<RegisterResponse>> Register(RegisterRequest request)
    {
        // 1. Validate input
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return ApplicationResult<RegisterResponse>.Fail(validationResult);
        }

        // 2. Normalize email
        var normalizedEmail = request.Email.ToLowerInvariant();

        // 3. Check if email already exists
        var emailExists = await _userRepository.EmailExistsAsync(normalizedEmail);
        if (emailExists)
        {
            return ApplicationResult<RegisterResponse>.Fail("An account with this email already exists.");
        }

        // 4. Create user entity
        var user = new User
        {
            Email = normalizedEmail,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // 5. Save to database
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        // 6. Return success response
        var response = new RegisterResponse
        {
            Message = "Account created successfully. You can now log in."
        };

        return ApplicationResult<RegisterResponse>.Succeed(response);
    }
}

