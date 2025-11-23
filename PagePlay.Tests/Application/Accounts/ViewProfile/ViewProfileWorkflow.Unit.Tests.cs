using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AwesomeAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Application.Accounts.Domain.Repository;
using PagePlay.Site.Application.Accounts.ViewProfile;
using PagePlay.Tests.Infrastructure.TestBases;

namespace PagePlay.Tests.Application.Accounts.ViewProfile;

public class ViewProfileWorkflowUnitTests : SetupTestFor<ViewProfileWorkflow>
{
    [Fact]
    public async Task Perform_WithAuthenticatedUser_ReturnsUserProfile()
    {
        // Arrange
        var request = new ViewProfileRequest();
        var userId = 123L;
        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            PasswordHash = "hashed_password",
            CreatedAt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc)
        };

        Mocker
            .GetSubstituteFor<IValidator<ViewProfileRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult());

        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())
        }));

        Mocker
            .GetSubstituteFor<IHttpContextAccessor>()
            .HttpContext
            .Returns(httpContext);

        Mocker
            .GetSubstituteFor<IUserRepository>()
            .GetById(userId)
            .Returns(user);

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Model.Should().NotBeNull();
        result.Model.Email.Should().Be("test@example.com");
        result.Model.CreatedAt.Should().Be(new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc));

        await Mocker
            .GetSubstituteFor<IUserRepository>()
            .Received(1)
            .GetById(userId);
    }

    [Fact]
    public async Task Perform_WithInvalidRequest_ReturnsValidationErrors()
    {
        // Arrange
        var request = new ViewProfileRequest();

        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Request", "Invalid request.")
        };

        Mocker
            .GetSubstituteFor<IValidator<ViewProfileRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult(validationFailures));

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Invalid request."));

        await Mocker
            .GetSubstituteFor<IUserRepository>()
            .DidNotReceive()
            .GetById(Arg.Any<long>());
    }

    [Fact]
    public async Task Perform_WithNoHttpContext_ReturnsAuthenticationError()
    {
        // Arrange
        var request = new ViewProfileRequest();

        Mocker
            .GetSubstituteFor<IValidator<ViewProfileRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult());

        Mocker
            .GetSubstituteFor<IHttpContextAccessor>()
            .HttpContext
            .Returns((HttpContext)null);

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message == "User not authenticated.");

        await Mocker
            .GetSubstituteFor<IUserRepository>()
            .DidNotReceive()
            .GetById(Arg.Any<long>());
    }

    [Fact]
    public async Task Perform_WithMissingUserIdClaim_ReturnsAuthenticationError()
    {
        // Arrange
        var request = new ViewProfileRequest();

        Mocker
            .GetSubstituteFor<IValidator<ViewProfileRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult());

        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("SomeOtherClaim", "value")
        }));

        Mocker
            .GetSubstituteFor<IHttpContextAccessor>()
            .HttpContext
            .Returns(httpContext);

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message == "User not authenticated.");

        await Mocker
            .GetSubstituteFor<IUserRepository>()
            .DidNotReceive()
            .GetById(Arg.Any<long>());
    }

    [Fact]
    public async Task Perform_WithInvalidUserIdClaim_ReturnsAuthenticationError()
    {
        // Arrange
        var request = new ViewProfileRequest();

        Mocker
            .GetSubstituteFor<IValidator<ViewProfileRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult());

        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "not-a-number")
        }));

        Mocker
            .GetSubstituteFor<IHttpContextAccessor>()
            .HttpContext
            .Returns(httpContext);

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message == "User not authenticated.");

        await Mocker
            .GetSubstituteFor<IUserRepository>()
            .DidNotReceive()
            .GetById(Arg.Any<long>());
    }

    [Fact]
    public async Task Perform_WithNonExistentUser_ReturnsUserNotFoundError()
    {
        // Arrange
        var request = new ViewProfileRequest();
        var userId = 999L;

        Mocker
            .GetSubstituteFor<IValidator<ViewProfileRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult());

        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())
        }));

        Mocker
            .GetSubstituteFor<IHttpContextAccessor>()
            .HttpContext
            .Returns(httpContext);

        Mocker
            .GetSubstituteFor<IUserRepository>()
            .GetById(userId)
            .Returns((User)null);

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message == "User not found.");

        await Mocker
            .GetSubstituteFor<IUserRepository>()
            .Received(1)
            .GetById(userId);
    }

    [Fact]
    public async Task Perform_ExtractsCorrectUserIdFromJwtClaim()
    {
        // Arrange
        var request = new ViewProfileRequest();
        var expectedUserId = 456L;
        var user = new User
        {
            Id = expectedUserId,
            Email = "specific@example.com",
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow
        };

        Mocker
            .GetSubstituteFor<IValidator<ViewProfileRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult());

        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, expectedUserId.ToString())
        }));

        Mocker
            .GetSubstituteFor<IHttpContextAccessor>()
            .HttpContext
            .Returns(httpContext);

        Mocker
            .GetSubstituteFor<IUserRepository>()
            .GetById(expectedUserId)
            .Returns(user);

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Model.Email.Should().Be("specific@example.com");

        await Mocker
            .GetSubstituteFor<IUserRepository>()
            .Received(1)
            .GetById(expectedUserId);
    }

    [Fact]
    public async Task Perform_ReturnsCorrectCreatedAtDate()
    {
        // Arrange
        var request = new ViewProfileRequest();
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
            .GetSubstituteFor<IValidator<ViewProfileRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult());

        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())
        }));

        Mocker
            .GetSubstituteFor<IHttpContextAccessor>()
            .HttpContext
            .Returns(httpContext);

        Mocker
            .GetSubstituteFor<IUserRepository>()
            .GetById(userId)
            .Returns(user);

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Model.CreatedAt.Should().Be(expectedCreatedAt);
    }
}
