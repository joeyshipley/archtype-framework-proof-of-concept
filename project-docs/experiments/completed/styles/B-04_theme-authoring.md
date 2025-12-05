# Closed-World UI: Theme Authoring

How designers control appearance without developer involvement.

---

## The Separation

| Domain | Owner | File |
|--------|-------|------|
| **Structure** | Developer | C# code |
| **Appearance** | Designer | Theme file |

Developers never write CSS. Designers never edit code. Neither crosses into the other's domain.

---

## Theme Authoring Principle

> **Units that change together live together.**

The theme is organized by container, not by element. When a designer says "make modals more spacious," they edit one section:

```yaml
modal:
  header: { padding: 5 }
  body: { padding: 5 }
  footer: { padding: 5 }
```

Not hunt across separate element files.

---

## Theme File Format

The recommended format is a **CSS-like DSL** or **YAML**. Both support container-centric authoring.

### YAML Format

```yaml
# tokens.yaml — design primitives
tokens:
  spacing:
    1: 0.25rem
    2: 0.5rem
    3: 0.75rem
    4: 1rem
    5: 1.25rem
    6: 1.5rem
    8: 2rem
    
  text:
    xs: 0.75rem
    sm: 0.875rem
    md: 1rem
    lg: 1.125rem
    xl: 1.25rem
    2xl: 1.5rem
    
  color:
    surface: "#ffffff"
    surface-raised: "#fafafa"
    border: "#e5e5e5"
    text-primary: "#171717"
    text-secondary: "#525252"
    accent: "#2563eb"
    positive: "#16a34a"
    warning: "#ca8a04"
    critical: "#dc2626"
    
  shadow:
    sm: "0 1px 2px rgba(0,0,0,0.05)"
    md: "0 4px 6px rgba(0,0,0,0.1)"
    lg: "0 10px 15px rgba(0,0,0,0.1)"
    
  radius:
    sm: "0.25rem"
    md: "0.375rem"
    lg: "0.5rem"
    full: "9999px"
```

```yaml
# components.yaml — container-centric styling
card:
  base:
    background: surface
    shadow: sm
    radius: md
  header:
    size: md
    weight: semibold
    padding: 4
  media:
    radius-top: md
  body:
    padding: 4
  footer:
    padding: 4
    gap: 2
    justify: end
  button:
    size: sm

modal:
  base:
    background: surface
    shadow: lg
    radius: lg
  header:
    size: lg
    weight: semibold
    padding: 5
    border-bottom: border
  body:
    padding: 5
  footer:
    padding: 5
    gap: 3
    justify: end
  button:
    size: md

alert:
  base:
    padding: 4
    radius: md
  header:
    size: sm
    weight: semibold
  body:
    size: sm

  # Alert tones
  tone-positive:
    background: positive-subtle
    border: positive
  tone-warning:
    background: warning-subtle
    border: warning
  tone-critical:
    background: critical-subtle
    border: critical
```

### CSS-like DSL Format (Alternative)

```css
card {
  base {
    background: surface;
    shadow: sm;
    radius: md;
  }
  
  header {
    size: md;
    weight: semibold;
    padding: 4;
  }
  
  body {
    padding: 4;
  }
  
  footer {
    padding: 4;
    gap: 2;
    justify: end;
  }
  
  button {
    size: sm;
  }
}

modal {
  base {
    background: surface;
    shadow: lg;
    radius: lg;
  }
  
  header {
    size: lg;
    weight: semibold;
    padding: 5;
    border-bottom: border;
  }
  
  body {
    padding: 5;
  }
  
  footer {
    padding: 5;
    gap: 3;
    justify: end;
  }
}
```

---

## Theme Properties

The theme uses semantic properties, not raw CSS.

### Spacing

Uses token scale (1-8), not pixel values:

| Property | Accepts | Maps To |
|----------|---------|---------|
| `padding` | 1-8 | `var(--spacing-N)` |
| `gap` | 1-8 | `var(--spacing-N)` |
| `margin` | 1-8 | `var(--spacing-N)` |

### Typography

| Property | Accepts | Maps To |
|----------|---------|---------|
| `size` | xs, sm, md, lg, xl, 2xl | `var(--text-N)` |
| `weight` | normal, medium, semibold, bold | font-weight value |
| `leading` | tight, normal, relaxed | line-height value |

### Color

| Property | Accepts | Maps To |
|----------|---------|---------|
| `background` | token name | `var(--color-N)` |
| `border` | token name | `var(--color-N)` |
| `text` | token name | `var(--color-N)` |

### Effects

| Property | Accepts | Maps To |
|----------|---------|---------|
| `shadow` | none, sm, md, lg | `var(--shadow-N)` |
| `radius` | none, sm, md, lg, full | `var(--radius-N)` |

### Layout

| Property | Accepts | Maps To |
|----------|---------|---------|
| `justify` | start, center, end, between | justify-content |
| `align` | start, center, end, stretch | align-items |
| `direction` | row, column | flex-direction |

---

## Context-Aware Styling

Elements styled differently based on container context:

```yaml
# Button default
button:
  base:
    padding-x: 4
    padding-y: 2
    radius: md
    
  importance-primary:
    background: accent
    text: white
  importance-secondary:
    background: transparent
    border: border
    text: text-primary

# Button in specific contexts
card:
  button:
    size: sm          # Smaller in cards
    
modal:
  button:
    size: md          # Standard in modals
    
toolbar:
  button:
    size: xs          # Compact in toolbars
    padding-x: 2
    padding-y: 1
```

The theme says "buttons in toolbars are size xs." Developers just write `new Button(...)` — they don't know or choose the size.

---

## Layout Purpose Mapping

The `For` enum maps to spacing in the theme:

```yaml
stack:
  for-actions:
    gap: 2
  for-fields:
    gap: 4
  for-content:
    gap: 3
  for-items:
    gap: 3
  for-sections:
    gap: 8
  for-inline:
    gap: 1

row:
  for-actions:
    gap: 2
  for-fields:
    gap: 4
  for-inline:
    gap: 1

grid:
  for-cards:
    gap: 4
  for-items:
    gap: 3
```

Developer writes `new Stack(For.Fields)`. Theme decides that means `gap: 4` (1rem).

---

## Tone Mapping

Semantic tones map to colors:

```yaml
tone:
  default:
    text: text-primary
    background: transparent
  muted:
    text: text-secondary
  accent:
    text: accent
  positive:
    text: positive
    background: positive-subtle
  warning:
    text: warning
    background: warning-subtle
  critical:
    text: critical
    background: critical-subtle
```

Developer writes `new Badge(Tone.Warning)`. Theme decides what "warning" looks like.

---

## Complete Rebranding

To rebrand the entire application:

1. **Update tokens** — New colors, typography, spacing scale
2. **Adjust components** — Different padding, shadows, radii

No code changes. The structure remains identical. Only appearance changes.

```yaml
# Before: Clean, minimal
tokens:
  color:
    accent: "#2563eb"     # Blue
  shadow:
    sm: "0 1px 2px rgba(0,0,0,0.05)"
    
card:
  base:
    shadow: sm
    radius: md

# After: Bold, vibrant
tokens:
  color:
    accent: "#dc2626"     # Red
  shadow:
    sm: "0 2px 8px rgba(0,0,0,0.15)"
    
card:
  base:
    shadow: md            # More prominent
    radius: lg            # Rounder
```

Same developer code. Completely different appearance.

---

## Theme Compilation

The theme file compiles to CSS. This is an implementation detail designers don't see.

**Theme:**
```yaml
modal:
  header:
    size: lg
    padding: 5
    border-bottom: border
```

**Generated CSS:**
```css
.modal > .header {
  font-size: var(--text-lg);
  padding: var(--spacing-5);
  border-bottom: 1px solid var(--color-border);
}
```

The compilation uses descendant selectors within `@layer` for predictable specificity.

---

## Workflow: Designer-Friendly Editing

### Phase 1: Direct File Editing

Designer edits YAML or DSL file directly. Good tooling support:
- JSON Schema for YAML validation and autocomplete
- Syntax highlighting
- Preview on save

### Phase 2: Visual Theme Editor (Future)

GUI tool where designer:
1. Sees live component previews
2. Adjusts values with visual controls
3. Changes propagate in real-time
4. Saves to YAML/DSL format

The file format becomes an implementation detail.

---

## Summary

| Aspect | Description |
|--------|-------------|
| **Organization** | Container-centric (modal, card, alert) |
| **Properties** | Semantic (padding: 4, not padding: 16px) |
| **Values** | Token references (accent, sm, md) |
| **Context** | Element-in-container styling |
| **Output** | Compiled to CSS (developers never see) |
| **Rebranding** | Token + component changes, no code changes |
