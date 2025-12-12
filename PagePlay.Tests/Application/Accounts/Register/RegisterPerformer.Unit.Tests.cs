using AwesomeAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Application.Accounts.Register;
using PagePlay.Site.Infrastructure.Data.Repositories;
using PagePlay.Site.Infrastructure.Data.Specifications;
using PagePlay.Site.Infrastructure.Security;
using PagePlay.Tests.Infrastructure.TestBases;

namespace PagePlay.Tests.Application.Accounts.Register;

public class RegisterPerformerUnitTests : SetupUnitTestFor<RegisterPerformer>
{
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

        Mocker
            .GetSubstituteFor<IValidator<RegisterRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult());

        Mocker
            .GetSubstituteFor<IRepository>()
            .Exists<User>(Arg.Any<Specification<User>>())
            .Returns(false);

        Mocker
            .GetSubstituteFor<IPasswordHasher>()
            .HashPassword(request.Password)
            .Returns("hashed_password");

        Mocker
            .GetSubstituteFor<IRepository>()
            .Add<User>(Arg.Any<User>())
            .Returns(callInfo =>
            {
                var user = callInfo.Arg<User>();
                user.Id = 123;
                return user;
            });

        Mocker
            .GetSubstituteFor<IRepository>()
            .SaveChanges()
            .Returns(Task.CompletedTask);

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Model.Should().NotBeNull();
        result.Model.UserId.Should().Be(123);

        await Mocker
            .GetSubstituteFor<IRepository>()
            .Received(1)
            .Add(Arg.Is<User>(u =>
                u.Email == request.Email &&
                u.PasswordHash == "hashed_password"
            ));

        await Mocker
            .GetSubstituteFor<IRepository>()
            .Received(1)
            .SaveChanges();
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

        Mocker
            .GetSubstituteFor<IValidator<RegisterRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult(validationFailures));

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Invalid email format."));
        result.Errors.Should().Contain(e => e.Message.Contains("Password must be at least 8 characters long."));
        result.Errors.Should().Contain(e => e.Message.Contains("Passwords do not match."));

        await Mocker
            .GetSubstituteFor<IRepository>()
            .DidNotReceive()
            .Exists<User>(Arg.Any<Specification<User>>());

        await Mocker
            .GetSubstituteFor<IRepository>()
            .DidNotReceive()
            .Add<User>(Arg.Any<User>());
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

        Mocker
            .GetSubstituteFor<IValidator<RegisterRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult());

        Mocker
            .GetSubstituteFor<IRepository>()
            .Exists<User>(Arg.Any<Specification<User>>())
            .Returns(true);

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message == "An account with this email already exists.");

        await Mocker
            .GetSubstituteFor<IRepository>()
            .DidNotReceive()
            .Add<User>(Arg.Any<User>());

        await Mocker
            .GetSubstituteFor<IRepository>()
            .DidNotReceive()
            .SaveChanges();
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

        Mocker
            .GetSubstituteFor<IValidator<RegisterRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult(validationFailures));

        // Act
        var result = await SUT.Perform(request);

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

        Mocker
            .GetSubstituteFor<IValidator<RegisterRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult(validationFailures));

        // Act
        var result = await SUT.Perform(request);

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

        Mocker
            .GetSubstituteFor<IValidator<RegisterRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult(validationFailures));

        // Act
        var result = await SUT.Perform(request);

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

        Mocker
            .GetSubstituteFor<IValidator<RegisterRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult());

        Mocker
            .GetSubstituteFor<IRepository>()
            .Exists<User>(Arg.Any<Specification<User>>())
            .Returns(false);

        Mocker
            .GetSubstituteFor<IPasswordHasher>()
            .HashPassword(request.Password)
            .Returns(hashedPassword);

        Mocker
            .GetSubstituteFor<IRepository>()
            .Add<User>(Arg.Any<User>())
            .Returns(callInfo =>
            {
                var user = callInfo.Arg<User>();
                user.Id = 456;
                return user;
            });

        Mocker
            .GetSubstituteFor<IRepository>()
            .SaveChanges()
            .Returns(Task.CompletedTask);

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Model.UserId.Should().Be(456);

        Mocker
            .GetSubstituteFor<IPasswordHasher>()
            .Received(1)
            .HashPassword(request.Password);

        await Mocker
            .GetSubstituteFor<IRepository>()
            .Received(1)
            .Add(Arg.Is<User>(u => u.PasswordHash == hashedPassword));
    }
}
