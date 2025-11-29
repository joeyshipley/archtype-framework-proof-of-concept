# Active Implementation: DataDomains for Reads, Workflows for Writes

**Status:** Planning Complete, Ready for Implementation
**Created:** 2025-11-29
**Last Updated:** 2025-11-29
**Architectural Decision:** Separate read/write concerns - DataDomains (queries) vs Workflows (commands)

---

## Context & Decision

### The Problem
We currently have two data fetching strategies that duplicate concerns:
1. **Workflows** - Including `ListTodos` which just reads data
2. **DataDomains** - Used by components to read data for rendering

**Example Duplication:**
```csharp
// ListTodos.Workflow.cs:30-31
private async Task<List<Todo>> getTodosByUserId() =>
    await _repository.List(Todo.ByUserId(currentUserContext.UserId.Value));

// TodosDomain.cs:46-47
private async Task<List<Todo>> fetchTodos(long userId) =>
    await _repository.List(Todo.ByUserId(userId));
```

Both do the same thing: fetch todos for a user. This creates confusion about which strategy to use.

### The Solution
**Clear separation of concerns:**
- **DataDomains (Loaders)** = All READ operations (queries)
- **Workflows** = All WRITE operations (commands - Create, Update, Delete)

This mirrors CQRS principles: Commands/Queries → Workflows/Loaders

### Why This Aligns With Architecture

**Consistent Complexity:**
- One read strategy (DataDomains)
- One mutation strategy (Workflows)
- No confusion about which to use

**Self-Enforcing Pattern:**
- Need to mutate? → Create a workflow
- Need to read? → Use DataDomain
- Framework makes the right thing obvious

**HTTP-First Philosophy:**
- Game-style data pre-fetching (DataLoader fetches all required domains)
- Page load → DataLoader fetches domains (like game level loading)
- User action → Workflow mutates → Framework re-fetches affected domains → OOB updates
- Clean separation: input (workflows) vs output (domains)

**Revealing Intent:**
- Workflows named after business actions: `CreateTodo`, `DeleteTodo`, `ToggleTodo`
- Domains named after data contexts: `TodosDomain`, `TodoAnalyticsDomain`

---

## Implementation Checklist

### Phase 1: Infrastructure Enhancements

#### 1.1 Add Error Handling to DataLoader
**File:** `PagePlay.Site/Infrastructure/Web/Data/DataLoader.cs`

**Current state:** DataLoader assumes all domain fetches succeed (no error handling)

**Changes needed:**
- [ ] Wrap domain fetching in try/catch
- [ ] Return error information (decide: throw exception vs return result object)
- [ ] Add logging for domain fetch failures
- [ ] Decide: Should DataLoader return `IApplicationResult<IDataContext>` or keep infrastructure-focused and handle errors at call sites?

**Decision needed:** Where should errors be handled?
- **Option A:** DataLoader returns `Task<IDataContext>` and throws exceptions (infrastructure-focused, let caller handle)
- **Option B:** DataLoader returns `Task<IApplicationResult<IDataContext>>` (application-focused, consistent with workflows)

**Recommendation:** Option A (keep infrastructure-focused), handle at route level for now

#### 1.2 Component Auto-Discovery (Bonus - Not Blocking)
**File:** `PagePlay.Site/Infrastructure/Web/Components/ComponentFactory.cs`

**Current state:** Hardcoded dictionary of component types
```csharp
private static readonly Dictionary<string, Type> _componentTypes = new()
{
    ["WelcomeWidget"] = typeof(IWelcomeWidget),
    ["AnalyticsStatsWidget"] = typeof(IAnalyticsStatsWidget)
};
```

**Changes needed:**
- [ ] Use reflection to discover all `IServerComponent` interfaces at startup
- [ ] Remove hardcoded dictionary
- [ ] Self-enforcing: implement `IServerComponent` → automatically discoverable

**Implementation:**
```csharp
private static readonly Dictionary<string, Type> _componentTypes = discoverComponents();

private static Dictionary<string, Type> discoverComponents()
{
    return typeof(IServerComponent).Assembly
        .GetTypes()
        .Where(t => t.IsInterface && typeof(IServerComponent).IsAssignableFrom(t))
        .Where(t => t != typeof(IServerComponent))
        .ToDictionary(
            t => t.Name.TrimStart('I'), // "IWelcomeWidget" → "WelcomeWidget"
            t => t
        );
}
```

**Priority:** Low (nice to have, not blocking main work)

---

### Phase 2: Remove ListTodos Workflow

#### 2.1 Delete ListTodos Workflow Files
- [ ] Delete `PagePlay.Site/Application/Todos/ListTodos/ListTodos.Workflow.cs`
- [ ] Delete `PagePlay.Site/Application/Todos/ListTodos/ListTodos.BoundaryContracts.cs`
- [ ] Delete `PagePlay.Site/Application/Todos/ListTodos/` directory (if empty)

#### 2.2 Update Todos Page Route
**File:** `PagePlay.Site/Pages/Todos/Todos.Route.cs`

**Before:**
```csharp
endpoints.MapGet(PAGE_ROUTE, async (
    IWorkflow<ListTodosWorkflowRequest, ListTodosWorkflowResponse> listWorkflow
) =>
{
    // Render todos
    var result = await listWorkflow.Perform(new ListTodosWorkflowRequest());
    var bodyContent = !result.Success
        ? _page.RenderError("Failed to load todos")
        : _page.RenderPage(result.Model.Todos);

    // Layout handles its own component composition
    var page = await _layout.RenderAsync("Todos", bodyContent);
    return Results.Content(page, "text/html");
})
.RequireAuthenticatedUser();
```

**After:**
```csharp
endpoints.MapGet(PAGE_ROUTE, async (
    IDataLoader dataLoader
) =>
{
    try
    {
        // Fetch todos via DataDomain (same as components use)
        var dataContext = await dataLoader.LoadDomainsAsync(new[] { "todos" });
        var todosData = dataContext.GetDomain<TodosDomainContext>("todos");

        var bodyContent = _page.RenderPage(todosData.List);
        var page = await _layout.RenderAsync("Todos", bodyContent);
        return Results.Content(page, "text/html");
    }
    catch (Exception ex)
    {
        // TODO: Add proper logging
        var bodyContent = _page.RenderError("Failed to load todos");
        var page = await _layout.RenderAsync("Todos", bodyContent);
        return Results.Content(page, "text/html");
    }
})
.RequireAuthenticatedUser();
```

**Changes:**
- [ ] Replace `IWorkflow<ListTodosWorkflowRequest, ListTodosWorkflowResponse>` parameter with `IDataLoader`
- [ ] Remove workflow invocation
- [ ] Call `dataLoader.LoadDomainsAsync(new[] { "todos" })`
- [ ] Get todos from `TodosDomainContext`: `todosData.List`
- [ ] Add error handling (try/catch)
- [ ] Add logging for failures

#### 2.3 Verify No Other References to ListTodos
- [ ] Search codebase for `ListTodos` references
- [ ] Search for `ListTodosWorkflow` imports
- [ ] Check test files for `ListTodos` usage
- [ ] Remove any remaining references

---

### Phase 3: Update Documentation

#### 3.1 Update README.CONSISTENT_COMPLEXITY_DESIGN.md
**File:** `.claude/docs/README.CONSISTENT_COMPLEXITY_DESIGN.md`

**Section to update:** "Workflow Pattern - Revealing Intent Class Structure"

**Add new section:**
```markdown
### Read vs Write Separation

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
- Eliminates duplication (one query path)
- Clear responsibilities (mutations vs queries)
- Consistent with game-style data pre-fetching pattern
- Workflows reveal business intent (actions), domains reveal data structure
```

#### 3.2 Update README.WEB_FRAMEWORK.md
**File:** `.claude/docs/README.WEB_FRAMEWORK.md`

**Section to update:** "Pattern 3: Data Pre-Fetching (Game Level Loading)"

**Add clarification:**
```markdown
### Data Pre-Fetching Implementation

**DataDomains are the single source of read operations:**

```
Page Load:
├─ Collect required domains (from page + components)
├─ DataLoader fetches ALL domains in parallel
├─ No N+1 queries, no waterfalls
├─ Page and components render from shared DataContext
└─ Clean "loading → interactive" transition

User Action (Mutation):
├─ User clicks "Create Todo" → Workflow executes
├─ Workflow validates, mutates data, returns success
├─ Framework identifies affected domains ("todos")
├─ DataLoader re-fetches ONLY affected domains
├─ Framework re-renders affected components (OOB)
└─ UI updates, ready for next action
```

**Key insight:**
- Workflows = user actions that change state
- DataDomains = data fetching for rendering
- No "list workflows" - reads happen through DataLoader
```

#### 3.3 Create Migration Guide for Future Features
**File:** `.claude/docs/workflow/read-write-pattern.md` (NEW FILE)

Create new document explaining when to use each pattern with examples.

**Content needed:**
- When to create a Workflow (any mutation)
- When to create a DataDomain (new read context)
- When to extend existing domain vs create new one
- Examples of correct and incorrect usage
- Decision tree: "Should I create a workflow or domain?"

---

### Phase 4: Verification

#### 4.1 Manual Testing
- [ ] Navigate to `/todos` page - should load todos
- [ ] Create a new todo - should work, WelcomeWidget should update
- [ ] Toggle a todo - should work, list should update
- [ ] Delete a todo - should work, WelcomeWidget should update
- [ ] Test error cases (database down, invalid data)

#### 4.2 Code Review
- [ ] Verify no remaining `ListTodos` references
- [ ] Check all Workflow files - should be CUD only
- [ ] Check all DataDomain files - should be R only
- [ ] Verify consistent error handling patterns
- [ ] Check logging is present for failures

#### 4.3 Architecture Validation
- [ ] Clear separation: workflows = mutations, domains = queries
- [ ] No duplication between workflows and domains
- [ ] Pattern is self-enforcing (obvious which to use)
- [ ] Documentation clearly explains the pattern

---

## Files Changed Summary

### Files to Delete
- `PagePlay.Site/Application/Todos/ListTodos/ListTodos.Workflow.cs`
- `PagePlay.Site/Application/Todos/ListTodos/ListTodos.BoundaryContracts.cs`
- `PagePlay.Site/Application/Todos/ListTodos/` directory (if empty)

### Files to Modify
- `PagePlay.Site/Pages/Todos/Todos.Route.cs` - Replace workflow with DataLoader
- `PagePlay.Site/Infrastructure/Web/Data/DataLoader.cs` - Add error handling (optional)
- `PagePlay.Site/Infrastructure/Web/Components/ComponentFactory.cs` - Auto-discovery (optional)
- `.claude/docs/README.CONSISTENT_COMPLEXITY_DESIGN.md` - Add read/write pattern
- `.claude/docs/README.WEB_FRAMEWORK.md` - Clarify data pre-fetching

### Files to Create
- `.claude/docs/workflow/read-write-pattern.md` - Migration guide for future features

---

## Open Questions

### 1. Error Handling Strategy
**Question:** Should DataLoader return `Task<IDataContext>` (throw exceptions) or `Task<IApplicationResult<IDataContext>>` (wrap errors)?

**Options:**
- **Option A:** Keep infrastructure-focused, throw exceptions, handle at route level
- **Option B:** Make application-focused, return result objects like workflows

**Recommendation:** Option A for now (keep infrastructure simple), revisit if error handling becomes inconsistent

**Decision:** _(To be filled during implementation)_

---

### 2. Logging Strategy
**Question:** Where should domain fetch failures be logged?

**Options:**
- **Option A:** In DataLoader (centralized)
- **Option B:** At route level (closer to user action)
- **Option C:** Both (DataLoader logs technical details, route logs user-facing context)

**Recommendation:** Option C (both)

**Decision:** _(To be filled during implementation)_

---

### 3. Component Auto-Discovery Priority
**Question:** Should we implement component auto-discovery now or later?

**Context:** Not blocking main work, but nice quality-of-life improvement

**Recommendation:** Implement if time permits, otherwise defer to future session

**Decision:** _(To be filled during implementation)_

---

## Success Criteria

### Must Have (Blocking)
- [ ] ListTodos workflow deleted
- [ ] Todos.Route.cs uses DataLoader
- [ ] Page loads correctly
- [ ] All interactions still work (create, toggle, delete)
- [ ] Documentation updated

### Should Have (Important)
- [ ] Error handling for domain fetch failures
- [ ] Logging for failures
- [ ] Migration guide document created

### Nice to Have (Optional)
- [ ] Component auto-discovery implemented
- [ ] Test coverage for new error paths

---

## Next Steps

When resuming this implementation:

1. **Read this document first** - Contains all context and decisions
2. **Review current state** - Check if any files have changed since planning
3. **Start with Phase 1** - Infrastructure enhancements (if doing them)
4. **Execute Phase 2** - Delete ListTodos, update route
5. **Complete Phase 3** - Update documentation
6. **Run Phase 4** - Verification and testing

---

## Notes for Future Sessions

### Context Preservation
This document preserves:
- ✅ Why we're making this change (architectural alignment)
- ✅ What needs to change (specific files and code)
- ✅ How to implement (code examples)
- ✅ How to verify (testing steps)
- ✅ Open questions (for decision-making)

### If You Need to Deviate
If you discover issues during implementation:
1. Update this document with findings
2. Add new "Open Questions" section
3. Document why deviation was necessary
4. Update "Files Changed Summary"

### Session Resume Protocol
1. Read `.claude/docs/AI_START.md` (standard session start)
2. Read this file (`.claude/docs/ACTIVE_IMPLEMENTATION.md`)
3. Check "Next Steps" section
4. Continue from last unchecked item

---

**Ready to implement!** 🚀
