using FluentValidation;
using FluentValidation.Results;
using Moq;
using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Application.Accounts.Domain.Repository;
using PagePlay.Site.Application.Accounts.Register;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Tests.Application.Accounts.Register;

public class RegisterWorkflowTests
{
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IValidator<RegisterRequest>> _validatorMock;
    private readonly RegisterWorkflow _workflow;

    public RegisterWorkflowTests()
    {
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _validatorMock = new Mock<IValidator<RegisterRequest>>();

        _workflow = new RegisterWorkflow(
            _passwordHasherMock.Object,
            _userRepositoryMock.Object,
            _validatorMock.Object
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

        _validatorMock
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());

        _userRepositoryMock
            .Setup(r => r.EmailExistsAsync(request.Email))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(h => h.HashPassword(request.Password))
            .Returns("hashed_password");

        _userRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        _userRepositoryMock
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _workflow.Perform(request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Model);
        Assert.Equal("Account created successfully. You can now log in.", result.Model.Message);

        _userRepositoryMock.Verify(r => r.AddAsync(It.Is<User>(u =>
            u.Email == request.Email &&
            u.PasswordHash == "hashed_password"
        )), Times.Once);

        _userRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
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

        _validatorMock
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await _workflow.Perform(request);

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, e => e.Message.Contains("Invalid email format."));
        Assert.Contains(result.Errors, e => e.Message.Contains("Password must be at least 8 characters long."));
        Assert.Contains(result.Errors, e => e.Message.Contains("Passwords do not match."));

        _userRepositoryMock.Verify(r => r.EmailExistsAsync(It.IsAny<string>()), Times.Never);
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
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

        _validatorMock
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());

        _userRepositoryMock
            .Setup(r => r.EmailExistsAsync(request.Email))
            .ReturnsAsync(true);

        // Act
        var result = await _workflow.Perform(request);

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, e => e.Message == "An account with this email already exists.");

        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        _userRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
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

        _validatorMock
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await _workflow.Perform(request);

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, e => e.Message.Contains("Email is required."));
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

        _validatorMock
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await _workflow.Perform(request);

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, e => e.Message.Contains("Password is required."));
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

        _validatorMock
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await _workflow.Perform(request);

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, e => e.Message.Contains("Passwords do not match."));
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

        _validatorMock
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());

        _userRepositoryMock
            .Setup(r => r.EmailExistsAsync(request.Email))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(h => h.HashPassword(request.Password))
            .Returns(hashedPassword);

        _userRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        _userRepositoryMock
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _workflow.Perform(request);

        // Assert
        Assert.True(result.Success);
        _passwordHasherMock.Verify(h => h.HashPassword(request.Password), Times.Once);
        _userRepositoryMock.Verify(r => r.AddAsync(It.Is<User>(u =>
            u.PasswordHash == hashedPassword
        )), Times.Once);
    }
}
