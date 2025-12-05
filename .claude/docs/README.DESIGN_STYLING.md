# Unified Styling Specification: Closed-World UI

**Version:** 1.0
**Last Updated:** 2025-12-05
**Status:** Living Specification

---

## Core Principle

> **Developers declare WHAT things ARE. Designers control HOW they look. Invalid output is impossible.**

Closed-World UI inverts the traditional styling model. Instead of developers writing CSS to describe appearance, they use a finite semantic vocabulary. The theme system—owned entirely by designers—maps that vocabulary to visual output.

**Key Constraint:** No escape hatches. If it's not in the vocabulary, it doesn't exist.

---

## The Three Requirements

### 1. Complete Vocabulary
The system must name everything a developer might need. If something isn't in the vocabulary, it cannot be built (vocabulary extension is a controlled, deliberate process).

### 2. Finite Semantic Variants
Developers choose semantic options, never raw values:
- `For.Fields` not `gap: 16px`
- `Importance.Primary` not `background: blue`
- `Tone.Warning` not `color: orange`

### 3. No Escape Hatches
These do not exist:
- `className` parameter
- `style` parameter
- Arbitrary wrapper components
- Direct CSS access

The C# type system IS the enforcement layer.

---

## Developer Experience

### What Developers Write

```csharp
new Page {
    new PageTitle("Dashboard"),
    new Section {
        new SectionTitle("Recent Activity"),
        new Card()
            .Header(new Text("Updates"), new Badge(Tone.Positive, "3"))
            .Body(new Stack(For.Items) {
                new Text("New comment on your post"),
                new Text("Profile view from Seattle")
            })
            .Footer(new Row(For.Actions) {
                new Button(Importance.Secondary, "Dismiss"),
                new Button(Importance.Primary, "View All")
            })
    }
}
```

### What Developers Never Write

```csharp
// None of these exist
className: "my-class"
style: "padding: 16px"
padding: 4
fontSize: "lg"
color: "#333"
```

---

## Complete Vocabulary

### Page Structure
- `Page` - Root container with max-width constraint
- `Section` - Major page divisions
- `PageTitle` - H1-level identity
- `SectionTitle` - H2-level identity

### Layout Primitives
- `Stack(For purpose)` - Vertical arrangement with semantic spacing
- `Row(For purpose)` - Horizontal arrangement with semantic spacing
- `Grid(For purpose, Columns columns)` - Two-dimensional layout

**Purpose Enum (For):**
- `Actions` - Tight grouping (buttons, toolbar)
- `Fields` - Comfortable spacing (form inputs)
- `Content` - Readable flow (paragraphs)
- `Items` - Consistent rhythm (list items)
- `Sections` - Generous separation (page divisions)
- `Inline` - Minimal gap (icon + label)
- `Cards` - Card grid spacing

**Columns Enum:**
- `One`, `Two`, `Three`, `Four`, `Auto`

### Containers
- `Card` - Bounded content unit with Header/Body/Footer slots

### Slots (Universal)
- `Header` - Top region (title, identity, badges)
- `Body` - Main content area
- `Footer` - Bottom region (actions)

Slots are universal—`Header` is `Header` everywhere. The theme controls how it appears in each container context.

### Text Elements
- `Text` - Standard body text
- `PageTitle` - Page-level heading
- `SectionTitle` - Section-level heading

### Interactive Elements
- `Button(Importance, string label)` - User actions
  - Importance: `Primary`, `Secondary`, `Tertiary`, `Ghost`
  - States: `Disabled(bool)`, `Loading(bool)`
  - HTMX: `Action(url)`, `Target(selector)`, `Swap(strategy)`, `ModelId(id)`

### Form Elements
- `Input` - Text input with validation states
  - Types: `Text`, `Email`, `Password`, `Number`, `Date`, `Tel`, `Url`, `Search`, `Hidden`
  - States: `Disabled(bool)`, `ReadOnly(bool)`
- `Label` - Field labels
- `Field` - Label + Input + error/help text grouping
- `Form` - Form container with HTMX support
- `Checkbox` - Boolean input with HTMX toggle support

### Feedback Elements
- `Alert(AlertTone, string message)` - System messages
  - Tones: `Neutral`, `Positive`, `Warning`, `Critical`
- `EmptyState(string message, EmptyStateSize)` - No-content messaging
  - Sizes: `Small`, `Medium`, `Large`

### List Elements
- `List(ListStyle)` - Container for list items
  - Styles: `Unordered`, `Ordered`, `Plain`
- `ListItem` - Individual list item with states
  - States: `Normal`, `Completed`, `Disabled`, `Error`

---

## Theme Architecture

### Organization Principle

> **"Units that change together live together."**

Themes are organized by **container**, not by element. When a designer says "make cards more spacious," they edit one section—not hunt across files.

### Two-Layer System

**Layer 1: Design Tokens (Primitive Values)**

Raw values that never change meaning:
```yaml
tokens:
  spacing: { 1: 0.25rem, 2: 0.5rem, ..., 8: 2rem }
  text: { xs: 0.75rem, sm: 0.875rem, md: 1rem, lg: 1.125rem, xl: 1.25rem, 2xl: 1.5rem }
  color: { surface: "#fff", accent: "#2563eb", ... }
  shadow: { sm: "...", md: "...", lg: "..." }
  radius: { sm: 0.25rem, md: 0.375rem, lg: 0.5rem, full: 9999px }
  duration: { fast: 150ms, normal: 300ms, slow: 500ms }
  font:
    weight-normal: 400
    weight-medium: 500
    weight-semibold: 600
    weight-bold: 700
  opacity: { subdued: 0.4, disabled: 0.5, subtle: 0.6 }
```

**Layer 2: Component Mappings (Semantic Properties)**

Container-centric styling that references tokens:
```yaml
card:
  base:
    background: surface
    shadow: sm
    radius: md
  header:
    size: md
    weight: semibold
    padding: 4
  body:
    padding: 4
  footer:
    padding: 4
    gap: 2
    justify: end
  button:
    size: sm    # Buttons in cards are smaller

modal:
  header:
    size: lg    # Modal headers are larger
    padding: 5
  button:
    size: md    # Modal buttons are standard
```

### Theme Properties (Semantic, Not Pixels)

| Property | Accepts | Maps To |
|----------|---------|---------|
| `padding`, `gap`, `margin` | 1-8 | `var(--spacing-N)` |
| `size` | xs, sm, md, lg, xl, 2xl | `var(--text-N)` |
| `weight` | normal, medium, semibold, bold | `var(--font-weight-N)` |
| `background`, `border`, `text` | token name | `var(--color-N)` |
| `shadow` | none, sm, md, lg | `var(--shadow-N)` |
| `radius` | none, sm, md, lg, full | `var(--radius-N)` |
| `duration` | fast, normal, slow | `var(--duration-N)` |
| `opacity` | subdued, disabled, subtle | `var(--opacity-N)` |

### Context-Aware Styling

Elements styled differently based on container:

```yaml
# Button default
button:
  base:
    padding-x: 4
    padding-y: 2
    weight: medium
    transition: fast

# Button in specific contexts
card:
  button:
    size: sm        # Smaller in cards

modal:
  button:
    size: md        # Standard in modals
```

Developer writes `new Button(...)`. Theme decides size based on context.

---

## Known Change Vectors

Every styling architecture must handle these change types:

| Change Vector | Example | Theme Strategy |
|---------------|---------|----------------|
| **Token-level** | Rebrand colors, adjust all spacing | Change token values, propagates everywhere |
| **Component-wide** | All cards get borders | Update `card.base` in theme |
| **Context-specific** | Sidebar cards differ from main cards | Add `sidebar.card` mapping |
| **Semantic** | All "de-emphasized" shifts | Update tone mappings |
| **One-off** | Specific screen needs | Create specific component mapping |

---

## Rendering Pipeline

```
Developer Code (C#)  →  Theme File (YAML)  →  HTML + CSS
     (WHAT)                  (HOW)              (OUTPUT)
```

### HTML Output

Clean semantic HTML with simple class names:

```html
<div class="card">
  <div class="header">
    <p class="text">Settings</p>
    <span class="badge badge--positive">New</span>
  </div>
  <div class="body">
    <div class="stack stack--fields">
      <p class="text">Configure preferences</p>
      <input class="input" type="email" placeholder="Email">
    </div>
  </div>
  <div class="footer">
    <button class="button button--secondary">Cancel</button>
    <button class="button button--primary">Save</button>
  </div>
</div>
```

### CSS Generation Strategy

**Use Cascade Layers for Predictable Specificity:**

```css
@layer tokens, base, components;

@layer tokens {
  :root {
    --spacing-4: 1rem;
    --text-md: 1rem;
    --font-weight-semibold: 600;
    --color-accent: #2563eb;
    --shadow-sm: 0 1px 2px rgba(0,0,0,0.05);
    --radius-md: 0.375rem;
    --duration-fast: 150ms;
    --opacity-disabled: 0.5;
  }
}

@layer base {
  .card { display: flex; flex-direction: column; }
  .button { display: inline-flex; cursor: pointer; }
}

@layer components {
  .card {
    background: var(--color-surface);
    box-shadow: var(--shadow-sm);
    border-radius: var(--radius-md);
  }

  .card > .header {
    font-size: var(--text-md);
    font-weight: var(--font-weight-semibold);
    padding: var(--spacing-4);
  }

  /* Context-aware: buttons in cards */
  .card .button {
    padding: var(--spacing-2) var(--spacing-3);
    font-size: var(--text-sm);
  }

  .button--primary {
    background: var(--color-accent);
    color: var(--color-white);
    transition: all var(--duration-fast) ease;
  }

  .button:disabled {
    opacity: var(--opacity-disabled);
    cursor: not-allowed;
  }
}
```

**Descendant Selectors for Context:**

```css
.card > .header { font-size: var(--text-md); }
.modal > .header { font-size: var(--text-lg); }
.card .button { font-size: var(--text-sm); }
.modal .button { font-size: var(--text-md); }
```

---

## Complete Token System

### Current Implementation Status

**Implemented:**
- `spacing`: 1-8 (1, 2, 3, 4, 5, 6, 8 - no 7)
- `text`: sm, md, lg
- `color`: 16 semantic colors
- `shadow`: sm, md
- `radius`: sm, md, lg

**Missing (Required for Designer Control):**
- `text.xs`, `text.xl`, `text.2xl` - Smaller text and heading sizes
- `color.white` - Explicit for inverse text on colored backgrounds
- `shadow.lg` - Heavier shadows for elevated components
- `radius.full` - Circular elements (avatars, pills)
- `duration.*` - Animation/transition timing (fast, normal, slow)
- `font.weight-*` - Font weights (normal, medium, semibold, bold)
- `opacity.*` - State opacity values (subdued, disabled, subtle)

### Why These Are Tokens

**Designer Control Test:** "If a designer wants to change this, can they do it without touching code?"

Currently NO for:
- Font weights (hardcoded in ThemeCompiler.cs as 400, 500, 600, 700)
- Opacity values (hardcoded as 0.4, 0.5, 0.6)
- Transition durations (hardcoded as 150ms, 500ms)

**These violate the separation principle.** Designers cannot rebrand, adjust feel, or tweak emphasis without developer involvement.

### Recommended Complete Token System

```yaml
tokens:
  # Spacing (complete)
  spacing:
    1: 0.25rem    # 4px
    2: 0.5rem     # 8px
    3: 0.75rem    # 12px
    4: 1rem       # 16px
    5: 1.25rem    # 20px
    6: 1.5rem     # 24px
    8: 2rem       # 32px

  # Typography (expand)
  text:
    xs: 0.75rem       # NEW - captions, fine print
    sm: 0.875rem      # 14px - small text
    md: 1rem          # 16px - body
    lg: 1.125rem      # 18px - large body
    xl: 1.25rem       # NEW - 20px - section headings (h2)
    2xl: 1.5rem       # NEW - 24px - page titles (h1)

  # Font (NEW CATEGORY - designer-controlled weights)
  font:
    weight-normal: 400      # Body text
    weight-medium: 500      # Labels, UI elements
    weight-semibold: 600    # Card headers, h2
    weight-bold: 700        # Page titles, h1

  # Color (add white)
  color:
    # Surface
    surface: "#ffffff"
    surface-raised: "#fafafa"
    white: "#ffffff"        # NEW - explicit for button text

    # Border/Outline
    border: "#e5e5e5"

    # Text
    text-primary: "#171717"
    text-secondary: "#525252"

    # Brand
    accent: "#2563eb"
    accent-hover: "#1d4ed8"

    # Feedback
    positive: "#16a34a"
    positive-subtle: "#f0fdf4"
    positive-dark: "#15803d"
    warning: "#d97706"
    warning-subtle: "#fffbeb"
    warning-dark: "#b45309"
    critical: "#dc2626"
    critical-subtle: "#fef2f2"
    critical-dark: "#991b1b"

  # Shadow (expand)
  shadow:
    sm: "0 1px 2px rgba(0,0,0,0.05)"
    md: "0 4px 6px rgba(0,0,0,0.1)"
    lg: "0 10px 15px rgba(0,0,0,0.1)"    # NEW - elevated modals

  # Radius (expand)
  radius:
    sm: 0.25rem
    md: 0.375rem
    lg: 0.5rem
    full: 9999px      # NEW - circular avatars, pills

  # Duration (NEW CATEGORY - designer-controlled timing)
  duration:
    fast: 150ms       # Quick interactions (hover, focus)
    normal: 300ms     # Standard transitions
    slow: 500ms       # Loading states, emphasis

  # Opacity (NEW CATEGORY - designer-controlled states)
  opacity:
    subdued: 0.4      # Very disabled/inactive
    disabled: 0.5     # Standard disabled state
    subtle: 0.6       # De-emphasized/completed
```

### What Should NOT Be Tokens

**Line-height:** Universal constant `1.5`. Making this a token adds indirection without value.

**Border widths:** Almost always `1px` (default) or `2px` (emphasis). These are universal UI constants, not design decisions.

**Layout calculations:** Values like `minmax(300px, 1fr)` in grid definitions are engineering calculations, not design primitives.

---

## Type System Enforcement

### Slot Constraints

```csharp
public interface IHeaderContent : IComponent { }
public interface IBodyContent : IComponent { }
public interface IFooterContent : IComponent { }

public record Text : IHeaderContent, IBodyContent, IFooterContent { }
public record Button : IHeaderContent, IFooterContent { }
public record Stack : IBodyContent { }

public record Card {
    public Header? Header { get; init; }
    public required Body Body { get; init; }
    public Footer? Footer { get; init; }
}
```

**Enforcement:**
- ✅ `new Footer { new Button(...) }` - compiles
- ❌ `new Header { new Stack(...) }` - won't compile (Stack is not IHeaderContent)
- ❌ `new Card { Header = ... }` - won't compile (Body required)

### No Escape Hatches

```csharp
// These properties don't exist on any component
card.ClassName = "custom";     // Doesn't compile
card.Style = "padding: 20px";  // Doesn't compile
button.Padding = 16;           // Doesn't compile
```

---

## Implementation Checklist

### Phase 1: Token System Completion
- [ ] Add missing text sizes (xs, xl, 2xl) to `default.theme.yaml`
- [ ] Add font.weight-* tokens
- [ ] Add duration.* tokens
- [ ] Add opacity.* tokens
- [ ] Add color.white, shadow.lg, radius.full

### Phase 2: ThemeCompiler Updates
- [ ] Generate CSS custom properties for new token categories
- [ ] Update `generateTokensLayer()` to handle font, duration, opacity
- [ ] Replace hardcoded font-weight values with token references
- [ ] Replace hardcoded opacity values with token references
- [ ] Replace hardcoded duration values with token references
- [ ] Update `generateComponentsLayer()` methods to use tokens

### Phase 3: Component Mapping
- [ ] Add weight properties to component mappings (button.base.weight: medium)
- [ ] Add duration properties to component mappings (button.base.transition: fast)
- [ ] Add opacity properties to state mappings (button.state-disabled.opacity: disabled)
- [ ] Verify context-specific styling works (card.button.size: sm)

### Phase 4: Validation
- [ ] Designer can change all font weights without code
- [ ] Designer can change all transition speeds without code
- [ ] Designer can change all disabled state opacity without code
- [ ] Generated CSS matches current output (no visual regressions)
- [ ] All known change vectors have clear theme-based solutions

---

## Key Implementation Files

| File | Purpose |
|------|---------|
| `Infrastructure/UI/Themes/default.theme.yaml` | Token definitions + component mappings |
| `Infrastructure/UI/Rendering/ThemeCompiler.cs` | YAML → CSS generation |
| `Infrastructure/UI/Vocabulary/*.cs` | C# component type definitions |
| `Infrastructure/UI/Rendering/HtmlRenderer.cs` | Component tree → HTML |
| `wwwroot/css/closed-world.css` | Generated output (auto-generated, do not edit) |

---

## Design Principles Summary

| Principle | Description |
|-----------|-------------|
| **Separation** | Developers: structure. Designers: appearance. Never cross. |
| **Semantic** | Declare WHAT, not HOW. `For.Fields` not `gap: 16px`. |
| **Complete** | Vocabulary covers all common UI patterns. |
| **Finite** | Bounded set of valid combinations. |
| **Impossible** | Invalid output doesn't compile. Not discouraged—prevented. |
| **Container-centric** | Theme organized by container. Units that change together live together. |
| **Token-driven** | All appearance values come from tokens. Raw values forbidden. |
| **Context-aware** | Same component can look different based on container. |

---

**This specification replaces:**
- B-00_README.md
- B-01_philosophy.md
- B-02_vocabulary.md
- B-03_type-system.md
- B-04_theme-authoring.md
- B-05_rendering.md
- Prior token analysis discussions

**Goal:** Single source of truth for Closed-World UI implementation.
