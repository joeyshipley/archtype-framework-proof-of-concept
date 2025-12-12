# Architecture Reference (Source of Truth)

**Last Updated:** 2025-12-12
**Purpose:** Canonical reference for current implementation. All other docs should align with this.

---

## Core Interfaces

### IView (Server-Rendered Views)
**File:** `Infrastructure/Web/Components/IView.cs`

Views are server-rendered components that declare data dependencies and render HTML from pre-loaded data.

```csharp
public interface IView
{
    string ViewId { get; }                    // Unique ID for DOM targeting and OOB
    DataDependencies Dependencies { get; }   // What data this view needs
    string Render(IDataContext data);        // Pure render function (no I/O)
}
```

**Key characteristics:**
- Views declare dependencies via `DataDependencies.From<TContext>()`
- `Render()` must be pure - no I/O, no side effects
- Views with no data needs use `DataDependencies.None`

**Example implementation:**
```csharp
public class TodosPage(IHtmlRenderer _renderer) : ITodosPageView
{
    public string ViewId => "todo-page";

    public DataDependencies Dependencies =>
        DataDependencies.From<TodosListDomainView>();

    public string Render(IDataContext data)
    {
        var todos = data.Get<TodosListDomainView>();
        return _renderer.Render(new Section().Id(ViewId).Children(...));
    }
}
```

---

### IDataProvider (Query-Side Data)
**File:** `Infrastructure/Web/Data/IDataProvider.cs`

Providers fetch domain data for views. They implement the query side of CQRS.

```csharp
public interface IDataProvider {}  // Marker for DI discovery

public interface IDataProvider<TContext> : IDataProvider
    where TContext : class, new()
{
    Task<TContext> FetchTyped(long userId);
}
```

**Example implementation:**
```csharp
public class TodosListProvider(IRepository _repository) : IDataProvider<TodosListDomainView>
{
    public async Task<TodosListDomainView> FetchTyped(long userId)
    {
        var todos = await _repository.List(Todo.ByUserId(userId));
        return new TodosListDomainView
        {
            List = todos.Select(TodoListEntry.FromTodo).ToList(),
            OpenCount = todos.Count(t => !t.IsCompleted),
            // ... other computed properties
        };
    }
}
```

---

### DomainView (Query Context)
**Convention:** `{Feature}.DomainView.cs` in `Application/{Feature}/Perspectives/{Perspective}/`

DomainViews are POCOs that hold query-side data. They include a static `DomainName` constant for mutation tracking.

```csharp
public class TodosListDomainView
{
    public const string DomainName = "todosList";  // Used for OOB targeting

    public List<TodoListEntry> List { get; set; } = new();
    public int OpenCount { get; set; }
    public int TotalCount { get; set; }
    public double CompletionRate { get; set; }
}
```

---

### IElement (UI Vocabulary)
**File:** `Infrastructure/UI/IElement.cs`

Elements are semantic UI building blocks that declare WHAT something is, not HOW it looks.

```csharp
public interface IElement
{
    IEnumerable<IElement> Children { get; }
}

public abstract record ElementBase : IElement, IEnumerable
{
    // Supports collection initializer syntax
    public void Add(IElement element);
}
```

**Available Elements (from HtmlRenderer):**
- **Layout:** `Page`, `Section`, `Stack`, `Row`, `Grid`, `Card`
- **Slots:** `Header`, `Body`, `Footer`
- **Text:** `Text`, `PageTitle`, `SectionTitle`
- **Forms:** `Form`, `Field`, `Input`, `Label`, `Button`, `Checkbox`
- **Lists:** `List`, `ListItem`
- **Feedback:** `Alert`, `EmptyState`

---

### PageInteractionBase (Command Handler)
**File:** `Infrastructure/Web/Pages/PageInteractionBase.cs`

Base class for handling user interactions (commands). Handles workflow execution, routing, and OOB orchestration.

```csharp
public abstract class PageInteractionBase<TRequest, TResponse, TView> : IEndpoint
    where TRequest : IWorkflowRequest
    where TResponse : IWorkflowResponse
    where TView : class
{
    protected abstract string RouteBase { get; }     // e.g., "todos"
    protected abstract string RouteAction { get; }  // e.g., "create"
    protected virtual DataMutations Mutates => null; // What domains change

    protected abstract Task<IResult> OnSuccess(TResponse response);
    protected abstract IResult RenderError(string message);

    // Helper methods for OOB responses
    protected Task<IResult> BuildOobResult();
    protected IResult BuildOobOnly(string oobHtml);
    protected Task<IResult> BuildOobResultWith(params string[] oobFragments);
}
```

**Example implementation:**
```csharp
public class CreateTodoInteraction(ITodosPageView page, IFrameworkOrchestrator _framework)
    : PageInteractionBase<CreateTodoWorkflowRequest, CreateTodoWorkflowResponse, ITodosPageView>(page, _framework)
{
    protected override string RouteBase => "todos";
    protected override string RouteAction => "create";
    protected override DataMutations Mutates => DataMutations.For(TodosListDomainView.DomainName);

    protected override async Task<IResult> OnSuccess(CreateTodoWorkflowResponse response)
    {
        var formReset = HtmlFragment.InjectOob(Page.RenderCreateForm());
        return await BuildOobResultWith(formReset);
    }

    protected override IResult RenderError(string message) =>
        BuildOobOnly(HtmlFragment.InjectOob(Page.RenderErrorNotification(message)));
}
```

---

## Data Flow

### Initial Page Load

```
Request → Page Route Handler
    │
    ├─→ Create View instances (via DI)
    │
    ├─→ FrameworkOrchestrator.RenderViewsAsync(views)
    │   │
    │   ├─→ Collect DataDependencies from all views
    │   │
    │   ├─→ DataLoader.With<T1>().With<T2>().Load()
    │   │   │
    │   │   └─→ Find IDataProvider<T> for each context type
    │   │   └─→ Call FetchTyped(userId) in parallel
    │   │   └─→ Return populated DataContext
    │   │
    │   └─→ Call view.Render(dataContext) for each view
    │       └─→ Inject data-view and data-domain attributes
    │
    └─→ Return HTML page
```

### Interaction (Mutation) Flow

```
Form Submit → POST /interaction/{route}/{action}
    │
    ├─→ PageInteractionBase.Handle()
    │   │
    │   ├─→ workflow.Perform(request)
    │   │   └─→ Returns metadata only (CQRS - no query data)
    │   │
    │   └─→ OnSuccess(response) or OnError(errors)
    │       │
    │       └─→ BuildOobResult() / BuildOobResultWith()
    │           │
    │           └─→ FrameworkOrchestrator.RenderMutationResponseAsync()
    │               │
    │               ├─→ Parse X-Component-Context header (ViewInfo[])
    │               │
    │               ├─→ Find views affected by DataMutations.Domains
    │               │
    │               ├─→ Re-fetch data for affected context types
    │               │
    │               └─→ Re-render affected views with hx-swap-oob="true"
    │
    └─→ Return OOB HTML fragments
```

---

## Key Patterns

### Type-Safe Data Loading (Fluent API)

```csharp
// Correct (current implementation)
var data = await _dataLoader
    .With<TodosListDomainView>()
    .With<UserProfileDomainView>()
    .Load();

var todos = data.Get<TodosListDomainView>();
var profile = data.Get<UserProfileDomainView>();
```

### DataDependencies Declaration

```csharp
// Single domain dependency
public DataDependencies Dependencies =>
    DataDependencies.From<TodosListDomainView>();

// No data dependency (static pages like Login)
public DataDependencies Dependencies => DataDependencies.None;
```

### DataMutations Declaration

```csharp
// Single domain mutation
protected override DataMutations Mutates =>
    DataMutations.For(TodosListDomainView.DomainName);

// Multiple domains
protected override DataMutations Mutates =>
    DataMutations.For(TodosListDomainView.DomainName, TodoAnalyticsDomainView.DomainName);

// No mutations (query-only interactions)
protected override DataMutations Mutates => null;
```

### OOB Response Patterns

```csharp
// Framework handles all OOB based on Mutates declaration
return await BuildOobResult();

// Framework OOB + manual fragments (e.g., form reset)
var formReset = HtmlFragment.InjectOob(Page.RenderCreateForm());
return await BuildOobResultWith(formReset);

// Manual OOB only (e.g., error notification, no component updates)
return BuildOobOnly(HtmlFragment.InjectOob(errorHtml));
```

---

## Creating New Features

### Adding a New Page Feature (View + Interaction + Provider)

**Order:** Provider → View → Interaction → Endpoints → DI Registration

#### Step 1: Create DomainView + Provider

**Location:** `Application/{Feature}/Perspectives/{Perspective}/`

```csharp
// {Name}.DomainView.cs
public class TodosListDomainView
{
    public const string DomainName = "todosList";  // Used for OOB targeting

    public List<TodoListEntry> List { get; set; } = new();
    public int OpenCount { get; set; }
    // ... other computed properties
}

// {Name}.Provider.cs
public class TodosListProvider(IRepository _repository) : IDataProvider<TodosListDomainView>
{
    public async Task<TodosListDomainView> FetchTyped(long userId)
    {
        var todos = await _repository.List(Todo.ByUserId(userId));
        return new TodosListDomainView
        {
            List = todos.Select(TodoListEntry.FromTodo).ToList(),
            OpenCount = todos.Count(t => !t.IsCompleted)
        };
    }
}
```

#### Step 2: Create View

**Location:** `Pages/{PageName}/{PageName}.Page.cs`

```csharp
// Define interface for DI
public interface ITodosPageView : IView { }

// Implement view
public class TodosPage(IHtmlRenderer _renderer) : ITodosPageView
{
    public string ViewId => "todos-page";

    public DataDependencies Dependencies =>
        DataDependencies.From<TodosListDomainView>();

    public string Render(IDataContext data)
    {
        var todos = data.Get<TodosListDomainView>();
        return _renderer.Render(new Section().Id(ViewId).Children(...));
    }
}
```

#### Step 3: Create Interaction (if mutations needed)

**Location:** `Pages/{PageName}/Interactions/{Action}.Interaction.cs`

```csharp
// Define marker interface for this page's interactions
public interface ITodosPageInteraction : IEndpoint { }

// Implement interaction
public class CreateTodoInteraction(ITodosPageView page, IFrameworkOrchestrator _framework)
    : PageInteractionBase<CreateTodoWorkflowRequest, CreateTodoWorkflowResponse, ITodosPageView>(page, _framework),
      ITodosPageInteraction
{
    protected override string RouteBase => "todos";
    protected override string RouteAction => "create";
    protected override DataMutations Mutates => DataMutations.For(TodosListDomainView.DomainName);

    protected override async Task<IResult> OnSuccess(CreateTodoWorkflowResponse response)
    {
        var formReset = HtmlFragment.InjectOob(Page.RenderCreateForm());
        return await BuildOobResultWith(formReset);
    }

    protected override IResult RenderError(string message) =>
        BuildOobOnly(HtmlFragment.InjectOob(Page.RenderErrorNotification(message)));
}
```

#### Step 4: Create Page Endpoints

**Location:** `Pages/{PageName}/{PageName}.PageEndpoints.cs`

```csharp
public class TodosPageEndpoints(ITodosPageView _page, IFrameworkOrchestrator _framework) : IClientEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/todos", async () => await _framework.RenderPageAsync(_page));
    }
}
```

#### Step 5: Register in DependencyResolver.cs

**Location:** `Infrastructure/Dependencies/DependencyResolver.cs`

```csharp
// In bindData() - add Provider
services.AddScoped<IDataProvider, TodosListProvider>();

// In bind{Domain}Workflows() - add Workflow (if new)
services.AddScoped<IWorkflow<CreateTodoWorkflowRequest, CreateTodoWorkflowResponse>, CreateTodoWorkflow>();

// In bindPageEndpoints() - add Endpoints
services.AddScoped<IClientEndpoint, TodosPageEndpoints>();

// In bindPageInteractions() - add Interaction
services.AddScoped<ITodosPageInteraction, CreateTodoInteraction>();
```

**Note:** Views with a specific interface (e.g., `ITodosPageView`) are auto-discovered. Validators are auto-registered via `AddValidatorsFromAssemblyContaining`.

---

### Adding an API Workflow Only

For API-only features (no page UI), create a vertical slice:

**Location:** `Application/{Domain}/{Feature}/`

| File | Purpose |
|------|---------|
| `{Feature}.BoundaryContracts.cs` | Request, Response, Validator |
| `{Feature}.Workflow.cs` | Business logic |
| `{Feature}.Endpoint.cs` | HTTP route registration |

```csharp
// {Feature}.Workflow.cs
public class UpdateProfileWorkflow(
    IValidator<UpdateProfileRequest> _validator,
    IRepository _repository
) : IWorkflow<UpdateProfileRequest, UpdateProfileResponse>
{
    public async Task<IApplicationResult<UpdateProfileResponse>> Perform(UpdateProfileRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ApplicationResult<UpdateProfileResponse>.Fail(validationResult);

        // Business logic here...

        return ApplicationResult<UpdateProfileResponse>.Succeed(new UpdateProfileResponse { });
    }
}
```

**Register:** Add workflow in `bind{Domain}Workflows()` section of DependencyResolver.

---

### Common Mistakes

| Mistake | Fix |
|---------|-----|
| Forgot DI registration | Add to appropriate section in `DependencyResolver.cs` |
| View not rendering data | Check `Dependencies` returns correct `DataDependencies.From<T>()` |
| OOB not updating after mutation | Check `Mutates` returns correct `DataMutations.For(DomainName)` |
| Provider not found at runtime | Ensure Provider registered as `IDataProvider` in `bindData()` |
| Interaction route not found | Check `RouteBase` and `RouteAction` match expected `/interaction/{base}/{action}` |

---

## Testing

All tests inherit from `SetupTestFor<T>` which auto-injects dependencies.

### Unit Tests (Mocked Dependencies)

```csharp
public class CreateTodoWorkflowUnitTests : SetupTestFor<CreateTodoWorkflow>
{
    [Fact]
    public async Task Perform_WithValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateTodoWorkflowRequest { Title = "Test" };

        Mocker
            .GetSubstituteFor<IValidator<CreateTodoWorkflowRequest>>()
            .ValidateAsync(request, default)
            .Returns(new ValidationResult());

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeTrue();
    }
}
```

### Integration Tests (Real Implementations)

```csharp
public class CreateTodoWorkflowIntegrationTests : SetupTestFor<CreateTodoWorkflow>
{
    [Fact]
    public async Task Perform_EndToEnd_WithFakeRepository()
    {
        // Arrange
        Fakes()
            .Replace<IRepository, InMemoryRepository>()
            .Use();

        var request = new CreateTodoWorkflowRequest { Title = "Test" };

        // Act
        var result = await SUT.Perform(request);

        // Assert
        result.Success.Should().BeTrue();
    }
}
```

### Key Patterns

| Pattern | Usage |
|---------|-------|
| `SUT` | System Under Test - auto-instantiated with dependencies |
| `Mocker.GetSubstituteFor<T>()` | Get/create mock for interface (NSubstitute) |
| `Fakes().Replace<TInterface, TImpl>().Use()` | Swap real implementation for integration tests |

### File Naming
- `{Component}.Unit.Tests.cs` - Unit tests
- `{Component}.Integration.Tests.cs` - Integration tests
- Location mirrors source: `PagePlay.Tests/Application/{Domain}/{Feature}/`

---

## Authentication

### Protected Endpoints

```csharp
public void Map(IEndpointRouteBuilder endpoints) =>
    endpoints.Register<UpdateProfileResponse>("/accounts/profile", handle)
        .RequireAuthenticatedUser();  // Populates LoggedInAuthContext
```

### Accessing Authenticated User

```csharp
public class UpdateProfileWorkflow(
    IValidator<UpdateProfileRequest> _validator,
    LoggedInAuthContext _authContext,  // Injected by framework
    IRepository _repository
) : IWorkflow<UpdateProfileRequest, UpdateProfileResponse>
{
    public async Task<IApplicationResult<UpdateProfileResponse>> Perform(UpdateProfileRequest request)
    {
        var userId = _authContext.UserId;  // Non-nullable, guaranteed populated
        var user = await _repository.GetById(userId);
        // ...
    }
}
```

**Key points:**
- `LoggedInAuthContext.UserId` is guaranteed populated by the auth filter
- No null checks needed - if workflow executes, user is authenticated
- Use `.RequireAuthenticatedUser()` on endpoint to enable

---

## File Organization

```
PagePlay.Site/
├── Application/
│   └── {Feature}/
│       ├── Models/              # Domain entities
│       ├── Perspectives/        # Query-side views of data
│       │   └── {Perspective}/
│       │       ├── {Name}.DomainView.cs   # Query context POCO
│       │       └── {Name}.Provider.cs     # IDataProvider implementation
│       └── Workflows/           # Command handlers
│           └── {Action}/
│               └── {Action}.Workflow.cs
│
├── Pages/
│   └── {PageName}/
│       ├── {PageName}.Page.cs           # IView implementation
│       ├── {PageName}.PageEndpoints.cs  # Route registration
│       └── Interactions/
│           └── {Action}.Interaction.cs  # PageInteractionBase implementations
│
└── Infrastructure/
    ├── UI/
    │   ├── IElement.cs          # Element marker interface
    │   ├── Vocabulary/          # Concrete element types
    │   └── Rendering/
    │       └── HtmlRenderer.cs  # Element → HTML
    │
    └── Web/
        ├── Components/
        │   ├── IView.cs         # View interface + DataDependencies + IDataContext
        │   ├── ViewFactory.cs   # Creates views by type name
        │   └── ViewContext.cs   # Client context parsing
        ├── Data/
        │   ├── IDataProvider.cs # Provider interface
        │   ├── DataLoader.cs    # Fluent data loading
        │   └── DataLoaderBuilder.cs
        ├── Framework/
        │   └── FrameworkOrchestrator.cs  # Coordinates rendering + OOB
        ├── Mutations/
        │   └── DataMutations.cs # Declares what domains change
        └── Pages/
            └── PageInteractionBase.cs  # Base class for interactions
```

---

## DOM Attributes

The framework automatically injects these attributes for OOB targeting:

| Attribute | Purpose | Example |
|-----------|---------|---------|
| `id` | DOM element ID (from ViewId) | `id="todo-page"` |
| `data-view` | View class name for re-instantiation | `data-view="TodosPage"` |
| `data-domain` | Domain name for mutation matching | `data-domain="todosList"` |

Client-side JavaScript collects these into `X-Component-Context` header:
```json
[{"id": "todo-page", "viewType": "TodosPage", "domain": "todosList"}]
```

---

## Routes

| Pattern | Purpose |
|---------|---------|
| `/` | Page routes |
| `/interaction/{base}/{action}` | Interaction endpoints |

---

## Terminology Quick Reference

| Term | Definition |
|------|------------|
| **View** | Server-rendered component implementing `IView` |
| **Element** | Semantic UI building block implementing `IElement` |
| **Provider** | Data fetcher implementing `IDataProvider<T>` |
| **DomainView** | Query-side POCO with `DomainName` constant |
| **Interaction** | Command handler extending `PageInteractionBase` |
| **DataMutations** | Declaration of which domains a command affects |
| **OOB (Out-of-Band)** | HTMX mechanism for updating multiple DOM elements |
