using AwesomeAssertions;
using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Application.Accounts.Domain.Repository;
using PagePlay.Site.Application.Accounts.Login;
using PagePlay.Site.Infrastructure.Security;
using PagePlay.Tests.Infrastructure.TestBases;

namespace PagePlay.Tests.Application.Accounts.Login;

public class LoginWorkflowIntegrationTests : SetupTestFor<LoginWorkflow>
{
    [Fact]
    public async Task Perform_EndToEnd_WithValidCredentials()
    {
        // Arrange
        // TODO: Use Fakes() to replace dependencies with real implementations
        // Example:
        // Fakes()
        //     .Replace<IUserRepository, InMemoryUserRepository>()
        //     .Replace<IPasswordHasher, RealPasswordHasher>()
        //     .Use();

        var request = new LoginRequest
        {
            Email = "integration@example.com",
            Password = "IntegrationPassword123!"
        };

        // TODO: Seed test data
        // Example: Create a user with known credentials in the test database

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Model.Should().NotBeNull();
        result.Model.UserId.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Perform_EndToEnd_WithInvalidCredentials()
    {
        // Arrange
        // TODO: Use Fakes() to replace dependencies with real implementations
        // Fakes()
        //     .Replace<IUserRepository, InMemoryUserRepository>()
        //     .Replace<IPasswordHasher, RealPasswordHasher>()
        //     .Use();

        var request = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "WrongPassword123!"
        };

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message == "Invalid email or password.");
    }

    [Fact]
    public async Task Perform_EndToEnd_ValidatorIntegration()
    {
        // Arrange
        // TODO: Use real validator instead of mock
        // Fakes()
        //     .UseReal<IValidator<LoginRequest>>()
        //     .Use();

        var request = new LoginRequest
        {
            Email = "not-an-email",
            Password = ""
        };

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        // Validator should catch format and empty errors
    }
}
