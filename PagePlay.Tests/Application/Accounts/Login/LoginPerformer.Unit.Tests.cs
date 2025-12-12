using AwesomeAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Application.Accounts.Login;
using PagePlay.Site.Infrastructure.Data.Repositories;
using PagePlay.Site.Infrastructure.Data.Specifications;
using PagePlay.Site.Infrastructure.Security;
using PagePlay.Tests.Infrastructure.TestBases;

namespace PagePlay.Tests.Application.Accounts.Login;

public class LoginPerformerUnitTests : SetupUnitTestFor<LoginPerformer>
{
    [Fact]
    public async Task Perform_WithValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var user = new User
        {
            Id = 123,
            Email = "test@example.com",
            PasswordHash = "hashed_password"
        };

        Mocker
            .GetSubstituteFor<IValidator<LoginRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult());

        Mocker
            .GetSubstituteFor<IRepository>()
            .Get<User>(Arg.Any<Specification<User>>())
            .Returns(user);

        Mocker
            .GetSubstituteFor<IPasswordHasher>()
            .VerifyPassword(request.Password, user.PasswordHash)
            .Returns(true);

        Mocker
            .GetSubstituteFor<IJwtTokenService>()
            .GenerateToken(Arg.Is<TokenClaims>(tc => tc.UserId == user.Id))
            .Returns("test_jwt_token");

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Model.Should().NotBeNull();
        result.Model.UserId.Should().Be(123);
        result.Model.Token.Should().Be("test_jwt_token");

        await Mocker
            .GetSubstituteFor<IRepository>()
            .Received(1)
            .Get<User>(Arg.Any<Specification<User>>());

        Mocker
            .GetSubstituteFor<IPasswordHasher>()
            .Received(1)
            .VerifyPassword(request.Password, user.PasswordHash);

        Mocker
            .GetSubstituteFor<IJwtTokenService>()
            .Received(1)
            .GenerateToken(Arg.Is<TokenClaims>(tc => tc.UserId == user.Id));
    }

    [Fact]
    public async Task Perform_WithInvalidRequest_ReturnsValidationErrors()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "invalid-email",
            Password = ""
        };

        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Email", "Invalid email format."),
            new ValidationFailure("Password", "Password is required.")
        };

        Mocker
            .GetSubstituteFor<IValidator<LoginRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult(validationFailures));

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Invalid email format."));
        result.Errors.Should().Contain(e => e.Message.Contains("Password is required."));

        await Mocker
            .GetSubstituteFor<IRepository>()
            .DidNotReceive()
            .Get<User>(Arg.Any<Specification<User>>());

        Mocker
            .GetSubstituteFor<IPasswordHasher>()
            .DidNotReceive()
            .VerifyPassword(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Perform_WithNonExistentEmail_ReturnsError()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "Password123!"
        };

        Mocker
            .GetSubstituteFor<IValidator<LoginRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult());

        Mocker
            .GetSubstituteFor<IRepository>()
            .Get<User>(Arg.Any<Specification<User>>())
            .Returns((User)null);

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message == "Invalid email or password.");

        await Mocker
            .GetSubstituteFor<IRepository>()
            .Received(1)
            .Get<User>(Arg.Any<Specification<User>>());

        Mocker
            .GetSubstituteFor<IPasswordHasher>()
            .DidNotReceive()
            .VerifyPassword(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Perform_WithIncorrectPassword_ReturnsError()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "WrongPassword123!"
        };

        var user = new User
        {
            Id = 123,
            Email = "test@example.com",
            PasswordHash = "hashed_password"
        };

        Mocker
            .GetSubstituteFor<IValidator<LoginRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult());

        Mocker
            .GetSubstituteFor<IRepository>()
            .Get<User>(Arg.Any<Specification<User>>())
            .Returns(user);

        Mocker
            .GetSubstituteFor<IPasswordHasher>()
            .VerifyPassword(request.Password, user.PasswordHash)
            .Returns(false);

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message == "Invalid email or password.");

        await Mocker
            .GetSubstituteFor<IRepository>()
            .Received(1)
            .Get<User>(Arg.Any<Specification<User>>());

        Mocker
            .GetSubstituteFor<IPasswordHasher>()
            .Received(1)
            .VerifyPassword(request.Password, user.PasswordHash);
    }

    [Fact]
    public async Task Perform_WithEmptyEmail_ReturnsValidationError()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "",
            Password = "Password123!"
        };

        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Email", "Email is required.")
        };

        Mocker
            .GetSubstituteFor<IValidator<LoginRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult(validationFailures));

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Email is required."));

        await Mocker
            .GetSubstituteFor<IRepository>()
            .DidNotReceive()
            .Get<User>(Arg.Any<Specification<User>>());
    }

    [Fact]
    public async Task Perform_WithEmptyPassword_ReturnsValidationError()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = ""
        };

        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Password", "Password is required.")
        };

        Mocker
            .GetSubstituteFor<IValidator<LoginRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult(validationFailures));

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Password is required."));

        await Mocker
            .GetSubstituteFor<IRepository>()
            .DidNotReceive()
            .Get<User>(Arg.Any<Specification<User>>());
    }

    [Fact]
    public async Task Perform_VerifiesPasswordWithCorrectHash()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "MySecurePassword123!"
        };

        var expectedHash = "expected_secure_hash";
        var user = new User
        {
            Id = 789,
            Email = "test@example.com",
            PasswordHash = expectedHash
        };

        Mocker
            .GetSubstituteFor<IValidator<LoginRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult());

        Mocker
            .GetSubstituteFor<IRepository>()
            .Get<User>(Arg.Any<Specification<User>>())
            .Returns(user);

        Mocker
            .GetSubstituteFor<IPasswordHasher>()
            .VerifyPassword(request.Password, expectedHash)
            .Returns(true);

        Mocker
            .GetSubstituteFor<IJwtTokenService>()
            .GenerateToken(Arg.Is<TokenClaims>(tc => tc.UserId == user.Id))
            .Returns("jwt_token_for_user_789");

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Model.UserId.Should().Be(789);

        Mocker
            .GetSubstituteFor<IPasswordHasher>()
            .Received(1)
            .VerifyPassword(request.Password, expectedHash);
    }

    [Fact]
    public async Task Perform_GeneratesJwtTokenOnSuccess()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "user@example.com",
            Password = "SecurePassword123!"
        };

        var user = new User
        {
            Id = 999,
            Email = "user@example.com",
            PasswordHash = "hashed_password"
        };

        var expectedToken = "expected_jwt_token_value";

        Mocker
            .GetSubstituteFor<IValidator<LoginRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult());

        Mocker
            .GetSubstituteFor<IRepository>()
            .Get<User>(Arg.Any<Specification<User>>())
            .Returns(user);

        Mocker
            .GetSubstituteFor<IPasswordHasher>()
            .VerifyPassword(request.Password, user.PasswordHash)
            .Returns(true);

        Mocker
            .GetSubstituteFor<IJwtTokenService>()
            .GenerateToken(Arg.Is<TokenClaims>(tc => tc.UserId == user.Id))
            .Returns(expectedToken);

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Model.Token.Should().Be(expectedToken);

        Mocker
            .GetSubstituteFor<IJwtTokenService>()
            .Received(1)
            .GenerateToken(Arg.Is<TokenClaims>(tc => tc.UserId == user.Id));
    }
}
