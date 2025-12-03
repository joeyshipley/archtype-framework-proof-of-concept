# Experiment: Component-First Architecture with Pure OOB & Minimal Workflow Responses

**Status:** Planning
**Created:** 2025-12-02
**Goal:** Establish component-first as the primary pattern with pure OOB updates and minimal workflow responses

---

## 🎯 Context Loading Instructions

To resume this exploration in a new session, read these files in order:

### Core Philosophy & Patterns
1. `.claude/docs/README.md` - Project philosophy and team velocity principles
2. `.claude/docs/README.CONSISTENT_COMPLEXITY_DESIGN.md` - Vertical slices and self-enforcing patterns
3. `.claude/docs/README.WEB_FRAMEWORK.md` - HTTP-First and turn-based architecture
4. `.claude/docs/README.SYNTAX_STYLE.md` - Code conventions

### Current Architecture (Components & Data)
5. `PagePlay.Site/Infrastructure/Web/Components/IServerComponent.cs` - Component interface and DataDependencies
6. `PagePlay.Site/Infrastructure/Web/Framework/FrameworkOrchestrator.cs` - Component rendering and OOB update system
7. `PagePlay.Site/Infrastructure/Web/Pages/PageInteractionBase.cs` - Base class for interactions
8. `PagePlay.Site/Infrastructure/Web/Html/HtmxForm.cs` - Form rendering with target attributes
9. `PagePlay.Site/Pages/Shared/Elements/Button.htmx.cs` - Button helper with target support

### Current Examples (Fragment Pattern)
10. `PagePlay.Site/Pages/Todos/Todos.Page.htmx.cs` - Page view with rendering methods
11. `PagePlay.Site/Pages/Todos/Todos.Route.cs` - Endpoint with manual data loading
12. `PagePlay.Site/Pages/Todos/Interactions/CreateTodo.Interaction.cs` - Create interaction
13. `PagePlay.Site/Pages/Todos/Interactions/ToggleTodo.Interaction.cs` - Toggle interaction (whole list rerender)
14. `PagePlay.Site/Pages/Todos/Interactions/DeleteTodo.Interaction.cs` - Delete interaction

### Component Examples (Reference)
15. `PagePlay.Site/Pages/Shared/WelcomeWidget.htmx.cs` - Example component with data dependencies
16. `PagePlay.Site/Pages/Shared/AnalyticsStatsWidget.htmx.cs` - Example with separate domain

### Workflows (Current Pattern)
17. `PagePlay.Site/Application/Todos/Workflows/CreateTodo/CreateTodo.Workflow.cs` - Returns TodoListEntry
18. `PagePlay.Site/Application/Todos/Workflows/ToggleTodo/ToggleTodo.Workflow.cs` - Returns List<TodoListEntry>
19. `PagePlay.Site/Application/Todos/Workflows/DeleteTodo/DeleteTodo.Workflow.cs` - Returns metadata

### Data Providers
20. `PagePlay.Site/Application/Todos/Perspectives/List/TodosList.Provider.cs` - Query provider
21. `PagePlay.Site/Application/Todos/Perspectives/List/TodoList.DomainView.cs` - DomainView with DomainName
22. `PagePlay.Site/Application/Todos/Perspectives/Analytics/TodoAnalytics.Provider.cs` - Analytics provider

### Related Experiments
23. `project-docs/experiments/domain-data-manifests.md` - Background on domain-level data fetching

---

## 🔍 Problem Statement

### Current State (Todos POC)
- **Fragment-based pattern**: Pages render HTML fragments directly
- **Target-based HTMX**: Forms specify `hx-target` for where updates go
- **Workflow responses contain query data**: Workflows fetch and return data for rendering
- **Manual OOB management**: Developers specify IDs and swap strategies

### Issues Discovered

**1. Multi-Domain Updates Don't Scale**
```csharp
// Current: Can only return ONE fragment
protected override async Task<IResult> OnSuccess(Response response)
{
    return await BuildHtmlFragmentResult(Page.RenderTodoList(response.Todos));
}

// Problem: What if you need to update 2+ unrelated areas?
// - Cart items list
// - Cart summary panel
// - Inventory badge in header
// - Notification toast
```

**2. Target Management is Client-Side Orchestration**
```csharp
// Form declares where response goes
HtmxForm.Render(
    new() {
        Action = "/interaction/todos/toggle",
        Target = "#todo-list",  // ← Client decides
        SwapStrategy = "innerHTML"
    },
    content
)
```
This violates server authority - server should decide what updates.

**3. Query Data in Workflows Violates CQRS**
```csharp
// Workflow returns query data
public class ToggleTodoWorkflowResponse
{
    public List<TodoListEntry> Todos { get; set; }  // ← Query data in command
}

// Then workflow fetches ALL todos
private async Task<List<Todo>> getTodosByUserId() =>
    await _repository.List<Todo>(Todo.ByUserId(currentUserContext.UserId.Value));
```

**4. Duplication Between Workflows and Providers**
```csharp
// ToggleTodoWorkflow (lines 49-60)
.OrderBy(t => t.IsCompleted)
.ThenByDescending(t => t.CreatedAt)
.Select(TodoListEntry.FromTodo)

// TodosListProvider (lines 25-30)
.OrderBy(t => t.IsCompleted)
.ThenByDescending(t => t.CreatedAt)
.Select(TodoListEntry.FromTodo)
```
Same logic in two places!

**5. ID Management is Manual**
```csharp
// Developer must remember to add matching IDs
<ul id="todosList">  // Must match domain name
```
Error-prone and not enforced.

---

## 💡 Proposed Solution

### Architecture Changes

**1. Component-First as Default Pattern**
- Every page area becomes a component
- Components declare data dependencies via `DataDependencies`
- Framework automatically loads data and manages updates

**2. Pure OOB Updates**
- Remove `Target` from HtmxForm and Button helpers (make optional)
- Framework automatically adds `hx-swap-oob` to component updates
- Server decides what updates via `DataMutations`

**3. Minimal Workflow Responses**
- Workflows return **metadata only** (created IDs, messages, flags)
- NO query data (lists, DTOs, transformed entities)
- Query data comes from DomainView providers

**4. Automatic ID Management**
- Framework injects IDs based on DomainView names
- Components get IDs from `ComponentId` property
- No manual ID wiring needed

---

## 🏗️ Core Concepts

### Concept 1: Components Own Rendering + Data Dependencies

**Component declares what it needs:**
```csharp
public class TodoListComponent(ITodosPageView _page) : IServerComponent
{
    public string ComponentId => "todo-list-component";

    // Declares dependency on todosList domain
    public DataDependencies Dependencies =>
        DataDependencies.From<TodosListProvider, TodosListDomainView>();

    // Renders using pre-loaded data
    public string Render(IDataContext data)
    {
        var todosData = data.Get<TodosListDomainView>();
        return $$"""
        <div id="{{ComponentId}}"
             data-component="TodoListComponent"
             data-domain="todosList">
            {{_page.RenderTodoList(todosData.List)}}
        </div>
        """;
    }
}
```

**Key insight:** Component knows what data it needs. Framework fetches it.

---

### Concept 2: Framework Orchestrates Multi-Domain Updates

**FrameworkOrchestrator handles everything:**

```
User Action (POST /interaction/todos/toggle)
  ↓
Interaction declares mutations: DataMutations.For("todosList", "todoAnalytics")
  ↓
Framework (FrameworkOrchestrator.RenderMutationResponseAsync):
  1. Parses component context from client (X-Component-Context header)
  2. Finds components affected by mutation (any component depending on "todosList" or "todoAnalytics")
  3. Re-fetches affected domains in parallel
  4. Re-renders affected components
  5. Injects hx-swap-oob="true" into each component HTML
  ↓
Returns combined HTML with all OOB updates
```

**No developer intervention needed** - declare mutations, framework handles the rest.

---

### Concept 3: Workflows are Commands (Mutate + Metadata)

**Current (wrong):**
```csharp
// Workflow = Command + Query
public async Task<IApplicationResult<Response>> Perform(...)
{
    // 1. Mutate
    todo.Toggle();
    await _repository.SaveChanges();

    // 2. Query (re-fetch data)
    var todos = await getTodosByUserId();

    // 3. Transform (duplicate provider logic)
    return Succeed(new Response {
        Todos = todos.OrderBy(...).Select(...).ToList()
    });
}
```

**Proposed (correct):**
```csharp
// Workflow = Command + Metadata only
public async Task<IApplicationResult<Response>> Perform(...)
{
    // 1. Mutate
    todo.Toggle();
    await _repository.SaveChanges();

    // 2. Return metadata (optional)
    return Succeed(new Response {
        Message = "Todo toggled successfully"
    });
}
```

**Components re-fetch via providers** - single source of truth.

---

### Concept 4: DomainView Providers are Queries

**All query logic lives in providers:**
```csharp
public class TodosListProvider(IRepository _repository) : IDataProvider<TodosListDomainView>
{
    public async Task<TodosListDomainView> FetchTyped(long userId)
    {
        var todos = await fetchData(userId);

        return new TodosListDomainView
        {
            List = transformToListEntries(todos),  // ← Transform here
            OpenCount = calculateOpenCount(todos),
            TotalCount = calculateTotalCount(todos),
            CompletionRate = calculateCompletionRate(todos)
        };
    }

    private List<TodoListEntry> transformToListEntries(List<Todo> todos) =>
        todos
            .OrderBy(t => t.IsCompleted)           // ← Ordering here
            .ThenByDescending(t => t.CreatedAt)
            .Select(TodoListEntry.FromTodo)
            .ToList();
}
```

**Single source of truth** - if ordering changes, change it once.

---

### Concept 5: Pure OOB = Server Authority

**Current (client decides):**
```csharp
// Form declares target
<form hx-post="/interaction" hx-target="#list" hx-swap="innerHTML">
```

**Proposed (server decides):**
```csharp
// Form just triggers action
<form hx-post="/interaction">

// Server response dictates updates
<div id="todo-list-component" hx-swap-oob="true">...</div>
<div id="analytics-component" hx-swap-oob="true">...</div>
<div id="notifications" hx-swap-oob="true">...</div>
```

**Server authority** - aligns with turn-based game architecture.

---

## 📊 Benefits of Component-First

### 1. Multi-Domain Updates are Free
**Before:** Manual multi-fragment rendering (40+ lines)
**After:** Declare mutations, framework handles everything (5 lines)

### 2. No Data Duplication
**Before:** Query logic in workflows AND providers
**After:** Query logic only in providers (CQRS)

### 3. Automatic OOB Management
**Before:** Developer adds IDs and `hx-swap-oob` manually
**After:** Framework injects automatically based on `ComponentId`

### 4. Reusable Components
**Before:** Each page has isolated render methods
**After:** Components reused across multiple pages

### 5. Parallel Data Loading
**Before:** Sequential or manual parallel fetching
**After:** Framework batches all domains, fetches in parallel

### 6. Server Authority
**Before:** Client HTML specifies targets
**After:** Server response dictates all updates

---

## 🔧 Implementation Areas

### Area 1: Make Target Optional in Forms/Buttons
**File:** `PagePlay.Site/Infrastructure/Web/Html/HtmxForm.cs`
**Change:** `Target` property becomes optional, conditionally rendered

### Area 2: Add OOB Helper Methods
**New:** `HtmlFragment.WithId()` and `HtmlFragment.WithOob()`
**Purpose:** Inject `id` and `hx-swap-oob` attributes into HTML

### Area 3: Enhance PageInteractionBase
**File:** `PagePlay.Site/Infrastructure/Web/Pages/PageInteractionBase.cs`
**Changes:**
- Auto-inject domain ID when `Mutates` has single domain
- Add `BuildOobResult()` method for explicit multi-fragment
- Keep existing `BuildHtmlFragmentResult()` for OOB components

### Area 4: Simplify Workflow Responses
**Files:** All workflow response classes
**Changes:**
- Remove query data (lists, DTOs)
- Keep only metadata (IDs, messages, flags)
- Empty responses for simple CRUD

### Area 5: Convert Todos to Components
**New files:**
- `TodoListComponent.cs` - Main list component
- `TodoStatsComponent.cs` - Stats/analytics component
**Updated:**
- Interactions return empty `BuildHtmlFragmentResult()`
- Route uses `FrameworkOrchestrator.RenderComponentsAsync()`

---

## 🎯 Success Criteria

### Architectural
- [ ] Components are the primary pattern (fragments as escape hatch)
- [ ] All query logic in DomainView providers (no duplication in workflows)
- [ ] Workflows return metadata only (no query data)
- [ ] Pure OOB updates (server decides, not client)
- [ ] Automatic ID management (no manual wiring)

### Code Quality
- [ ] ~50% less code for multi-domain features
- [ ] No duplication between workflows and providers
- [ ] Consistent pattern across all features
- [ ] Clear separation: Commands (workflows) vs Queries (providers)

### Developer Experience
- [ ] Simple features still simple (minimal boilerplate)
- [ ] Complex features (multi-domain) become simple
- [ ] Framework handles OOB automatically
- [ ] Components reusable across pages

---

## 🚧 Open Questions

### 1. Component Granularity
**Question:** How granular should components be?
- Page-level components (one per page)?
- Feature-level components (list, form, stats)?
- Element-level components (individual todo item)?

**Current thinking:** Feature-level is sweet spot (list, stats, notifications)

### 2. Form Target Deprecation
**Question:** Remove target entirely or keep as optional?

**Options:**
- A) Remove completely (breaking change, force migration)
- B) Make optional (backward compatible, deprecate gradually)
- C) Hybrid (auto-detect: if empty response, use OOB; if content, use target)

**Current thinking:** Option B (make optional, recommend OOB in docs)

### 3. Workflow Response Base Classes
**Question:** Should we provide base response classes?

**Options:**
```csharp
// Empty response
public class EmptyWorkflowResponse : IWorkflowResponse { }

// With ID
public class WorkflowResponseWithId : IWorkflowResponse
{
    public long CreatedId { get; set; }
}

// With message
public class WorkflowResponseWithMessage : IWorkflowResponse
{
    public string Message { get; set; }
}
```

**Current thinking:** Yes, provide common base classes to reduce boilerplate

### 4. Performance - Double Fetching
**Question:** Is re-fetching data acceptable?

**Context:**
- Workflow mutates data
- Framework re-fetches via provider
- Two DB queries instead of one

**Current thinking:**
- Acceptable trade-off for cleaner architecture
- Most queries are sub-millisecond
- Can optimize later with caching if needed
- CQRS purity more valuable than micro-optimization

### 5. Non-Component Pages
**Question:** What about static pages (About, Terms, etc.)?

**Options:**
- A) Everything is a component (even static content)
- B) Allow fragment escape hatch for simple pages
- C) Hybrid (static sections don't need components)

**Current thinking:** Option B (fragments allowed for truly simple cases)

---

## 📝 Next Steps (For Implementation Session)

### Phase 1: Infrastructure (Non-Breaking)
1. Add `HtmlFragment` helper class with `WithId()` and `WithOob()` methods
2. Make `Target` optional in `HtmxFormData` and `RouteData`
3. Update form/button renderers to conditionally include target attributes
4. Add `BuildOobResult()` overloads to `PageInteractionBase`

### Phase 2: Todos Conversion (Proof of Concept)
1. Create `TodoListComponent` with `TodosListDomainView` dependency
2. Create `TodoStatsComponent` with `TodoAnalyticsDomainView` dependency
3. Update interactions to use empty `BuildHtmlFragmentResult()`
4. Update workflows to return metadata only
5. Update route to use `FrameworkOrchestrator.RenderComponentsAsync()`

### Phase 3: Validation
1. Test multi-domain updates (toggle todo updates list + stats)
2. Verify OOB attributes injected correctly
3. Measure code reduction (before/after line count)
4. Document pattern for future features

### Phase 4: Documentation
1. Update `.claude/docs/` with component-first pattern
2. Add examples to guide
3. Document workflow response conventions
4. Create migration guide for existing features

---

## 🔗 Related Patterns

### CQRS (Command Query Responsibility Segregation)
- **Commands:** Workflows (mutate data, return metadata)
- **Queries:** DomainView providers (fetch data, transform for display)
- **Never mix:** Commands don't return query results

### Game-Style Turn-Based Architecture
- **Client:** Sends action (form submit)
- **Server:** Processes turn (workflow)
- **Server:** Sends state deltas (OOB component updates)
- **Client:** Applies updates (HTMX swaps)

### Vertical Slice Architecture
- Each feature is self-contained
- Components = rendering layer within slice
- Workflows = command layer within slice
- Providers = query layer within slice

---

## 📚 Key Insights from Exploration

### 1. "Do we lose anything if everything becomes a component?"
**Answer:** No significant losses. Gains vastly outweigh costs.
- Trivial pages have slight overhead (extra file)
- Real apps benefit enormously (50% less code, no duplication)

### 2. "Should workflows return query data?"
**Answer:** No. Workflows should return metadata only.
- With components, query data is unused (framework re-fetches)
- Violates CQRS (commands shouldn't contain queries)
- Creates duplication (logic in workflows AND providers)

### 3. "Can we hide ID management from developers?"
**Answer:** Yes, using domain-centric auto-injection.
- Single domain mutation → Framework injects domain ID automatically
- Multi-domain → Developer uses explicit `BuildOobResult()`
- Components → IDs come from `ComponentId` property

### 4. "Is pure OOB HTMX abuse?"
**Answer:** No, it's an architectural choice aligned with server authority.
- HTMX designed OOB for "additional updates"
- But nothing prohibits pure OOB
- Fits server-authority philosophy better than target-based

### 5. "What about multi-domain apps?"
**Answer:** Components are essential, fragments don't scale.
- Fragment pattern breaks down with 2+ unrelated updates
- Component pattern handles N-domain updates automatically
- FrameworkOrchestrator already designed for this

---

## 🎓 Architecture Philosophy

This experiment reinforces core framework principles:

**Server Authority:** Server decides all updates via OOB, not client-side targets

**Self-Enforcing Patterns:** Components enforce dependencies, IDs, and OOB automatically

**Consistent Complexity:** Every feature follows same pattern (component + domain + workflow)

**CQRS:** Clear separation between commands (workflows) and queries (providers)

**Turn-Based:** Client sends action → Server processes → Server sends deltas → Client applies

**Framework Handles Complexity:** Multi-domain updates, parallel fetching, OOB injection = framework responsibility

---

**Last Updated:** 2025-12-02
**Next Session:** Implement Phase 1 infrastructure changes
