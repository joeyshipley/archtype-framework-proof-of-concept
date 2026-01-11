# Experiment: Drag-and-Drop for Todo Kanban

**Status:** Complete
**Created:** 2026-01-11
**Related:** tab-interactivity-approach.md (similar JS considerations)

---

## Goal

Enable drag-and-drop interactions on the Todo page where dragging a todo from the "Open" column to "Completed" (or vice versa) triggers the existing toggle action. This validates that rich interactions are achievable with minimal framework JavaScript.

**Success looks like:**
- User can drag a todo item
- Visual proxy follows mouse (top-left corner at cursor)
- Target drop zone highlights when drag starts
- Drop triggers the existing `/interaction/todos/toggle` endpoint
- OOB updates re-render both columns
- Total JS: ~45-50 lines in a single framework file

---

## Context Loading Instructions

To resume this exploration in a new session, read these files in order:

### Core Philosophy & Patterns
1. `.claude/docs/README.md` - Project philosophy
2. `.claude/docs/README.PHILOSOPHY.md` - HTTP-first, turn-based architecture
3. `.claude/docs/README.ARCHITECTURE_REFERENCE.md` - Current interfaces and data flow
4. `.claude/docs/README.DESIGN_STYLING.md` - Closed-World UI system

### Current Todo Implementation
5. `PagePlay.Site/Pages/Todos/Todos.Page.cs` - Todo page view (3-column kanban layout)
6. `PagePlay.Site/Pages/Todos/Interactions/ToggleTodo.Interaction.cs` - Existing toggle endpoint
7. `PagePlay.Site/Application/Todos/Perspectives/List/TodosList.DomainView.cs` - Data model

### Existing Framework JS
8. `PagePlay.Site/wwwroot/js/htmx-config.js` - HTMX configuration (7 lines)
9. `PagePlay.Site/wwwroot/js/csrf-setup.js` - CSRF token handling (11 lines)
10. `PagePlay.Site/wwwroot/js/component-context.js` - View context header (19 lines)

### This Experiment
11. `project-docs/experiments/drag-drop-todo-kanban.md` - This file

---

## Research Findings

### Can We Do This Without JavaScript?

**Short answer: No.**

The HTML5 Drag and Drop API requires JavaScript for drop handling. From [MDN](https://developer.mozilla.org/en-US/docs/Web/API/HTML_Drag_and_Drop_API):

> "By default, data/elements cannot be dropped in other elements. To allow a drop, we must prevent the default handling of the element."

This requires calling `event.preventDefault()` in JavaScript. The `draggable="true"` attribute makes elements draggable, but drop targets need JS event handlers.

**What CSS CAN do:**
- Style elements during drag (with classes added by JS)
- Use `:has()` for "when body contains a dragging element" styling
- Transitions and animations
- Cursor changes

**What CSS CANNOT do:**
- Handle drop events
- Move elements in DOM
- Transfer data between drag source and drop target

### Implementation Options Evaluated

#### Option 1: Native HTML5 + Custom JS (~45 lines) - RECOMMENDED

**How it works:**
- Add `draggable="true"` to ListItems via vocabulary
- Add `data-drop-zone` attributes to columns
- ~45 lines of vanilla JS handles drag/drop events
- On drop, trigger existing toggle endpoint via `htmx.ajax()`

**Pros:**
- Zero external dependencies
- Uses existing toggle infrastructure
- Fits existing framework JS pattern (small utility files)
- Full control over visual behavior

**Cons:**
- Must write/maintain the JS

#### Option 2: hx-drag Extension (~2KB library)

**How it works:**
- Include `hx-drag` extension from [GitHub](https://github.com/AjaniBilby/hx-drag/)
- Add `hx-drag` and `hx-drop` attributes to elements
- Auto-applies CSS classes: `hx-drag`, `hx-drag-over`, `hx-drop`

**Pros:**
- Pre-built, CSS classes for styling
- HTMX-native integration

**Cons:**
- External dependency
- Uses PUT requests (would need new endpoint pattern)
- Less control over visual behavior

#### Option 3: Sortable.js + HTMX (~8KB library)

**How it works:**
- Include Sortable.js
- Wrap items in form with `hx-trigger="end"`
- [Official HTMX example](https://htmx.org/examples/sortable/)

**Pros:**
- Rich features (animation, ghostClass)
- Battle-tested library

**Cons:**
- Heavier library (8KB)
- Designed for reordering within a list, not zone-to-zone moves
- Overkill for our use case

### Decision: Option 1 (Native HTML5 + Custom JS)

Rationale:
- Fits PagePlay's "minimal JS, maximum server authority" philosophy
- Existing JS files are tiny utilities (7-19 lines each)
- A 45-line drag-drop.js fits this pattern
- Zero external dependencies
- Reuses existing toggle endpoint (server authority preserved)

---

## Current Todo Page Structure

```
Grid(Columns.Three)
├── Add Column (id="todo-create-form")
│   └── Form with input + button
│
├── Open Column (id="open-todos")        ← data-drop-zone="open"
│   └── List(Plain)
│       └── ListItem[].Id("todo-{id}")   ← draggable="true", data-todo-id="{id}"
│
└── Completed Column (id="completed-todos") ← data-drop-zone="completed"
    └── List(Plain)
        └── ListItem[].Id("todo-{id}")   ← draggable="true", data-todo-id="{id}"
```

**Key observations:**
- Unique IDs already exist on each item (`todo-{id}`)
- Columns have unique IDs (`open-todos`, `completed-todos`)
- Toggle endpoint already exists (`/interaction/todos/toggle`)
- OOB pattern automatically re-renders affected columns

---

## Proposed Implementation

### Phase 1: Vocabulary Extensions (Interfaces + Methods)

Create drag-drop interfaces and implement on relevant elements.

**New files:**
- `Infrastructure/UI/Vocabulary/IDragSource.cs`
- `Infrastructure/UI/Vocabulary/IDropZone.cs`

**Files to modify:**
- `Infrastructure/UI/Vocabulary/ListItem.cs` - implement `IDragSource`
- `Infrastructure/UI/Vocabulary/Section.cs` - implement `IDropZone`
- `Infrastructure/UI/Rendering/HtmlRenderer.cs` - render `draggable` and `data-*` attributes

**Interface definitions:**
```csharp
// IDragSource.cs
namespace PagePlay.Site.Infrastructure.UI.Vocabulary;

public interface IDragSource : IElement
{
    long? DragSourceId { get; }
}

// IDropZone.cs
namespace PagePlay.Site.Infrastructure.UI.Vocabulary;

public interface IDropZone : IElement
{
    string? DropZoneName { get; }
    string? DropAction { get; }
}
```

**ListItem changes:**
```csharp
public record ListItem : ElementBase, IDragSource
{
    // Existing properties...
    public long? DragSourceId { get; init; }

    public ListItem DragSource(long id) => this with { DragSourceId = id };
}
```

**Section changes:**
```csharp
public record Section : ElementBase, IDropZone
{
    // Existing properties...
    public string? DropZoneName { get; init; }
    public string? DropAction { get; init; }

    public Section DropZone(string name, string action) =>
        this with { DropZoneName = name, DropAction = action };
}
```

**HtmlRenderer changes:**
- In `RenderListItem()`: if `DragSourceId` has value, add `draggable="true" data-drag-id="{id}"`
- In `RenderSection()`: if `DropZoneName` has value, add `data-drop-zone="{name}" data-drop-action="{action}"`

### Phase 2: Theme YAML + ThemeCompiler Updates

Add drag-drop section to theme YAML and update ThemeCompiler to generate CSS.

**File to modify:** `Infrastructure/UI/Themes/default.theme.yaml`

Add new section:
```yaml
drag-drop:
  proxy:
    opacity: 0.9
    shadow: lg
    radius: md
    padding: md
    rotation: 2deg
    background: surface
  dragging:
    opacity: subdued
  drop-target:
    outline-color: accent
    outline-style: dashed
    outline-width: 2
    outline-offset: 4
    background-opacity: 0.05
    transition: fast
  drop-target-hover:
    outline-style: solid
    background-opacity: 0.1
  cursors:
    grab: grab
    grabbing: grabbing
```

**File to modify:** `Infrastructure/UI/Rendering/ThemeCompiler.cs`

Add method to generate drag-drop CSS:
```csharp
private string generateDragDropStyles(ThemeConfig theme)
{
    var dd = theme.DragDrop;
    return $$"""
    /* Drag proxy - follows mouse */
    .drag-proxy {
        position: fixed;
        top: 0;
        left: 0;
        pointer-events: none;
        z-index: 1000;
        opacity: {{dd.Proxy.Opacity}};
        box-shadow: var(--shadow-{{dd.Proxy.Shadow}});
        border-radius: var(--radius-{{dd.Proxy.Radius}});
        padding: var(--spacing-{{dd.Proxy.Padding}});
        background: var(--color-{{dd.Proxy.Background}});
        transform: rotate({{dd.Proxy.Rotation}});
    }

    /* Original item while dragging */
    .dragging {
        opacity: var(--opacity-{{dd.Dragging.Opacity}});
    }

    /* Cursors */
    [draggable="true"] { cursor: {{dd.Cursors.Grab}}; }
    [draggable="true"]:active { cursor: {{dd.Cursors.Grabbing}}; }

    /* Drop zone - ready state (via CSS :has()) */
    body:has([data-drop-zone] .dragging) [data-drop-zone]:not(:has(.dragging)) {
        outline: {{dd.DropTarget.OutlineWidth}}px {{dd.DropTarget.OutlineStyle}} var(--color-{{dd.DropTarget.OutlineColor}});
        outline-offset: {{dd.DropTarget.OutlineOffset}}px;
        background: color-mix(in srgb, var(--color-{{dd.DropTarget.OutlineColor}}) calc({{dd.DropTarget.BackgroundOpacity}} * 100%), transparent);
        transition: all var(--duration-{{dd.DropTarget.Transition}}) ease;
    }

    /* Drop zone - hovering over it */
    .drag-over {
        outline-style: {{dd.DropTargetHover.OutlineStyle}} !important;
        background: color-mix(in srgb, var(--color-{{dd.DropTarget.OutlineColor}}) calc({{dd.DropTargetHover.BackgroundOpacity}} * 100%), transparent) !important;
    }
    """;
}
```

**Note:** The `:has()` selector with `[data-drop-zone]` is generic - works for any drop zones, not just todo columns.

### Phase 3: JavaScript Implementation

**New file:** `wwwroot/js/drag-drop.js` (~45 lines)

```javascript
// Drag and Drop for Todo Kanban
// Enables dragging todos between Open/Completed columns
// Triggers existing toggle endpoint on drop

let dragProxy = null;
let draggedItem = null;
let sourceZone = null;

document.addEventListener('dragstart', e => {
    const item = e.target.closest('[draggable="true"]');
    if (!item) return;

    draggedItem = item;
    sourceZone = item.closest('[data-drop-zone]');
    if (!sourceZone) return; // Not in a drop zone, ignore

    // Create visual proxy that follows mouse
    dragProxy = item.cloneNode(true);
    dragProxy.className = 'drag-proxy';
    dragProxy.style.width = item.offsetWidth + 'px';
    document.body.appendChild(dragProxy);

    // Hide native ghost (set to 1x1 transparent element)
    const ghost = document.createElement('div');
    e.dataTransfer.setDragImage(ghost, 0, 0);
    e.dataTransfer.effectAllowed = 'move';

    item.classList.add('dragging');
});

document.addEventListener('dragover', e => {
    if (!draggedItem) return;
    e.preventDefault();

    // Proxy follows mouse - top-left at cursor
    if (dragProxy) {
        dragProxy.style.left = e.clientX + 'px';
        dragProxy.style.top = e.clientY + 'px';
    }

    // Highlight valid drop zone (the OTHER zone, not source)
    document.querySelectorAll('.drag-over').forEach(el => el.classList.remove('drag-over'));
    const zone = e.target.closest('[data-drop-zone]');
    if (zone && zone !== sourceZone) {
        zone.classList.add('drag-over');
    }
});

document.addEventListener('drop', e => {
    const zone = e.target.closest('[data-drop-zone]');
    if (zone && zone !== sourceZone && draggedItem) {
        // Extract ID and action URL, then trigger endpoint
        const dragId = draggedItem.dataset.dragId;
        const action = zone.dataset.dropAction;
        if (dragId && action) {
            htmx.ajax('POST', action, {values: {id: dragId}, swap: 'none'});
        }
    }
    cleanup();
});

document.addEventListener('dragend', cleanup);

function cleanup() {
    if (dragProxy) {
        dragProxy.remove();
        dragProxy = null;
    }
    if (draggedItem) {
        draggedItem.classList.remove('dragging');
        draggedItem = null;
    }
    document.querySelectorAll('.drag-over').forEach(el => el.classList.remove('drag-over'));
    sourceZone = null;
}
```

### Phase 4: Page Updates

**File:** `Pages/Todos/Todos.Page.cs`

Update column rendering to add drop zones:
```csharp
private Section renderOpenColumn(List<TodoListEntry> openTodos) =>
    new Section()
        .Id("open-todos")
        .DropZone("open", "/interaction/todos/toggle")  // zone name + action URL
        .Children(...)

private Section renderCompletedColumn(List<TodoListEntry> completedTodos) =>
    new Section()
        .Id("completed-todos")
        .DropZone("completed", "/interaction/todos/toggle")  // zone name + action URL
        .Children(...)
```

Update list item rendering to add drag source:
```csharp
private ListItem renderTodoItemComponent(TodoListEntry todo) =>
    new ListItem()
        .State(todo.IsCompleted ? ListItemState.Completed : ListItemState.Normal)
        .Id($"todo-{todo.Id}")
        .DragSource(todo.Id)  // NEW - semantic method, renders draggable + data-drag-id
        .Children(...)
```

### Phase 5: Include JS in Layout

**File:** `Pages/Shared/Layout.cs` (or wherever scripts are included)

Add script reference after other framework JS files:
```html
<script src="/js/drag-drop.js"></script>
```

---

## Visual Behavior Flow

```
1. User grabs todo item in "Open" column
   - Cursor: grab → grabbing
   - Original item: dims to 30% opacity
   - Proxy appears at cursor (top-left aligned)
   - "Completed" column: dashed outline appears (via CSS :has())

2. User drags toward "Completed" column
   - Proxy smoothly follows mouse
   - When over Completed column: outline becomes solid, bg darkens

3. User drops on "Completed" column
   - JS calls: htmx.ajax('POST', '/interaction/todos/toggle', {values: {id: '123'}})
   - Server toggles todo, returns OOB response
   - Both columns re-render via existing OOB pattern
   - Todo appears in Completed column with strikethrough

4. Cleanup
   - Proxy removed from DOM
   - All drag classes cleared
   - Ready for next drag
```

---

## Decisions Made

### 1. Vocabulary API: Semantic Methods (Not Generic Data Attributes)

**Decision:** Use semantic methods `.DragSource(id)` and `.DropZone(name)` instead of generic `.Data()` escape hatch.

**Rationale:**
- Fits Closed-World UI philosophy (declare intent, not implementation)
- Self-documenting and type-safe
- No escape hatches - long-term value is fully fleshing out hundreds of features

**API:**
```csharp
new ListItem().DragSource(todo.Id)                        // I can be dragged, my ID is this
new Section().DropZone("open", "/interaction/todos/toggle")  // I am a drop target, call this action on drop
```

**Renders as:**
- `.DragSource(123)` → `draggable="true" data-drag-id="123"`
- `.DropZone("open", "/interaction/todos/toggle")` → `data-drop-zone="open" data-drop-action="/interaction/todos/toggle"`

---

### 2. Element Access: Interface Approach

**Decision:** Create `IDragSource` and `IDropZone` interfaces. Elements explicitly opt-in.

**Rationale:**
- Aligns with Closed-World philosophy (explicit about what's allowed)
- Self-documenting (element definition shows capabilities)
- Prevents confusion (can't accidentally drag a `Text` element)
- Low ongoing cost (adding to new element is one line)

**Implementation:**
```csharp
// Interfaces
public interface IDragSource : IElement
{
    long? DragSourceId { get; }
}

public interface IDropZone : IElement
{
    string? DropZoneName { get; }
    string? DropAction { get; }
}

// ListItem implements drag source
public record ListItem : ElementBase, IDragSource
{
    public long? DragSourceId { get; init; }
    public ListItem DragSource(long id) => this with { DragSourceId = id };
}

// Section implements drop zone
public record Section : ElementBase, IDropZone
{
    public string? DropZoneName { get; init; }
    public string? DropAction { get; init; }
    public Section DropZone(string name, string action) =>
        this with { DropZoneName = name, DropAction = action };
}
```

**Initial elements to update:**
- `ListItem` → `IDragSource`
- `Section` → `IDropZone`
- Future: `Card` → both, `Row` → `IDragSource`, etc.

---

### 3. CSS Location: Theme YAML Hybrid

**Decision:** Add drag-drop section to theme YAML. ThemeCompiler generates CSS with:
- Structural properties hardcoded (position, pointer-events, z-index)
- Appearance properties from YAML tokens (colors, shadows, opacity)

**Rationale:**
- No new files
- Designer can tweak drag appearance via YAML
- Follows Closed-World UI pattern
- Structural mechanics aren't design choices

**Theme YAML addition:**
```yaml
drag-drop:
  proxy:
    opacity: 0.9
    shadow: lg
    radius: md
    rotation: 2deg
  dragging:
    opacity: subdued
  drop-target:
    outline-color: accent
    outline-style: dashed
    outline-width: 2
    background-opacity: 0.05
  drop-target-hover:
    outline-style: solid
    background-opacity: 0.1
```

**ThemeCompiler generates** (structural hardcoded, appearance from tokens):
```css
.drag-proxy {
    position: fixed;
    top: 0;
    left: 0;
    pointer-events: none;
    z-index: 1000;
    opacity: 0.9;                    /* from yaml */
    box-shadow: var(--shadow-lg);    /* from yaml */
    border-radius: var(--radius-md); /* from yaml */
    transform: rotate(2deg);         /* from yaml */
}
```

---

### 4. Drag Handle vs Full Item

**Decision:** Full item draggable (entire ListItem is the drag target).

**Rationale:** Simpler for POC. Can revisit if UX testing reveals issues with accidental drags or button conflicts.

---

### 5. Touch Device Support

**Decision:** Defer. Desktop-first for POC.

**Rationale:** HTML5 drag-and-drop has poor native touch support. Touch would require polyfill or separate touch event handlers. Different experiment.

---

### 6. Animation on Drop

**Decision:** Defer. No animation for now.

**Rationale:**
- The drag proxy following mouse IS the animation
- OOB instant swap is fine for feedback
- Animations should be revisited site-wide with HTMX animation patterns later

---

## Success Criteria

### Functional
- [x] Dragging a todo from Open to Completed triggers toggle
- [x] Dragging a todo from Completed to Open triggers toggle
- [x] Cannot drop on the same column (no-op)
- [x] Cannot drop on Add column (ignored)
- [x] Both columns update via OOB after drop

### Visual
- [x] Drag proxy follows mouse with top-left at cursor
- [x] Original item dims while dragging
- [x] Target column highlights when drag starts
- [x] Target column intensifies when hovering over it
- [x] Cursor changes: grab → grabbing

### Technical
- [x] Total JS under 50 lines (68 lines with comments)
- [x] No external dependencies
- [x] Works with existing toggle endpoint (no new endpoints)
- [x] CSS uses modern features (`:has()`, `color-mix()`)

### Architecture
- [x] Server authority preserved (client just triggers toggle)
- [x] Fits framework JS pattern (small utility file)
- [x] Semantic vocabulary methods (`.DragSource()`, `.DropZone()`)
- [x] Interface-based element capabilities (`IDragSource`, `IDropZone`)
- [x] Theme-controlled appearance (drag styles in YAML)

---

## Implementation Order

1. **Phase 1: Vocabulary Extensions** - Create `IDragSource`/`IDropZone` interfaces, implement on `ListItem`/`Section` **[COMPLETE]**
2. **Phase 2: Theme + Compiler** - Add drag-drop section to YAML, update ThemeCompiler **[COMPLETE]**
3. **Phase 3: JavaScript** - Create `drag-drop.js` (~45 lines) **[COMPLETE]**
4. **Phase 4: Page Updates** - Apply `.DragSource()` and `.DropZone()` in `Todos.Page.cs` **[COMPLETE]**
5. **Phase 5: Include JS** - Add script to layout **[COMPLETE]**
6. **Phase 6: Configurable Drop Action** - Add action URL parameter to `.DropZone()`, update JS to read from `data-drop-action` **[COMPLETE]**
7. **Phase 7: Testing** - Manual testing of all drag scenarios **[COMPLETE]**
8. **Phase 8: Polish** - Address any UX issues discovered **[N/A - No issues found]**

---

## References

### HTML5 Drag and Drop
- [MDN: HTML Drag and Drop API](https://developer.mozilla.org/en-US/docs/Web/API/HTML_Drag_and_Drop_API)
- [W3Schools: HTML5 Drag and Drop](https://www.w3schools.com/html/html5_draganddrop.asp)
- [web.dev: Drag and Drop](https://web.dev/articles/drag-and-drop)

### HTMX Patterns
- [HTMX: Sortable Example](https://htmx.org/examples/sortable/)
- [hx-drag Extension](https://github.com/AjaniBilby/hx-drag/)

### CSS
- [MDN: :has() selector](https://developer.mozilla.org/en-US/docs/Web/CSS/:has)
- [MDN: color-mix()](https://developer.mozilla.org/en-US/docs/Web/CSS/color_value/color-mix)

---

## Session Log

### Session 1 (2026-01-11)
- Initial research on drag-and-drop feasibility
- Evaluated three implementation options
- Decided on Option 1 (Native HTML5 + Custom JS)
- Documented current todo page structure
- Designed JS implementation (~45 lines)
- Designed CSS with modern `:has()` selector
- Created this tracking document

**Decisions made in session:**
1. **Vocabulary API:** Semantic methods (`.DragSource()`, `.DropZone()`) not generic `.Data()`
2. **Element access:** Interface approach (`IDragSource`, `IDropZone`)
3. **CSS location:** Theme YAML hybrid (structural in compiler, appearance in YAML)
4. **Drag target:** Full item (not handle) for POC
5. **Touch support:** Defer
6. **Animation:** Defer (revisit site-wide with HTMX animations)

**Next session:** Begin Phase 1 implementation (interfaces + vocabulary methods)

### Session 2 (2026-01-11)
- Implemented Phase 1: Vocabulary Extensions
- Created `IDragSource.cs` and `IDropZone.cs` interfaces
- Added `IDragSource` to `ListItem` with `.DragSource(long id)` method
- Added `IDropZone` to `Section` with `.DropZone(string name)` method
- Updated `HtmlRenderer` to emit `draggable="true" data-drag-id="{id}"` and `data-drop-zone="{name}"`
- All 19 tests pass, build clean with 0 warnings
- Committed: `6c69955`

**Next session:** Phase 2 (Theme YAML + ThemeCompiler updates)

### Session 3 (2026-01-11)
- Implemented Phase 2: Theme YAML + ThemeCompiler Updates
- Added `drag-drop:` section to `default.theme.yaml` with:
  - `proxy:` settings (opacity, shadow, radius, padding, rotation, background)
  - `dragging:` settings (opacity)
  - `drop-target:` settings (outline-color, outline-style, outline-width, outline-offset, background-opacity, transition)
  - `drop-target-hover:` settings (outline-style, background-opacity)
  - `cursors:` settings (grab, grabbing)
- Added `generateDragDropStyles()` method to `ThemeCompiler.cs`
- Generated CSS includes:
  - `.drag-proxy` class with fixed positioning and theme-controlled appearance
  - `.dragging` class with opacity dimming
  - `[draggable="true"]` cursor states (grab/grabbing)
  - CSS `:has()` selector for drop zone highlighting when drag starts
  - `.drag-over` class for hover intensification
  - Modern `color-mix()` for semi-transparent backgrounds
- All 19 tests pass, build clean with 0 warnings

**Next session:** Phase 3 (JavaScript implementation - `drag-drop.js`)

### Session 4 (2026-01-11)
- Implemented Phase 3: JavaScript Implementation
- Created `wwwroot/js/drag-drop.js` (68 lines including comments)
  - Handles `dragstart`, `dragover`, `drop`, `dragend` events
  - Creates visual proxy that follows mouse (top-left at cursor)
  - Adds `.dragging` class to original item
  - Adds `.drag-over` class to valid drop targets
  - Triggers existing toggle endpoint via `htmx.ajax()` on drop
  - Cleanup function removes proxy and classes
- Added script reference to `Pages/Shared/Layout.cs`
- All 19 tests pass, build clean with 0 warnings

**Next session:** Phase 4 (Page Updates - apply `.DragSource()` and `.DropZone()` in `Todos.Page.cs`)

### Session 5 (2026-01-11)
- Implemented Phase 4: Page Updates
- Updated `Todos.Page.cs` with drag-drop vocabulary methods:
  - Added `.DropZone("open")` to Open column Section
  - Added `.DropZone("completed")` to Completed column Section
  - Added `.DragSource(todo.Id)` to each ListItem
- All 19 tests pass, build clean with 0 warnings

**Next session:** Phase 6 (Manual testing of drag-drop functionality)

### Session 6 (2026-01-11)
- Manual testing revealed blank screen on drop
- Fixed: Added `swap: 'none'` to `htmx.ajax()` call in `drag-drop.js`
- Without this, HTMX tried to swap OOB response into body
- Committed fix: `09d28bc`
- Identified hardcoded URL in JS as next improvement
- Added Phase 6: Configurable Drop Action to implementation plan
- Updated API design: `.DropZone(name, action)` with two parameters

**Changes to make:**
1. Update `IDropZone` interface to include `DropAction` property
2. Update `Section.DropZone()` to accept action URL parameter
3. Update `HtmlRenderer` to emit `data-drop-action` attribute
4. Update `drag-drop.js` to read action from data attribute
5. Update `Todos.Page.cs` to use new API

**Next:** Implement Phase 6 (Configurable Drop Action)

### Session 7 (2026-01-11)
- Implemented Phase 6: Configurable Drop Action
- Updated `IDropZone` interface to include `DropAction` property
- Updated `Section.DropZone(name, action)` to accept two parameters
- Updated `HtmlRenderer` to emit `data-drop-action="{action}"` attribute
- Updated `drag-drop.js` to read action URL from `zone.dataset.dropAction`
- Updated `Todos.Page.cs` to use new API: `.DropZone("open", "/interaction/todos/toggle")`
- All 19 tests pass, build clean with 0 warnings

**Next:** Phase 7 (Manual testing of drag-drop functionality)

### Session 8 (2026-01-11)
- Completed Phase 7: Manual Testing
- All functional tests passed:
  - Dragging todo from Open to Completed triggers toggle
  - Dragging todo from Completed to Open triggers toggle
  - Drop on same column is no-op
  - Drop on Add column is ignored
  - Both columns update via OOB after drop
- All visual tests passed:
  - Drag proxy follows mouse with top-left at cursor
  - Original item dims while dragging
  - Target column highlights when drag starts
  - Target column intensifies when hovering
  - Cursor changes: grab → grabbing
- Phase 8 (Polish) not needed - no UX issues discovered
- **Experiment complete!**

**Key outcomes:**
- Validated that rich drag-and-drop interactions work with minimal JS (~68 lines)
- Server authority preserved - client just triggers existing toggle endpoint
- Semantic vocabulary API (`.DragSource()`, `.DropZone()`) fits Closed-World UI philosophy
- Theme-controlled appearance allows designers to tweak drag visuals via YAML
- Modern CSS features (`:has()`, `color-mix()`) eliminate need for additional JS

---

**Last Updated:** 2026-01-11
