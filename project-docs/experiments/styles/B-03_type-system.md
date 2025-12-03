# Closed-World UI: Type System & Enforcement

How C# enforces valid combinations and prevents invalid output.

---

## The Enforcement Principle

> Invalid output must be **impossible**, not discouraged.

The compiler is the enforcement layer. If it compiles, it's valid. If it's invalid, it doesn't compile.

---

## Configuration vs Composition

The type system distinguishes two kinds of input:

| Configuration | Composition |
|---------------|-------------|
| Properties on a component | Children within a component |
| Intrinsic properties | Contents |
| Single value | Multiple items |
| Modifies the component | Lives inside the component |

**Configuration examples:**
- `Importance.Primary` on Button
- `For.Fields` on Stack
- `Tone.Warning` on Badge

**Composition examples:**
- Button's label text
- Card's Body contents
- Stack's child elements

---

## Container Slot Enforcement

Containers declare which slots they have and which are required.

```csharp
public record Card {
    public Header? Header { get; init; }
    public Media? Media { get; init; }
    public required Body Body { get; init; }
    public Footer? Footer { get; init; }
}

public record Modal {
    public required Header Header { get; init; }
    public required Body Body { get; init; }
    public Footer? Footer { get; init; }
}

public record Alert {
    public Header? Header { get; init; }
    public required Body Body { get; init; }
    // Note: no Footer slot
}
```

**What this enforces:**

```csharp
// ✅ Compiles — Body is provided
new Card {
    Body = new Body { new Text { "Content" } }
}

// ❌ Won't compile — Body is required
new Card {
    Header = new Header { "Title" }
}

// ❌ Won't compile — Alert has no Footer slot
new Alert {
    Body = new Body { "Message" },
    Footer = new Footer { }  // Property doesn't exist
}
```

---

## Slot Content Constraints

Each slot type constrains what it can contain.

```csharp
// Marker interfaces for content categories
public interface IHeaderContent { }
public interface IBodyContent { }
public interface IFooterContent { }
public interface IMediaContent { }

// Slots accept specific content types
public record Header : IContainer<IHeaderContent> { }
public record Body : IContainer<IBodyContent> { }
public record Footer : IContainer<IFooterContent> { }
public record Media : IContainer<IMediaContent> { }

// Elements implement the interfaces they belong to
public record Text : IHeaderContent, IBodyContent, IFooterContent { }
public record Button : IHeaderContent, IFooterContent { }
public record Icon : IHeaderContent { }
public record Badge : IHeaderContent { }
public record Image : IMediaContent, IBodyContent { }
public record Stack : IBodyContent { }
```

**What this enforces:**

```csharp
// ✅ Compiles — Button is IFooterContent
new Footer { new Button(Importance.Primary) { "Save" } }

// ✅ Compiles — Text is IHeaderContent
new Header { new Text { "Title" } }

// ❌ Won't compile — Stack is not IHeaderContent
new Header { new Stack(For.Fields) { ... } }

// ❌ Won't compile — Image (by default) is not IFooterContent
new Footer { new Image("/photo.jpg") }
```

---

## Layout Purpose Enforcement

Layout primitives require a purpose, not a size.

```csharp
public enum For {
    Actions,
    Fields,
    Content,
    Items,
    Sections,
    Inline
}

public record Stack(For For);
public record Row(For For);
public record Grid(For For, Columns Columns = Columns.Auto);
```

**What this enforces:**

```csharp
// ✅ Compiles — purpose is specified
new Stack(For.Fields) { ... }

// ❌ Won't compile — no parameterless constructor
new Stack() { ... }

// ❌ Won't compile — Gap doesn't exist as a parameter
new Stack(Gap.Sm) { ... }
```

---

## Enum Constraints

All variant options are enums, not strings or numbers.

```csharp
public enum Importance { Primary, Secondary, Tertiary, Ghost }
public enum Tone { Default, Muted, Accent, Positive, Warning, Critical }
public enum Columns { One, Two, Three, Four, Auto }
public enum ValidationState { None, Valid, Invalid }
```

**What this enforces:**

```csharp
// ✅ Compiles — valid enum value
new Button(Importance.Primary)

// ❌ Won't compile — not a valid Importance value
new Button(Importance.Large)

// ❌ Won't compile — string is not Importance
new Button("primary")

// ❌ Won't compile — can't use arbitrary values
new Grid(For.Cards, Columns: 5)  // Only 1-4 or Auto exist
```

---

## What Cannot Be Expressed

The type system makes these impossible:

### No Arbitrary Styling

```csharp
// These properties don't exist on any type
card.ClassName = "my-override";
card.Style = "padding: 20px";
card.Padding = 16;
button.FontSize = "lg";
text.Color = "#333";
```

### No Size Parameters (Where Context Should Decide)

```csharp
// Size parameter doesn't exist on Button
new Button(Importance.Primary, Size: ButtonSize.Large)

// Size parameter doesn't exist on Input
new Input(InputType.Text, Size: InputSize.Small)
```

Size is determined by context (Button in Toolbar vs Button in Footer).

### No Generic Wrappers

```csharp
// These types don't exist
new Div { ... }
new Span { ... }
new Wrapper { ... }
new Box(padding: 4) { ... }
```

### No Raw Values in Layout

```csharp
// Gap doesn't accept pixel values or tokens
new Stack(Gap: 8) { ... }
new Stack(Gap: "md") { ... }
new Row(Justify: "space-between") { ... }
```

---

## The Complete Constraint Chain

```
Developer writes code
        ↓
Type system validates:
  - Only vocabulary types exist
  - Containers have defined slots
  - Slots accept defined content types
  - Parameters are enum-constrained
  - No styling properties exist
        ↓
Compilation succeeds or fails
        ↓
If it compiles → guaranteed valid output
```

---

## Extension Points (Controlled)

When the vocabulary genuinely lacks something, extension happens through:

1. **New type definition** — Added to the vocabulary by framework maintainers
2. **New interface implementation** — Element added to a content category
3. **New enum value** — New semantic option

Extension does NOT happen through:
- `className` or `style` parameters
- Generic wrapper components
- Raw CSS injection
- "Escape hatch" patterns

The vocabulary grows deliberately. Individual developers cannot circumvent it.

---

## Analyzer Rules (Defense in Depth)

Beyond the type system, Roslyn analyzers provide additional enforcement:

| Rule | Catches |
|------|---------|
| No raw HTML strings | `var html = "<div style='...'>"` |
| No inline styles | Any `style=` in generated output |
| No class manipulation | Direct DOM class modification |

These catch attempts to work around the type system through string manipulation.

---

## Summary

| Mechanism | What It Enforces |
|-----------|------------------|
| **Required properties** | Slots that must be filled |
| **Interface constraints** | What content goes where |
| **Enum parameters** | Finite, semantic options |
| **Absence of properties** | No styling escape hatches |
| **Analyzers** | No string-based workarounds |

The developer's only choice is: **what IS this?**

The system handles everything else.
