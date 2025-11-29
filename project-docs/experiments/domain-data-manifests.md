# Experiment: Domain-Level Data Manifests with OOB Updates

**Status:** In Progress
**Started:** 2025-11-29
**Goal:** Implement domain-level data manifests with two-phase rendering and HTMX OOB updates

---

## 🎯 **Experiment Overview**

### **Problem Statement**
Currently, components have two bad options:
1. **Tight coupling** - Parent passes data to children (breaks encapsulation)
2. **N+1 queries** - Each component fetches its own data (performance issue)

### **Solution Architecture**
Implement **Domain-Level Data Manifests** where:
- **Components** declare which domain they depend on (e.g., "todos")
- **Domains** provide all data for that domain in one fetch
- **Interactions** declare which domains they mutate
- **Framework** automatically re-renders affected components with OOB updates

### **Key Benefits**
- ✅ No parent/child data coupling
- ✅ No N+1 queries (domain fetches all data once)
- ✅ Add new component → no interaction changes needed
- ✅ Automatic OOB updates when data changes
- ✅ Parallel data fetching for multiple domains

---

## 🏗️ **Architecture Principles**

### **1. Domain-Oriented Data**
```
Domain = Bounded context that owns related data
Examples: "todos", "accounts", "notifications"

Each domain provides ALL its data in one fetch:
  TodosDomain.FetchAll() → { list, openCount, longestStreak, totalCount }

Components take what they need:
  WelcomeWidget uses: openCount
  TodoList uses: list
  TodoStreak uses: longestStreak
```

### **2. Two-Phase Rendering**
```
Phase 1: Data Loading (async, parallel)
  - Collect all domains needed by components
  - Fetch each domain's data (parallel across domains)
  - Build unified DataContext

Phase 2: Rendering (sync, pure)
  - Pass DataContext to all components
  - Components render with pre-loaded data
  - No I/O during render phase
```

### **3. Declarative Mutations**
```
Interactions declare WHAT they affect, not HOW to update:
  CreateTodoInteraction.Mutates → ["todos"]

Framework figures out:
  - Which components depend on "todos" domain
  - Re-fetch todos domain data
  - Re-render affected components
  - Return OOB updates
```

### **4. Client-Side Context**
```
HTML declares component dependencies:
  <div id="welcome-widget" data-domain="todos">

HTMX sends context with requests:
  X-Component-Context: [{ id: "welcome-widget", domain: "todos" }]

Server uses context to determine what to update
```

---

## 📋 **Implementation Checklist**

### **Phase 1: Core Infrastructure** 🔄 In Progress

#### **Task 1.1: Create Data Domain Contracts** ✅
**File:** `PagePlay.Site/Infrastructure/Web/Data/IDataDomain.cs`

**Definition:**
```csharp
namespace PagePlay.Site.Infrastructure.Web.Data;

/// <summary>
/// Represents a bounded context that provides related data.
/// Examples: "todos", "accounts", "notifications"
/// </summary>
public interface IDataDomain
{
    /// <summary>
    /// Unique name for this domain (e.g., "todos")
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Fetches ALL data this domain provides in one query.
    /// Called during initial page render and when domain is mutated.
    /// </summary>
    Task<DomainDataContext> FetchAllAsync(long userId);
}

/// <summary>
/// Container for all data a domain provides.
/// Components access data by key: context["openCount"]
/// </summary>
public class DomainDataContext
{
    private readonly Dictionary<string, object> _data = new();

    public object this[string key]
    {
        get => _data[key];
        set => _data[key] = value;
    }

    public T Get<T>(string key) => (T)_data[key];

    public bool Contains(string key) => _data.ContainsKey(key);
}
```

**Acceptance Criteria:**
- ✅ `IDataDomain` interface defined - Complete
- ✅ `DomainDataContext` dictionary wrapper created - Complete
- ✅ Type-safe `Get<T>()` method works - Complete (tested)
- ✅ Can set/get values by string key - Complete (tested)

---

#### **Task 1.2: Create Component Data Contracts** ✅
**File:** `PagePlay.Site/Infrastructure/Web/Components/IServerComponent.cs`

**Definition:**
```csharp
namespace PagePlay.Site.Infrastructure.Web.Components;

/// <summary>
/// Server-rendered component that declares data dependencies
/// and renders HTML from pre-loaded data.
/// </summary>
public interface IServerComponent
{
    /// <summary>
    /// Unique identifier for this component instance.
    /// Used as DOM element ID and for OOB targeting.
    /// </summary>
    string ComponentId { get; }

    /// <summary>
    /// Data dependencies this component needs to render.
    /// </summary>
    DataDependencies Dependencies { get; }

    /// <summary>
    /// Renders HTML using pre-loaded data.
    /// This method must be pure (no I/O, no side effects).
    /// </summary>
    string Render(IDataContext data);
}

/// <summary>
/// Declares which domain(s) a component depends on.
/// </summary>
public class DataDependencies
{
    public string Domain { get; private set; }
    public List<string> RequiredKeys { get; private set; } = new();

    public static DataDependencies From(string domain) => new() { Domain = domain };

    public DataDependencies Require<T>(string key)
    {
        RequiredKeys.Add(key);
        return this;
    }
}

/// <summary>
/// Provides access to all loaded domain data.
/// </summary>
public interface IDataContext
{
    /// <summary>
    /// Gets data from a specific domain.
    /// </summary>
    T Get<T>(string domain, string key);

    /// <summary>
    /// Checks if a domain has been loaded.
    /// </summary>
    bool HasDomain(string domain);
}

public class DataContext : IDataContext
{
    private readonly Dictionary<string, DomainDataContext> _domains = new();

    public void AddDomain(string domainName, DomainDataContext domainData)
    {
        _domains[domainName] = domainData;
    }

    public T Get<T>(string domain, string key)
    {
        if (!_domains.ContainsKey(domain))
            throw new InvalidOperationException($"Domain '{domain}' not loaded");

        return _domains[domain].Get<T>(key);
    }

    public bool HasDomain(string domain) => _domains.ContainsKey(domain);
}
```

**Acceptance Criteria:**
- ✅ `IServerComponent` interface defined
- ✅ `DataDependencies` fluent builder works: `DataDependencies.From("todos").Require<int>("openCount")`
- ✅ `IDataContext` and `DataContext` implementation complete
- ✅ Can get data: `context.Get<int>("todos", "openCount")`
- ✅ Throws clear exception if domain not loaded

---

#### **Task 1.3: Create Mutation Contracts** ✅
**File:** `PagePlay.Site/Infrastructure/Web/Mutations/DataMutations.cs`

**Definition:**
```csharp
namespace PagePlay.Site.Infrastructure.Web.Mutations;

/// <summary>
/// Declares which domains are mutated by an interaction.
/// Framework uses this to determine which components to re-render.
/// </summary>
public class DataMutations
{
    public List<string> Domains { get; private set; } = new();

    /// <summary>
    /// Creates a mutation declaration for one or more domains.
    /// </summary>
    public static DataMutations For(params string[] domains)
    {
        return new DataMutations { Domains = domains.ToList() };
    }
}
```

**Acceptance Criteria:**
- ✅ `DataMutations` class created - Complete
- ✅ `DataMutations.For("todos")` works - Complete (tested)
- ✅ `DataMutations.For("todos", "notifications")` supports multiple domains - Complete (tested)
- ✅ `Domains` property is publicly accessible - Complete (tested)

---

#### **Task 1.4: Create Component Context Parser** ⬜
**File:** `PagePlay.Site/Infrastructure/Web/Components/ComponentContext.cs`

**Definition:**
```csharp
namespace PagePlay.Site.Infrastructure.Web.Components;

/// <summary>
/// Represents client-side component information sent with each request.
/// Tells server which components are currently on the page.
/// </summary>
public class ComponentInfo
{
    public string Id { get; set; } = string.Empty;
    public string ComponentType { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
}

/// <summary>
/// Parses component context from client HTTP headers.
/// </summary>
public interface IComponentContextParser
{
    /// <summary>
    /// Parses X-Component-Context header into component info list.
    /// Returns empty list if header missing or invalid.
    /// </summary>
    List<ComponentInfo> Parse(string? contextJson);
}

public class ComponentContextParser : IComponentContextParser
{
    public List<ComponentInfo> Parse(string? contextJson)
    {
        if (string.IsNullOrWhiteSpace(contextJson))
            return new List<ComponentInfo>();

        try
        {
            var components = JsonSerializer.Deserialize<List<ComponentInfo>>(contextJson);
            return components ?? new List<ComponentInfo>();
        }
        catch (JsonException)
        {
            // Log warning? For now, return empty list
            return new List<ComponentInfo>();
        }
    }
}
```

**Acceptance Criteria:**
- ✅ `ComponentInfo` DTO defined
- ✅ `IComponentContextParser` interface defined
- ✅ `Parse()` returns empty list if null/empty
- ✅ `Parse()` deserializes valid JSON
- ✅ `Parse()` handles invalid JSON gracefully (returns empty list)

---

### **Phase 2: TodosDomain Implementation** ⬜ Not Started

#### **Task 2.1: Implement TodosDomain** ⬜
**File:** `PagePlay.Site/Application/Todos/Domain/TodosDomain.cs`

**Definition:**
```csharp
namespace PagePlay.Site.Application.Todos.Domain;

using PagePlay.Site.Application.Todos.Domain.Repository;
using PagePlay.Site.Infrastructure.Web.Data;

/// <summary>
/// Data domain for todo-related data.
/// Fetches all todo data in one query and provides it to components.
/// </summary>
public class TodosDomain(ITodoRepository _todoRepository) : IDataDomain
{
    public string Name => "todos";

    public async Task<DomainDataContext> FetchAllAsync(long userId)
    {
        // Fetch todos once
        var todos = await _todoRepository.GetTodosForUser(userId);

        // Calculate all derived data
        var openCount = todos.Count(t => !t.IsCompleted);
        var totalCount = todos.Count;
        var completionRate = totalCount > 0
            ? (double)todos.Count(t => t.IsCompleted) / totalCount
            : 0.0;

        // Return everything this domain provides
        return new DomainDataContext
        {
            ["list"] = todos,
            ["openCount"] = openCount,
            ["totalCount"] = totalCount,
            ["completionRate"] = completionRate
        };
    }
}
```

**Acceptance Criteria:**
- ✅ `TodosDomain` implements `IDataDomain`
- ✅ `Name` returns "todos"
- ✅ `FetchAllAsync()` queries repository once
- ✅ Returns context with: list, openCount, totalCount, completionRate
- ✅ Handles empty todo list (no divide by zero)

---

#### **Task 2.2: Register TodosDomain in DI** ⬜
**File:** `PagePlay.Site/Infrastructure/DependencyResolver.cs`

**Changes:**
```csharp
// Add to RegisterInfrastructure() or similar
services.AddScoped<IDataDomain, TodosDomain>();
```

**Acceptance Criteria:**
- ✅ `TodosDomain` registered in DI container
- ✅ Can inject `IEnumerable<IDataDomain>` and TodosDomain appears
- ✅ Can inject `TodosDomain` directly

---

### **Phase 3: Convert Existing Components** ⬜ Not Started

#### **Task 3.1: Convert WelcomeWidget to IServerComponent** ⬜
**File:** `PagePlay.Site/Pages/Shared/WelcomeWidget.htmx.cs` (new file)

**Implementation:**
```csharp
namespace PagePlay.Site.Pages.Shared;

using PagePlay.Site.Infrastructure.Web.Components;

public interface IWelcomeWidget : IServerComponent { }

public class WelcomeWidget : IWelcomeWidget
{
    public string ComponentId => "welcome-widget";

    public DataDependencies Dependencies => DataDependencies
        .From("todos")
        .Require<int>("openCount");

    public string Render(IDataContext data)
    {
        var count = data.Get<int>("todos", "openCount");

        // language=html
        return $$"""
        <div id="{{ComponentId}}"
             data-component="WelcomeWidget"
             data-domain="todos">
            Welcome, you have {{count}} open todos
        </div>
        """;
    }
}
```

**Acceptance Criteria:**
- ✅ `WelcomeWidget` implements `IServerComponent`
- ✅ Declares dependency on "todos" domain, "openCount" key
- ✅ Renders HTML with correct data-attributes
- ✅ Gets data from `IDataContext`
- ✅ Registered in DI as `IWelcomeWidget`

---

#### **Task 3.2: Update Layout to Include WelcomeWidget** ⬜
**File:** `PagePlay.Site/Pages/Shared/Layout.htmx.cs`

**Changes:**
```csharp
public class Layout(
    IAntiforgeryTokenProvider _antiforgeryTokenProvider,
    INavView _nav,
    IWelcomeWidget _welcomeWidget  // ← NEW
) : IPageLayout
{
    // For now, Layout still renders synchronously
    // WelcomeWidget HTML will be passed in as string
    public string Render(string title, string bodyContent, string? welcomeWidgetHtml = null)
    {
        return $$"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
            <meta charset="utf-8" />
            <meta name="viewport" content="width=device-width, initial-scale=1.0" />
            <meta name="csrf-token" content="{{_antiforgeryTokenProvider.GetRequestToken()}}" />
            <title>{{title}} - PagePlay</title>
            <script src="https://unpkg.com/htmx.org@1.9.10"></script>
            <script src="/js/htmx-config.js"></script>
            <link rel="stylesheet" href="/css/site.css" />
        </head>
        <body>
            {{_nav.Render()}}
            {{welcomeWidgetHtml ?? ""}}
            <main>
                {{bodyContent}}
            </main>
            <script src="/js/csrf-setup.js"></script>
        </body>
        </html>
        """;
    }
}
```

**Acceptance Criteria:**
- ✅ Layout accepts optional `welcomeWidgetHtml` parameter
- ✅ Renders widget HTML between nav and main
- ✅ Backwards compatible (null = no widget)

---

### **Phase 4: Framework Orchestration** ⬜ Not Started

#### **Task 4.1: Create Data Loader Service** ⬜
**File:** `PagePlay.Site/Infrastructure/Web/Data/DataLoader.cs`

**Definition:**
```csharp
namespace PagePlay.Site.Infrastructure.Web.Data;

using PagePlay.Site.Infrastructure.Security;

/// <summary>
/// Loads data for specified domains in parallel.
/// </summary>
public interface IDataLoader
{
    /// <summary>
    /// Fetches data for all specified domains in parallel.
    /// Returns unified DataContext with all domain data.
    /// </summary>
    Task<IDataContext> LoadDomainsAsync(IEnumerable<string> domainNames);
}

public class DataLoader(
    IEnumerable<IDataDomain> _domains,
    IUserIdentityService _userIdentity
) : IDataLoader
{
    public async Task<IDataContext> LoadDomainsAsync(IEnumerable<string> domainNames)
    {
        var userId = _userIdentity.GetCurrentUserId();
        if (!userId.HasValue)
            throw new InvalidOperationException("User must be authenticated to load domain data");

        var domainList = domainNames.Distinct().ToList();
        var dataContext = new DataContext();

        // Find matching domains
        var domainsToLoad = _domains
            .Where(d => domainList.Contains(d.Name))
            .ToList();

        // Fetch all domains in parallel
        var fetchTasks = domainsToLoad.Select(domain =>
            domain.FetchAllAsync(userId.Value)
        );
        var results = await Task.WhenAll(fetchTasks);

        // Add to context
        for (int i = 0; i < domainsToLoad.Count; i++)
        {
            dataContext.AddDomain(domainsToLoad[i].Name, results[i]);
        }

        return dataContext;
    }
}
```

**Acceptance Criteria:**
- ✅ `IDataLoader` interface defined
- ✅ Fetches multiple domains in parallel (Task.WhenAll)
- ✅ Returns unified `IDataContext`
- ✅ Throws exception if user not authenticated
- ✅ Handles duplicate domain names (Distinct)
- ✅ Registered in DI

---

#### **Task 4.2: Create Component Factory** ⬜
**File:** `PagePlay.Site/Infrastructure/Web/Components/ComponentFactory.cs`

**Definition:**
```csharp
namespace PagePlay.Site.Infrastructure.Web.Components;

/// <summary>
/// Creates component instances from type names.
/// Used when re-rendering components from client context.
/// </summary>
public interface IComponentFactory
{
    /// <summary>
    /// Creates component instance by type name.
    /// Returns null if type not found or not a valid component.
    /// </summary>
    IServerComponent? Create(string componentTypeName);
}

public class ComponentFactory(IServiceProvider _serviceProvider) : IComponentFactory
{
    private static readonly Dictionary<string, Type> _componentTypes = new()
    {
        ["WelcomeWidget"] = typeof(IWelcomeWidget),
        // More components will be added as they're converted
    };

    public IServerComponent? Create(string componentTypeName)
    {
        if (!_componentTypes.TryGetValue(componentTypeName, out var componentType))
            return null;

        var instance = _serviceProvider.GetService(componentType);
        return instance as IServerComponent;
    }
}
```

**Acceptance Criteria:**
- ✅ `IComponentFactory` interface defined
- ✅ Can create component by string name
- ✅ Returns null for unknown types
- ✅ Uses DI to resolve component instances
- ✅ Registered in DI

**Note:** This registry approach is temporary. Future: use reflection or source generation.

---

#### **Task 4.3: Create Framework Orchestration Service** ⬜
**File:** `PagePlay.Site/Infrastructure/Web/Framework/FrameworkOrchestrator.cs`

**Definition:**
```csharp
namespace PagePlay.Site.Infrastructure.Web.Framework;

using PagePlay.Site.Infrastructure.Web.Components;
using PagePlay.Site.Infrastructure.Web.Data;
using PagePlay.Site.Infrastructure.Web.Mutations;

/// <summary>
/// Orchestrates framework operations: data loading, component rendering, OOB updates.
/// </summary>
public interface IFrameworkOrchestrator
{
    /// <summary>
    /// Loads data for all components and renders them.
    /// Used for initial page load.
    /// </summary>
    Task<Dictionary<string, string>> RenderComponentsAsync(IEnumerable<IServerComponent> components);

    /// <summary>
    /// Handles mutation response by re-rendering affected components.
    /// Used for interaction responses.
    /// </summary>
    Task<IResult> RenderMutationResponseAsync(
        DataMutations mutations,
        string? componentContextJson
    );
}

public class FrameworkOrchestrator(
    IDataLoader _dataLoader,
    IComponentContextParser _contextParser,
    IComponentFactory _componentFactory
) : IFrameworkOrchestrator
{
    public async Task<Dictionary<string, string>> RenderComponentsAsync(
        IEnumerable<IServerComponent> components)
    {
        var componentList = components.ToList();

        // 1. Collect all required domains
        var requiredDomains = componentList
            .Select(c => c.Dependencies.Domain)
            .Distinct()
            .ToList();

        // 2. Load all domains in parallel
        var dataContext = await _dataLoader.LoadDomainsAsync(requiredDomains);

        // 3. Render all components
        var renderedComponents = new Dictionary<string, string>();
        foreach (var component in componentList)
        {
            var html = component.Render(dataContext);
            renderedComponents[component.ComponentId] = html;
        }

        return renderedComponents;
    }

    public async Task<IResult> RenderMutationResponseAsync(
        DataMutations mutations,
        string? componentContextJson)
    {
        // 1. Parse component context from client
        var pageComponents = _contextParser.Parse(componentContextJson);

        // 2. Find components affected by mutation
        var affectedComponents = pageComponents
            .Where(c => mutations.Domains.Contains(c.Domain))
            .ToList();

        if (affectedComponents.Count == 0)
            return Results.Ok(); // No components to update

        // 3. Re-fetch mutated domains
        var dataContext = await _dataLoader.LoadDomainsAsync(mutations.Domains);

        // 4. Re-render affected components
        var updates = new List<string>();
        foreach (var componentInfo in affectedComponents)
        {
            var component = _componentFactory.Create(componentInfo.ComponentType);
            if (component == null) continue;

            var html = component.Render(dataContext);

            // Wrap with OOB swap directive
            var oobHtml = $$"""
            <div id="{{componentInfo.Id}}" hx-swap-oob="true">
                {{html}}
            </div>
            """;

            updates.Add(oobHtml);
        }

        return Results.Content(string.Join("\n", updates), "text/html");
    }
}
```

**Acceptance Criteria:**
- ✅ `IFrameworkOrchestrator` interface defined
- ✅ `RenderComponentsAsync()` loads domains and renders components
- ✅ `RenderMutationResponseAsync()` handles OOB updates
- ✅ Wraps updated HTML with `hx-swap-oob="true"`
- ✅ Returns empty OK if no components affected
- ✅ Registered in DI

---

### **Phase 5: Update Todos Page** ⬜ Not Started

#### **Task 5.1: Update TodosPageEndpoints to Use Framework** ⬜
**File:** `PagePlay.Site/Pages/Todos/Todos.Route.cs`

**Changes:**
```csharp
public class TodosPageEndpoints(
    IPageLayout _layout,
    ITodosPageView _page,
    IEnumerable<ITodosPageInteraction> _interactions,
    IWelcomeWidget _welcomeWidget,  // ← NEW
    IFrameworkOrchestrator _framework  // ← NEW
) : IClientEndpoint
{
    public const string PAGE_ROUTE = "todos";

    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(PAGE_ROUTE, async (
            IWorkflow<ListTodosWorkflowRequest, ListTodosWorkflowResponse> listWorkflow
        ) =>
        {
            // Define page components
            var components = new IServerComponent[] { _welcomeWidget };

            // Framework loads data and renders components
            var renderedComponents = await _framework.RenderComponentsAsync(components);
            var welcomeHtml = renderedComponents[_welcomeWidget.ComponentId];

            // Render todos (existing pattern for now)
            var result = await listWorkflow.Perform(new ListTodosWorkflowRequest());
            var todosHtml = !result.Success
                ? _page.RenderError("Failed to load todos")
                : _page.RenderPage(result.Model.Todos);

            // Combine everything
            var bodyContent = welcomeHtml + todosHtml;
            var page = _layout.Render("Todos", bodyContent);

            return Results.Content(page, "text/html");
        })
        .RequireAuthenticatedUser();

        foreach (var interaction in _interactions)
            interaction.Map(endpoints);
    }
}
```

**Acceptance Criteria:**
- ✅ Uses `IFrameworkOrchestrator` to render WelcomeWidget
- ✅ Still renders todos using existing workflow (no breaking changes yet)
- ✅ Page loads successfully with both welcome widget and todo list
- ✅ Welcome widget shows correct open todo count

---

#### **Task 5.2: Update CreateTodoInteraction to Use Mutations** ⬜
**File:** `PagePlay.Site/Pages/Todos/Interactions/CreateTodo.Interaction.cs`

**Changes:**
```csharp
using PagePlay.Site.Infrastructure.Web.Framework;
using PagePlay.Site.Infrastructure.Web.Mutations;

public class CreateTodoInteraction(
    ITodosPageView page,
    IFrameworkOrchestrator _framework  // ← NEW
) : PageInteractionBase<CreateTodoWorkflowRequest, CreateTodoWorkflowResponse, ITodosPageView>(page),
      ITodosPageInteraction
{
    protected override string RouteBase => TodosPageEndpoints.PAGE_ROUTE;
    protected override string Action => "create";

    // NEW: Declare what this interaction mutates
    protected virtual DataMutations Mutates => DataMutations.For("todos");

    protected override async Task<IResult> OnSuccess(CreateTodoWorkflowResponse response)
    {
        // Get component context from request header
        var contextHeader = HttpContext.Request.Headers["X-Component-Context"].ToString();

        // Framework handles re-rendering
        return await _framework.RenderMutationResponseAsync(Mutates, contextHeader);
    }

    protected override IResult RenderError(string message) =>
        Results.Content(Page.RenderErrorNotification(message), "text/html");
}
```

**Wait - this reveals we need HttpContext access in PageInteractionBase!**

**Required:** Update `PageInteractionBase` to expose `HttpContext`.

**Acceptance Criteria:**
- ✅ `CreateTodoInteraction` declares `Mutates` property
- ✅ Uses `IFrameworkOrchestrator` for success response
- ✅ Can access `X-Component-Context` header
- ✅ Creating todo updates welcome widget count via OOB

---

#### **Task 5.3: Add HttpContext to PageInteractionBase** ⬜
**File:** `PagePlay.Site/Infrastructure/Web/Pages/PageInteractionBase.cs`

**Changes:**
```csharp
public abstract class PageInteractionBase<TRequest, TResponse, TView> : IEndpoint
    where TRequest : IWorkflowRequest
    where TResponse : IWorkflowResponse
    where TView : class
{
    protected readonly TView Page;
    protected HttpContext HttpContext { get; private set; } = null!;  // ← NEW

    // ... existing code ...

    private async Task<IResult> Handle(
        [FromForm] TRequest request,
        IWorkflow<TRequest, TResponse> workflow,
        HttpContext httpContext  // ← NEW parameter
    )
    {
        HttpContext = httpContext;  // ← Store for access in derived classes

        var result = await workflow.Perform(request);

        return result.Success
            ? OnSuccess(result.Model)
            : OnError(result.Errors);
    }
}
```

**Acceptance Criteria:**
- ✅ `HttpContext` property added to base class
- ✅ Populated during `Handle()` execution
- ✅ Accessible in derived class `OnSuccess()` methods
- ✅ All existing interactions still compile and work

---

### **Phase 6: Client-Side HTMX Extension** ⬜ Not Started

#### **Task 6.1: Create Component Context Extension** ⬜
**File:** `PagePlay.Site/wwwroot/js/component-context.js` (new file)

**Implementation:**
```javascript
// HTMX extension that sends component context with each request
htmx.defineExtension('component-context', {
    onEvent: function(name, evt) {
        if (name === 'htmx:configRequest') {
            // Find all components on page
            const components = document.querySelectorAll('[data-component]');

            // Build context array
            const context = Array.from(components).map(el => ({
                id: el.id,
                type: el.dataset.component,
                domain: el.dataset.domain
            }));

            // Add to request headers
            evt.detail.headers['X-Component-Context'] = JSON.stringify(context);
        }
    }
});
```

**Acceptance Criteria:**
- ✅ Extension defined and loaded
- ✅ Finds all `[data-component]` elements
- ✅ Extracts id, type, domain from data attributes
- ✅ Sends as `X-Component-Context` header
- ✅ JSON is valid and parseable

---

#### **Task 6.2: Load Extension in Layout** ⬜
**File:** `PagePlay.Site/Pages/Shared/Layout.htmx.cs`

**Changes:**
```html
<script src="https://unpkg.com/htmx.org@1.9.10"></script>
<script src="/js/component-context.js"></script>  <!-- ← NEW -->
<script src="/js/htmx-config.js"></script>
```

**And update body:**
```html
<body hx-ext="component-context">  <!-- ← NEW: Enable extension -->
```

**Acceptance Criteria:**
- ✅ Extension script loaded before htmx-config.js
- ✅ Extension enabled on body tag
- ✅ All HTMX requests include X-Component-Context header
- ✅ Can see header in browser dev tools

---

### **Phase 7: Testing & Validation** ⬜ Not Started

#### **Task 7.1: Manual Testing Checklist** ⬜

**Test Cases:**
1. ✅ Load /todos page
   - Welcome widget appears
   - Shows correct open todo count
   - Todo list displays

2. ✅ Create new todo
   - Todo appears in list
   - Welcome widget count increments (OOB update)
   - No full page refresh

3. ✅ Toggle todo completion
   - Todo visual state updates
   - Welcome widget count decrements (OOB update)

4. ✅ Delete todo
   - Todo removed from list
   - Welcome widget count updates (OOB update)

5. ✅ Open browser dev tools
   - Verify `X-Component-Context` header on POST requests
   - Verify OOB responses contain updated component HTML
   - Verify `hx-swap-oob="true"` in responses

6. ✅ Test with authentication
   - Logged out: welcome widget shows generic message (or doesn't appear)
   - Logged in: shows personalized count

---

#### **Task 7.2: Write Unit Tests** ⬜

**Test Files to Create:**

1. `TodosDomain.Tests.cs`
   - ✅ FetchAllAsync returns correct data structure
   - ✅ Handles empty todo list
   - ✅ Calculates openCount correctly
   - ✅ Calculates completionRate correctly

2. `DataLoader.Tests.cs`
   - ✅ Loads single domain
   - ✅ Loads multiple domains in parallel
   - ✅ Handles unknown domain gracefully
   - ✅ Requires authenticated user

3. `ComponentContextParser.Tests.cs`
   - ✅ Parses valid JSON
   - ✅ Returns empty list for null input
   - ✅ Returns empty list for invalid JSON
   - ✅ Handles malformed data gracefully

4. `FrameworkOrchestrator.Tests.cs`
   - ✅ RenderComponentsAsync loads correct domains
   - ✅ RenderMutationResponseAsync finds affected components
   - ✅ Returns OOB-wrapped HTML
   - ✅ Returns OK if no components affected

---

### **Phase 8: Documentation** ⬜ Not Started

#### **Task 8.1: Update Architecture Docs** ⬜
**File:** `.claude/docs/README.DOMAIN_DATA_MANIFESTS.md` (new file)

**Content:**
- Explanation of domain-level data architecture
- How to create new domains
- How to create components that depend on domains
- How interactions declare mutations
- Diagram of data flow
- Examples

---

#### **Task 8.2: Update Developer Workflow Docs** ⬜
**File:** `.claude/docs/workflow/implementation-mode.md`

**Add section:**
- How to add new page components
- How to create data domains
- How to declare mutations in interactions

---

### **Phase 9: Cleanup & Polish** ⬜ Not Started

#### **Task 9.1: Remove Temporary Code** ⬜
- Remove ComponentFactory registry (replace with reflection or source gen)
- Consider removing IWelcomeWidget interface (use concrete type?)
- Clean up any TODO comments added during implementation

---

#### **Task 9.2: Performance Review** ⬜
- Measure page load time (before/after)
- Measure interaction response time
- Verify parallel domain loading works
- Profile with 10+ components on page

---

## 🚦 **Current Status**

**Active Phase:** Phase 1 - Core Infrastructure
**Next Task:** Task 1.4 - Create Component Context Parser
**Blockers:** None
**Completed:** Task 1.1 ✅, Task 1.2 ✅, Task 1.3 ✅

---

## 📝 **Notes & Decisions**

### **Session 1 (2025-11-29)**
- Decided on domain-level mutations (not granular keys)
- Chose client-side context approach (not server session)
- Will implement request caching LATER (not part of this experiment)
- Framework should over-fetch for now (domain fetches all, optimize later)
- Only visible components for production, but all components for now
- Open to HTMX extensions if they perfect concepts

### **Design Decisions**
1. **Why domains, not granular keys?**
   - Prevents coupling between interactions and components
   - Adding new component doesn't require updating interactions
   - Domain owns its data shape (centralized)

2. **Why client-side context?**
   - No server-side session storage needed
   - Always reflects current page state
   - Works with multiple tabs/windows
   - Components can mount/unmount dynamically

3. **Why two-phase rendering?**
   - Clear separation: data loading (async) vs rendering (pure)
   - Enables parallel domain fetching
   - Makes rendering testable (pure functions)
   - Compile-time safety (can't query during render)

4. **Why over-fetch initially?**
   - Simpler implementation
   - Premature optimization is root of all evil
   - Can add caching/smart-fetching later
   - Validates architecture first

---

## 🔄 **Session Handoff Protocol**

**When resuming work on this experiment:**

1. Read this document top-to-bottom
2. Check "Current Status" section for active phase/task
3. Review "Notes & Decisions" for context
4. Locate the next ⬜ unchecked task
5. Read that task's:
   - Definition/implementation
   - Acceptance criteria
   - File path
6. Implement and test
7. Mark task ✅ when complete
8. Update "Current Status" section
9. Add any new decisions/learnings to "Notes & Decisions"

**Do NOT:**
- Skip ahead to later phases
- Deviate from defined architecture
- Add features not in this document
- Change decisions without updating "Notes & Decisions"

---

## 🎯 **Success Criteria for Experiment**

This experiment is considered successful when:

1. ✅ Welcome widget displays on todos page with correct count
2. ✅ Creating todo updates welcome widget via OOB (no full page refresh)
3. ✅ Toggling/deleting todo updates welcome widget via OOB
4. ✅ No N+1 queries (domain fetches data once)
5. ✅ No coupling (can add new component without changing interactions)
6. ✅ All acceptance criteria met for all tasks
7. ✅ Manual testing checklist passes
8. ✅ Unit tests written and passing

**If successful:** Consider converting more components and pages.
**If unsuccessful:** Document learnings and pivot approach.

---

**Last Updated:** 2025-11-29
**Document Version:** 1.0
