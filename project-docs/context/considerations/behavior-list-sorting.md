# Server-Authoritative Surgical List Updates with Client-Side Sorting

**Status:** Pattern Consideration
**Created:** 2025-11-23
**Context:** Server-authoritative architecture with surgical DOM updates

---

## Purpose

A pattern for handling list mutations (create, update, delete) that reduces database load and network traffic by returning only the changed item(s) while maintaining server authority over sort order and business logic.

**Core principle:** Server sends sort instructions and minimal data, client executes sort locally using existing DOM data.

---

## The Problem

**Traditional approach: Full list refetch**

When a user modifies a list item (toggle todo, delete row, create entry):
```
1. Client: PATCH /api/todos/5/toggle
2. Server: UPDATE todo SET completed = true WHERE id = 5
3. Server: SELECT * FROM todos WHERE user_id = 1 ORDER BY completed, created_at  -- Refetch ALL items
4. Server: Render full list HTML (50 items)
5. Client: Replace entire <ul> with new HTML
```

**Costs:**
- Database: 2 queries (1 mutation + 1 full SELECT)
- Network: Transfer 50 items when only 1 changed
- DOM: Replace 50 `<li>` elements when only 1 changed
- Time: ~10-15ms database + network transfer time

**For a 50-item list:**
- Changed data: 1 item (~200 bytes)
- Transferred data: 50 items (~10KB HTML)
- Waste ratio: 50x more data than necessary

---

## The Solution

**Surgical updates with client-side sorting:**

```
1. Client: PATCH /api/todos/5/toggle
2. Server: UPDATE todo SET completed = true WHERE id = 5
3. Server: SELECT * FROM todos WHERE id = 5  -- Fetch ONLY changed item
4. Server: Render single <li> with data attributes for sorting
5. Client: Idiomorph swaps just the changed <li>
6. Client: Reads data-sort instruction from parent <ul>
7. Client: Re-sorts all <li> children using their data-* attributes
```

**Costs:**
- Database: 1 query (mutation returns changed row, or separate SELECT for 1 item)
- Network: Transfer 1 item (~200 bytes)
- DOM: Replace 1 `<li>`, re-sort existing elements
- Time: ~5-7ms database + ~0.1ms client sort

**Savings for 50-item list:**
- 50% fewer database queries
- 98% less network traffic
- 98% less DOM churn
- ~40% faster overall

---

## Implementation Pattern

### Step 1: Add Sort Data to HTML

**Server renders list with sortable data attributes:**

```html
<!-- Initial page load: Include sort metadata on every item -->
<ul class="todo-list" data-sort="completed-asc,created-desc">
  <li id="todo-1"
      data-completed="false"
      data-created="2024-11-20T10:30:00Z"
      data-title="Buy milk">
    <input type="checkbox" /> Buy milk
  </li>
  <li id="todo-2"
      data-completed="true"
      data-created="2024-11-19T08:15:00Z"
      data-title="Walk dog">
    <input type="checkbox" checked /> Walk dog
  </li>
  <li id="todo-3"
      data-completed="false"
      data-created="2024-11-21T14:45:00Z"
      data-title="Write docs">
    <input type="checkbox" /> Write docs
  </li>
</ul>
```

**Key attributes:**
- `data-sort` on parent: Server's sort instruction (source of truth)
- `data-completed`, `data-created`, `data-title`: Sort keys on each item
- `id`: Required for idiomorph to identify which element to swap

---

### Step 2: Return Surgical Updates

**Server endpoint returns ONLY the changed item:**

```csharp
// Example: Toggle endpoint
[HttpPatch("/api/todos/{id}/toggle")]
public async Task<IResult> Toggle(int id)
{
    // Mutation
    var todo = await _repository.GetByIdTracked(id);
    todo.IsCompleted = !todo.IsCompleted;
    await _repository.SaveChanges();

    // Return ONLY the changed item with all sort attributes
    var html = RenderTodoItem(todo);
    return Results.Content(html, "text/html");
}

private string RenderTodoItem(Todo todo)
{
    return $"""
    <li id="todo-{todo.Id}"
        data-completed="{todo.IsCompleted.ToString().ToLower()}"
        data-created="{todo.CreatedAt:o}"
        data-title="{HttpUtility.HtmlAttributeEncode(todo.Title)}">
      <input type="checkbox" {(todo.IsCompleted ? "checked" : "")} />
      {HttpUtility.HtmlEncode(todo.Title)}
    </li>
    """;
}
```

**HTMX configuration:**
```html
<input type="checkbox"
       hx-patch="/api/todos/{{id}}/toggle"
       hx-target="#todo-{{id}}"
       hx-swap="outerHTML" />
```

**What happens:**
1. HTMX sends PATCH request
2. Server returns single `<li>` element
3. `hx-target="#todo-{{id}}"` tells HTMX which element to replace
4. `hx-swap="outerHTML"` replaces the entire `<li>`
5. Idiomorph smoothly morphs just that one element

---

### Step 3: Client-Side Sort After Swap

**JavaScript re-sorts after every HTMX swap:**

```javascript
// After any HTMX swap, re-sort the list according to server's instructions
document.body.addEventListener('htmx:afterSwap', (e) => {
  const list = e.target.closest('.todo-list');
  if (!list) return;

  const sortInstruction = list.dataset.sort; // e.g., "completed-asc,created-desc"
  if (!sortInstruction) return;

  sortList(list, sortInstruction);
});

function sortList(listElement, sortInstruction) {
  const rules = parseSortRules(sortInstruction); // Parse "completed-asc,created-desc"
  const items = Array.from(listElement.children);

  items.sort((a, b) => {
    for (const rule of rules) {
      const comparison = compareByRule(a, b, rule);
      if (comparison !== 0) return comparison;
    }
    return 0;
  });

  // Re-append items in sorted order (DOM moves them, doesn't recreate)
  items.forEach(item => listElement.appendChild(item));
}

function compareByRule(itemA, itemB, rule) {
  const { field, direction } = rule; // { field: "completed", direction: "asc" }
  const valueA = itemA.dataset[field];
  const valueB = itemB.dataset[field];

  let comparison = 0;

  // Type-specific comparison
  if (field === 'completed') {
    // Boolean: false < true
    comparison = (valueA === 'true') - (valueB === 'true');
  } else if (field === 'created') {
    // ISO date string comparison
    comparison = valueA.localeCompare(valueB);
  } else {
    // String comparison
    comparison = valueA.localeCompare(valueB);
  }

  return direction === 'desc' ? -comparison : comparison;
}

function parseSortRules(instruction) {
  // "completed-asc,created-desc" → [{ field: "completed", direction: "asc" }, ...]
  return instruction.split(',').map(rule => {
    const [field, direction] = rule.split('-');
    return { field, direction };
  });
}
```

---

### Step 4: Handle Different Operations

#### **Toggle (Update)**
```
Server: Returns updated <li>
Client: Replaces <li>, re-sorts
Database: 1 query (SELECT + UPDATE or UPDATE RETURNING)
```

#### **Create**
```html
<!-- HTMX appends new item to list -->
<form hx-post="/api/todos"
      hx-target=".todo-list"
      hx-swap="beforeend">
  <input name="title" />
</form>
```

```
Server: Returns new <li> with all data attributes
Client: Appends <li> to list, re-sorts
Database: 1 query (INSERT RETURNING)
```

#### **Delete**
```html
<button hx-delete="/api/todos/{{id}}"
        hx-target="#todo-{{id}}"
        hx-swap="outerHTML">Delete</button>
```

```
Server: Returns 200 OK (no content needed)
Client: HTMX removes <li>, list already sorted
Database: 1 query (DELETE)
```

---

## When to Use This Pattern

### ✅ **Good fit when:**

1. **List size: 20-500 items**
   - Below 20: Full refetch is fast enough, not worth complexity
   - Above 500: Need pagination anyway (different pattern)

2. **Frequent mutations**
   - Users toggle/update items often (todo lists, task boards)
   - Creating/deleting items multiple times per session
   - High mutation-to-view ratio

3. **Sort order is important**
   - Items need to stay sorted after mutations (completed items to bottom)
   - Multiple sort criteria (status + date + priority)
   - Server controls sort rules (business logic, not user preference)

4. **Server-authoritative architecture**
   - You're using HTMX, Hotwire, or similar hypermedia approach
   - Server owns business logic and validation
   - Client enhances with progressive JavaScript

5. **Moderate network conditions**
   - Bandwidth-constrained environments (mobile, international users)
   - Cost-per-byte matters (metered connections)
   - Reducing payload size improves performance meaningfully

---

### ❌ **Poor fit when:**

1. **Tiny lists (< 20 items)**
   - Full refetch: ~2KB HTML, ~8ms
   - Surgical update: ~200 bytes HTML + sort JS overhead, ~7ms
   - Savings: 1ms, not worth the complexity

2. **Huge lists (> 500 items)**
   - Sorting 500+ DOM elements: ~5-10ms client-side
   - Server-side ORDER BY on indexed column: ~1-2ms
   - You need pagination/virtualization, not client sorting

3. **Real-time collaborative lists**
   - Other users' changes arrive via WebSocket
   - Need operational transforms or CRDTs
   - Different architecture required

4. **Complex sort logic**
   - Sort depends on computed values not in data attributes
   - Sort requires database joins or aggregations
   - Business logic too complex for client-side replication

5. **User-controlled sort order**
   - Drag-and-drop reordering (manual sort)
   - User saves custom sort preferences
   - Server doesn't dictate order

6. **SPA architecture**
   - Already maintaining client-side state
   - Have full list in JavaScript memory
   - Framework handles sorting natively (React, Vue, etc.)

---

## Trade-offs and Considerations

### **Architectural Complexity**

**Added complexity:**
- ✍️ Must include sort-relevant data in data attributes
- ✍️ Need client-side JS sort logic (~50 lines)
- ✍️ Must keep data attributes in sync with rendered content
- ✍️ More surface area for bugs (client/server sort mismatch)

**Reduced complexity:**
- ✅ No need to fetch entire list after mutations
- ✅ No need to track "which items changed" for diffing
- ✅ Simpler database queries (no ORDER BY on refetch)

**Net:** Slight increase in complexity, but stays within "consistent complexity" budget for medium-sized lists.

---

### **Performance Trade-offs**

**What you gain:**
- 50% fewer database queries (1 instead of 2)
- 95-98% less network traffic (1 item vs full list)
- 95-98% less DOM churn (swap 1 element vs 50)
- Better for bandwidth-constrained users

**What you pay:**
- ~0.1ms client-side sort time (negligible for < 200 items)
- Slightly larger initial payload (data attributes on every item)
- More client-side JavaScript (~1-2KB for sort logic)

**Break-even point:**
- Lists with 20+ items where mutations are common
- Below 20 items: Full refetch is simpler and equally fast
- Above 200 items: Consider pagination or virtualization

---

### **Data Consistency**

**Server authority maintained:**
- ✅ Server sends sort instruction via `data-sort` attribute
- ✅ Server controls which fields are sortable
- ✅ Client never invents sort rules, only executes server's instructions
- ✅ If server changes sort logic, client automatically follows

**Potential issues:**
- ⚠️ Data attributes must match rendered HTML (e.g., `data-title` matches visible title)
- ⚠️ If server sends incomplete data attributes, sort may break
- ⚠️ Client and server must agree on data type comparisons (dates, booleans, strings)

**Mitigation:**
- Validate data attributes in server-side rendering helpers
- Unit test sort logic matches server's ORDER BY clause
- Use consistent date formats (ISO 8601)

---

### **Scaling Considerations**

**As list grows:**

| List Size | Full Refetch | Surgical + Sort | Winner |
|-----------|--------------|-----------------|--------|
| 10 items  | ~5ms, 1KB    | ~5ms, 200B      | Tie    |
| 50 items  | ~10ms, 5KB   | ~6ms, 200B      | Surgical |
| 200 items | ~15ms, 20KB  | ~8ms, 200B      | Surgical |
| 500 items | ~25ms, 50KB  | ~12ms, 200B     | Surgical |
| 1000 items| ~50ms, 100KB | ~25ms, 200B     | Pagination needed |

**Inflection points:**
- Below 20 items: Not worth it
- 20-500 items: Sweet spot for this pattern
- Above 500 items: Pagination > client-side sort

---

## Implementation Checklist

When implementing this pattern, ensure:

- [ ] **Initial page load includes all sort data**
  - Every `<li>` has required data attributes (completed, created, etc.)
  - Parent `<ul>` has `data-sort` instruction
  - Attributes use consistent types (ISO dates, lowercase booleans)

- [ ] **Mutation endpoints return single items**
  - Toggle: Returns updated `<li>` with all data attributes
  - Create: Returns new `<li>` with all data attributes
  - Delete: Returns 200 OK, HTMX removes element

- [ ] **HTMX configured for surgical swaps**
  - `hx-target="#specific-item-id"` for updates
  - `hx-swap="outerHTML"` to replace entire element
  - `hx-swap="beforeend"` for creates (append to list)
  - `hx-swap="delete"` for deletes (optional, HTMX removes on 200)

- [ ] **Client-side sort executes after swaps**
  - Listen to `htmx:afterSwap` event
  - Parse `data-sort` instruction from parent
  - Re-sort all `<li>` children using data attributes
  - Handle multiple sort criteria (primary, secondary, tertiary)

- [ ] **Sort logic matches server's ORDER BY**
  - Unit test client sort against server's expected order
  - Test edge cases (null values, ties, mixed types)
  - Document sort algorithm in both client and server code

- [ ] **Graceful degradation**
  - Works without JavaScript (HTMX still swaps, just unsorted)
  - Sort attributes don't break styling or accessibility
  - Screen readers ignore data attributes

---

## Testing Strategy

### **Unit Tests**

**Server-side:**
```csharp
[Test]
public void RenderTodoItem_IncludesAllSortAttributes()
{
    var todo = new Todo {
        Id = 1,
        Title = "Test",
        IsCompleted = true,
        CreatedAt = DateTime.Parse("2024-11-20")
    };

    var html = RenderTodoItem(todo);

    Assert.Contains("data-completed=\"true\"", html);
    Assert.Contains("data-created=\"2024-11-20", html);
    Assert.Contains("data-title=\"Test\"", html);
}
```

**Client-side:**
```javascript
test('sortList orders by completed then created', () => {
  const list = document.createElement('ul');
  list.innerHTML = `
    <li data-completed="true" data-created="2024-11-20">Done old</li>
    <li data-completed="false" data-created="2024-11-21">Todo new</li>
    <li data-completed="false" data-created="2024-11-19">Todo old</li>
  `;
  list.dataset.sort = 'completed-asc,created-desc';

  sortList(list, list.dataset.sort);

  const items = Array.from(list.children).map(li => li.textContent);
  expect(items).toEqual(['Todo new', 'Todo old', 'Done old']);
});
```

---

### **Integration Tests**

```csharp
[Test]
public async Task ToggleTodo_ReturnsOnlyChangedItem()
{
    // Arrange
    var todo = await CreateTodo("Test todo");

    // Act
    var response = await _client.PatchAsync($"/api/todos/{todo.Id}/toggle");
    var html = await response.Content.ReadAsStringAsync();

    // Assert
    Assert.Single(ParseHtmlElements(html)); // Only 1 <li> returned
    Assert.Contains($"id=\"todo-{todo.Id}\"", html);
    Assert.Contains("data-completed=\"true\"", html);
}
```

---

### **End-to-End Tests**

```javascript
test('toggling todo re-sorts list', async () => {
  // Create 3 todos: 2 incomplete, 1 complete
  await createTodo('First');  // incomplete
  await createTodo('Second'); // incomplete
  await createTodo('Third');  // incomplete, then toggle to complete

  await toggleTodo('Third');

  // Wait for HTMX swap + client sort
  await page.waitForTimeout(100);

  const order = await page.$$eval('.todo-item', items =>
    items.map(el => el.textContent.trim())
  );

  expect(order).toEqual(['First', 'Second', 'Third']); // Complete goes last
});
```

---

## Common Pitfalls

### **1. Data attribute types don't match sort logic**

**Problem:**
```html
<li data-completed="True"> <!-- C# boolean renders as "True" -->
```

```javascript
// Comparison fails because "True" !== "true"
comparison = (valueA === 'true') - (valueB === 'true');
```

**Solution:** Normalize to lowercase in server rendering
```csharp
data-completed="{todo.IsCompleted.ToString().ToLower()}"
```

---

### **2. Missing data attributes after mutations**

**Problem:**
```csharp
// Forgot to include data-created in updated HTML
return $"<li id='todo-{id}' data-completed='true'>...</li>";
```

**Result:** Sort breaks because `data-created` is undefined after swap.

**Solution:** Use a rendering helper that always includes all attributes
```csharp
private string RenderTodoItem(Todo todo)
{
    // Centralized function ensures consistency
    return TodoItemTemplate.Render(todo);
}
```

---

### **3. Sort instruction out of sync with rendered order**

**Problem:**
```html
<!-- Server says "sort by created-desc" -->
<ul data-sort="created-desc">
  <!-- But server rendered in completed-asc order -->
  <li data-completed="false">Todo</li>
  <li data-completed="true">Done</li>
</ul>
```

**Result:** Initial page load shows one order, after first mutation shows different order (jarring).

**Solution:** Client should sort on page load too:
```javascript
// On DOMContentLoaded, sort all lists to match data-sort instruction
document.querySelectorAll('[data-sort]').forEach(list => {
  sortList(list, list.dataset.sort);
});
```

Or better: Server renders in correct order initially, client maintains it after mutations.

---

### **4. Forgetting to handle ties**

**Problem:**
```javascript
// Only sorts by one field, ignores secondary sort
items.sort((a, b) => {
  return a.dataset.completed.localeCompare(b.dataset.completed);
  // Missing: What if both are completed? Need secondary sort by created date.
});
```

**Result:** Items with same completion status appear in random order.

**Solution:** Support multi-criteria sorting:
```javascript
items.sort((a, b) => {
  for (const rule of rules) { // Loop through all sort rules
    const comparison = compareByRule(a, b, rule);
    if (comparison !== 0) return comparison; // Return first non-tie
  }
  return 0; // All fields tied
});
```

---

## Evolution Path

### **Phase 1: Start with full refetch**
```csharp
// Simple: Return entire list after every mutation
return Results.Content(RenderTodoList(todos), "text/html");
```

**When this is enough:**
- Lists with < 20 items
- Low mutation frequency
- Development/prototyping phase

---

### **Phase 2: Add surgical updates, server-side sort**
```csharp
// Return only changed item, server does ORDER BY on every request
var todos = await _repository.GetByUserId(userId, sortBy: "completed,created-desc");
return Results.Content(RenderTodoList(todos), "text/html");
```

**When to move here:**
- Lists grow to 20-50 items
- Mutations become frequent
- Network payload becomes noticeable

---

### **Phase 3: Add client-side sorting**
```csharp
// Return only changed item, client sorts locally
return Results.Content(RenderTodoItem(todo), "text/html");
```

```javascript
// Client re-sorts after swap
document.body.addEventListener('htmx:afterSwap', sortAfterSwap);
```

**When to move here:**
- Lists grow to 50-500 items
- Database ORDER BY becomes bottleneck
- Want to reduce query complexity

---

### **Phase 4: Add pagination**
```csharp
// Return paginated view with sort
var page = await _repository.GetPage(userId, pageNum, pageSize, sortBy);
return Results.Content(RenderTodoPage(page), "text/html");
```

**When to move here:**
- Lists grow beyond 500 items
- Client-side sort takes > 10ms
- Need to reduce initial payload size

---

## Alternatives

### **Alternative 1: Server-side sort with every mutation**

**Pattern:** Always refetch full list, server does ORDER BY

**Pros:**
- Simpler: No client-side sort logic
- Server controls everything
- No data attributes needed

**Cons:**
- 2 database queries per mutation (UPDATE + SELECT with ORDER BY)
- Full list transferred every time
- More DOM churn

**When to choose:** Lists < 50 items, simplicity > performance

---

### **Alternative 2: SPA with client-side state**

**Pattern:** React/Vue/Svelte holds list in memory, client-side mutations

**Pros:**
- Instant feedback (no server round-trip for UI update)
- Client owns sorting/filtering
- Rich interactions (drag-drop, animations)

**Cons:**
- Larger bundle size (React ~140KB vs this pattern ~2KB)
- Client/server state sync complexity
- Offline/sync logic required

**When to choose:** Complex UIs, real-time collaboration, offline-first apps

---

### **Alternative 3: Optimistic updates + eventual consistency**

**Pattern:** Update UI immediately, sync to server in background

**Pros:**
- Feels instant (< 16ms)
- Better perceived performance
- Works well with idiomorph

**Cons:**
- Need rollback logic if server rejects
- More complex state management
- Potential for inconsistency

**When to choose:** High-latency networks, mobile apps, real-time feel required

---

### **Alternative 4: Virtual scrolling/pagination**

**Pattern:** Only render visible items, load more on scroll

**Pros:**
- Handles unlimited list sizes
- Constant DOM size (50-100 rendered items)
- Fast initial load

**Cons:**
- Complexity: Viewport calculations, scroll position tracking
- Accessibility challenges (screen readers)
- Breaks browser find (Ctrl+F)

**When to choose:** Lists with 1000+ items, infinite scroll pattern

---

## Real-World Examples

### **Good use case: Todo app (this project)**
- 50-100 todos per user
- Frequent toggles (5-10 per minute)
- Sort by completion + creation date
- **Savings:** ~5ms per toggle, 95% less traffic

---

### **Good use case: Email inbox**
- 200-500 emails per folder
- Mark read/unread, star, archive
- Sort by date, sender, read status
- **Savings:** ~10ms per action, 90% less traffic

---

### **Good use case: Project task board**
- 100-300 tasks
- Update status, assignee, priority
- Sort by status + due date + priority
- **Savings:** ~8ms per update, 95% less traffic

---

### **Poor use case: Social media feed**
- Infinite scroll (1000+ posts)
- Real-time updates from other users
- Complex sorting (engagement, recency, personalization)
- **Better approach:** Pagination + WebSocket updates

---

### **Poor use case: Spreadsheet**
- 1000+ rows, 20+ columns
- Frequent edits across multiple cells
- User-controlled sort (click column headers)
- **Better approach:** SPA with virtual scrolling

---

### **Poor use case: Collaborative document**
- Real-time edits from multiple users
- Operational transforms needed
- Offline editing support
- **Better approach:** CRDT-based architecture (Yjs, Automerge)

---

## Budget Guidance

**Development time:**
- Initial implementation: 4-6 hours
  - Server rendering helpers: 1-2 hours
  - Client sort logic: 2-3 hours
  - Testing: 1-2 hours
- Per new sorted list: 30-60 minutes
  - Add data attributes to rendering
  - Configure HTMX targets
  - Test sort behavior

**Maintenance cost:**
- Low: Once working, rarely changes
- Risk: Forgetting data attributes when adding new mutation endpoints
- Mitigation: Centralized rendering helpers, integration tests

**Performance budget:**
- Client JS: ~1-2KB gzipped (sort logic)
- Initial payload increase: ~10-20% (data attributes)
- Per-mutation payload decrease: ~95% (1 item vs full list)
- Net: Better for users after first mutation

---

## Decision Template

Use this template when deciding whether to implement this pattern:

```
## Should we use surgical updates + client-side sorting?

**List characteristics:**
- List size: ___ items (target: 20-500)
- Mutation frequency: ___ per user session (target: 5+)
- Network conditions: ___ (mobile/international/bandwidth-sensitive?)

**Complexity tolerance:**
- Team comfort with client-side JS: ___/10
- Willingness to maintain data attribute sync: ___/10
- Need for server authority: ___/10

**Performance requirements:**
- Current mutation time: ___ ms
- Target mutation time: ___ ms
- Current payload size: ___ KB
- Target payload size: ___ KB

**Decision:**
- [ ] Use this pattern (meets criteria above)
- [ ] Stick with full refetch (simpler, fast enough)
- [ ] Use pagination instead (list too large)
- [ ] Use SPA instead (need richer interactions)

**Justification:**
___
```

---

## References

**Related patterns:**
- CQRS (Command Query Responsibility Segregation): Similar idea of separating mutations from queries
- Optimistic UI: Related but different (updates before server confirmation)
- Virtual scrolling: Alternative for very large lists

**Similar approaches in other frameworks:**
- Phoenix LiveView: Server sends diffs, client patches DOM
- Laravel Livewire: Server re-renders components, client morphs DOM
- Hotwire Turbo: Server sends HTML fragments, client swaps

**This pattern compared:**
- More surgical than Hotwire (1 item vs full frame)
- Less complex than LiveView (no diff algorithm needed)
- More server-authoritative than Livewire (server controls sort rules)

---

**Last Updated:** 2025-11-23
**Status:** Pattern consideration - document before implementation
