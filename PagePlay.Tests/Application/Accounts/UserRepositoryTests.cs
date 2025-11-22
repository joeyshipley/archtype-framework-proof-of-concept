using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Application.Accounts.Domain.Repository;
using PagePlay.Tests.Infrastructure.Database;

namespace PagePlay.Tests.Application.Accounts;

public class UserRepositoryTests
{
    [Fact]
    public async Task GetByEmailAsync_WhenUserExists_ReturnsUser()
    {
        // Arrange
        var context = new TestAppDbContext();
        var repository = new UserRepository(context);

        var user = new User
        {
            Email = "test@example.com",
            PasswordHash = "hashed_password",
            CreatedAt = DateTime.UtcNow
        };

        await repository.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByEmail("test@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        // Arrange
        var context = new TestAppDbContext();
        var repository = new UserRepository(context);

        // Act
        var result = await repository.GetByEmail("nonexistent@example.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task EmailExistsAsync_WhenUserExists_ReturnsTrue()
    {
        // Arrange
        var context = new TestAppDbContext();
        var repository = new UserRepository(context);

        var user = new User
        {
            Email = "existing@example.com",
            PasswordHash = "hashed_password",
            CreatedAt = DateTime.UtcNow
        };

        await repository.Add(user);
        await context.SaveChangesAsync();

        // Act
        var exists = await repository.EmailExists("existing@example.com");

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task EmailExistsAsync_WhenUserDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var context = new TestAppDbContext();
        var repository = new UserRepository(context);

        // Act
        var exists = await repository.EmailExists("nonexistent@example.com");

        // Assert
        Assert.False(exists);
    }
}
