# Read/Write Pattern Guide

**Version:** 1.0
**Last Updated:** 2025-11-29
**Status:** Active Pattern

---

## Core Principle

**Clear separation following CQRS principles:**
- **DataDomains (Loaders)** = All READ operations (queries)
- **Workflows** = All WRITE operations (commands - Create, Update, Delete)

This eliminates duplication and creates self-enforcing patterns where the right choice is obvious.

---

## Decision Tree

### Should I create a Workflow or use a DataDomain?

```
Is this operation changing data?
├─ YES → Create a Workflow
│  ├─ Creating new records? → CreateX workflow
│  ├─ Updating existing records? → UpdateX workflow
│  ├─ Deleting records? → DeleteX workflow
│  └─ Complex business operation? → PerformX workflow
│
└─ NO → Use DataDomain via DataLoader
   ├─ New data context? → Create new DataDomain
   ├─ Existing domain has data? → Use existing DataDomain
   └─ Need different shape? → Extend DataDomain context
```

---

## When to Create a Workflow

**Create a workflow when you need to:**
- Create new records (CreateTodo, RegisterUser)
- Update existing records (UpdateTodo, ChangePassword)
- Delete records (DeleteTodo, ArchiveProject)
- Perform business operations that mutate state (ToggleTodo, PublishPost)

**Workflow characteristics:**
- Named after business actions (verbs: Create, Update, Delete, Toggle, Publish)
- Contains validation, authorization, business logic
- Mutates database state
- Returns success/failure result
- Located in `Application/{Domain}/{Operation}/` folder

**Example:**

```csharp
// ✅ Correct - Mutation workflow
public class CreateTodoWorkflow(
    ITodoRepository _repository,
    IValidator<CreateTodoRequest> _validator,
    ICurrentUserContext _currentUser
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

    private async Task<ValidationResult> validate(CreateTodoRequest request) =>
        await _validator.ValidateAsync(request);

    private Todo createTodo(CreateTodoRequest request) =>
        Todo.Create(request.Text, _currentUser.UserId.Value);

    private async Task saveTodo(Todo todo)
    {
        await _repository.Add(todo);
        await _repository.SaveChanges();
    }

    private IApplicationResult<CreateTodoResponse> buildErrorResponse(ValidationResult result) =>
        ApplicationResult<CreateTodoResponse>.Fail(result);

    private IApplicationResult<CreateTodoResponse> buildSuccessResponse() =>
        ApplicationResult<CreateTodoResponse>.Succeed(new CreateTodoResponse());
}
```

---

## When to Use DataDomain

**Use DataDomain when you need to:**
- Display data on a page
- Render data in a component
- Fetch data after a mutation (OOB updates)
- Query data for read-only operations

**DataDomain characteristics:**
- Named after data contexts (nouns: Todos, Analytics, UserProfile)
- Contains only data fetching logic
- No mutations, no business logic
- Returns strongly-typed context objects
- Located in `Application/{Domain}.Domain/` folder

**Example:**

```csharp
// ✅ Correct - Read-only domain
public class TodosDomain(ITodoRepository _repository) : IDataDomain<TodosDomainContext>
{
    public string Name => "todos";

    public async Task<TodosDomainContext> FetchTypedAsync(long userId)
    {
        var todos = await fetchTodos(userId);
        var openCount = calculateOpenCount(todos);
        var totalCount = todos.Count;
        var completionRate = calculateCompletionRate(totalCount, openCount);

        return new TodosDomainContext
        {
            List = todos.Select(mapToListEntry).ToList(),
            OpenCount = openCount,
            TotalCount = totalCount,
            CompletionRate = completionRate
        };
    }

    private async Task<List<Todo>> fetchTodos(long userId) =>
        await _repository.List(Todo.ByUserId(userId));

    private int calculateOpenCount(List<Todo> todos) =>
        todos.Count(t => !t.IsCompleted);

    private double calculateCompletionRate(int total, int open) =>
        total == 0 ? 0 : (total - open) / (double)total;

    private TodoListEntry mapToListEntry(Todo todo) =>
        new TodoListEntry
        {
            Id = todo.Id,
            Text = todo.Text,
            IsCompleted = todo.IsCompleted,
            CreatedAt = todo.CreatedAt
        };
}

// Context returned by the domain
public class TodosDomainContext
{
    public List<TodoListEntry> List { get; set; } = new();
    public int OpenCount { get; set; }
    public int TotalCount { get; set; }
    public double CompletionRate { get; set; }
}
```

---

## Using DataDomains in Pages

**Pattern:**

```csharp
public class TodosPageEndpoints(
    IPageLayout _layout,
    ITodosPageView _page,
    ILogger<TodosPageEndpoints> _logger
) : IClientEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("todos", async (IDataLoader dataLoader) =>
        {
            try
            {
                // Fetch data via DataLoader
                var dataContext = await dataLoader.LoadDomainsAsync(new[] { "todos" });
                var todosData = dataContext.GetDomain<TodosDomainContext>("todos");

                // Render page
                var bodyContent = _page.RenderPage(todosData.List);
                var page = await _layout.RenderAsync("Todos", bodyContent);
                return Results.Content(page, "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load todos page");
                var bodyContent = _page.RenderError("Failed to load todos");
                var page = await _layout.RenderAsync("Todos", bodyContent);
                return Results.Content(page, "text/html");
            }
        })
        .RequireAuthenticatedUser();
    }
}
```

---

## Using DataDomains in Components

**Pattern:**

```csharp
public class WelcomeWidget(ICurrentUserContext _currentUserContext) : IWelcomeWidget
{
    public string ComponentId => "welcome-widget";

    // Declare data dependency
    public DataDependencies Dependencies =>
        DataDependencies.From<TodosDomain, TodosDomainContext>();

    // Render using pre-loaded data
    public string Render(IDataContext data)
    {
        var todosData = data.GetDomain<TodosDomainContext>("todos");

        return $@"
            <div id=""{ComponentId}"" class=""welcome-widget"">
                <h2>Welcome back!</h2>
                <p>You have {todosData.OpenCount} todos to complete.</p>
            </div>
        ";
    }
}
```

---

## Common Mistakes

### ❌ Incorrect: Creating "List" Workflows

```csharp
// ❌ Don't do this - reads don't belong in workflows
public class ListTodosWorkflow(ITodoRepository _repository)
    : IWorkflow<ListTodosRequest, ListTodosResponse>
{
    public async Task<IApplicationResult<ListTodosResponse>> Perform(ListTodosRequest request)
    {
        var todos = await _repository.List(Todo.ByUserId(request.UserId));
        return ApplicationResult<ListTodosResponse>.Succeed(
            new ListTodosResponse { Todos = todos }
        );
    }
}

// ✅ Do this instead - use DataDomain
public class TodosDomain(ITodoRepository _repository) : IDataDomain<TodosDomainContext>
{
    public async Task<TodosDomainContext> FetchTypedAsync(long userId)
    {
        var todos = await _repository.List(Todo.ByUserId(userId));
        return new TodosDomainContext { List = todos };
    }
}
```

### ❌ Incorrect: Mutating in DataDomains

```csharp
// ❌ Don't do this - mutations don't belong in domains
public class TodosDomain(ITodoRepository _repository) : IDataDomain<TodosDomainContext>
{
    public async Task<TodosDomainContext> FetchTypedAsync(long userId)
    {
        var todos = await _repository.List(Todo.ByUserId(userId));

        // ❌ NO! Don't mutate in domain
        foreach (var todo in todos.Where(t => t.IsOld()))
            await _repository.Delete(todo);

        return new TodosDomainContext { List = todos };
    }
}

// ✅ Do this instead - create workflow for mutations
public class ArchiveOldTodosWorkflow(ITodoRepository _repository)
    : IWorkflow<ArchiveOldTodosRequest, ArchiveOldTodosResponse>
{
    public async Task<IApplicationResult<ArchiveOldTodosResponse>> Perform(
        ArchiveOldTodosRequest request)
    {
        var oldTodos = await findOldTodos();
        await archiveTodos(oldTodos);
        return buildSuccessResponse();
    }
}
```

### ❌ Incorrect: Fetching Data in Workflows

```csharp
// ❌ Don't do this - workflows shouldn't fetch for display
public class GetTodoDetailsWorkflow(ITodoRepository _repository)
    : IWorkflow<GetTodoDetailsRequest, GetTodoDetailsResponse>
{
    public async Task<IApplicationResult<GetTodoDetailsResponse>> Perform(
        GetTodoDetailsRequest request)
    {
        // ❌ This is just reading data - use DataDomain instead
        var todo = await _repository.GetById(request.TodoId);
        return ApplicationResult<GetTodoDetailsResponse>.Succeed(
            new GetTodoDetailsResponse { Todo = todo }
        );
    }
}

// ✅ Do this instead - extend or create DataDomain
public class TodoDetailsDomain(ITodoRepository _repository)
    : IDataDomain<TodoDetailsDomainContext>
{
    public string Name => "todoDetails";

    public async Task<TodoDetailsDomainContext> FetchTypedAsync(long userId)
    {
        // Fetch details for rendering
        var todo = await _repository.GetById(userId);
        return new TodoDetailsDomainContext { Todo = todo };
    }
}
```

---

## When to Extend vs Create New Domain

### Extend Existing Domain When:
- New data fits the same conceptual context (Todos domain)
- Components need related data from same domain
- Data comes from same repository/aggregate

### Create New Domain When:
- Data represents different conceptual context (Analytics vs Todos)
- Data comes from different aggregate root
- Data has different access patterns or performance characteristics

**Example:**

```csharp
// ✅ Correct - TodosDomain contains todo list data
public class TodosDomain : IDataDomain<TodosDomainContext>
{
    // List, counts, completion rate - all "todo list" concerns
}

// ✅ Correct - TodoAnalyticsDomain contains analytics data
public class TodoAnalyticsDomain : IDataDomain<TodoAnalyticsDomainContext>
{
    // Trends, streaks, insights - separate "analytics" concern
}

// ❌ Incorrect - Don't combine unrelated concerns
public class TodosAndAnalyticsDomain : IDataDomain<TodosAndAnalyticsContext>
{
    // Mixed concerns - hard to reason about
}
```

---

## Migration Checklist

When refactoring from workflows to DataDomains:

- [ ] Identify all "List" or "Get" workflows
- [ ] Check if workflow only reads data (no mutations)
- [ ] Create or extend DataDomain for the data
- [ ] Update page routes to use DataLoader
- [ ] Update components to declare dependencies
- [ ] Delete the workflow files
- [ ] Verify no remaining references to workflow
- [ ] Test page loads and component rendering

---

## Benefits of This Pattern

### Eliminates Duplication
- One place for reading data (DataDomains)
- One place for mutating data (Workflows)
- No confusion about where code belongs

### Self-Enforcing
- Obvious which pattern to use
- Framework guides you to correct choice
- Violations feel awkward and wrong

### Consistent Complexity
- All reads follow same pattern
- All writes follow same pattern
- Predictable structure across codebase

### Game-Style Data Pre-Fetching
- DataLoader fetches all domains in parallel
- No N+1 queries, no waterfalls
- Clean "loading → interactive" transition

### Clear Business Intent
- Workflows named after business actions (CreateTodo)
- Domains named after data contexts (TodosDomain)
- Code reveals what system does

---

## Summary

**Read operations:**
- ✅ Use DataDomain via DataLoader
- ✅ Named after data contexts (nouns)
- ✅ Returns strongly-typed context
- ❌ Never create "List" or "Get" workflows

**Write operations:**
- ✅ Create a Workflow
- ✅ Named after business actions (verbs)
- ✅ Contains validation and business logic
- ❌ Never mutate in DataDomains

**When in doubt:**
- Does it change data? → Workflow
- Does it just read data? → DataDomain

---

**Next:** Apply this pattern consistently across all features. The right way is the easy way.
