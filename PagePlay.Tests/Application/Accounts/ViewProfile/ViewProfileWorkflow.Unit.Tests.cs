using AwesomeAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Application.Accounts.ViewProfile;
using PagePlay.Site.Infrastructure.Data.Repositories;
using PagePlay.Site.Infrastructure.Data.Specifications;
using PagePlay.Site.Infrastructure.Security;
using PagePlay.Tests.Infrastructure.TestBases;

namespace PagePlay.Tests.Application.Accounts.ViewProfile;

public class ViewProfileWorkflowUnitTests : SetupUnitTestFor<ViewProfileWorkflow>
{
    [Fact]
    public async Task Perform_WithAuthenticatedUser_ReturnsUserProfile()
    {
        // Arrange
        var request = new ViewProfileWorkflowRequest();
        var userId = 123L;
        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            PasswordHash = "hashed_password",
            CreatedAt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc)
        };

        Mocker
            .GetSubstituteFor<IValidator<ViewProfileWorkflowRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult());

        Mocker
            .GetSubstituteFor<LoggedInAuthContext>()
            .UserId
            .Returns(userId);

        Mocker
            .GetSubstituteFor<IRepository>()
            .Get<User>(Arg.Any<Specification<User>>())
            .Returns(user);

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Model.Should().NotBeNull();
        result.Model.Email.Should().Be("test@example.com");
        result.Model.CreatedAt.Should().Be(new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc));

        await Mocker
            .GetSubstituteFor<IRepository>()
            .Received(1)
            .Get<User>(Arg.Any<Specification<User>>());
    }

    [Fact]
    public async Task Perform_WithInvalidRequest_ReturnsValidationErrors()
    {
        // Arrange
        var request = new ViewProfileWorkflowRequest();

        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Request", "Invalid request.")
        };

        Mocker
            .GetSubstituteFor<IValidator<ViewProfileWorkflowRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult(validationFailures));

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Invalid request."));

        await Mocker
            .GetSubstituteFor<IRepository>()
            .DidNotReceive()
            .Get<User>(Arg.Any<Specification<User>>());
    }

    [Fact]
    public async Task Perform_WithNonExistentUser_ReturnsUserNotFoundError()
    {
        // Arrange
        var request = new ViewProfileWorkflowRequest();
        var userId = 999L;

        Mocker
            .GetSubstituteFor<IValidator<ViewProfileWorkflowRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult());

        Mocker
            .GetSubstituteFor<LoggedInAuthContext>()
            .UserId
            .Returns(userId);

        Mocker
            .GetSubstituteFor<IRepository>()
            .Get<User>(Arg.Any<Specification<User>>())
            .Returns((User)null);

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message == "User not found.");

        await Mocker
            .GetSubstituteFor<IRepository>()
            .Received(1)
            .Get<User>(Arg.Any<Specification<User>>());
    }

    [Fact]
    public async Task Perform_UsesCorrectUserIdFromAuthContext()
    {
        // Arrange
        var request = new ViewProfileWorkflowRequest();
        var expectedUserId = 456L;
        var user = new User
        {
            Id = expectedUserId,
            Email = "specific@example.com",
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow
        };

        Mocker
            .GetSubstituteFor<IValidator<ViewProfileWorkflowRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult());

        Mocker
            .GetSubstituteFor<LoggedInAuthContext>()
            .UserId
            .Returns(expectedUserId);

        Mocker
            .GetSubstituteFor<IRepository>()
            .Get<User>(Arg.Any<Specification<User>>())
            .Returns(user);

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Model.Email.Should().Be("specific@example.com");

        await Mocker
            .GetSubstituteFor<IRepository>()
            .Received(1)
            .Get<User>(Arg.Any<Specification<User>>());
    }

    [Fact]
    public async Task Perform_ReturnsCorrectCreatedAtDate()
    {
        // Arrange
        var request = new ViewProfileWorkflowRequest();
        var userId = 789L;
        var expectedCreatedAt = new DateTime(2023, 5, 20, 14, 30, 45, DateTimeKind.Utc);
        var user = new User
        {
            Id = userId,
            Email = "date-test@example.com",
            PasswordHash = "hash",
            CreatedAt = expectedCreatedAt
        };

        Mocker
            .GetSubstituteFor<IValidator<ViewProfileWorkflowRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult());

        Mocker
            .GetSubstituteFor<LoggedInAuthContext>()
            .UserId
            .Returns(userId);

        Mocker
            .GetSubstituteFor<IRepository>()
            .Get<User>(Arg.Any<Specification<User>>())
            .Returns(user);

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Model.CreatedAt.Should().Be(expectedCreatedAt);
    }
}
