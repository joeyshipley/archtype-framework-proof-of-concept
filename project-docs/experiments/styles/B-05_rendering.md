# Closed-World UI: Rendering & Output

How semantic types become HTML and CSS.

---

## The Rendering Pipeline

```
Developer Code          Theme File           Output
      ↓                      ↓                  ↓
┌─────────────┐      ┌─────────────┐     ┌─────────────┐
│  Semantic   │      │  Container- │     │    HTML     │
│   Types     │ ───→ │   Centric   │ ──→ │     +       │
│  (C#)       │      │   Styles    │     │    CSS      │
└─────────────┘      └─────────────┘     └─────────────┘
```

1. **Developer writes** semantic C# types
2. **Framework resolves** component tree with context
3. **Theme provides** appearance rules
4. **Renderer outputs** HTML with semantic classes
5. **CSS generated** from theme (build time or pre-compiled)

---

## HTML Output Strategy

The renderer produces clean, semantic HTML with simple class names.

### Developer Code

```csharp
new Card {
    Header = new Header { "Settings", new Badge(Tone.Positive) { "New" } },
    Body = new Body {
        new Stack(For.Fields) {
            new Text { "Configure your preferences below." },
            new Input(InputType.Email, Placeholder: "Email")
        }
    },
    Footer = new Footer {
        new Button(Importance.Secondary) { "Cancel" },
        new Button(Importance.Primary) { "Save" }
    }
}
```

### Generated HTML

```html
<div class="card">
  <div class="header">
    <span class="text">Settings</span>
    <span class="badge badge--positive">New</span>
  </div>
  <div class="body">
    <div class="stack stack--fields">
      <p class="text">Configure your preferences below.</p>
      <input class="input" type="email" placeholder="Email">
    </div>
  </div>
  <div class="footer">
    <button class="button button--secondary">Cancel</button>
    <button class="button button--primary">Save</button>
  </div>
</div>
```

### Class Naming Convention

| Element | Base Class | Modifiers |
|---------|------------|-----------|
| Container | `.card`, `.modal` | — |
| Slot | `.header`, `.body`, `.footer` | — |
| Component | `.button`, `.input`, `.badge` | `--primary`, `--warning` |
| Layout | `.stack`, `.row`, `.grid` | `--fields`, `--actions` |

Modifiers only appear when a semantic variant is specified:
- `Importance.Primary` → `button--primary`
- `Tone.Warning` → `badge--warning`
- `For.Fields` → `stack--fields`

---

## CSS Generation Strategy

CSS is generated from the theme file. Developers never write or see it.

### Using Cascade Layers

```css
@layer tokens, base, components, utilities;

@layer tokens {
  :root {
    --spacing-1: 0.25rem;
    --spacing-2: 0.5rem;
    --spacing-3: 0.75rem;
    --spacing-4: 1rem;
    --spacing-5: 1.25rem;
    
    --text-sm: 0.875rem;
    --text-md: 1rem;
    --text-lg: 1.125rem;
    
    --color-surface: #ffffff;
    --color-border: #e5e5e5;
    --color-accent: #2563eb;
    
    --shadow-sm: 0 1px 2px rgba(0,0,0,0.05);
    --shadow-md: 0 4px 6px rgba(0,0,0,0.1);
    
    --radius-md: 0.375rem;
    --radius-lg: 0.5rem;
  }
}

@layer base {
  .button {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    border-radius: var(--radius-md);
    font-weight: 500;
    cursor: pointer;
  }
  
  .stack {
    display: flex;
    flex-direction: column;
  }
  
  .header {
    font-weight: 600;
  }
}

@layer components {
  /* Button variants */
  .button--primary {
    background: var(--color-accent);
    color: white;
  }
  
  .button--secondary {
    background: transparent;
    border: 1px solid var(--color-border);
  }
  
  /* Stack purposes */
  .stack--fields {
    gap: var(--spacing-4);
  }
  
  .stack--actions {
    gap: var(--spacing-2);
  }
  
  /* Card structure */
  .card {
    background: var(--color-surface);
    border-radius: var(--radius-md);
    box-shadow: var(--shadow-sm);
  }
  
  .card > .header {
    font-size: var(--text-md);
    padding: var(--spacing-4);
  }
  
  .card > .body {
    padding: var(--spacing-4);
  }
  
  .card > .footer {
    padding: var(--spacing-4);
    display: flex;
    justify-content: flex-end;
    gap: var(--spacing-2);
  }
  
  /* Modal structure */
  .modal {
    background: var(--color-surface);
    border-radius: var(--radius-lg);
    box-shadow: var(--shadow-lg);
  }
  
  .modal > .header {
    font-size: var(--text-lg);
    padding: var(--spacing-5);
    border-bottom: 1px solid var(--color-border);
  }
  
  .modal > .body {
    padding: var(--spacing-5);
  }
  
  .modal > .footer {
    padding: var(--spacing-5);
    display: flex;
    justify-content: flex-end;
    gap: var(--spacing-3);
  }
  
  /* Context-aware button sizing */
  .card .button {
    padding: var(--spacing-2) var(--spacing-3);
    font-size: var(--text-sm);
  }
  
  .modal .button {
    padding: var(--spacing-2) var(--spacing-4);
    font-size: var(--text-md);
  }
  
  .toolbar .button {
    padding: var(--spacing-1) var(--spacing-2);
    font-size: var(--text-sm);
  }
}
```

### Descendant Selectors for Context

Context-aware styling uses descendant selectors:

```css
/* Header appears differently in different containers */
.card > .header { font-size: var(--text-md); }
.modal > .header { font-size: var(--text-lg); }
.alert > .header { font-size: var(--text-sm); }

/* Button sizing by context */
.card .button { font-size: var(--text-sm); }
.modal .button { font-size: var(--text-md); }
.toolbar .button { font-size: var(--text-xs); }
```

Direct child selectors (`.card > .header`) prevent unintended deep matching.

---

## Build Strategies

### Strategy A: Pre-compiled Complete CSS

Since the vocabulary is finite, generate ALL possible CSS upfront:
- Every component base style
- Every variant combination
- Every context (element in container)

**Pros:**
- Simple, no build-time analysis
- Works immediately

**Cons:**
- Includes unused styles (bounded, but present)
- Larger initial file

**File size estimate:** 20-50KB gzipped for a complete system.

### Strategy B: Usage-Analyzed CSS

Analyze the codebase at build time:
1. Find all component instantiations via Roslyn
2. Determine which variants are used
3. Generate only required CSS

**Pros:**
- Minimal CSS output
- No unused styles

**Cons:**
- Requires build-time source analysis
- More complex build pipeline

### Strategy C: Atomic Composition

Map variants to atomic utility classes internally:

```css
/* Internal utilities */
.p-4 { padding: var(--spacing-4); }
.gap-2 { gap: var(--spacing-2); }
.shadow-sm { box-shadow: var(--shadow-sm); }
.text-md { font-size: var(--text-md); }
```

Components compose atomic classes:

```html
<div class="card bg-surface shadow-sm rounded-md">
  <div class="header p-4 text-md font-semibold">
```

**Pros:**
- Maximum CSS reuse
- Tiny file sizes

**Cons:**
- Verbose HTML
- Harder to debug (many classes)

### Recommended: Strategy A or B

Pre-compiled (A) for simplicity, usage-analyzed (B) for optimization. Avoid atomic (C) unless file size is critical — it sacrifices debuggability.

---

## Context Tracking

The renderer tracks ancestry to resolve context-aware styling.

```csharp
public class RenderContext {
    public Stack<string> Ancestry { get; }
    
    public void PushContainer(string containerType) {
        Ancestry.Push(containerType);
    }
    
    public void PopContainer() {
        Ancestry.Pop();
    }
    
    public string? ParentContainer => 
        Ancestry.Count > 0 ? Ancestry.Peek() : null;
}
```

During rendering:

```csharp
// When rendering a Card
context.PushContainer("card");
RenderChildren(card.Header, context);
RenderChildren(card.Body, context);
RenderChildren(card.Footer, context);
context.PopContainer();
```

The HTML output doesn't need context classes — CSS descendant selectors handle it. But if modifier classes are preferred:

```html
<!-- With context modifiers (alternative approach) -->
<div class="header header--in-card">
```

---

## Server-Side Rendering Flow

Since this is a server-rendered framework with no client JS:

1. **Request arrives**
2. **C# code builds** component tree
3. **Renderer walks** tree, outputs HTML string
4. **HTML includes** `<link>` to pre-compiled CSS
5. **Response sent** — browser renders immediately

No hydration. No client-side framework. Just HTML + CSS.

```csharp
public string Render(IComponent root) {
    var context = new RenderContext();
    var builder = new StringBuilder();
    
    RenderComponent(root, context, builder);
    
    return builder.ToString();
}

private void RenderComponent(IComponent component, RenderContext ctx, StringBuilder sb) {
    // Get CSS classes for this component
    var classes = GetClasses(component, ctx);
    
    // Open tag
    sb.Append($"<{component.Tag} class=\"{classes}\">");
    
    // Track context for containers
    if (component is IContainer container) {
        ctx.PushContainer(container.ContainerType);
    }
    
    // Render children
    foreach (var child in component.Children) {
        RenderComponent(child, ctx, sb);
    }
    
    // Pop context
    if (component is IContainer) {
        ctx.PopContainer();
    }
    
    // Close tag
    sb.Append($"</{component.Tag}>");
}
```

---

## Output Guarantees

Because the type system enforces validity, the renderer can guarantee:

| Guarantee | Why |
|-----------|-----|
| **No invalid class names** | Classes come from vocabulary, not user input |
| **No missing styles** | Every vocabulary item has theme coverage |
| **No specificity conflicts** | `@layer` controls cascade |
| **Semantic HTML** | Tags chosen by component type |
| **Accessible defaults** | Baked into component rendering |

---

## Summary

| Aspect | Approach |
|--------|----------|
| **HTML classes** | Simple semantic names (`.card`, `.button--primary`) |
| **Context styling** | Descendant selectors (`.card > .header`) |
| **CSS organization** | `@layer` for predictable specificity |
| **Build strategy** | Pre-compiled or usage-analyzed |
| **Context tracking** | Renderer ancestry stack |
| **Delivery** | Server-rendered, no client JS |
