You are helping the user create test files for a workflow following the consistent testing pattern.

## User will provide:
- Workflow name (e.g., "RegisterWorkflow", "UpdateProfileWorkflow")
- Domain area (e.g., "Accounts", "Projects")

## Your tasks:

1. **Determine test folder structure:**
   - Test location: `PagePlay.Tests/Application/{Domain}/{Feature}/`
   - Tests are siblings: `{Feature}.Unit.Tests.cs` and `{Feature}.Integration.Tests.cs`

2. **Check what files already exist:**
   - Use Glob to find existing test files in the location
   - Identify which test files need to be created (unit, integration, or both)

3. **Find the workflow file to understand dependencies:**
   - Read `PagePlay.Site/Application/{Domain}/{Feature}/{Feature}.Workflow.cs`
   - Identify dependencies in the primary constructor
   - Understand the request/response types

4. **Create test files:**

   **A. {Feature}.Unit.Tests.cs** (if not exists):
   ```csharp
   using AwesomeAssertions;
   using FluentValidation;
   using FluentValidation.Results;
   using NSubstitute;
   using PagePlay.Site.Application.{Domain}.{Feature};
   using PagePlay.Tests.Infrastructure.TestBases;

   namespace PagePlay.Tests.Application.{Domain}.{Feature};

   public class {Feature}UnitTests : SetupTestFor<{Feature}Workflow>
   {
       [Fact]
       public async Task Perform_WithValidRequest_ReturnsSuccess()
       {
           // Arrange
           var request = new {Feature}Request
           {
               // TODO: Set request properties
           };

           Mocker
               .GetSubstituteFor<IValidator<{Feature}Request>>()
               .ValidateAsync(request, default)
               .Returns(new ValidationResult());

           // TODO: Mock other dependencies as needed
           // Example:
           // Mocker
           //     .GetSubstituteFor<ISomeDependency>()
           //     .SomeMethod(Arg.Any<string>())
           //     .Returns(expectedValue);

           // Act
           var result = await SUT.Perform(request);

           // Assert
           result.Success.Should().BeTrue();
           result.Model.Should().NotBeNull();
           // TODO: Add more specific assertions
       }

       [Fact]
       public async Task Perform_WithInvalidRequest_ReturnsValidationErrors()
       {
           // Arrange
           var request = new {Feature}Request
           {
               // TODO: Set invalid request properties
           };

           var validationFailures = new List<ValidationFailure>
           {
               new ValidationFailure("PropertyName", "Error message.")
           };

           Mocker
               .GetSubstituteFor<IValidator<{Feature}Request>>()
               .ValidateAsync(request, default)
               .Returns(new ValidationResult(validationFailures));

           // Act
           var result = await SUT.Perform(request);

           // Assert
           result.Success.Should().BeFalse();
           result.Errors.Should().NotBeEmpty();
           result.Errors.Should().Contain(e => e.Message.Contains("Error message."));
       }

       // TODO: Add more test cases for different scenarios
   }
   ```

   **B. {Feature}.Integration.Tests.cs** (if not exists):
   ```csharp
   using AwesomeAssertions;
   using PagePlay.Site.Application.{Domain}.{Feature};
   using PagePlay.Tests.Infrastructure.TestBases;

   namespace PagePlay.Tests.Application.{Domain}.{Feature};

   public class {Feature}IntegrationTests : SetupTestFor<{Feature}Workflow>
   {
       [Fact]
       public async Task Perform_EndToEnd_WithFakeImplementations()
       {
           // Arrange
           // TODO: Use Fakes() to replace dependencies with real implementations
           // Example:
           // Fakes()
           //     .Replace<IRepository, InMemoryRepository>()
           //     .Use();

           var request = new {Feature}Request
           {
               // TODO: Set request properties
           };

           // Act
           var result = await SUT.Perform(request);

           // Assert
           result.Success.Should().BeTrue();
           // TODO: Add assertions for integration behavior
       }

       // TODO: Add more integration test cases
   }
   ```

5. **Provide guidance:**
   - Tell user which test files were created
   - List the dependencies found in the workflow (so they know what to mock)
   - Remind them to:
     - Fill in TODO comments with actual test data
     - Add more test cases for edge cases
     - Mock all dependencies identified in the workflow
     - Use `Mocker.GetSubstituteFor<T>()` for unit tests
     - Use `Fakes().Replace<TInterface, TImplementation>()` for integration tests

## Important notes:
- All tests inherit from `SetupTestFor<T>`
- Unit tests use `Mocker.GetSubstituteFor<T>()` for mocking
- Integration tests use `Fakes().Replace<TInterface, TImpl>().Use()` for fake injection
- Test files are siblings in the same folder: `{Feature}.Unit.Tests.cs` and `{Feature}.Integration.Tests.cs`
- Follow AAA pattern: Arrange, Act, Assert
- Use AwesomeAssertions for fluent assertions (`.Should()` syntax)
- Keep TODO comments so user knows what to customize
