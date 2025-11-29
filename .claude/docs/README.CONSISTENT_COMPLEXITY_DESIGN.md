# Consistent Complexity Design

**Version:** 1.0
**Last Updated:** 2025-11-22
**Status:** Foundation Document

---

## Core Principle

**Every vertical slice should have similar complexity.** When features vary wildly in implementation complexity, it indicates architectural problems—either the framework is missing abstractions, or the feature is too large and should be decomposed.

---

## What is Consistent Complexity?

In a well-designed system, adding a new feature should feel similar to adding the previous feature. The code structure, patterns, and amount of work should be predictable.

**Good Signs:**
- New CRUD feature takes ~2 hours, just like the last one
- Each workflow follows the same structure: validation → business logic → persistence
- Repository usage is identical across features
- Most files are 50-200 lines

**Bad Signs:**
- One feature is 50 lines, another is 500 lines
- Some features require custom infrastructure, others don't
- Copy-pasting code because abstraction doesn't fit
- "This feature is special" justifications

---

## Benefits

### For Feature Development
When complexity is consistent, developers focus on business logic, not infrastructure:
- **No plumbing decisions** - Framework provides test bases, DI setup, repository patterns
- **Predictable patterns** - Find similar feature, copy structure, ship faster
- **Obvious deviations** - When something feels harder than expected, it signals missing framework support
- **Muscle memory** - Same patterns across features means less context switching

### For AI Agents
AI excels at pattern repetition:
- Consistent patterns → accurate code generation
- Predictable structure → fewer hallucinations
- Similar complexity → better estimates
- Clear conventions → less guidance needed

### For Architectural Integrity
Constraints prevent individuals from breaking consistency with clever solutions:
- **Self-enforcing patterns** - Framework patterns make the right way the easy way
- **No "special" features** - If a feature needs special handling, extract to framework
- **Code review focus** - Reviewers check "does this match the pattern?" not "is this clever?"
- **Prevents clever one-offs** - Individual optimization breaks team velocity

### For Teams
- **Faster code reviews** - reviewers know what to expect
- **Easier estimation** - story points become meaningful
- **Lower maintenance burden** - less cognitive switching between styles
- **Better knowledge sharing** - patterns transfer across features

---

## Vertical Slice Architecture

We organize by feature, not by technical layer:

```
Application/
├── Accounts/
│   ├── Login/
│   │   ├── Login.Workflow.cs
│   │   ├── Login.BoundaryContracts.cs
│   │   └── Login.Endpoint.cs
│   ├── Register/
│   └── ViewProfile/
├── Accounts.Domain/
│   ├── Models/
│   └── Repository/
├── Projects/
│   ├── CreateProject/
│   ├── ListProjects/
│   └── UpdateProject/
└── Projects.Domain/
    ├── Models/
    └── Repository/
```

**Each feature is self-contained:**
- Request/Response contracts
- Validation logic
- Business workflow
- API endpoint

**Key principle: Start with single-method features**
- Each folder represents ONE operation (CreateProject, UpdateProject, ListProjects)
- No "UserManager" or "ProjectService" god classes
- Feature complexity is immediately visible by file count
- Features naturally stay small and focused

---

## Examples of Consistent Complexity

### CRUD Operations

Every CRUD workflow should look similar:

```csharp
// Create
public class CreateProjectWorkflow(IProjectRepository _repo)
{
    public async Task<CreateProjectResponse> ExecuteAsync(CreateProjectRequest request)
    {
        // 1. Validate
        // 2. Map to domain model
        // 3. Save via repository
        // 4. Return response
    }
}

// List
public class ListProjectsWorkflow(IProjectRepository _repo)
{
    public async Task<ListProjectsResponse> ExecuteAsync(ListProjectsRequest request)
    {
        // 1. Build specification from request
        // 2. Query via repository
        // 3. Map to response
        // 4. Return response
    }
}
```

**If one CRUD operation is 10x more complex**, ask:
- Is this feature actually CRUD, or something else?
- Are we missing framework support (pagination, filtering, sorting)?
- Should this be multiple smaller features?

---

## Self-Enforcing Patterns

Code should make the right thing easy and the wrong thing hard.

### Example: Migration Location

**Before (documented):**
```bash
# README: "Always use --output-dir for migrations"
dotnet ef migrations add MyMigration --output-dir Infrastructure/Database/Migrations
```
❌ Relies on memory, easy to forget

**After (enforced):**
```csharp
// AppDbContextFactory automatically uses correct location
// EF Core discovers existing migrations and follows their namespace
dotnet ef migrations add MyMigration
```
✅ Can't do it wrong

### Example: Repository Pattern

**Before (manual):**
```csharp
// Every feature manually writes SQL or EF queries
var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
```
❌ Inconsistent, easy to write N+1 queries

**After (abstracted):**
```csharp
// Generic repository + specifications
var user = await _userRepo.GetAsync(UserSpecifications.ByEmail(email));
```
✅ Consistent pattern, reusable specifications

---

## When Complexity Varies

Sometimes features genuinely differ in complexity. How do we handle this?

### Strategy 1: Extract Infrastructure

If many features need similar complexity, extract it to infrastructure:

```
Multiple features need pagination →
  Extract IPaginationService

Multiple features need file upload →
  Extract IFileStorageService

Multiple features need email →
  Extract IEmailService
```

Now all features using these services have consistent complexity again.

### Strategy 2: Keep Single-Method Features

**We don't build "User Management" as one feature.** We start decomposed:

```
Application/
├── Users/
│   ├── CreateUser/           # One operation
│   ├── UpdateUser/           # One operation
│   ├── DeleteUser/           # One operation
│   ├── ListUsers/            # One operation
│   └── UpdateUserPermissions/ # One operation
└── Users.Domain/
    ├── Models/
    └── Repository/
```

Each folder is a single method/operation. No god classes, no managers, no services that do multiple things.

### Strategy 3: Accept the Complexity (Rarely)

Sometimes a feature is genuinely more complex:
- Payment processing with refunds, disputes, webhooks
- Real-time collaboration with CRDT merging
- Complex approval workflows with state machines

**When to accept:**
- Complexity is inherent to business domain (not accidental)
- Feature is well-isolated (doesn't leak complexity)
- Team explicitly documented why this is special

---

## Measuring Consistency

### Code Metrics
- **Lines per feature**: Should be within 2x range (50-100 or 100-200, not 50-500)
- **Files per feature**: Most features have 3-5 files
- **Dependencies per feature**: Similar import counts

### Time Metrics
- **Implementation time**: Similar story point estimates
- **Review time**: Most PRs take similar time to review
- **Onboarding time**: New dev can ship 2nd feature faster than 1st

### Smell Tests
- "This feature is different" explanations
- Custom abstractions for one feature
- Copy-paste with modifications (pattern doesn't fit)

---

## Patterns We Use

### DI Registration - Centralized
All dependency injection in `DependencyResolver.cs`:
```csharp
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IProjectRepository, ProjectRepository>();
```
✅ One place to see all registrations
❌ Never scatter `services.Add*` across Program.cs or elsewhere

### Repository Pattern - Generic + Specifications
```csharp
public interface IRepository<T>
{
    Task<T?> GetAsync(Specification<T> spec);
    Task<List<T>> ListAsync(Specification<T> spec);
    Task AddAsync(T entity);
}
```
✅ Every repository follows same contract
❌ Never write custom repository without good reason

### Migrations - Auto-discovered
```csharp
// AppDbContextFactory reads from appsettings.json
// EF Core finds migrations by namespace
```
✅ No flags, no manual paths
❌ Never hard-code connection strings

### Vertical Slices - Single-Method Features
```
Feature/
  ├── Feature.Workflow.cs      # Business logic
  ├── Feature.BoundaryContracts.cs  # Request/Response
  └── Feature.Endpoint.cs       # API routing
```
✅ Each folder = one operation (CreateUser, UpdateUser, etc.)
✅ Everything related to feature in one place
❌ Never create god classes (UserManager, ProjectService)
❌ Never split by technical layer (Models/, Controllers/, Services/)

### Testing Pattern - Framework-Provided Base

**Every test inherits from `SetupTestFor<T>`:**
```csharp
public class RegisterWorkflowUnitTests : SetupTestFor<RegisterWorkflow>
{
    [Fact]
    public async Task Perform_WithValidRequest_ReturnsSuccess()
    {
        // Mocker automatically provides mocked dependencies
        Mocker
            .GetSubstituteFor<IUserRepository>()
            .EmailExists(Arg.Any<string>())
            .Returns(false);

        var result = await SUT.Perform(request);

        result.Success.Should().BeTrue();
    }
}
```

**Integration tests use same base with `Fakes()` for selective real implementations:**
```csharp
public class RegisterWorkflowIntegrationTests : SetupTestFor<RegisterWorkflow>
{
    [Fact]
    public async Task Perform_EndToEnd_WithFakeRepository()
    {
        // Use real implementations instead of mocks
        Fakes()
            .Replace<IUserRepository, InMemoryUserRepository>()
            .Replace<IPasswordHasher, FakePasswordHasher>()
            .Use();

        // Test with real behavior
        var result = await SUT.Perform(request);
        result.Success.Should().BeTrue();
    }
}
```

**Why framework-provided test base:**
- Developers never write test setup boilerplate
- Adding dependencies to class under test doesn't break tests (AutoSubstitute handles it)
- Consistent test structure across entire codebase
- Same base for unit and integration tests—only what's faked changes

**Framework responsibility vs Feature responsibility:**
- **Framework provides:** Test bases, DI registration, repository pattern, migration discovery, fake injection
- **Features use:** Existing patterns without thinking about infrastructure
- **If feature work feels complex:** Extract infrastructure to framework

✅ Framework handles complexity once, features stay simple forever
❌ Never solve infrastructure problems at feature level

### Workflow Pattern - Revealing Intent Class Structure

Every workflow follows a consistent internal structure that reveals business intent while hiding implementation details.

**Core principles:**

1. **Single entry point reveals process** - The public `Perform()` method reads like a business process story
2. **Temporal coupling through data flow** - Method signatures enforce correct ordering through dependencies
3. **Response transformation isolation** - All output shaping lives in dedicated `response()` methods
4. **Implementation details hidden** - Private helper methods hide "how" while entry point shows "what"

**Example:**

```csharp
public class RegisterWorkflow(
    IPasswordHasher _passwordHasher,
    IUserRepository _userRepository,
    IValidator<RegisterRequest> _validator
) : IWorkflow<RegisterRequest, RegisterResponse>
{
    // Entry point reveals business intent - readable without implementation details
    public async Task<IApplicationResult<RegisterResponse>> Perform(RegisterRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return response(validationResult);

        var user = createUser(request);

        // Temporal coupling: must create user before checking email
        var emailExists = await checkEmailExists(user.Email);
        if (emailExists)
            return response("An account with this email already exists.");

        await saveUser(user);
        return response();
    }

    // Response transformation - isolated, easy to modify
    private IApplicationResult<RegisterResponse> response(ValidationResult validationResult) =>
        ApplicationResult<RegisterResponse>.Fail(validationResult);

    private IApplicationResult<RegisterResponse> response(string errorMessage) =>
        ApplicationResult<RegisterResponse>.Fail(errorMessage);

    private IApplicationResult<RegisterResponse> response() =>
        ApplicationResult<RegisterResponse>.Succeed(
            new RegisterResponse { Message = "Account created successfully. You can now log in." }
        );

    // Implementation details - drill down only when needed
    private async Task<ValidationResult> validate(RegisterRequest request) =>
        await _validator.ValidateAsync(request);

    private async Task<bool> checkEmailExists(string email) =>
        await _userRepository.EmailExists(email);

    private User createUser(RegisterRequest request) =>
        User.Create(request.Email, _passwordHasher.HashPassword(request.Password));

    private async Task saveUser(User user)
    {
        await _userRepository.Add(user);
        await _userRepository.SaveChanges();
    }
}
```

**Benefits:**

- **Communication** - Developers and business stakeholders can discuss `Perform()` using shared vocabulary
- **Maintenance** - Clear where to add steps (entry point), change logic (helper methods), or modify output (response methods)
- **Self-documenting** - The code structure itself explains the process flow
- **Type-safe ordering** - Compiler enforces temporal dependencies through method signatures
- **Consistent complexity** - Every workflow follows this pattern, making them predictable

**When reviewing workflows:**
- Can you understand the business process by reading only `Perform()`?
- Are temporal dependencies enforced through data flow (not just ordering)?
- Is response shaping isolated from business logic?
- Are implementation details hidden in appropriately-named helper methods?

### Read vs Write Separation

**Clear separation of concerns following CQRS principles:**

**DataDomains handle all reads (queries):**
- Used by pages for initial data fetching
- Used by components for rendering
- Used by framework for OOB updates after mutations
- Examples: `TodosDomain`, `TodoAnalyticsDomain`

**Workflows handle all writes (commands):**
- Create, Update, Delete operations
- Business logic, validation, authorization
- Examples: `CreateTodo`, `UpdateTodo`, `DeleteTodo`, `ToggleTodo`

**Pattern:**
- Need to read data? → Use DataDomain via DataLoader
- Need to mutate data? → Create a Workflow
- No "read workflows" - reads go through DataDomains

**Why this separation?**
- Eliminates duplication (one query path, one mutation path)
- Clear responsibilities (mutations vs queries)
- Consistent with game-style data pre-fetching pattern
- Workflows reveal business intent (actions), domains reveal data structure
- Self-enforcing: obvious which pattern to use for any given task

**Example:**

```csharp
// ✅ Correct - Reading data through DataDomain
public class TodosPageEndpoints(
    IDataLoader _dataLoader,
    IPageLayout _layout
) : IClientEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("todos", async (IDataLoader dataLoader) =>
        {
            var dataContext = await dataLoader.LoadDomainsAsync(new[] { "todos" });
            var todosData = dataContext.GetDomain<TodosDomainContext>("todos");

            var bodyContent = _page.RenderPage(todosData.List);
            var page = await _layout.RenderAsync("Todos", bodyContent);
            return Results.Content(page, "text/html");
        });
    }
}

// ✅ Correct - Mutating data through Workflow
public class CreateTodoWorkflow(
    ITodoRepository _repository,
    IValidator<CreateTodoRequest> _validator
) : IWorkflow<CreateTodoRequest, CreateTodoResponse>
{
    public async Task<IApplicationResult<CreateTodoResponse>> Perform(CreateTodoRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return buildErrorResponse(validationResult);

        var todo = createTodo(request);
        await saveTodo(todo);
        return buildSuccessResponse();
    }
    // ... helper methods
}

// ❌ Incorrect - Don't create "ListTodos" workflow
public class ListTodosWorkflow // Don't do this - use DataDomain instead
{
    public async Task<ListTodosResponse> Perform(ListTodosRequest request)
    {
        // Reading data doesn't belong in a workflow
        var todos = await _repository.List(specification);
        return new ListTodosResponse { Todos = todos };
    }
}
```

---

## For AI Agents and Framework Development

When implementing features, AI should:

1. **Use framework infrastructure** - Tests inherit from `SetupTestFor<T>`, workflows use repositories, DI is centralized
2. **Start with single operation** - Each feature folder does ONE thing (CreateUser, not UserManagement)
3. **Follow read/write separation** - DataDomains for queries, Workflows for commands
4. **Look for existing patterns** - Find similar feature, copy structure exactly
5. **Measure deviation** - If new feature is 2x bigger, ask why
6. **Flag complexity** - "This feature seems more complex than others, should we extract to framework?"

**When implementing reads:**
- Use DataDomain via DataLoader
- Never create "List" or "Get" workflows
- Data fetching happens in DataDomains, not Workflows

**When implementing writes:**
- Create a Workflow for the mutation (Create, Update, Delete)
- Workflow names reveal business actions
- Follow revealing intent pattern

**When infrastructure is missing:**
- Feature code reveals need for framework support (pagination, file upload, email)
- Extract to framework infrastructure, not feature-level abstraction
- Update all features to use new framework capability

AI should NOT:
- Create custom patterns for one feature
- Skip existing abstractions "because it's easier"
- Manually mock dependencies in tests (use `SetupTestFor<T>`)
- Create god classes (UserManager, ProjectService)
- Put multiple operations in one feature folder
- Create "List" or "Get" workflows (use DataDomains instead)

---

## Summary

**Consistent Complexity means:**
- Features feel similar in size and structure
- Patterns are reusable across features
- Junior devs and AI can predict what code looks like
- Deviations are signals to improve architecture

**We achieve this through:**
- Self-enforcing code patterns
- Vertical slice architecture
- Generic abstractions (repositories, specifications)
- Centralized infrastructure (DI, migrations, config)

**When complexity varies:**
- Extract to infrastructure (if many features need it)
- Decompose the feature (if one feature is too big)
- Accept rarely (and document why)

---

**The goal: A codebase where adding Feature N+1 feels like adding Feature N.**
