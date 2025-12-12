# Phase 2 Implementation Plan: Todos Component-First Conversion

> **⚠️ HISTORICAL DOCUMENT**
>
> This plan was created during the component-first architecture experiment (December 2025).
> Code examples and terminology may not reflect current implementation.
>
> **Terminology changes since this document:**
> - `IServerComponent` → `IView`
> - `ComponentId` → `ViewId`
> - `DataDependencies.From<Provider, Context>()` → `DataDependencies.From<Context>()`
> - `data-component` attribute → `data-view`
>
> **For current patterns, see:** `.claude/docs/README.ARCHITECTURE_REFERENCE.md`

**Created:** 2025-12-02
**Status:** Completed (Historical)
**Experiment:** component-first-architecture.md
**Phase:** 2 of 4

---

## 📋 Executive Summary

Convert the Todos feature from fragment-based rendering to component-first architecture. This validates the infrastructure built in Phase 1 and establishes the pattern for all future features.

**Current State:** Fragment-based with target-driven updates and query data in workflows
**Target State:** Component-first with pure OOB updates and metadata-only workflows

---

## 🎯 Goals

1. **Validate Phase 1 infrastructure** - Prove the component-first pattern works end-to-end
2. **Establish reference implementation** - Create pattern for all future features
3. **Measure improvements** - Quantify code reduction and complexity elimination
4. **Preserve functionality** - Zero user-visible changes (same behavior, better architecture)

---

## 📊 Current State Analysis

### Files Inventory

**Page Layer (3 files):**
- `Todos.Page.htmx.cs` - 152 lines, contains all rendering logic
- `Todos.Route.cs` - 49 lines, manual data loading and rendering
- `ITodosPageView` interface - 8 rendering methods

**Interaction Layer (3 files):**
- `CreateTodo.Interaction.cs` - 28 lines, renders `RenderSuccessfulTodoCreation()`
- `ToggleTodo.Interaction.cs` - 27 lines, renders entire list via `RenderTodoList()`
- `DeleteTodo.Interaction.cs` - 38 lines, renders empty fragment (good!)

**Workflow Layer (3 files):**
- `CreateTodo.Workflow.cs` - 44 lines, returns `TodoListEntry` (query data)
- `ToggleTodo.Workflow.cs` - 61 lines, fetches ALL todos + sorting (query logic)
- `DeleteTodo.Workflow.cs` - 52 lines, returns metadata only (good!)

**Domain/Query Layer (2 files):**
- `TodosListProvider.cs` - 46 lines, query logic (sorting, transformations)
- `TodosListDomainView.cs` - 16 lines, domain view contract

### Key Issues Identified

#### 1. Query Logic Duplication
**Location:** ToggleTodo.Workflow.cs (lines 49-60) vs TodosListProvider.cs (lines 25-30)

Both implement identical sorting/transformation logic:
```csharp
// In ToggleTodoWorkflow (lines 55-59)
.OrderBy(t => t.IsCompleted)
.ThenByDescending(t => t.CreatedAt)
.Select(TodoListEntry.FromTodo)

// In TodosListProvider (lines 27-30)
.OrderBy(t => t.IsCompleted)
.ThenByDescending(t => t.CreatedAt)
.Select(TodoListEntry.FromTodo)
```

**Impact:** Maintenance burden, inconsistency risk, violates DRY

#### 2. Target-Based HTMX (Client Authority)
**Location:** Todos.Page.htmx.cs

Forms specify where responses go:
- Line 40: `Target = "#todo-list-ul"` (create form)
- Line 91: `Target = "#todo-list"` (toggle form)
- Line 105: `target: $"#todo-{todo.Id}"` (delete button)

**Impact:** Violates server authority principle, limits multi-domain updates

#### 3. Query Data in Commands
**Location:** CreateTodo.Workflow.cs, ToggleTodo.Workflow.cs

Workflows return transformed query data:
```csharp
// CreateTodoWorkflowResponse
public TodoListEntry Todo { get; set; }  // ← Query data

// ToggleTodoWorkflowResponse
public List<TodoListEntry> Todos { get; set; }  // ← Query data
```

**Impact:** Violates CQRS, unused with component-first architecture

#### 4. Interaction Rendering Logic
**Location:** All interaction files

Interactions render HTML directly:
- CreateTodo: Calls `Page.RenderSuccessfulTodoCreation()` (multi-fragment with OOB)
- ToggleTodo: Calls `Page.RenderTodoList()` (entire list)
- DeleteTodo: Empty (good, but inconsistent pattern)

**Impact:** Inconsistent patterns, coupling to page rendering

---

## 🏗️ Implementation Tasks

### Task 1: Create TodoListComponent ⭐

**Priority:** Critical (foundation for everything else)
**File:** `PagePlay.Site/Pages/Todos/Components/TodoListComponent.cs` (NEW)
**Lines:** ~60

#### Implementation

```csharp
using PagePlay.Site.Application.Todos.Perspectives.List;
using PagePlay.Site.Infrastructure.Web.Components;
using PagePlay.Site.Pages.Shared.Elements;
using PagePlay.Site.Infrastructure.Web.Html;

namespace PagePlay.Site.Pages.Todos.Components;

public class TodoListComponent(ITodosPageView _page) : IServerComponent
{
    public string ComponentId => "todo-list-component";

    public DataDependencies Dependencies =>
        DataDependencies.From<TodosListProvider, TodosListDomainView>();

    public string Render(IDataContext data)
    {
        var todosData = data.Get<TodosListDomainView>();

        // Wrap the existing rendering logic in a component div
        return $$"""
        <div id="{{ComponentId}}"
             data-component="TodoListComponent"
             data-domain="{{TodosListDomainView.DomainName}}">
            {{_page.RenderTodoList(todosData.List)}}
        </div>
        """;
    }
}
```

#### Key Decisions

**✅ Reuse existing page rendering methods**
- No need to duplicate `RenderTodoList()` logic
- Component wraps existing rendering in component container
- Minimal code, maximum reuse

**✅ ComponentId matches domain-level granularity**
- Component updates whole list (not individual items)
- Matches existing behavior (ToggleTodo re-renders entire list)
- Can optimize to item-level later if needed

**✅ data-domain attribute for debugging**
- Makes it clear which domain this component depends on
- Useful for troubleshooting OOB updates
- Optional but recommended

#### Testing Checklist

- [ ] Component compiles
- [ ] `ComponentId` is unique across all components
- [ ] `Dependencies` references correct Provider and DomainView types
- [ ] `Render()` produces valid HTML with root element
- [ ] Root element has `id="{{ComponentId}}"`

---

### Task 2: Update CreateTodo Interaction

**Priority:** High
**File:** `PagePlay.Site/Pages/Todos/Interactions/CreateTodo.Interaction.cs`
**Changes:** -5 lines, simpler

#### Current Implementation (Lines 20-24)

```csharp
protected override async Task<IResult> OnSuccess(CreateTodoWorkflowResponse response)
{
    var content = Page.RenderSuccessfulTodoCreation(response.Todo);
    return await BuildHtmlFragmentResult(content);
}
```

#### New Implementation

```csharp
protected override async Task<IResult> OnSuccess(CreateTodoWorkflowResponse response)
{
    return await BuildOobResult();
}
```

#### What Changes

**Before:**
1. Workflow returns `TodoListEntry`
2. Interaction renders new item + form reset (two fragments with OOB)
3. Response has main content + OOB update

**After:**
1. Workflow returns metadata only (created ID)
2. Interaction returns pure OOB response
3. Framework re-renders `TodoListComponent` automatically
4. HTMX swaps entire component

#### Impact Analysis

**Code Reduction:** 3 lines removed (60% reduction in method)
**Behavior Change:** List re-renders entire list vs prepending single item
**Performance:** Negligible (list is small, provider query is cached)
**UX:** Identical (user sees new todo appear, form clears)

#### Testing Checklist

- [ ] Create todo shows new item in list
- [ ] Form clears after submission
- [ ] Network tab shows OOB response
- [ ] Response HTML contains `hx-swap-oob="true"`
- [ ] No JavaScript errors in console

---

### Task 3: Update ToggleTodo Interaction

**Priority:** High
**File:** `PagePlay.Site/Pages/Todos/Interactions/ToggleTodo.Interaction.cs`
**Changes:** -4 lines, simpler

#### Current Implementation (Lines 19-23)

```csharp
protected override async Task<IResult> OnSuccess(ToggleTodoWorkflowResponse response)
{
    var content = Page.RenderTodoList(response.Todos);
    return await BuildHtmlFragmentResult(content);
}
```

#### New Implementation

```csharp
protected override async Task<IResult> OnSuccess(ToggleTodoWorkflowResponse response)
{
    return await BuildOobResult();
}
```

#### What Changes

**Before:**
1. Workflow fetches ALL todos, sorts, transforms to DTOs
2. Interaction renders entire list
3. Target-based update to `#todo-list`

**After:**
1. Workflow toggles todo, saves, returns empty response
2. Interaction returns pure OOB
3. Framework re-fetches via provider, re-renders component

#### Impact Analysis

**Code Reduction:** 2 lines removed (67% reduction in method)
**Logic Elimination:** Query logic removed from workflow (lines 49-60)
**Duplication Removed:** Sorting/transformation now only in provider
**Behavior:** Identical (whole list updates)

#### Testing Checklist

- [ ] Toggle updates checkbox icon
- [ ] Todo moves to bottom when completed
- [ ] Completed todos have correct CSS class
- [ ] Network response shows OOB update
- [ ] No flash of unstyled content

---

### Task 4: Update DeleteTodo Interaction

**Priority:** Medium (already mostly correct)
**File:** `PagePlay.Site/Pages/Todos/Interactions/DeleteTodo.Interaction.cs`
**Changes:** 0 lines (already uses `BuildHtmlFragmentResult()` with no content)

#### Current Implementation (Lines 21-24)

```csharp
protected override async Task<IResult> OnSuccess(DeleteTodoWorkflowResponse response)
{
    return await BuildHtmlFragmentResult();
}
```

#### Analysis

This interaction **already follows the component-first pattern**!
- Returns empty fragment (`BuildHtmlFragmentResult()` with no args)
- Relies on framework OOB update via `Mutates` declaration
- No manual rendering

#### Decision: Keep As-Is ✅

No changes needed. The `BuildHtmlFragmentResult()` without content is functionally equivalent to `BuildOobResult()`.

**Optional improvement:** Change to `BuildOobResult()` for consistency (pure semantics, no behavior change)

#### Testing Checklist

- [ ] Delete removes todo from list
- [ ] List re-renders via OOB update
- [ ] Error handling still works (notification appears)

---

### Task 5: Remove Target Attributes from Forms

**Priority:** High (enables pure server authority)
**File:** `PagePlay.Site/Pages/Todos/Todos.Page.htmx.cs`
**Changes:** -3 lines, cleaner

#### Change 1: Create Form (Lines 36-42)

**Before:**
```csharp
HtmxForm.Render(
    new()
    {
        Action = "/interaction/todos/create",
        Target = "#todo-list-ul",       // ← Remove
        SwapStrategy = "afterbegin"     // ← Remove
    },
    ...
)
```

**After:**
```csharp
HtmxForm.Render(
    new()
    {
        Action = "/interaction/todos/create"
        // No Target - pure OOB
    },
    ...
)
```

#### Change 2: Toggle Form (Lines 87-93)

**Before:**
```csharp
HtmxForm.Render(
    new()
    {
        Action = "/interaction/todos/toggle",
        Target = "#todo-list",  // ← Remove
        CssClass = "todo-toggle-form"
    },
    ...
)
```

**After:**
```csharp
HtmxForm.Render(
    new()
    {
        Action = "/interaction/todos/toggle",
        CssClass = "todo-toggle-form"
        // No Target - pure OOB
    },
    ...
)
```

#### Change 3: Delete Button (Lines 100-107)

**Before:**
```csharp
ButtonDelete.Render(
    endpoint: "/interaction/todos/delete",
    id: todo.Id,
    tag: "todo",
    content: $$"""×""",
    target: $"#todo-{todo.Id}",      // ← Remove
    swapStrategy: "outerHTML"        // ← Remove
)
```

**After:**
```csharp
ButtonDelete.Render(
    endpoint: "/interaction/todos/delete",
    id: todo.Id,
    tag: "todo",
    content: $$"""×"""
    // target and swapStrategy removed - pure OOB
)
```

#### Impact

**Server Authority:** Server now decides what updates (not client HTML)
**Flexibility:** Can easily add multi-domain updates (e.g., notifications, analytics)
**Simplicity:** No need to coordinate IDs between client HTML and server responses

#### Testing Checklist

- [ ] Forms still submit correctly
- [ ] No HTMX errors in console
- [ ] Updates still apply correctly
- [ ] Multiple updates can be sent in one response (future-proof)

---

### Task 6: Update Todos Route

**Priority:** Critical (wires everything together)
**File:** `PagePlay.Site/Pages/Todos/Todos.Route.cs`
**Changes:** Refactor to use components + orchestrator

#### Current Implementation (Lines 22-33)

```csharp
endpoints.MapGet(PAGE_ROUTE, async (
    IDataLoader dataLoader
) =>
{
    try
    {
        var ctx = await dataLoader.With<TodosListDomainView>().Load();
        var todosData = ctx.Get<TodosListDomainView>();

        var bodyContent = _page.RenderPage(todosData.List);
        var page = await _layout.RenderAsync("Todos", bodyContent);
        return Results.Content(page, "text/html");
    }
    ...
})
```

#### New Implementation - Option A: Component-First (RECOMMENDED)

```csharp
endpoints.MapGet(PAGE_ROUTE, async (
    IFrameworkOrchestrator orchestrator
) =>
{
    try
    {
        // Define page components
        var components = new List<IServerComponent>
        {
            _todoListComponent  // Injected via constructor
        };

        // Framework loads data and renders all components
        var renderedComponents = await orchestrator.RenderComponentsAsync(components);

        // Compose page from rendered components
        var bodyContent = _page.RenderPageWithComponents(renderedComponents);
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
```

#### New Implementation - Option B: Hybrid (SIMPLER FOR NOW)

Keep existing manual data loading but use component for rendering:

```csharp
endpoints.MapGet(PAGE_ROUTE, async (
    IDataLoader dataLoader
) =>
{
    try
    {
        var ctx = await dataLoader.With<TodosListDomainView>().Load();

        // Create component and render
        var todoListComponent = new TodoListComponent(_page);
        var todoListHtml = todoListComponent.Render(ctx);

        // Compose page (create or update RenderPageWithComponent method)
        var bodyContent = _page.RenderPageWithComponent(todoListHtml);
        var page = await _layout.RenderAsync("Todos", bodyContent);
        return Results.Content(page, "text/html");
    }
    ...
})
```

#### Decision Point

**Recommendation:** Use **Option B (Hybrid)** for Phase 2

**Rationale:**
- Simpler migration path
- Fewer new moving parts
- Can refactor to Option A in Phase 3
- Validates component rendering without full orchestrator refactor

#### Required Changes to Todos.Page.htmx.cs

Add new method:

```csharp
// language=html
public string RenderPageWithComponent(string todoListComponentHtml) =>
$$"""
<div class="todo-page">
    <h1>My Todos</h1>
    <div id="notifications"></div>
    {{RenderCreateForm()}}
    {{todoListComponentHtml}}
</div>
""";
```

Or update existing method to accept optional component HTML:

```csharp
public string RenderPage(List<TodoListEntry> todos = null, string componentHtml = null)
{
    var listContent = componentHtml ?? RenderTodoList(todos);

    return $$"""
    <div class="todo-page">
        <h1>My Todos</h1>
        <div id="notifications"></div>
        {{RenderCreateForm()}}
        {{listContent}}
    </div>
    """;
}
```

#### Testing Checklist

- [ ] Page loads successfully
- [ ] Todo list renders with component wrapper
- [ ] Component has correct ID in HTML
- [ ] Data loads correctly
- [ ] Error handling still works

---

### Task 7: Simplify Workflow Responses

**Priority:** High (eliminates duplication)
**Files:** 3 workflow response classes
**Changes:** Remove query data, keep metadata only

#### Change 1: CreateTodo Response

**File:** `CreateTodo.BoundaryContracts.cs`

**Before:**
```csharp
public class CreateTodoWorkflowResponse
{
    public TodoListEntry Todo { get; set; }
}
```

**After:**
```csharp
public class CreateTodoWorkflowResponse
{
    public long CreatedId { get; set; }
    // Metadata only - query data comes from provider
}
```

**Workflow Change (CreateTodo.Workflow.cs, line 39):**
```csharp
// Before:
private CreateTodoWorkflowResponse buildResponse(Todo todo) =>
    new CreateTodoWorkflowResponse
    {
        Todo = TodoListEntry.FromTodo(todo)
    };

// After:
private CreateTodoWorkflowResponse buildResponse(Todo todo) =>
    new CreateTodoWorkflowResponse
    {
        CreatedId = todo.Id
    };
```

#### Change 2: ToggleTodo Response

**File:** `ToggleTodo.BoundaryContracts.cs`

**Before:**
```csharp
public class ToggleTodoWorkflowResponse
{
    public List<TodoListEntry> Todos { get; set; } = new();
}
```

**After:**
```csharp
public class ToggleTodoWorkflowResponse
{
    // Empty - no metadata needed for toggle
}
```

**Workflow Changes (ToggleTodo.Workflow.cs):**

Remove query logic (lines 33-34, 49-60):
```csharp
// REMOVE these lines:
var todos = await getTodosByUserId();
return Succeed(buildResponse(todos));

private async Task<List<Todo>> getTodosByUserId() => ...
private ToggleTodoWorkflowResponse buildResponse(List<Todo> todos) => ...

// REPLACE line 33-34 with:
return Succeed(new ToggleTodoWorkflowResponse());
```

#### Change 3: DeleteTodo Response

**Status:** Already correct! ✅

```csharp
public class DeleteTodoWorkflowResponse
{
    public long Id { get; set; }
    public string Message { get; set; }
}
```

**No changes needed** - already follows metadata-only pattern.

#### Impact Summary

**Lines Removed:**
- CreateTodo: ~5 lines (transformation logic)
- ToggleTodo: ~28 lines (query + transformation logic)
- DeleteTodo: 0 lines (already correct)
- **Total: ~33 lines removed**

**Duplication Eliminated:**
- Sorting logic no longer in ToggleTodo workflow
- Transformation logic no longer in CreateTodo workflow
- Single source of truth: TodosListProvider

**CQRS Compliance:**
- ✅ Commands (workflows) only mutate and return metadata
- ✅ Queries (providers) only fetch and transform data
- ✅ Clear separation of concerns

#### Testing Checklist

- [ ] All workflows compile
- [ ] CreateTodo returns created ID
- [ ] ToggleTodo returns empty response
- [ ] DeleteTodo behavior unchanged
- [ ] Interactions still work with new responses

---

### Task 8: Update Component Registration (DI)

**Priority:** High
**File:** `PagePlay.Site/Infrastructure/DependencyInjection/DependencyResolver.cs`
**Changes:** Register new component

#### Add Registration

```csharp
// Components
services.AddScoped<TodoListComponent>();
```

#### Also Update Todos Route Constructor

**File:** `Todos.Route.cs`

If using Option A (full orchestrator), inject component:

```csharp
public class TodosPageEndpoints(
    IPageLayout _layout,
    ITodosPageView _page,
    IEnumerable<ITodosPageInteraction> _interactions,
    ILogger<TodosPageEndpoints> _logger,
    TodoListComponent _todoListComponent  // ← Add this
) : IClientEndpoint
```

If using Option B (hybrid), no constructor changes needed (instantiate inline).

---

## 🧪 Testing Plan

### Unit Tests (Future Work)

Component-first architecture enables easier testing:

```csharp
[Fact]
public void TodoListComponent_WithEmptyList_RendersEmptyState()
{
    // Arrange
    var page = new TodosPage();
    var component = new TodoListComponent(page);
    var dataContext = CreateDataContext(new TodosListDomainView { List = [] });

    // Act
    var html = component.Render(dataContext);

    // Assert
    html.Should().Contain("No todos yet");
}
```

### Integration Tests (Phase 3)

Test full flow with component updates:

```csharp
[Fact]
public async Task CreateTodo_RendersOobUpdate()
{
    // Arrange
    var client = CreateTestClient();

    // Act
    var response = await client.PostAsync("/interaction/todos/create", ...);
    var html = await response.Content.ReadAsStringAsync();

    // Assert
    html.Should().Contain("hx-swap-oob=\"true\"");
    html.Should().Contain("id=\"todo-list-component\"");
}
```

### Manual Testing Checklist

#### Scenario 1: Create Todo
1. Navigate to `/todos`
2. Enter "Test todo" in input
3. Click "Add Todo"
4. **Expected:** New todo appears in list, form clears
5. **Verify:** Network tab shows OOB response with `hx-swap-oob="true"`

#### Scenario 2: Toggle Todo
1. Click checkbox on any todo
2. **Expected:** Checkbox updates, todo moves to bottom if completed
3. **Verify:** Entire list updates via OOB, correct CSS classes applied

#### Scenario 3: Delete Todo
1. Click × button on any todo
2. **Expected:** Todo disappears from list
3. **Verify:** List updates via OOB

#### Scenario 4: Multi-Domain (Future)
1. Create todo
2. **Expected:** List updates AND analytics widget updates (if exists)
3. **Verify:** Multiple components receive OOB updates in single response

#### Browser Testing
- [ ] Chrome (latest)
- [ ] Firefox (latest)
- [ ] Safari (latest)
- [ ] Mobile Safari (iOS)
- [ ] Chrome Mobile (Android)

#### Network Analysis
- [ ] No 404s or failed requests
- [ ] OOB responses contain correct `hx-swap-oob` attributes
- [ ] Component IDs match between page load and OOB updates
- [ ] No duplicate component IDs in DOM

---

## 📏 Success Metrics

### Code Metrics

**Before Phase 2:**
- Total lines: ~330 (page + interactions + workflows)
- Duplication: 15 lines (sorting/transformation logic)
- Query logic in commands: 2 workflows (CreateTodo, ToggleTodo)

**After Phase 2 (Expected):**
- Total lines: ~250 (24% reduction)
- Duplication: 0 lines
- Query logic in commands: 0 workflows

### Architecture Metrics

**CQRS Compliance:**
- Before: 2/3 workflows violate (67%)
- After: 0/3 workflows violate (100% compliance)

**Server Authority:**
- Before: 3/3 forms use client targets (0% server authority)
- After: 0/3 forms use targets (100% server authority)

**Component Reusability:**
- Before: 0 reusable components
- After: 1 component (TodoListComponent) usable across multiple pages

---

## 🚨 Risk Assessment

### Risk 1: Performance Regression

**Risk:** Re-fetching data via provider might be slower than workflow returning cached data

**Mitigation:**
- Most queries are <5ms (verified in existing code)
- Provider queries are identical to workflow queries
- Can add caching layer if needed (future optimization)

**Probability:** Low
**Impact:** Low (if occurs, <10ms difference)

### Risk 2: OOB Not Working

**Risk:** HTMX OOB updates might not work as expected

**Mitigation:**
- Phase 1 infrastructure already tested
- WelcomeWidget already uses OOB pattern successfully
- Incremental rollout (one interaction at a time)

**Probability:** Very Low
**Impact:** Medium (would need to rollback)

### Risk 3: Component ID Collisions

**Risk:** Multiple components might have same ID

**Mitigation:**
- Use descriptive, unique component IDs
- Framework could validate uniqueness (future enhancement)
- Browser DevTools will show errors

**Probability:** Low
**Impact:** High (broken updates)

**Prevention:** Manual review of all component IDs

### Risk 4: Breaking Changes

**Risk:** Changes might break existing functionality

**Mitigation:**
- Comprehensive testing checklist
- Manual testing before commit
- Keep old code in git history (easy rollback)

**Probability:** Low
**Impact:** Medium (user-facing bugs)

---

## 🔄 Rollback Plan

If Phase 2 fails, rollback is straightforward:

### Git Rollback
```bash
git revert <phase-2-commit>
```

### Manual Rollback (if needed)

1. **Restore Target attributes** in forms (Todos.Page.htmx.cs)
2. **Restore interaction rendering** (CreateTodo, ToggleTodo)
3. **Restore workflow responses** (add query data back)
4. **Remove TodoListComponent** (delete file)
5. **Restore route** to original implementation

**Estimated Time:** 15 minutes

---

## 📋 Implementation Order

### Phase 2A: Foundation (Do First)
1. ✅ Create TodoListComponent (Task 1)
2. ✅ Update Todos route to use component (Task 6, Option B)
3. ✅ Register component in DI (Task 8)
4. ✅ Manual test: Page loads correctly

### Phase 2B: Interactions (Do Second)
5. ✅ Update CreateTodo interaction (Task 2)
6. ✅ Update ToggleTodo interaction (Task 3)
7. ✅ Review DeleteTodo (Task 4, no changes)
8. ✅ Manual test: All interactions work

### Phase 2C: Forms (Do Third)
9. ✅ Remove Target from CreateTodo form (Task 5.1)
10. ✅ Remove Target from ToggleTodo form (Task 5.2)
11. ✅ Remove Target from DeleteTodo button (Task 5.3)
12. ✅ Manual test: All forms still work

### Phase 2D: Workflows (Do Fourth)
13. ✅ Simplify CreateTodo response (Task 7.1)
14. ✅ Simplify ToggleTodo response (Task 7.2)
15. ✅ Verify DeleteTodo (Task 7.3, already correct)
16. ✅ Manual test: End-to-end flow works

### Phase 2E: Validation (Do Last)
17. ✅ Run full manual testing checklist
18. ✅ Verify metrics (code reduction, CQRS compliance)
19. ✅ Update experiment document with results
20. ✅ Commit with detailed message

**Estimated Total Time:** 2-3 hours

---

## 📝 Commit Message Template

```
Implement Phase 2: Convert Todos to component-first architecture

Changes:
- Add TodoListComponent with IServerComponent interface
- Update interactions to use BuildOobResult() for pure OOB
- Remove Target attributes from all forms (pure server authority)
- Simplify workflow responses to metadata only (CQRS compliant)
- Update Todos route to use component rendering

Impact:
- 24% code reduction (~80 lines removed)
- Zero query logic duplication
- 100% CQRS compliance
- 100% server authority (no client-side targets)

Testing:
- Manual testing: All CRUD operations work correctly
- Network analysis: OOB updates render correctly
- No user-visible behavior changes

Related: project-docs/experiments/component-first-architecture.md

Co-Authored-By: Claude <noreply@anthropic.com>
```

---

## 🎓 Key Learnings (To Document After Implementation)

Track these insights during implementation:

1. **What was easier than expected?**
2. **What was harder than expected?**
3. **What patterns emerged?**
4. **What would you do differently?**
5. **What questions remain for Phase 3?**

---

## 📚 Next Phase Preview

### Phase 3: Validation & Measurement

After Phase 2 completion:

1. **Performance testing** - Measure actual response times
2. **Code metrics** - Verify predicted reductions
3. **Multi-domain test** - Add TodoAnalyticsWidget
4. **Developer experience** - Assess pattern clarity
5. **Documentation** - Update architecture docs

### Phase 4: Documentation

Document the pattern for team:

1. Component-first pattern guide
2. Migration guide for existing features
3. Architecture decision records
4. Code examples and templates
5. Testing patterns

---

**Last Updated:** 2025-12-02
**Next Action:** Begin Task 1 (Create TodoListComponent)
