# Todo Domains - Multi-Domain Pattern Example

This directory demonstrates the **multi-domain pattern** for separating data concerns by computational cost and usage frequency.

## Domain Separation

### TodosDomain (`"todos"`)
**Purpose**: Fast queries for basic CRUD operations

**Provides**:
- `list` - List<TodoListEntry> (sorted by completion, then creation date)
- `openCount` - int (number of incomplete todos)
- `totalCount` - int (total number of todos)
- `completionRate` - double (percentage completed, 0.0-1.0)

**Query Cost**: Low (single query + simple filters/sorts)

**Used By**:
- Todos list page (`Pages/Todos/Todos.Page.htmx.cs`)
- WelcomeWidget (`Pages/Shared/WelcomeWidget.htmx.cs`)

**Mutated By**:
- CreateTodo interaction → `DataMutations.For("todos")`
- ToggleTodo interaction → `DataMutations.For("todos")`
- DeleteTodo interaction → `DataMutations.For("todos")`

---

### TodoAnalyticsDomain (`"todoAnalytics"`)
**Purpose**: Expensive analytics for dashboard/reporting

**Provides**:
- `completionTrend` - Dictionary<string, int> (last 7 days completion counts)
- `longestStreak` - int (longest consecutive completion days)
- `averageCompletionTime` - double (avg hours from creation to completion)
- `productivityScore` - int (completion percentage as 0-100)
- `weeklyBreakdown` - Dictionary<string, object> (created vs completed this week)

**Query Cost**: High (complex aggregations, historical analysis)

**Used By**:
- AnalyticsStatsWidget (`Pages/Shared/AnalyticsStatsWidget.htmx.cs`)
- Future analytics dashboard pages

**Mutated By**:
- Would require explicit mutation: `DataMutations.For("todos", "todoAnalytics")`
- Currently NOT mutated by basic CRUD operations (by design)

---

## Why Separate These Domains?

### Problem: Monolithic Domain
```csharp
// Bad: One domain that does everything
public class TodosDomain : IDataDomain
{
    public async Task<DomainDataContext> FetchAllAsync(long userId)
    {
        var todos = await _repository.List(Todo.ByUserId(userId));

        // Simple data (always needed)
        var openCount = todos.Count(t => !t.IsCompleted);

        // Expensive analytics (rarely needed)
        var completionTrend = calculateCompletionTrend(todos); // 🔥 Expensive!
        var longestStreak = calculateLongestStreak(todos);     // 🔥 Expensive!

        // Every interaction that mutates "todos" pays this cost
        // Even simple CRUD pages that don't show analytics
    }
}
```

**Result**: Every todo creation/toggle/delete triggers expensive analytics calculations, even when no analytics components are on the page.

### Solution: Domain Separation
```csharp
// Good: Separate by cost and usage
TodosDomain          → Fast, used on every page
TodoAnalyticsDomain  → Expensive, used only on analytics pages
```

**Benefits**:
1. **Performance**: CRUD pages don't pay for analytics they don't use
2. **Clarity**: Each domain has clear responsibility
3. **Independent Evolution**: Change analytics without touching CRUD
4. **Selective Updates**: Interactions choose which domains to mutate

---

## Usage Examples

### Example 1: Basic CRUD Page (Only TodosDomain)

**Component declares dependency**:
```csharp
public class WelcomeWidget : IServerComponent
{
    public DataDependencies Dependencies => DataDependencies
        .From("todos")
        .Require<int>("openCount");

    public string Render(IDataContext data)
    {
        var count = data.Get<int>("todos", "openCount");
        return $"<p>You have {count} open todos</p>";
    }
}
```

**Interaction mutates domain**:
```csharp
public class CreateTodoInteraction : PageInteractionBase
{
    protected virtual DataMutations Mutates => DataMutations.For("todos");
    // Only TodosDomain.FetchAllAsync() is called
    // Only WelcomeWidget is re-rendered
    // AnalyticsStatsWidget is NOT re-rendered (different domain)
}
```

**Flow**:
1. User creates todo
2. Framework checks: which components depend on `"todos"`? → WelcomeWidget
3. Framework fetches: `TodosDomain.FetchAllAsync()` (fast)
4. Framework re-renders: WelcomeWidget with new count
5. Analytics NOT touched (not mutated, not needed)

---

### Example 2: Analytics Dashboard (Multiple Domains)

**Component declares dependency**:
```csharp
public class AnalyticsStatsWidget : IServerComponent
{
    public DataDependencies Dependencies => DataDependencies
        .From("todoAnalytics")
        .Require<int>("productivityScore")
        .Require<int>("longestStreak");

    public string Render(IDataContext data)
    {
        var score = data.Get<int>("todoAnalytics", "productivityScore");
        var streak = data.Get<int>("todoAnalytics", "longestStreak");
        return $"<p>Score: {score}%, Streak: {streak} days</p>";
    }
}
```

**Hypothetical interaction that affects both**:
```csharp
public class CompleteMultipleTodosInteraction : PageInteractionBase
{
    // Affects both domains because completing multiple todos
    // significantly changes analytics (streak, productivity score)
    protected virtual DataMutations Mutates =>
        DataMutations.For("todos", "todoAnalytics");

    // Framework will:
    // 1. Fetch TodosDomain.FetchAllAsync()
    // 2. Fetch TodoAnalyticsDomain.FetchAllAsync() (in parallel)
    // 3. Re-render components depending on "todos" (WelcomeWidget)
    // 4. Re-render components depending on "todoAnalytics" (AnalyticsStatsWidget)
}
```

---

## When to Create a New Domain?

### ✅ Create New Domain When:
- **Computational cost differs significantly**
  - Simple count (< 1ms) vs complex aggregation (> 50ms)
- **Different pages need different data**
  - CRUD page needs list, analytics page needs trends
- **Usage frequency differs**
  - List used on every page, analytics used on one dashboard
- **Independent bounded contexts**
  - Todos vs Notifications vs Accounts

### ❌ Keep in Same Domain When:
- Data always fetched together
- Similar computational cost
- Tightly coupled lifecycle
- Simple derivations (filter, sort, count)

---

## Pattern Rules

### Rule 1: Each Domain Fetches Once Per Request
```csharp
// Framework automatically deduplicates
DataMutations.For("todos", "todos")
// TodosDomain.FetchAllAsync() called ONCE, not twice
```

### Rule 2: Domains are Fetched in Parallel
```csharp
DataMutations.For("todos", "todoAnalytics", "notifications")
// All three FetchAllAsync() methods run concurrently via Task.WhenAll()
```

### Rule 3: Components Depend on ONE Domain
```csharp
// Current implementation constraint
public DataDependencies Dependencies => DataDependencies
    .From("todos")  // Can only specify one domain
    .Require<int>("openCount");
```

If you need data from multiple domains, create a higher-level domain or multiple components.

### Rule 4: Domains Own Their Data Shape
```csharp
// TodosDomain decides what "openCount" means
context["openCount"] = todos.Count(t => !t.IsCompleted);

// Components consume what the domain provides
var count = data.Get<int>("todos", "openCount");
```

Interactions don't control the shape—domains do. This decouples interactions from components.

---

## Testing Strategy

### Unit Test: Domain in Isolation
```csharp
[Fact]
public async Task FetchAllAsync_CalculatesProductivityScore()
{
    var domain = new TodoAnalyticsDomain(_mockRepository);

    var result = await domain.FetchAllAsync(userId: 1);

    var score = result.Get<int>("productivityScore");
    Assert.Equal(75, score); // 3 of 4 completed = 75%
}
```

### Integration Test: Component + Domain
```csharp
[Fact]
public void Render_WithAnalyticsData_ShowsScore()
{
    var widget = new AnalyticsStatsWidget();
    var dataContext = new DataContext();
    dataContext.AddDomain("todoAnalytics", CreateAnalyticsContext());

    var html = widget.Render(dataContext);

    Assert.Contains("75%", html);
}
```

---

## Current Implementation Status

| Domain | Status | Used By | Mutated By |
|--------|--------|---------|------------|
| `todos` | ✅ Implemented | WelcomeWidget, Todos page | All todo CRUD interactions |
| `todoAnalytics` | ✅ Implemented (demo only) | AnalyticsStatsWidget | None (would need explicit mutation) |

The `todoAnalytics` domain and `AnalyticsStatsWidget` are **demonstration code** showing the multi-domain pattern. They are registered and functional, but not currently used in the application.

---

## Future Considerations

### Caching Strategy (Not Yet Implemented)
```csharp
// Potential optimization: cache within request scope
public class TodoAnalyticsDomain : IDataDomain
{
    private DomainDataContext? _cachedContext;

    public async Task<DomainDataContext> FetchAllAsync(long userId)
    {
        if (_cachedContext != null) return _cachedContext;

        // Expensive calculation only once per request
        _cachedContext = await expensiveAnalyticsCalculation(userId);
        return _cachedContext;
    }
}
```

### Cross-Domain Queries (Not Supported)
Currently each domain queries independently. If analytics needs data from multiple sources, it must query them all:

```csharp
public async Task<DomainDataContext> FetchAllAsync(long userId)
{
    var todos = await _todoRepository.List(...);
    var projects = await _projectRepository.List(...); // Separate query

    var crossDomainAnalytics = analyzeTodosAndProjects(todos, projects);
    // ...
}
```

Alternative: Create a higher-level domain that composes others (not yet implemented).

---

## Summary

This multi-domain pattern demonstrates:
- ✅ Multiple domains can coexist (`todos`, `todoAnalytics`)
- ✅ Components declare single domain dependency
- ✅ Interactions can mutate multiple domains
- ✅ Framework handles parallel fetching and selective re-rendering
- ✅ Performance optimization through domain separation
- ✅ Clean bounded contexts

The pattern scales as your application grows—add new domains for new contexts without coupling to existing ones.
