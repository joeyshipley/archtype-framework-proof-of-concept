using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Application.Accounts.Domain.Repository;
using PagePlay.Site.Infrastructure.Database;
using PagePlay.Tests.Infrastructure.Database;
using PagePlay.Tests.Infrastructure.TestBases;

namespace PagePlay.Tests.Application.Accounts;

public class UserRepositoryIntegrationTests : SetupIntegrationTestFor<IUserRepository>
{
    [Fact]
    public async Task GetByEmailAsync_WhenUserExists_ReturnsUser()
    {
        // Arrange
        Fakes()
            .Replace<IDbContextFactory<AppDbContext>, TestDbContextFactory>()
            .Use();

        var user = new User
        {
            Email = "test@example.com",
            PasswordHash = "hashed_password",
            CreatedAt = DateTime.UtcNow
        };

        await SUT.Add(user);
        await SUT.SaveChanges();

        // Act
        var result = await SUT.GetByEmail("test@example.com");

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        // Arrange
        Fakes()
            .Replace<IDbContextFactory<AppDbContext>, TestDbContextFactory>()
            .Use();

        // Act
        var result = await SUT.GetByEmail("nonexistent@example.com");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task EmailExistsAsync_WhenUserExists_ReturnsTrue()
    {
        // Arrange
        Fakes()
            .Replace<IDbContextFactory<AppDbContext>, TestDbContextFactory>()
            .Use();

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
        Fakes()
            .Replace<IDbContextFactory<AppDbContext>, TestDbContextFactory>()
            .Use();

        // Act
        var exists = await SUT.EmailExists("nonexistent@example.com");

        // Assert
        exists.Should().BeFalse();
    }
}
