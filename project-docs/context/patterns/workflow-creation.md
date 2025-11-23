# Workflow Creation Pattern

This document describes how to create a new workflow following the vertical slice architecture pattern.

---

## When to Use

Use this pattern when creating a new feature that requires:
- HTTP endpoint
- Request/response contracts
- Business logic workflow
- Validation

Examples: `RegisterUser`, `UpdateProfile`, `CreateProject`, `DeleteAccount`

---

## Context Needed

- Feature name (e.g., "UpdateProfile", "CreateProject")
- Domain area (e.g., "Accounts", "Projects")

---

## Steps

### 1. Determine folder structure

**Site location:** `PagePlay.Site/Application/{Domain}/{Feature}/`
**Test location:** `PagePlay.Tests/Application/{Domain}/{Feature}/`

Example:
- Feature: `UpdateProfile`
- Domain: `Accounts`
- Site: `PagePlay.Site/Application/Accounts/UpdateProfile/`
- Test: `PagePlay.Tests/Application/Accounts/UpdateProfile/`

### 2. Check what files already exist

Use Glob to find existing files in both locations:
- Check if `{Feature}.BoundaryContracts.cs` exists
- Check if `{Feature}.Workflow.cs` exists
- Check if `{Feature}.Endpoint.cs` exists

Only create files that don't already exist.

### 3. Create the vertical slice files

Each feature consists of 3 files in the same folder:

#### A. {Feature}.BoundaryContracts.cs

Contains Request, Response, and Validator classes.

```csharp
using FluentValidation;
using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Application.{Domain}.{Feature};

public class {Feature}Response : IResponse
{
    // TODO: Add response properties
}

public class {Feature}Request : IRequest
{
    // TODO: Add request properties
}

public class {Feature}RequestValidator : AbstractValidator<{Feature}Request>
{
    public {Feature}RequestValidator()
    {
        // TODO: Add validation rules
        // Example:
        // RuleFor(x => x.Email)
        //     .NotEmpty().WithMessage("Email is required.")
        //     .EmailAddress().WithMessage("Email must be valid.");
    }
}
```

#### B. {Feature}.Workflow.cs

Contains business logic following the Revealing Intent Class Structure pattern.

```csharp
using FluentValidation;
using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Application.{Domain}.{Feature};

public class {Feature}Workflow(
    IValidator<{Feature}Request> _validator
) : IWorkflow<{Feature}Request, {Feature}Response>
{
    public async Task<IApplicationResult<{Feature}Response>> Perform({Feature}Request request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return response(validationResult);

        // TODO: Implement business logic
        // Example steps:
        // 1. Retrieve necessary data from repositories
        // 2. Execute business logic
        // 3. Persist changes
        // 4. Return success response

        return response();
    }

    private async Task<FluentValidation.Results.ValidationResult> validate({Feature}Request request) =>
        await _validator.ValidateAsync(request);

    private IApplicationResult<{Feature}Response> response(FluentValidation.Results.ValidationResult validationResult) =>
        ApplicationResult<{Feature}Response>.Fail(validationResult);

    private IApplicationResult<{Feature}Response> response(string errorMessage) =>
        ApplicationResult<{Feature}Response>.Fail(errorMessage);

    private IApplicationResult<{Feature}Response> response()
    {
        // TODO: Build success response
        return ApplicationResult<{Feature}Response>.Succeed(
            new {Feature}Response { }
        );
    }
}
```

**Key workflow principles:**
- Public `Perform()` method reveals business intent (reads like a story)
- Private helper methods hide implementation details
- Use lower camel case for private methods (e.g., `validate`, `checkEmailExists`)
- Multiple `response()` overloads handle different result types
- Follow Revealing Intent Class Structure pattern (see `.claude/docs/README.CONSISTENT_COMPLEXITY_DESIGN.md`)

#### C. {Feature}.Endpoint.cs

Contains HTTP endpoint registration.

```csharp
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Routing;

namespace PagePlay.Site.Application.{Domain}.{Feature};

public class {Feature}Endpoint : I{Domain}Endpoint
{
    public void Map(IEndpointRouteBuilder endpoints) =>
        endpoints.Register<{Feature}Response>("/{endpoint-route}", handle);

    private async Task<IResult> handle(
        {Feature}Request request,
        IWorkflow<{Feature}Request, {Feature}Response> workflow
    ) => Respond.With(await workflow.Perform(request));
}
```

**Key points:**
- No constructor needed - endpoints have no dependencies
- Workflow injected as handler method parameter (resolved per-request)
- Request must be first parameter, workflow second parameter
- Replace `{endpoint-route}` with the actual HTTP route (e.g., `/accounts/update-profile`)

**For authenticated endpoints:**
```csharp
public void Map(IEndpointRouteBuilder endpoints) =>
    endpoints.Register<{Feature}Response>("/{endpoint-route}", handle)
        .RequireAuthenticatedUser();  // Populates LoggedInAuthContext
```

### 4. Add dependencies to workflow (if needed)

If your workflow needs repositories or other services, add them to the primary constructor:

```csharp
public class {Feature}Workflow(
    IValidator<{Feature}Request> _validator,
    IUserRepository _userRepository,
    IPasswordHasher _passwordHasher
) : IWorkflow<{Feature}Request, {Feature}Response>
{
    // Constructor parameters automatically become fields with _ prefix
    // Use them directly: _userRepository.GetByIdAsync(userId)
}
```

**For authenticated workflows:**

If your endpoint uses `.RequireAuthenticatedUser()`, inject `LoggedInAuthContext` to access the authenticated user's ID:

```csharp
using PagePlay.Site.Infrastructure.Security;

public class {Feature}Workflow(
    IValidator<{Feature}Request> _validator,
    LoggedInAuthContext _authContext,  // Injected by framework
    IUserRepository _userRepository
) : IWorkflow<{Feature}Request, {Feature}Response>
{
    public async Task<IApplicationResult<{Feature}Response>> Perform({Feature}Request request)
    {
        var userId = _authContext.UserId;  // Non-nullable, guaranteed populated
        var user = await _userRepository.GetById(userId);
        // ... rest of workflow
    }
}
```

**Key points:**
- `LoggedInAuthContext.UserId` is guaranteed to be populated by the filter
- No null checks needed - if workflow executes, user is authenticated
- Framework populates context before workflow runs

### 5. Provide next steps

After creating the workflow files:

1. **Tell user which files were created**
   - List each file with its full path

2. **Remind them to fill in TODOs:**
   - Add request/response properties
   - Add validation rules
   - Implement business logic
   - Set endpoint route

3. **Suggest creating tests:**
   - "Next, create test files by reading `.claude/docs/patterns/test-creation.md`"
   - Or use `/test-create` command for convenience

4. **Note automatic registration:**
   - Workflows are automatically registered via `AutoRegisterWorkflows()` in DependencyResolver
   - No manual DI registration needed

---

## Important Notes

### Architecture Patterns:
- **Vertical slice:** One feature = one folder with all related files
- **Single responsibility:** Each workflow does ONE operation (CreateProject, not ProjectManager)
- **Primary constructors:** Use C# 12+ syntax with `_` prefix for dependencies
- **Revealing Intent:** Workflow `Perform()` method reads like business process story
- **Lower camel case:** Private methods use `lowerCamelCase` (see `.claude/docs/README.SYNTAX_STYLE.md`)

### File Organization:
- All 3 files go in the same feature folder
- Feature folder name matches the operation (e.g., `UpdateProfile`, not `Profile`)
- Test files mirror the structure in test project

### Dependency Injection:
- Workflows automatically registered via `AutoRegisterWorkflows()`
- Validators automatically registered via `AddValidatorsFromAssemblyContaining<>`
- No manual registration needed in most cases

### Next Steps:
- After creating workflow, create tests using test-creation.md pattern
- Fill in all TODO comments
- Run tests to verify scaffolding works
- Implement business logic iteratively with TDD

---

## Example: Complete UpdateProfile Workflow

**File structure:**
```
PagePlay.Site/Application/Accounts/UpdateProfile/
├── UpdateProfile.BoundaryContracts.cs
├── UpdateProfile.Workflow.cs
└── UpdateProfile.Endpoint.cs
```

**UpdateProfile.BoundaryContracts.cs:**
```csharp
using FluentValidation;
using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Application.Accounts.UpdateProfile;

public class UpdateProfileResponse : IResponse
{
    public string Message { get; set; } = string.Empty;
}

public class UpdateProfileRequest : IRequest
{
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be valid.");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Display name is required.")
            .MaximumLength(100).WithMessage("Display name must be 100 characters or less.");
    }
}
```

This example shows the complete pattern in action.
