using AwesomeAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Application.Accounts.Domain.Repository;
using PagePlay.Site.Application.Accounts.Register;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Tests.Application.Accounts.Register;

public class RegisterWorkflowTests
{
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IValidator<RegisterRequest> _validator = Substitute.For<IValidator<RegisterRequest>>();
    private readonly RegisterWorkflow _workflow;

    public RegisterWorkflowTests()
    {
        _workflow = new RegisterWorkflow(
            _passwordHasher,
            _userRepository,
            _validator
        );
    }

    [Fact]
    public async Task Perform_WithValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        _validator
            .ValidateAsync(request, default)
            .Returns(new ValidationResult());

        _userRepository
            .EmailExists(request.Email)
            .Returns(false);

        _passwordHasher
            .HashPassword(request.Password)
            .Returns("hashed_password");

        _userRepository
            .Add(Arg.Any<User>())
            .Returns(callInfo => callInfo.Arg<User>());

        _userRepository
            .SaveChanges()
            .Returns(Task.CompletedTask);

        // Act
        var result = await _workflow.Perform(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Model.Should().NotBeNull();
        result.Model.Message.Should().Be("Account created successfully. You can now log in.");

        await _userRepository.Received(1).Add(Arg.Is<User>(u =>
            u.Email == request.Email &&
            u.PasswordHash == "hashed_password"
        ));

        await _userRepository.Received(1).SaveChanges();
    }

    [Fact]
    public async Task Perform_WithInvalidRequest_ReturnsValidationErrors()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "invalid-email",
            Password = "short",
            ConfirmPassword = "different"
        };

        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Email", "Invalid email format."),
            new ValidationFailure("Password", "Password must be at least 8 characters long."),
            new ValidationFailure("ConfirmPassword", "Passwords do not match.")
        };

        _validator
            .ValidateAsync(request, default)
            .Returns(new ValidationResult(validationFailures));

        // Act
        var result = await _workflow.Perform(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Invalid email format."));
        result.Errors.Should().Contain(e => e.Message.Contains("Password must be at least 8 characters long."));
        result.Errors.Should().Contain(e => e.Message.Contains("Passwords do not match."));

        await _userRepository.DidNotReceive().EmailExists(Arg.Any<string>());
        await _userRepository.DidNotReceive().Add(Arg.Any<User>());
    }

    [Fact]
    public async Task Perform_WithExistingEmail_ReturnsError()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "existing@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        _validator
            .ValidateAsync(request, default)
            .Returns(new ValidationResult());

        _userRepository
            .EmailExists(request.Email)
            .Returns(true);

        // Act
        var result = await _workflow.Perform(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message == "An account with this email already exists.");

        await _userRepository.DidNotReceive().Add(Arg.Any<User>());
        await _userRepository.DidNotReceive().SaveChanges();
    }

    [Fact]
    public async Task Perform_WithEmptyEmail_ReturnsValidationError()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Email", "Email is required.")
        };

        _validator
            .ValidateAsync(request, default)
            .Returns(new ValidationResult(validationFailures));

        // Act
        var result = await _workflow.Perform(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Email is required."));
    }

    [Fact]
    public async Task Perform_WithEmptyPassword_ReturnsValidationError()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "",
            ConfirmPassword = ""
        };

        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Password", "Password is required.")
        };

        _validator
            .ValidateAsync(request, default)
            .Returns(new ValidationResult(validationFailures));

        // Act
        var result = await _workflow.Perform(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Password is required."));
    }

    [Fact]
    public async Task Perform_PasswordsDoNotMatch_ReturnsValidationError()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            ConfirmPassword = "DifferentPassword123!"
        };

        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("ConfirmPassword", "Passwords do not match.")
        };

        _validator
            .ValidateAsync(request, default)
            .Returns(new ValidationResult(validationFailures));

        // Act
        var result = await _workflow.Perform(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Passwords do not match."));
    }

    [Fact]
    public async Task Perform_HashesPasswordCorrectly()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "MySecurePassword123!",
            ConfirmPassword = "MySecurePassword123!"
        };

        var hashedPassword = "securely_hashed_password_value";

        _validator
            .ValidateAsync(request, default)
            .Returns(new ValidationResult());

        _userRepository
            .EmailExists(request.Email)
            .Returns(false);

        _passwordHasher
            .HashPassword(request.Password)
            .Returns(hashedPassword);

        _userRepository
            .Add(Arg.Any<User>())
            .Returns(callInfo => callInfo.Arg<User>());

        _userRepository
            .SaveChanges()
            .Returns(Task.CompletedTask);

        // Act
        var result = await _workflow.Perform(request);

        // Assert
        result.Success.Should().BeTrue();
        _passwordHasher.Received(1).HashPassword(request.Password);
        await _userRepository.Received(1).Add(Arg.Is<User>(u =>
            u.PasswordHash == hashedPassword
        ));
    }
}
