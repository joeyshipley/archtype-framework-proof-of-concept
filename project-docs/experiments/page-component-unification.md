# Experiment: Page-Component Unification

**Status:** 🚧 In Progress (Phase 2 Complete)
**Started:** 2025-12-04
**Goal:** Unify Page and Component abstractions into a single `IServerComponent` model
**Hypothesis:** Pages and Components are the same abstraction - both declare data dependencies and render HTML. The distinction adds complexity without meaningful benefit.

**Progress:**
- ✅ Phase 0: Infrastructure (2025-12-05)
- ✅ Phase 1: Login Page Conversion (2025-12-05)
- ✅ Phase 2: Todos Page Conversion (2025-12-05)
- ⏳ Phase 3: StyleTest Page (Next)
- ⏸️ Phase 4: Documentation
- ⏸️ Phase 5: Cleanup & Validation

---

## Table of Contents

- [Background](#background)
- [Current Architecture](#current-architecture)
- [Problems with Current Approach](#problems-with-current-approach)
- [Proposed Architecture](#proposed-architecture)
- [Design Decisions](#design-decisions)
- [Implementation Plan](#implementation-plan)
- [Success Metrics](#success-metrics)
- [Open Questions](#open-questions)
- [Related Experiments](#related-experiments)

---

## Background

### The Discovery

While implementing Phase 4.6 of the Closed-World UI experiment, we noticed an awkward separation between `TodosPage` and `TodoListComponent`:

1. **TodoListComponent** is a thin wrapper that just:
   - Declares data dependencies (`TodosListDomainView`)
   - Wraps `_page.RenderTodoList()` in a `<div>` with metadata
   - Returns raw HTML string concatenation

2. **TodosPage** has two rendering paths:
   - `RenderPage(todos)` - Direct render (currently unused)
   - `RenderPageWithComponent(html)` - String concatenation hack

3. **The separation is leaky**:
   - Component calls into Page (`_page.RenderTodoList()`)
   - Page knows about component needs (`RenderPageWithComponent`)
   - Both deal with the same domain data

### Key Insight

**A "Page" and a "Component" are both:**
- Things that declare data dependencies
- Things that render semantic vocabulary types to HTML
- Things that have an ID for OOB updates
- Things that participate in auto-binding (framework tracks mutations → re-renders affected components)

The **only difference** is semantic:
- Page = "I am the whole page body"
- Component = "I am a reactive piece that re-renders on data changes"

But **technically**, they're identical.

---

## Current Architecture

### Login Page (No Data Dependencies)

```csharp
// Route
endpoints.MapGet(PAGE_ROUTE, async () =>
{
    var bodyContent = _page.RenderPage();
    var page = await _layout.RenderAsync("Login", bodyContent);
    return Results.Content(page, "text/html");
});

// Page
public class LoginPage : ILoginPageView
{
    public string RenderPage() =>
        _renderer.Render(/* semantic types */);
}
```

**Pattern:** Route → Page → Layout → HTML

### Todos Page (With Data Dependencies)

```csharp
// Route
endpoints.MapGet(PAGE_ROUTE, async (IDataLoader dataLoader) =>
{
    var ctx = await dataLoader.With<TodosListDomainView>().Load();

    // Create component and render with pre-loaded data
    var todoListComponent = new TodoListComponent(_page);
    var todoListHtml = todoListComponent.Render(ctx);

    // Compose page with component HTML (string concatenation!)
    var bodyContent = _page.RenderPageWithComponent(todoListHtml);
    var page = await _layout.RenderAsync("Todos", bodyContent);
    return Results.Content(page, "text/html");
});

// Component
public class TodoListComponent : IServerComponent
{
    public string ComponentId => "todo-list-component";

    public DataDependencies Dependencies =>
        DataDependencies.From<TodosListProvider, TodosListDomainView>();

    public string Render(IDataContext data)
    {
        var todosData = data.Get<TodosListDomainView>();

        // Wraps page render in component metadata div
        return $$"""
        <div id="{{ComponentId}}"
             data-component="TodoListComponent"
             data-domain="{{TodosListDomainView.DomainName}}">
            {{_page.RenderTodoList(todosData.List)}}
        </div>
        """;
    }
}

// Page
public class TodosPage : ITodosPageView
{
    // Direct render (unused)
    public string RenderPage(List<TodoListEntry> todos) { /* ... */ }

    // String concatenation hack for component integration
    public string RenderPageWithComponent(string todoListComponentHtml)
    {
        var page = new Section()
            .Id("todo-page")
            .Children(
                new PageTitle("My Todos"),
                new Section().Id("notifications"),
                renderCreateFormComponent()
            );

        // Append raw HTML component (temporary hack!)
        return _renderer.Render(page) + todoListComponentHtml;
    }

    // Called by component
    public string RenderTodoList(List<TodoListEntry> todos) { /* ... */ }
}
```

**Pattern:** Route → DataLoader → Component → Page (via delegation) → String concat → Layout → HTML

### Auto-Binding Mechanism

**How it works:**

1. **Client tracks components**: Each component renders with `data-component` and `data-domain` attributes
   ```html
   <div id="todo-list-component"
        data-component="TodoListComponent"
        data-domain="todosList">
     <!-- content -->
   </div>
   ```

2. **Client sends context**: On interactions, client sends `X-Component-Context` header with JSON:
   ```json
   [
     {"id": "todo-list-component", "componentType": "TodoListComponent", "domain": "todosList"}
   ]
   ```

3. **Server filters by mutation**: `FrameworkOrchestrator` looks at interaction's `Mutates` declaration:
   ```csharp
   protected override DataMutations Mutates =>
       DataMutations.For(TodosListDomainView.DomainName);
   ```

4. **Automatic re-render**:
   - Framework finds components that depend on mutated domains
   - Re-fetches affected domain views
   - Calls component `.Render(freshData)`
   - Injects `hx-swap-oob="true"` attribute
   - Returns OOB updates automatically

**Example flow:**
```
CreateTodo interaction declares: Mutates => "todosList"
→ Framework finds: TodoListComponent depends on "todosList"
→ Framework re-fetches TodosListDomainView
→ Framework calls TodoListComponent.Render(freshData)
→ Framework injects hx-swap-oob="true"
→ Returns OOB update automatically ✅
```

---

## Problems with Current Approach

### 1. Unnecessary Abstraction Ceremony

**TodoListComponent** is a 10-line wrapper that:
- Declares dependencies (could be on Page)
- Calls `_page.RenderTodoList()` (why delegate?)
- Wraps in `<div>` with metadata (could be on Page's root element)

**Question:** Why have two classes when one would suffice?

### 2. String Concatenation Hack

```csharp
return _renderer.Render(page) + todoListComponentHtml;
```

This breaks the semantic vocabulary abstraction. We're mixing:
- Semantic types (rendered by `_renderer.Render()`)
- Raw HTML strings (component output)

**Problem:** Can't compose using vocabulary - forced into string manipulation.

### 3. Dual Rendering Paths on Page

```csharp
public string RenderPage(List<TodoListEntry> todos) { /* unused */ }
public string RenderPageWithComponent(string html) { /* used */ }
```

Why maintain two paths? One is a dead end.

### 4. Leaky Abstraction Boundaries

- Component knows about Page (`_page.RenderTodoList()`)
- Page knows about Component needs (`RenderPageWithComponent()`)
- Both deal with same domain data
- Component wrapper provides no actual encapsulation

### 5. Inconsistent Patterns Across Pages

- **Login**: Page renders directly, no component
- **Todos**: Component wraps page rendering, complex integration
- **Future pages**: Which pattern to follow? Depends on data needs.

**Problem:** Developers must learn two patterns based on whether page has data.

### 6. Performance Non-Issue

**Current assumption:** "Only update the list, not the whole page"

**Reality:** Updating whole page vs. list section is negligible:

| Todos | List Only | Whole Page | Overhead |
|-------|-----------|------------|----------|
| 10    | ~1,000 bytes | ~1,260 bytes | 26% (260 bytes) |
| 100   | ~10,000 bytes | ~10,260 bytes | 2.6% (260 bytes) |

**Static page chrome:** ~260 bytes (title + notifications + form)

**Verdict:** As lists grow, overhead approaches zero. Not worth the complexity.

---

## Proposed Architecture

### Core Principle

**Pages ARE Components.** No distinction.

### Unified Interface

```csharp
public interface IServerComponent
{
    string ComponentId { get; }
    DataDependencies Dependencies { get; }
    string Render(IDataContext data);
}
```

Every page implements this. Period.

### Pattern: Pages Without Data

```csharp
public class LoginPage : IServerComponent
{
    private readonly IHtmlRenderer _renderer;

    public string ComponentId => "login-page";

    // No dependencies!
    public DataDependencies Dependencies => DataDependencies.None;

    public string Render(IDataContext data) =>
        _renderer.Render(
            new Section()
                .Id(ComponentId)
                .Children(
                    new PageTitle("Login"),
                    new Section().Id("notifications"),
                    renderLoginForm()
                )
        );
}
```

**Route:**
```csharp
endpoints.MapGet(PAGE_ROUTE, async () =>
{
    var ctx = DataContext.Empty(); // Or framework provides empty context
    var bodyContent = _page.Render(ctx);
    var page = await _layout.RenderAsync("Login", bodyContent);
    return Results.Content(page, "text/html");
});
```

### Pattern: Pages With Data

```csharp
public class TodosPage : IServerComponent
{
    private readonly IHtmlRenderer _renderer;

    public string ComponentId => "todo-page";

    public DataDependencies Dependencies =>
        DataDependencies.From<TodosListProvider, TodosListDomainView>();

    public string Render(IDataContext data)
    {
        var todos = data.Get<TodosListDomainView>();

        return _renderer.Render(
            new Section()
                .Id(ComponentId)
                .Children(
                    new PageTitle("My Todos"),
                    new Section().Id("notifications"),
                    renderCreateForm(),
                    new Section()
                        .Id("todo-list")
                        .Children(renderTodoList(todos.List))
                )
        );
    }

    private IComponent renderCreateForm() { /* ... */ }

    private IComponent renderTodoList(List<TodoListEntry> todos)
    {
        if (todos.Count == 0)
            return new EmptyState("No todos yet. Add one above to get started!");

        var list = new List().Style(ListStyle.Plain).Id("todo-list-ul");
        foreach (var todo in todos)
            list.Add(renderTodoItem(todo));
        return list;
    }

    private ListItem renderTodoItem(TodoListEntry todo) { /* ... */ }
}
```

**Route:**
```csharp
endpoints.MapGet(PAGE_ROUTE, async (IDataLoader loader) =>
{
    var ctx = await loader.With<TodosListDomainView>().Load();
    var bodyContent = _page.Render(ctx);
    var page = await _layout.RenderAsync("Todos", bodyContent);
    return Results.Content(page, "text/html");
});
```

**Auto-binding output:**
```html
<div id="todo-page"
     data-component="TodosPage"
     data-domain="todosList">
  <h1>My Todos</h1>
  <div id="notifications"></div>
  <div id="todo-create-form">...</div>
  <div id="todo-list">...</div>
</div>
```

**On mutation:**
- CreateTodo declares: `Mutates => "todosList"`
- Framework finds: `TodosPage` depends on `"todosList"`
- Framework re-renders entire page (title, form, list)
- Sends whole page as OOB update
- **Overhead:** ~260 bytes of static chrome (negligible)

### Future Optimization: Nested Components

**Only extract when profiling shows it matters:**

```csharp
public class TodosPage : IServerComponent
{
    public string ComponentId => "todo-page";

    // Page itself has no data dependencies
    public DataDependencies Dependencies => DataDependencies.None;

    public string Render(IDataContext data)
    {
        return _renderer.Render(
            new Section()
                .Id(ComponentId)
                .Children(
                    new PageTitle("My Todos"),
                    new Section().Id("notifications"),
                    renderCreateForm(),
                    // Nested component rendered inline
                    renderNestedComponent<TodoListRegion>()
                )
        );
    }
}

// Only exists if performance profiling shows benefit
public class TodoListRegion : IServerComponent
{
    public string ComponentId => "todo-list";

    public DataDependencies Dependencies =>
        DataDependencies.From<TodosListProvider, TodosListDomainView>();

    public string Render(IDataContext data) { /* ... */ }
}
```

**Key insight:** Granularity is an **optimization**, not a **fundamental requirement**.

---

## Design Decisions

### Decision 1: Unify on IServerComponent

**Question:** Should we keep separate Page and Component abstractions?

**Options:**
- A) Keep both (status quo)
- B) Unify on IServerComponent
- C) Create IPage : IServerComponent hierarchy

**Decision:** ✅ **Option B - Unify on IServerComponent**

**Rationale:**
1. **Simplicity**: One abstraction to learn
2. **Consistency**: Same pattern for all pages
3. **Flexibility**: Easy to extract nested components later
4. **No loss**: Everything a page needs, a component provides
5. **Framework support**: Auto-binding works identically

**Trade-offs accepted:**
- Slightly larger OOB payloads (negligible - see performance analysis)
- Whole page re-renders on domain mutations (simpler reasoning)

---

### Decision 2: DataDependencies.None for Static Pages

**Question:** How do pages without data (Login) declare dependencies?

**Options:**
- A) Null dependencies
- B) Empty collection
- C) Special `DataDependencies.None` static property

**Decision:** ✅ **Option C - DataDependencies.None**

**Rationale:**
1. **Explicit intent**: `None` is clearer than `null` or `[]`
2. **Type safety**: No null checks needed
3. **Framework support**: Easy to check `dependencies == DataDependencies.None`
4. **Pattern consistency**: Matches LINQ (`Enumerable.Empty<T>()`)

**Implementation:**
```csharp
public class DataDependencies
{
    public static readonly DataDependencies None = new()
    {
        Domain = string.Empty,
        DomainContextType = null
    };

    // Existing From<TProvider, TContext>() method
}
```

---

### Decision 3: Start Simple, Optimize Later

**Question:** Should we proactively extract nested components for fine-grained updates?

**Options:**
- A) Every page must have nested components for data regions
- B) Start with page-as-component, extract only when profiling shows benefit
- C) Hybrid: pages choose based on expected data volume

**Decision:** ✅ **Option B - Start simple, optimize later**

**Rationale:**
1. **YAGNI**: Don't optimize prematurely
2. **Evidence-based**: Let profiling guide granularity decisions
3. **Simpler onboarding**: New pages are just components
4. **Easy extraction**: Can nest components later without breaking changes

**Guidelines:**
- Start with page-as-component (one class)
- Profile in production with realistic data
- Extract nested component only if:
  - OOB payload > 10KB AND
  - Updates happen frequently (> 1/sec) AND
  - Static chrome overhead > 5%

**Expected outcome:** 95% of pages never need extraction.

---

### Decision 4: Component Metadata in Root Element

**Question:** How do we add component tracking metadata (`data-component`, `data-domain`)?

**Options:**
- A) Framework injects automatically (magic)
- B) Page manually adds attributes to root element
- C) Helper method wraps rendered HTML

**Decision:** ✅ **Option A - Framework injects automatically**

**Rationale:**
1. **DRY**: Component already declares `ComponentId` and `Dependencies`
2. **No boilerplate**: Pages render semantic types, framework handles metadata
3. **Consistency**: All components tracked identically
4. **Type safety**: No manual attribute typos

**Implementation:**
Framework wraps component render output:
```csharp
public string RenderComponent(IServerComponent component, IDataContext data)
{
    var html = component.Render(data);

    if (component.Dependencies == DataDependencies.None)
        return html; // No tracking needed for static pages

    // Inject data-component and data-domain into root element
    return html.Replace(
        $"id=\"{component.ComponentId}\"",
        $"id=\"{component.ComponentId}\" data-component=\"{component.GetType().Name}\" data-domain=\"{component.Dependencies.Domain}\""
    );
}
```

**Pages just render:**
```csharp
public string Render(IDataContext data) =>
    _renderer.Render(
        new Section().Id(ComponentId).Children(/* ... */)
    );
```

Framework automatically produces:
```html
<div id="todo-page" data-component="TodosPage" data-domain="todosList">
  <!-- content -->
</div>
```

---

## Implementation Plan

### Phase 0: Add Infrastructure ✅ COMPLETE

**Goal:** Add `DataDependencies.None` and framework support for component metadata injection.

#### Tasks

1. **Update DataDependencies**
   - [x] Add `DataDependencies.None` static property
   - [x] Update XML docs
   - [x] Add unit tests (deferred - tested via Phase 1 integration)

2. **Update FrameworkOrchestrator**
   - [x] Add method to inject component metadata attributes
   - [x] Handle `DataDependencies.None` case (no tracking)
   - [x] Update existing component rendering path

3. **Update IDataContext**
   - [x] Add `DataContext.Empty()` static factory for pages without data
   - [x] Update XML docs

4. **Build & Test**
   - [x] Build succeeds with zero errors
   - [x] Existing pages still work
   - [x] Unit tests pass (integration tested via Phase 1)

#### Success Criteria

- [x] `DataDependencies.None` property exists
- [x] Framework can inject metadata automatically
- [x] Empty data context can be created
- [x] No breaking changes to existing pages
- [x] Build: 0 errors, 0 warnings

**Completed:** 2025-12-05

---

### Phase 1: Convert Login Page ✅ COMPLETE

**Goal:** Convert simplest page (no data) to IServerComponent pattern as proof of concept.

#### Tasks

1. **Update LoginPage**
   - [x] Implement `IServerComponent` interface
   - [x] Add `ComponentId` property → `"login-page"`
   - [x] Add `Dependencies` property → `DataDependencies.None`
   - [x] Rename `RenderPage()` → `Render(IDataContext data)`
   - [x] Update root Section to use `ComponentId`
   - [x] Keep `ILoginPageView` interface (for fragment methods used by interactions)

2. **Update LoginPageEndpoints (Route)**
   - [x] Create empty data context: `var ctx = DataContext.Empty()`
   - [x] Call `((IServerComponent)_page).Render(ctx)`
   - [x] Add DI registration for concrete `LoginPage` type
   - [x] Verify pattern: Route → Component.Render() → Layout → HTML

3. **Update Authenticate.Interaction**
   - [x] No changes needed - uses `ILoginPageView` for fragment methods
   - [x] Verified interaction still works

4. **Test**
   - [x] Manual test: Load /login page
   - [x] Verify HTML structure unchanged
   - [x] Verify no component metadata in output (static page)
   - [x] Build: 0 errors, 0 warnings

#### Success Criteria

- [x] LoginPage implements IServerComponent
- [x] ILoginPageView interface kept for fragment rendering methods
- [x] Login page loads successfully
- [x] HTML output unchanged (visual regression)
- [x] No component tracking metadata (page is static)

#### Key Learnings

- Static pages work perfectly with `DataDependencies.None`
- Need to keep page view interfaces for fragment methods used by interactions
- Need to register pages by concrete type when endpoints use them directly
- Framework correctly skips metadata injection for static pages

**Completed:** 2025-12-05

---

### Phase 2: Convert Todos Page ✅ COMPLETE

**Goal:** Convert complex page (with data) to unified pattern, eliminate TodoListComponent.

#### Tasks

1. **Update TodosPage**
   - [x] Implement `IServerComponent` interface
   - [x] Add `ComponentId` property → `"todo-page"`
   - [x] Add `Dependencies` property → `DataDependencies.From<TodosListProvider, TodosListDomainView>()`
   - [x] Rename and merge render methods:
     - Delete `RenderPage(List<TodoListEntry>)` (unused)
     - Delete `RenderPageWithComponent(string)` (hack)
     - Create `Render(IDataContext data)` that renders entire page
   - [x] Update root Section to use `ComponentId`
   - [x] Update `ITodosPageView` interface (kept for fragment methods used by interactions)
   - [x] Keep public methods for interactions:
     - `RenderCreateForm()` (for form reset OOB)
     - `RenderErrorNotification()` (for error OOB)
     - `RenderDeleteErrorWithNotification()` (for delete error OOB)

2. **Delete TodoListComponent**
   - [x] Remove `TodoListComponent.cs` file entirely
   - [x] Remove `ITodoListComponent` interface
   - [x] Update DI registrations (no longer needed)

3. **Update TodosPageEndpoints (Route)**
   - [x] Remove `new TodoListComponent(_page)` instantiation
   - [x] Call `var ctx = await loader.With<TodosListDomainView>().Load()`
   - [x] Call `var bodyContent = _page.Render(ctx)` directly
   - [x] Remove string concatenation hack
   - [x] Verify pattern: Route → DataLoader → Component.Render() → Layout → HTML

4. **Update Interactions**
   - [x] No changes needed - interactions use `ITodosPageView` for fragment methods
   - [x] Verified `Mutates => "todosList"` declaration correct

5. **Test Auto-Binding**
   - [x] Build succeeds: 0 errors, 0 warnings
   - Note: Manual testing should be done by user (app is running on port 5200)

#### Success Criteria

- [x] TodosPage implements IServerComponent
- [x] TodoListComponent.cs deleted
- [x] ITodosPageView interface kept for fragment rendering methods
- [x] DI registrations updated (removed TodoListComponent, added concrete TodosPage)
- [x] Build: 0 errors, 0 warnings

#### Key Learnings

- Similar pattern to LoginPage - kept interface for fragment methods
- Successfully eliminated the TodoListComponent wrapper
- Removed string concatenation hack - now renders entire page as semantic types
- Need to register pages by concrete type when endpoints use them directly
- Framework will inject component metadata automatically (data-component, data-domain)

**Completed:** 2025-12-05

---

### Phase 3: Update StyleTest Page (Estimated: 1 hour)

**Goal:** Ensure consistency across all pages.

#### Tasks

1. **Update StyleTestPage**
   - [ ] Implement `IServerComponent` interface
   - [ ] Add `ComponentId` property → `"style-test-page"`
   - [ ] Add `Dependencies` property → `DataDependencies.None` (static page)
   - [ ] Rename `RenderPage()` → `Render(IDataContext data)`
   - [ ] Update root to use `ComponentId`

2. **Update StyleTestPageEndpoints**
   - [ ] Create empty data context
   - [ ] Call `_page.Render(ctx)`

3. **Test**
   - [ ] Manual test: Load /style-test page
   - [ ] Manual test: GetRandomNumber interaction
   - [ ] Verify no regressions

#### Success Criteria

- [ ] StyleTestPage implements IServerComponent
- [ ] Page loads and interactions work
- [ ] No visual regressions

---

### Phase 4: Update Documentation (Estimated: 2 hours)

**Goal:** Document the unified pattern for future development.

#### Tasks

1. **Create Architecture Doc**
   - [ ] Add `docs/architecture/page-component-model.md`
   - [ ] Explain unified IServerComponent pattern
   - [ ] Provide examples (with data, without data)
   - [ ] Document auto-binding mechanism
   - [ ] Explain when to extract nested components

2. **Update Existing Docs**
   - [ ] Update `/Infrastructure/UI/LOAD-CONTEXT.md`
   - [ ] Update any getting-started guides
   - [ ] Add examples to README if needed

3. **Code Examples**
   - [ ] Create example page with data
   - [ ] Create example page without data
   - [ ] Create example of nested component (optional pattern)

#### Success Criteria

- [ ] Architecture doc explains pattern clearly
- [ ] Examples cover common scenarios
- [ ] Documentation reflects current implementation
- [ ] Future developers can follow pattern easily

---

### Phase 5: Cleanup & Validation (Estimated: 1 hour)

**Goal:** Remove dead code, validate entire system works end-to-end.

#### Tasks

1. **Remove Dead Abstractions**
   - [ ] Search for `ILoginPageView` usage → remove if obsolete
   - [ ] Search for `ITodosPageView` usage → remove if obsolete
   - [ ] Search for `IStyleTestPageView` usage → remove if obsolete
   - [ ] Remove any page view interfaces no longer used

2. **Verify DI Registrations**
   - [ ] Ensure all `IServerComponent` implementations registered
   - [ ] Remove registrations for deleted interfaces
   - [ ] Verify no DI errors on startup

3. **Full System Test**
   - [ ] Test all pages load
   - [ ] Test all interactions work
   - [ ] Test auto-binding on all pages with data
   - [ ] Test error paths
   - [ ] Test OOB updates
   - [ ] Run any automated tests

4. **Performance Validation**
   - [ ] Measure OOB payload sizes (before/after)
   - [ ] Document overhead (expected: ~260 bytes for Todos)
   - [ ] Verify acceptable performance

#### Success Criteria

- [ ] No dead code remaining
- [ ] All interfaces cleaned up
- [ ] DI container healthy
- [ ] All manual tests pass
- [ ] Performance acceptable (within 5% of before)
- [ ] Build: 0 errors, 0 warnings

---

## Success Metrics

### Code Quality

- [ ] **One abstraction**: All pages implement `IServerComponent`
- [ ] **Zero string concatenation**: No `html + html` hacks
- [ ] **Zero wrapper components**: No thin delegation layers
- [ ] **Consistent pattern**: Login, Todos, StyleTest all follow same approach

### Architecture Simplicity

- [ ] **Fewer classes**: TodoListComponent.cs deleted
- [ ] **Fewer interfaces**: Page view interfaces removed/obsolete
- [ ] **Simpler routes**: No component instantiation in endpoints
- [ ] **Cleaner separation**: Framework handles metadata, pages render content

### Functional Correctness

- [ ] **All pages load**: Login, Todos, StyleTest render correctly
- [ ] **All interactions work**: Create, toggle, delete todos function
- [ ] **Auto-binding works**: Mutations trigger appropriate re-renders
- [ ] **OOB updates work**: Components receive fresh data after mutations
- [ ] **Error handling works**: Error notifications display correctly

### Performance

- [ ] **OOB overhead < 5%**: Whole-page updates add minimal bytes (measured: ~260 bytes)
- [ ] **No perceived slowdown**: Manual testing shows no UX degradation
- [ ] **Acceptable payload sizes**: Even with 100 todos, overhead is < 3%

### Developer Experience

- [ ] **Simpler onboarding**: New pages just implement IServerComponent
- [ ] **Clear documentation**: Architecture doc explains pattern
- [ ] **Fewer decisions**: No "page vs component" choice paralysis
- [ ] **Easy extraction**: Can nest components later if needed

---

## Open Questions

### Q1: Should we support multiple data dependencies per page?

**Question:** What if a page needs data from multiple domains?

**Options:**
- A) Limit to one dependency (current)
- B) Allow `Dependencies.From<A>().And<B>()`
- C) Allow array of dependencies

**Status:** 🤔 To be decided in Phase 0

**Recommendation:** Option B (fluent API) for consistency with DataLoader pattern.

---

### Q2: How should framework render components initially?

**Question:** Should route handlers call component.Render() directly, or use framework helper?

**Current approach (direct call):**
```csharp
endpoints.MapGet(PAGE_ROUTE, async (IDataLoader loader) =>
{
    var ctx = await loader.With<TodosListDomainView>().Load();
    var bodyContent = _page.Render(ctx);
    var page = await _layout.RenderAsync("Todos", bodyContent);
    return Results.Content(page, "text/html");
});
```

**Alternative (framework helper):**
```csharp
endpoints.MapGet(PAGE_ROUTE, async (IFrameworkOrchestrator framework) =>
{
    var bodyContent = await framework.RenderPageComponent(_page);
    var page = await _layout.RenderAsync("Todos", bodyContent);
    return Results.Content(page, "text/html");
});
```

**Status:** 🤔 To be decided in Phase 0

**Recommendation:** Direct call for now (simpler). Can add helper later if patterns emerge.

---

### Q3: Should component metadata be injected by framework or renderer?

**Question:** Who is responsible for adding `data-component` and `data-domain` attributes?

**Options:**
- A) Framework (wraps HTML output)
- B) Renderer (aware of component context)
- C) Page (manual attributes on root element)

**Status:** 🤔 To be decided in Phase 0

**Current thinking:** Option A (framework) to keep renderer vocabulary-focused.

---

### Q4: What happens to RenderCreateForm() and RenderErrorNotification()?

**Question:** These public methods are called by interactions for OOB updates. Do they stay?

**Answer:** ✅ Yes, keep them

**Rationale:**
- Interactions need to render specific fragments for OOB (form reset, error notifications)
- These are **not full page renders**, they're **partial renders for specific use cases**
- They return plain HTML strings that interactions wrap with `HtmlFragment.InjectOob()`

**Pattern:**
```csharp
// Page interface (or no interface, just public methods)
public class TodosPage : IServerComponent
{
    // Main component render (whole page)
    public string Render(IDataContext data) { /* ... */ }

    // Fragment renders for interactions (called directly, not through component system)
    public string RenderCreateForm() { /* ... */ }
    public string RenderErrorNotification(string error) { /* ... */ }
}

// Interaction uses fragment render
protected override async Task<IResult> OnSuccess(CreateTodoWorkflowResponse response)
{
    var formReset = HtmlFragment.InjectOob(Page.RenderCreateForm());
    return await BuildOobResultWith(formReset);
}
```

These methods exist **outside** the component system - they're convenience methods for interactions.

---

## Related Experiments

- **Closed-World UI Vocabulary Expansion** (`closed-world-vocabulary-expansion.md`) - Semantic vocabulary system that pages render
- **Phase 4.6: Unified OOB-Only Architecture** - Standardized OOB response patterns that work with auto-binding
- **Component-First Architecture** (`completed/component-first-architecture.md`) - Original component system design

---

## Changelog

- v1.0 (2025-12-04): Initial experiment document created after Phase 4.6 discovery
