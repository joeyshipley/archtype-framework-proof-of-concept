using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Application.Accounts.Domain.Repository;
using PagePlay.Site.Infrastructure.Database;
using PagePlay.Tests.Infrastructure.Database;
using PagePlay.Tests.Infrastructure.TestBases;
using static PagePlay.Site.Application.Accounts.Domain.Repository.UserSpecifications;

namespace PagePlay.Tests.Application.Accounts;

public class UserRepositoryIntegrationTests : SetupIntegrationTestFor<IUserRepository>
{
    public UserRepositoryIntegrationTests() 
    {
        Fakes()
            .Replace<IDbContextFactory<AppDbContext>, TestDbContextFactory>()
            .Use();
    }
    
    [Fact]
    public async Task GetByEmailAsync_WhenUserExists_ReturnsUser()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            PasswordHash = "hashed_password",
            CreatedAt = DateTime.UtcNow
        };

        await SUT.Add(user);
        await SUT.SaveChanges();

        // Act
        var result = await SUT.Get(ByEmail("test@example.com"));

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        // Arrange
        // Act
        var result = await SUT.Get(ByEmail("nonexistent@example.com"));

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task EmailExistsAsync_WhenUserExists_ReturnsTrue()
    {
        // Arrange
        var user = new User
        {
            Email = "existing@example.com",
            PasswordHash = "hashed_password",
            CreatedAt = DateTime.UtcNow
        };

        await SUT.Add(user);
        await SUT.SaveChanges();

        // Act
        var exists = await SUT.EmailExists("existing@example.com");

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task EmailExistsAsync_WhenUserDoesNotExist_ReturnsFalse()
    {
        // Arrange
        // Act
        var exists = await SUT.EmailExists("nonexistent@example.com");

        // Assert
        exists.Should().BeFalse();
    }
}
