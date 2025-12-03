# Experiment: Closed-World UI Vocabulary Expansion

**Status:** Phase 0 - Planning
**Started:** 2025-12-02
**Goal:** Expand the Closed-World UI vocabulary to support converting Todos and Login pages from raw HTML to semantic component types

---

## Hypothesis

The current Closed-World UI vocabulary (Page, Card, Stack, Row, Grid, Button, Text) is insufficient for real-world forms and list-based UIs. By adding 12 critical vocabulary elements (Input, Label, Field, Form, Checkbox, Alert, List, ListItem, etc.), we can achieve 100% semantic type coverage for the Todos and Login pages with zero HTML escape hatches.

**Success means:**
- Login page: 100% convertible to semantic types
- Todos page: 100% convertible to semantic types
- Zero raw HTML strings in page rendering code
- Zero CSS class names in page code (theme controls all)
- Type system prevents invalid structures (compilation enforces correctness)

---

## Current State Assessment

### What We Have (Existing Vocabulary)

**File:** `PagePlay.Site/Infrastructure/UI/Vocabulary/`

1. **Page Structure** (`PageStructure.cs`):
   - `Page` - Root container
   - `Section` - Major page division
   - `PageTitle` - H1 heading
   - `SectionTitle` - H2 heading

2. **Containers** (`Card.cs`, `Slots.cs`):
   - `Card` with `Header`, `Body`, `Footer` slots
   - Slot enforcement via `required` keyword

3. **Layout** (`Layout.cs`):
   - `Stack` - Vertical arrangement with `For` enum (Actions, Fields, Content, Items, Sections, Inline, Cards)
   - `Row` - Horizontal arrangement with `For` enum
   - `Grid` - Two-dimensional layout with `Columns` enum (One, Two, Three, Four, Auto)

4. **Interactive** (`Button.cs`):
   - `Button` with `Importance` enum (Primary, Secondary, Tertiary, Ghost)
   - HTMX support: `Action`, `Target`, `Swap`, `ModelId`
   - States: `Disabled`, `Loading`

5. **Text** (`Text.cs`):
   - `Text` - Simple text content (renders as `<p>`)

**Total:** 11 vocabulary elements across 6 files

### What We Need (Gap Analysis)

Analyzed pages:
- `Pages/Login/Login.Page.htmx.cs` - Form with email/password inputs, error/success alerts
- `Pages/Todos/Todos.Page.htmx.cs` - Form with text input, todo list, checkboxes, delete buttons, notifications

**Identified Gaps:** 24 missing elements across 6 categories

---

## Priority Matrix

### Tier 1: Must-Have for Conversion (12 elements)

| Element | Category | Why Critical | Used In |
|---------|----------|--------------|---------|
| `Input` | Form | Core input element (text, email, password, hidden) | Login (2x), Todos (1x) |
| `Label` | Form | Accessibility requirement for inputs | Login (2x) |
| `Field` | Form | Groups Label + Input semantically | Login (2x) |
| `Form` | Form | Container for form submission | Login (1x), Todos (3x) |
| `Button.Type` | Form | Need Submit type for form buttons | Login (1x), Todos (2x) |
| `Checkbox` | Interactive | Todo toggle interaction | Todos (per item) |
| `Alert` | Feedback | Error/success notifications | Login (2x), Todos (2x) |
| `EmptyState` | Feedback | Empty list message | Todos (1x) |
| `List` | List | Container for todo items | Todos (1x) |
| `ListItem` | List | Individual todo rendering | Todos (per item) |
| `ListItem.State` | List | Completed vs Normal styling | Todos (per item) |
| `Hidden Input` | Form | Used in toggle/delete forms | Todos (per item) |

### Tier 2: Nice-to-Have for Polish (8 elements)

| Element | Category | Why Useful | Priority |
|---------|----------|------------|----------|
| `Icon` | Content | Delete button (×), checkbox icons (☐/☑) | P1 |
| `Divider` | Content | Visual separator (`<hr>`) | P2 |
| `Text.Display` | Content | Inline text variant (`<span>`) | P1 |
| `Link` | Interactive | Navigation links | P2 |
| `LoadingIndicator` | Feedback | Standalone loading state | P3 |
| `NotificationContainer` | Layout | Notification area (can use Stack) | P3 |
| `InputGroup` | Form | Input + Button side-by-side (can use Row) | P3 |
| `Toggle` | Interactive | Switch variant (Checkbox sufficient) | P3 |

### Tier 3: Future Considerations (Not Planned)

- ActionMenu/Dropdown
- Tooltip
- Modal/Dialog
- Tabs
- Accordion
- Table

---

## Design Decisions

### Decision 1: No Client-Side Validation (Server Authority)

**Date:** 2025-12-02
**Decision:** Remove `Required`, `MaxLength`, `MinLength` from Input vocabulary
**Rationale:**

In a server-authoritative architecture with HTMX and FluentValidation:
1. **Server is the validation authority** - FluentValidation rules in workflow validators are the single source of truth
2. **No duplication** - Maintaining validation rules in two places (server + client) adds maintenance burden with minimal value
3. **Fast enough feedback** - HTMX round trip (~100ms) is acceptable for validation feedback
4. **Simpler vocabulary** - Fewer properties = clearer mental model
5. **Prevents drift** - Client validation can't get out of sync with server validation
6. **Can't derive from FluentValidation** - Rules are code in separate validator classes, not attributes on properties

**What we keep:**
- `Type` (InputType.Email, etc.) - Semantic intent + mobile keyboard optimization
- `Name` - Form field mapping (must match workflow request property, camelCase)
- `Placeholder` - User guidance
- `Value` - Initial value
- `Disabled`, `ReadOnly` - Semantic states

**What we remove:**
- `Required` - Duplicate of FluentValidation `.NotEmpty()` rule
- `MaxLength` - Duplicate of FluentValidation `.MaximumLength()` rule
- `MinLength` - Duplicate of FluentValidation `.MinimumLength()` rule

**Validation pattern:**
```csharp
// 1. FluentValidation (Server - Single Source of Truth)
public class LoginRequestValidator : AbstractValidator<LoginWorkflowRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}

// 2. Workflow validates and returns errors
var validationResult = await validator.ValidateAsync(request);
if (!validationResult.IsValid)
{
    return ApplicationResult<LoginWorkflowResponse>.Fail(validationResult);
    // -> validationResult.ToResponseErrors()
    // -> IEnumerable<ResponseErrorEntry> { Message, Property (camelCase) }
}

// 3. View (Client - No Duplication)
new Input
{
    Name = "email",  // Matches LoginWorkflowRequest.Email (camelCase)
    Type = InputType.Email,  // Semantic + mobile keyboard hint
    Placeholder = "Enter email"
    // Server validates, returns errors via HTMX
}

// 4. Field shows server-returned errors
// Pattern A: Field-level error (inline below input)
new Field
{
    Label = new Label("Email"),
    Input = new Input { Name = "email", Type = InputType.Email },
    ErrorText = errors.FirstOrDefault(e => e.Property == "email")?.Message,
    HasError = errors.Any(e => e.Property == "email")
}

// Pattern B: Form-level error (Alert at top)
new Form { Action = "/interaction/login/authenticate" }
{
    errors.Any()
        ? new Alert("Please fix the errors below", AlertTone.Critical)
        : null,
    new Stack(For.Fields, /* fields... */)
}
```

**Why not derive validation from FluentValidation?**
- Rules are in separate validator classes (not property attributes)
- Would require executing validator at render time (expensive, architecturally wrong)
- Validation logic intentionally separated from data models
- No practical way to inspect rules without running validation

**Future consideration:** Could explore compile-time code generation from validators, but starting simple with Option A (no client validation).

---

## Detailed Element Specifications

### 1. Input Element

**Purpose:** Core form input with semantic type (no client validation duplication)

```csharp
// File: Infrastructure/UI/Vocabulary/FormElements.cs

public enum InputType
{
    Text,
    Email,
    Password,
    Hidden,
    Number,
    Date,
    Tel,
    Url,
    Search
}

public record Input : IFieldContent, IBodyContent
{
    public required string Name { get; init; }
    public InputType Type { get; init; } = InputType.Text;
    public string? Placeholder { get; init; }
    public string? Value { get; init; }
    public bool Disabled { get; init; }
    public bool ReadOnly { get; init; }
    public string? Id { get; init; }

    public IEnumerable<IComponent> Children => Enumerable.Empty<IComponent>();
}
```

**Usage Example:**
```csharp
new Input
{
    Name = "email",
    Type = InputType.Email,  // Semantic type for mobile keyboard
    Placeholder = "Enter email"
    // No Required, no MaxLength - server validates
}
```

**HTML Output:**
```html
<input class="input input--email"
       name="email"
       type="email"
       placeholder="Enter email" />
```

**Theme Mapping:**
```yaml
# default.theme.yaml
input:
  base:
    padding: 2 3  # vertical horizontal
    border: border
    radius: md
    size: md
  type-email:
    # Email-specific styling if needed
  state-disabled:
    opacity: 0.5
    cursor: not-allowed
```

---

### 2. Label Element

**Purpose:** Accessible label for form inputs

```csharp
public record Label : IFieldContent, IBodyContent
{
    private readonly string _text;
    public string Text => _text;
    public string? For { get; init; }

    public IEnumerable<IComponent> Children => Enumerable.Empty<IComponent>();

    public Label(string text)
    {
        _text = text;
    }
}
```

**Usage Example:**
```csharp
new Label("Email")
{
    For = "email"
}
```

**HTML Output:**
```html
<label class="label" for="email">Email</label>
```

**Theme Mapping:**
```yaml
label:
  base:
    size: sm
    weight: medium
    color: text-primary
    margin-bottom: 1
```

---

### 3. Field Element

**Purpose:** Semantic grouping of Label + Input with server-side error display

```csharp
public record Field : ComponentBase, IBodyContent
{
    public Label? Label { get; init; }
    public required Input Input { get; init; }
    public Text? HelpText { get; init; }
    public string? ErrorMessage { get; init; }  // From server validation
    public bool HasError { get; init; }
}
```

**Usage Example (No Errors):**
```csharp
new Field
{
    Label = new Label("Email"),
    Input = new Input
    {
        Name = "email",
        Type = InputType.Email,
        Placeholder = "Enter email"
    }
}
```

**Usage Example (With Server Error):**
```csharp
// After validation failure, errors returned from workflow
IEnumerable<ResponseErrorEntry> errors = validationResult.ToResponseErrors();

new Field
{
    Label = new Label("Email"),
    Input = new Input { Name = "email", Type = InputType.Email },
    ErrorMessage = errors.FirstOrDefault(e => e.Property == "email")?.Message,
    HasError = errors.Any(e => e.Property == "email")
}
```

**HTML Output (No Error):**
```html
<div class="field">
    <label class="field__label" for="email">Email</label>
    <input class="field__input" name="email" type="email" placeholder="Enter email" />
</div>
```

**HTML Output (With Error):**
```html
<div class="field field--error">
    <label class="field__label" for="email">Email</label>
    <input class="field__input field__input--error" name="email" type="email" placeholder="Enter email" />
    <p class="field__error">Invalid email format.</p>
</div>
```

**Theme Mapping:**
```yaml
field:
  base:
    gap: 1  # Space between label and input
  label:
    margin-bottom: 1
  input:
    width: 100%
  state-error:
    input:
      border-color: critical
      background: critical-subtle
  help-text:
    size: sm
    color: text-secondary
    margin-top: 1
  error-message:
    size: sm
    color: critical
    margin-top: 1
```

---

### 4. Form Element

**Purpose:** Form container with HTMX support

```csharp
public record Form : ComponentBase, IBodyContent
{
    public required string Action { get; init; }
    public string Method { get; init; } = "post";
    public string? Id { get; init; }
    public string? Target { get; init; }
    public SwapStrategy Swap { get; init; } = SwapStrategy.InnerHTML;
}
```

**Usage Example:**
```csharp
new Form
{
    Action = "/interaction/login/authenticate",
    Id = "login-form"
}
{
    new Stack(For.Fields,
        new Field { Label = new Label("Email"), Input = new Input { Name = "email", Type = InputType.Email } },
        new Field { Label = new Label("Password"), Input = new Input { Name = "password", Type = InputType.Password } }
    ),
    new Button(Importance.Primary, "Login") { Type = ButtonType.Submit }
}
```

**HTML Output:**
```html
<form class="form" id="login-form" hx-post="/interaction/login/authenticate" hx-swap="innerHTML">
    <div class="stack stack--fields">
        <div class="field">...</div>
        <div class="field">...</div>
    </div>
    <button class="button button--primary" type="submit">Login</button>
</form>
```

---

### 5. Button.Type Enhancement

**Purpose:** Support submit/reset button types

```csharp
// File: Infrastructure/UI/Vocabulary/Button.cs (existing file)

public enum ButtonType
{
    Button,   // type="button" (default)
    Submit,   // type="submit"
    Reset     // type="reset"
}

public record Button : IHeaderContent, IFooterContent
{
    // ... existing properties ...
    public ButtonType Type { get; init; } = ButtonType.Button;
}
```

**Usage Example:**
```csharp
new Button(Importance.Primary, "Login")
{
    Type = ButtonType.Submit
}
```

**HTML Output:**
```html
<button class="button button--primary" type="submit">Login</button>
```

---

### 6. Checkbox Element

**Purpose:** Checkbox input with HTMX support for interactive toggles

```csharp
// File: Infrastructure/UI/Vocabulary/FormElements.cs

public record Checkbox : IFieldContent, IBodyContent
{
    public required string Name { get; init; }
    public bool Checked { get; init; }
    public string? Value { get; init; }
    public bool Disabled { get; init; }
    public string? Id { get; init; }

    // HTMX support for interactive checkboxes
    public string? Action { get; init; }
    public string? Target { get; init; }
    public SwapStrategy Swap { get; init; } = SwapStrategy.InnerHTML;
    public long? ModelId { get; init; }

    public IEnumerable<IComponent> Children => Enumerable.Empty<IComponent>();
}
```

**Usage Example (Traditional):**
```csharp
new Checkbox
{
    Name = "completed",
    Checked = todo.IsCompleted,
    Value = todo.Id.ToString()
}
```

**Usage Example (HTMX Interactive):**
```csharp
new Checkbox
{
    Name = "id",
    Checked = todo.IsCompleted,
    Action = "/interaction/todos/toggle",
    ModelId = todo.Id
}
```

**HTML Output (HTMX):**
```html
<input type="checkbox"
       class="checkbox"
       name="id"
       checked
       hx-post="/interaction/todos/toggle"
       hx-vals='{"id": 123}'
       hx-swap="innerHTML"
       hx-trigger="change" />
```

**Theme Mapping:**
```yaml
checkbox:
  base:
    size: 4  # 16px
    border: border
    radius: sm
    cursor: pointer
  checked:
    background: accent
    border-color: accent
  disabled:
    opacity: 0.5
    cursor: not-allowed
```

---

### 7. Alert Element

**Purpose:** User feedback for errors, success, warnings, info

```csharp
// File: Infrastructure/UI/Vocabulary/FeedbackElements.cs

public enum AlertTone
{
    Neutral,   // Informational (default)
    Positive,  // Success
    Warning,   // Warning/Caution
    Critical   // Error/Danger
}

public record Alert : IComponent, IBodyContent
{
    private readonly string _message;
    public IEnumerable<IComponent> Children => Enumerable.Empty<IComponent>();

    public string Message => _message;
    public AlertTone Tone { get; init; }
    public bool Dismissible { get; init; }
    public string? Id { get; init; }

    public Alert(string message, AlertTone tone = AlertTone.Neutral)
    {
        _message = message;
        Tone = tone;
    }
}
```

**Usage Example:**
```csharp
// Error
new Alert("Invalid email or password", AlertTone.Critical)

// Success
new Alert("Login successful!", AlertTone.Positive)

// Warning
new Alert("Your session will expire in 5 minutes", AlertTone.Warning)
```

**HTML Output:**
```html
<div class="alert alert--critical" role="alert">
    <p>Invalid email or password</p>
</div>
```

**Theme Mapping:**
```yaml
alert:
  base:
    padding: 3 4
    radius: md
    border-width: 1
  tone-neutral:
    background: surface-raised
    border-color: border
    text: text-primary
  tone-positive:
    background: positive-subtle
    border-color: positive
    text: positive-dark
  tone-warning:
    background: warning-subtle
    border-color: warning
    text: warning-dark
  tone-critical:
    background: critical-subtle
    border-color: critical
    text: critical-dark
```

---

### 8. EmptyState Element

**Purpose:** Empty list or no-content messaging

```csharp
// File: Infrastructure/UI/Vocabulary/FeedbackElements.cs

public enum EmptyStateSize
{
    Small,   // Brief inline message
    Medium,  // Default card-style
    Large    // Full-page with illustration
}

public record EmptyState : IComponent, IBodyContent
{
    private readonly string _message;
    public IEnumerable<IComponent> Children => Enumerable.Empty<IComponent>();

    public string Message => _message;
    public EmptyStateSize Size { get; init; } = EmptyStateSize.Medium;
    public string? ActionLabel { get; init; }
    public string? ActionUrl { get; init; }

    public EmptyState(string message)
    {
        _message = message;
    }
}
```

**Usage Example:**
```csharp
new EmptyState("No todos yet. Add one above to get started!")
{
    Size = EmptyStateSize.Small
}
```

**HTML Output:**
```html
<div class="empty-state empty-state--small">
    <p class="empty-state__message">No todos yet. Add one above to get started!</p>
</div>
```

**Theme Mapping:**
```yaml
empty-state:
  size-small:
    padding: 3
    text-size: sm
    text-color: text-secondary
  size-medium:
    padding: 6
    text-size: md
    text-align: center
  size-large:
    padding: 8
    text-size: lg
    text-align: center
```

---

### 9. List Element

**Purpose:** Container for list items

```csharp
// File: Infrastructure/UI/Vocabulary/ListElements.cs

public enum ListStyle
{
    Unordered,  // Bullets (default)
    Ordered,    // Numbers
    Plain       // No markers
}

public record List : ComponentBase, IBodyContent
{
    public ListStyle Style { get; init; } = ListStyle.Unordered;
    public string? Id { get; init; }
}
```

**Usage Example:**
```csharp
new List { Style = ListStyle.Plain, Id = "todo-list-ul" }
{
    new ListItem { /* ... */ },
    new ListItem { /* ... */ }
}
```

**HTML Output:**
```html
<ul class="list list--plain" id="todo-list-ul">
    <li class="list__item">...</li>
    <li class="list__item">...</li>
</ul>
```

**Theme Mapping:**
```yaml
list:
  style-unordered:
    list-style: disc
    padding-left: 5
  style-ordered:
    list-style: decimal
    padding-left: 5
  style-plain:
    list-style: none
    padding-left: 0
```

---

### 10. ListItem Element

**Purpose:** Individual list item with semantic state

```csharp
// File: Infrastructure/UI/Vocabulary/ListElements.cs

public enum ListItemState
{
    Normal,
    Completed,
    Disabled,
    Error
}

public record ListItem : ComponentBase
{
    public ListItemState State { get; init; } = ListItemState.Normal;
    public string? Id { get; init; }
}
```

**Usage Example:**
```csharp
new ListItem
{
    State = todo.IsCompleted ? ListItemState.Completed : ListItemState.Normal,
    Id = $"todo-{todo.Id}"
}
{
    new Row(For.Items,
        new Checkbox { Name = "id", Checked = todo.IsCompleted, ModelId = todo.Id },
        new Text(todo.Title),
        new Button(Importance.Ghost, "×") { Action = "/interaction/todos/delete", ModelId = todo.Id }
    )
}
```

**HTML Output:**
```html
<li class="list__item list__item--completed" id="todo-123">
    <div class="row row--items">
        <input type="checkbox" checked />
        <p class="text">Buy milk</p>
        <button class="button button--ghost">×</button>
    </div>
</li>
```

**Theme Mapping:**
```yaml
list-item:
  base:
    padding: 2 0
  state-normal:
    opacity: 1
  state-completed:
    opacity: 0.6
    text-decoration: line-through
  state-disabled:
    opacity: 0.4
    cursor: not-allowed
  state-error:
    background: critical-subtle
    border-left: 2px solid critical
```

---

## Implementation Phases

### Phase 0: Planning (Complete)
**Status:** ✅ Complete
**Deliverable:** This experiment document
**Date:** 2025-12-02

---

### Phase 1: Core Form Elements
**Status:** 🔲 Not Started
**Goal:** Enable Login page conversion
**Estimated Effort:** 1 week

**Tasks:**
1. Create `Infrastructure/UI/Vocabulary/FormElements.cs`
   - [ ] Define `InputType` enum
   - [ ] Implement `Input` record
   - [ ] Implement `Label` record
   - [ ] Implement `Field` record
   - [ ] Implement `Form` record
   - [ ] Implement `Checkbox` record

2. Update `Infrastructure/UI/Vocabulary/Button.cs`
   - [ ] Add `ButtonType` enum
   - [ ] Add `Type` property to Button
   - [ ] Add `IFieldContent` interface to Button

3. Update `Infrastructure/UI/IComponent.cs`
   - [ ] Add `IFieldContent` interface

4. Update `Infrastructure/UI/Rendering/HtmlRenderer.cs`
   - [ ] Implement `renderInput()` method
   - [ ] Implement `renderLabel()` method
   - [ ] Implement `renderField()` method
   - [ ] Implement `renderForm()` method
   - [ ] Implement `renderCheckbox()` method
   - [ ] Update `renderButton()` to handle Type property

5. Update `Infrastructure/UI/Themes/default.theme.yaml`
   - [ ] Add input styling tokens
   - [ ] Add label styling tokens
   - [ ] Add field styling tokens
   - [ ] Add form styling tokens
   - [ ] Add checkbox styling tokens

6. Update `Infrastructure/UI/Rendering/ThemeCompiler.cs`
   - [ ] Generate CSS for input elements
   - [ ] Generate CSS for label elements
   - [ ] Generate CSS for field elements
   - [ ] Generate CSS for form elements
   - [ ] Generate CSS for checkbox elements

**Success Criteria:**
- [ ] Login page compiles using only semantic types
- [ ] Login form renders with correct HTML structure
- [ ] Form submission works with HTMX
- [ ] Visual regression: Login page looks identical to current

**Blocked By:** None
**Blocks:** Phase 2

---

### Phase 2: Feedback Elements
**Status:** 🔲 Not Started
**Goal:** Add error/success notifications
**Estimated Effort:** 3 days

**Tasks:**
1. Create `Infrastructure/UI/Vocabulary/FeedbackElements.cs`
   - [ ] Define `AlertTone` enum
   - [ ] Implement `Alert` record
   - [ ] Define `EmptyStateSize` enum
   - [ ] Implement `EmptyState` record

2. Update `Infrastructure/UI/Rendering/HtmlRenderer.cs`
   - [ ] Implement `renderAlert()` method
   - [ ] Implement `renderEmptyState()` method

3. Update `Infrastructure/UI/Themes/default.theme.yaml`
   - [ ] Add alert styling (all tones)
   - [ ] Add empty state styling

4. Update `Infrastructure/UI/Rendering/ThemeCompiler.cs`
   - [ ] Generate CSS for alert elements (all tone variants)
   - [ ] Generate CSS for empty state elements

**Success Criteria:**
- [ ] Alert renders with correct tone styling
- [ ] EmptyState renders appropriately
- [ ] Login error/success messages use Alert
- [ ] Visual regression: Alerts look identical to current

**Blocked By:** Phase 1
**Blocks:** Phase 3

---

### Phase 3: List Elements
**Status:** 🔲 Not Started
**Goal:** Enable Todos page conversion
**Estimated Effort:** 3 days

**Tasks:**
1. Create `Infrastructure/UI/Vocabulary/ListElements.cs`
   - [ ] Define `ListStyle` enum
   - [ ] Implement `List` record
   - [ ] Define `ListItemState` enum
   - [ ] Implement `ListItem` record

2. Update `Infrastructure/UI/Rendering/HtmlRenderer.cs`
   - [ ] Implement `renderList()` method
   - [ ] Implement `renderListItem()` method with state handling

3. Update `Infrastructure/UI/Themes/default.theme.yaml`
   - [ ] Add list styling (all styles)
   - [ ] Add list item styling (all states)

4. Update `Infrastructure/UI/Rendering/ThemeCompiler.cs`
   - [ ] Generate CSS for list elements
   - [ ] Generate CSS for list item states

**Success Criteria:**
- [ ] Todos page compiles using only semantic types
- [ ] Todo list renders with correct HTML structure
- [ ] Completed state styling works correctly
- [ ] Visual regression: Todos page looks identical to current

**Blocked By:** Phase 1, Phase 2
**Blocks:** Phase 4

---

### Phase 4: Login Page Conversion
**Status:** 🔲 Not Started
**Goal:** Convert Login page to Closed-World UI
**Estimated Effort:** 2 days

**Tasks:**
1. Update `Pages/Login/Login.Page.htmx.cs`
   - [ ] Add `IHtmlRenderer` constructor dependency
   - [ ] Convert `RenderLoginForm()` to use semantic types
   - [ ] Convert `RenderError()` to use Alert
   - [ ] Convert `RenderSuccess()` to use Alert
   - [ ] Remove all raw HTML strings

2. Test login functionality
   - [ ] Manual test: successful login
   - [ ] Manual test: invalid credentials error
   - [ ] Manual test: empty form validation
   - [ ] Visual regression test

**Success Criteria:**
- [ ] Zero raw HTML in Login.Page.htmx.cs
- [ ] Zero CSS class names in code
- [ ] All login scenarios work correctly
- [ ] Visual appearance unchanged

**Blocked By:** Phase 1, Phase 2
**Blocks:** Phase 5

---

### Phase 5: Todos Page Conversion
**Status:** 🔲 Not Started
**Goal:** Convert Todos page to Closed-World UI
**Estimated Effort:** 3 days

**Tasks:**
1. Update `Pages/Todos/Todos.Page.htmx.cs`
   - [ ] Add `IHtmlRenderer` constructor dependency
   - [ ] Convert `RenderPage()` to use semantic types
   - [ ] Convert `RenderCreateForm()` to use Form element
   - [ ] Convert `RenderTodoList()` to use List element
   - [ ] Convert `RenderTodoItem()` to use ListItem + Checkbox
   - [ ] Convert error/notification methods to use Alert
   - [ ] Remove all raw HTML strings

2. Test todos functionality
   - [ ] Manual test: add todo
   - [ ] Manual test: toggle todo completion
   - [ ] Manual test: delete todo
   - [ ] Manual test: empty list state
   - [ ] Manual test: error notifications
   - [ ] Visual regression test

**Success Criteria:**
- [ ] Zero raw HTML in Todos.Page.htmx.cs
- [ ] Zero CSS class names in code
- [ ] All todo operations work correctly
- [ ] Completion state styling works
- [ ] Visual appearance unchanged

**Blocked By:** Phase 1, Phase 2, Phase 3
**Blocks:** Phase 6

---

### Phase 6: Polish & Documentation
**Status:** 🔲 Not Started
**Goal:** Complete vocabulary expansion and document learnings
**Estimated Effort:** 2 days

**Tasks:**
1. Optional Tier 2 elements (if time permits)
   - [ ] Icon element
   - [ ] Text.Display property
   - [ ] Divider element
   - [ ] Link element

2. Documentation
   - [ ] Update `/Infrastructure/UI/LOAD-CONTEXT.md` with new files
   - [ ] Update experiment doc with results
   - [ ] Document any design decisions or gotchas
   - [ ] Create examples for each new element

3. Code cleanup
   - [ ] Remove unused `ButtonDelete.Render()` helper
   - [ ] Remove unused `HtmxForm.Render()` helper
   - [ ] Consolidate duplicated patterns

**Success Criteria:**
- [ ] Documentation reflects final vocabulary
- [ ] Examples demonstrate all elements
- [ ] No dead code remaining
- [ ] Experiment marked Complete

**Blocked By:** Phase 4, Phase 5
**Blocks:** None

---

## Success Metrics

### Code Quality
- [ ] **Zero raw HTML strings** in Login and Todos pages
- [ ] **Zero CSS class names** in page rendering code
- [ ] **Zero inline styles** (impossible via type system)
- [ ] **Zero escape hatches** (no className properties exist)

### Conversion Completeness
- [ ] **Login page:** 100% semantic types
- [ ] **Todos page:** 100% semantic types
- [ ] **StyleTest page:** Already 100% (reference)

### Type Safety
- [ ] **Invalid structures don't compile** (type system enforcement)
- [ ] **State is semantic** (enums, not CSS classes)
- [ ] **Theme controls all appearance** (zero visual decisions in page code)

### Functional Parity
- [ ] **Login:** All scenarios work identically
- [ ] **Todos:** All operations work identically
- [ ] **Visual regression:** Pages look pixel-identical to before

### Maintainability
- [ ] **New form:** ~60% fewer lines (50 HTML → 20 semantic types)
- [ ] **Style changes:** Theme-only (zero page code changes)
- [ ] **New variations:** Add enum value, not new elements

---

## Open Questions

### Q1: Input Validation State
**Question:** How should validation errors be displayed?

**Options:**
- A) `Field.ErrorText` property (inline below input)
- B) Separate `Alert` element above form
- C) Both patterns supported

**Decision:** ✅ Option C - Support both patterns
**Date:** 2025-12-02

**Pattern:**
- Field-level errors: `Field.ErrorText` + `Field.HasError` (from server validation)
- Form-level errors: `Alert` element above form
- Server validates via workflow, returns errors, HTMX swaps response
- No client-side validation duplication (see Design Decision 1)

---

### Q2: Checkbox Rendering Strategy
**Question:** Should Checkbox render as native `<input type="checkbox">` or styled element?

**Options:**
- A) Native checkbox (accessibility, simplicity)
- B) Custom styled element (more control)
- C) Theme-controlled (render strategy varies by theme)

**Decision:** TBD in Phase 1

**Recommendation:** Option A initially (native), Option C long-term

---

### Q3: Form Validation Pattern
**Question:** Should Form element handle validation state automatically?

**Options:**
- A) Form.HasError property shows Alert automatically
- B) Developer manually adds Alert element
- C) Both (Form.HasError + optional explicit Alert)

**Decision:** ✅ Option B - Explicit Alert
**Date:** 2025-12-02

**Rationale:** Keep it simple. Developer explicitly adds Alert element when needed. Maintains full control over error messaging without magic behavior. Consistent with server-authority principle.

---

### Q4: Icon Implementation Strategy
**Question:** How should icons be rendered?

**Options:**
- A) SVG sprites (single file, symbol references)
- B) Inline SVG (full SVG markup per icon)
- C) Icon font (web font with ligatures)
- D) Unicode characters (simplest, limited)

**Decision:** TBD in Phase 6 (Tier 2 element)

**Recommendation:** Option A (SVG sprites) for scalability

---

### Q5: Empty State Placement
**Question:** Should EmptyState be inside List or replace it?

**Options:**
- A) Inside List as child
- B) Replace List entirely (conditional)

**Decision:** TBD in Phase 3

**Recommendation:** Option B (replace List) - semantically clearer, type-safer

---

## Reference Materials

### Files Created/Modified by This Experiment

**New Files:**
1. `/Infrastructure/UI/Vocabulary/FormElements.cs` - Input, Label, Field, Form, Checkbox
2. `/Infrastructure/UI/Vocabulary/FeedbackElements.cs` - Alert, EmptyState
3. `/Infrastructure/UI/Vocabulary/ListElements.cs` - List, ListItem

**Modified Files:**
1. `/Infrastructure/UI/Vocabulary/Button.cs` - Add ButtonType enum and Type property
2. `/Infrastructure/UI/IComponent.cs` - Add IFieldContent interface
3. `/Infrastructure/UI/Rendering/HtmlRenderer.cs` - Add 6+ new render methods
4. `/Infrastructure/UI/Rendering/ThemeCompiler.cs` - Add CSS generation for new elements
5. `/Infrastructure/UI/Themes/default.theme.yaml` - Add styling for all new elements
6. `/Pages/Login/Login.Page.htmx.cs` - Convert to semantic types
7. `/Pages/Todos/Todos.Page.htmx.cs` - Convert to semantic types

### Related Experiments
- `/project-docs/experiments/component-first-architecture.md` - Server-side component pattern (Phase 5 complete)
- `/project-docs/experiments/styles/` - Closed-World UI philosophy and specifications

### Code Examples

**Before (Raw HTML):**
```csharp
public string RenderLoginForm() =>
$$"""
<div id="login-form" class="login-form">
    <form hx-post="/interaction/login/authenticate">
        <div class="login-input-group">
            <label for="email">Email</label>
            <input id="email" name="email" type="email" placeholder="Enter email" required maxlength="100" />
        </div>
        <div class="login-input-group">
            <label for="password">Password</label>
            <input id="password" name="password" type="password" placeholder="Enter password" required />
        </div>
        <button type="submit">Login</button>
    </form>
</div>
""";
```

**After (Closed-World UI):**
```csharp
public string RenderLoginForm()
{
    var form = new Form
    {
        Action = "/interaction/login/authenticate",
        Id = "login-form"
    }
    {
        new Stack(For.Fields,
            new Field
            {
                Label = new Label("Email"),
                Input = new Input
                {
                    Name = "email",
                    Type = InputType.Email,
                    Placeholder = "Enter email"
                    // No Required, no MaxLength - server validates via LoginCommand
                }
            },
            new Field
            {
                Label = new Label("Password"),
                Input = new Input
                {
                    Name = "password",
                    Type = InputType.Password,
                    Placeholder = "Enter password"
                    // Server validates via LoginCommand
                }
            }
        ),
        new Button(Importance.Primary, "Login") { Type = ButtonType.Submit }
    };

    return _renderer.Render(form);
}
```

**Benefits Demonstrated:**
- Type-safe: Invalid structures won't compile
- No CSS classes or styling decisions in code
- Theme controls all appearance
- Clear semantic structure
- Accessible by default

---

## Lessons Learned

### What Worked Well
_(To be filled in during/after implementation)_

### What Didn't Work
_(To be filled in during/after implementation)_

### Design Decisions Made
_(To be filled in during implementation)_

### Would Do Differently Next Time
_(To be filled in after completion)_

---

## Conclusion

**Status:** Phase 0 Complete - Ready for Implementation

This experiment will prove whether the Closed-World UI philosophy can scale beyond simple cards and buttons to support real-world forms and interactive lists. Success means achieving 100% semantic type coverage with zero HTML escape hatches for both Login and Todos pages.

**Key Design Decision:** We've chosen server-authority over client-validation duplication. The Input element declares semantic type (email, password, etc.) but doesn't duplicate validation rules (Required, MaxLength). Server validates via workflow commands, returns errors via HTMX. This keeps the vocabulary simple, prevents drift, and maintains single source of truth.

**Next Step:** Begin Phase 1 - Core Form Elements

---

**Document Version:** 1.2
**Last Updated:** 2025-12-02
**Maintained By:** Development Team
**Changelog:**
- v1.2 (2025-12-02): Updated validation pattern to reflect FluentValidation + ResponseErrorEntry architecture
- v1.1 (2025-12-02): Added Design Decision 1 - No Client-Side Validation (Server Authority)
- v1.0 (2025-12-02): Initial document created
