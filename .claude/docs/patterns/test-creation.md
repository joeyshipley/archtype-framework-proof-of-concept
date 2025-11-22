# Test Creation Pattern

This document describes how to create test files following the consistent testing pattern using `SetupTestFor<T>` base class.

---

## Two Test Patterns

### Pattern A: Testing Workflows
Use when testing a workflow class that implements `IWorkflow<TRequest, TResponse>`.

### Pattern B: Testing Standalone Components
Use when testing utilities, repositories, domain models, validators, or other non-workflow components.

Both patterns use the same `SetupTestFor<T>` infrastructure—only the target and test structure differ.

---

## Pattern A: Testing Workflows

### When to Use
- Testing a workflow class (e.g., `RegisterWorkflow`, `UpdateProfileWorkflow`)
- Workflow implements `IWorkflow<TRequest, TResponse>`
- Tests focus on the `Perform()` method

### Context Needed
- Workflow name (e.g., "RegisterWorkflow", "UpdateProfileWorkflow")
- Domain area (e.g., "Accounts", "Projects")

### Steps

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

---

## Pattern B: Testing Standalone Components

### When to Use
- Testing utilities, repositories, domain models, validators
- Testing any non-workflow component
- Component has public methods that need testing

### Context Needed
- Component name (e.g., "UserRepository", "EmailValidator", "User")
- Component location (namespace/folder path)
- Component type (Repository, Utility, Domain Model, etc.)

### Steps

1. **Determine test folder structure:**
   - Match the source folder structure in test project
   - Example: `PagePlay.Site/Application/Accounts.Domain/Repository/UserRepository.cs`
   - Test location: `PagePlay.Tests/Application/Accounts.Domain/Repository/`
   - Test file: `UserRepository.Unit.Tests.cs`

2. **Check what files already exist:**
   - Use Glob to find existing test files in the location
   - Identify if test file needs to be created

3. **Read the component file to understand:**
   - Public methods that need testing
   - Dependencies in the constructor
   - Return types and behaviors

4. **Create test file:**

   **{Component}.Unit.Tests.cs**:
   ```csharp
   using AwesomeAssertions;
   using NSubstitute;
   using PagePlay.Site.Application.{Namespace}.{Component};
   using PagePlay.Tests.Infrastructure.TestBases;

   namespace PagePlay.Tests.Application.{Namespace};

   public class {Component}UnitTests : SetupTestFor<{Component}>
   {
       [Fact]
       public async Task MethodName_WithValidInput_ReturnsExpectedResult()
       {
           // Arrange
           // TODO: Set up test data
           // TODO: Mock dependencies if needed
           // Example:
           // Mocker
           //     .GetSubstituteFor<IDependency>()
           //     .Method(Arg.Any<string>())
           //     .Returns(expectedValue);

           // Act
           var result = await SUT.MethodName(testInput);

           // Assert
           result.Should().NotBeNull();
           // TODO: Add specific assertions
       }

       [Fact]
       public async Task MethodName_WithInvalidInput_ThrowsException()
       {
           // Arrange
           // TODO: Set up invalid test data

           // Act & Assert
           await Assert.ThrowsAsync<ExpectedException>(async () =>
               await SUT.MethodName(invalidInput)
           );
       }

       // TODO: Add more test cases for different scenarios
   }
   ```

5. **For repositories, consider integration tests:**

   **{Component}.Integration.Tests.cs**:
   ```csharp
   using AwesomeAssertions;
   using PagePlay.Site.Application.{Namespace}.{Component};
   using PagePlay.Tests.Infrastructure.TestBases;

   namespace PagePlay.Tests.Application.{Namespace};

   public class {Component}IntegrationTests : SetupTestFor<{Component}>
   {
       [Fact]
       public async Task MethodName_EndToEnd_WithRealDatabase()
       {
           // Arrange
           // TODO: Use Fakes() to replace dependencies with real implementations
           // Example:
           // Fakes()
           //     .Replace<IDbContext, InMemoryDbContext>()
           //     .Use();

           // TODO: Set up test data

           // Act
           var result = await SUT.MethodName(testInput);

           // Assert
           result.Should().NotBeNull();
           // TODO: Add assertions for integration behavior
       }

       // TODO: Add more integration test cases
   }
   ```

6. **Provide guidance:**
   - Tell user which test file was created
   - List the public methods found in the component
   - List the dependencies found (so they know what to mock)
   - Remind them to:
     - Fill in TODO comments with actual test data
     - Add test cases for each public method
     - Add edge case tests (null inputs, invalid data, etc.)
     - Use `Mocker.GetSubstituteFor<T>()` for mocking dependencies

---

## Important Notes

### For All Tests:
- All tests inherit from `SetupTestFor<T>` where T is the class under test
- `SUT` property gives access to the System Under Test with all dependencies auto-injected
- Unit tests use `Mocker.GetSubstituteFor<T>()` for mocking
- Integration tests use `Fakes().Replace<TInterface, TImplementation>().Use()` for fake injection
- Follow AAA pattern: Arrange, Act, Assert
- Use AwesomeAssertions for fluent assertions (`.Should()` syntax)
- Keep TODO comments so user knows what to customize

### Naming Conventions:
- Test class: `{ComponentName}UnitTests` or `{ComponentName}IntegrationTests`
- Test method: `MethodName_Scenario_ExpectedOutcome`
- Follow project syntax style guide (see `.claude/docs/README.SYNTAX_STYLE.md`)

### File Organization:
- Unit tests and integration tests are siblings in the same folder
- Test folder structure mirrors source folder structure
- One test class per component
