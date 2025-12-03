# Closed-World UI: Vocabulary

The complete set of semantic types available to developers.

---

## Vocabulary Principles

1. **Finite** — A bounded set of named elements
2. **Semantic** — Names describe what something IS, not how it looks
3. **Complete** — Covers all common UI patterns
4. **Composable** — Elements combine in predictable ways

---

## Page Structure

Top-level organization of views.

| Element | Purpose | Example |
|---------|---------|---------|
| `Page` | Root container for a view | Dashboard, Settings screen |
| `Section` | Major division of a page | "Recent Activity", "Statistics" |
| `PageTitle` | H1-level page identity | "Dashboard" |
| `SectionTitle` | H2-level section identity | "Recent Activity" |

```csharp
new Page {
    new PageTitle { "Dashboard" },
    new Section {
        new SectionTitle { "Recent Activity" },
        // section content
    }
}
```

---

## Containers

Bounded visual units with internal structure.

| Container | Slots | Required Slots | Use Case |
|-----------|-------|----------------|----------|
| `Card` | Header, Media, Body, Footer | Body | Content grouping |
| `Modal` | Header, Body, Footer | Header, Body | Overlay dialogs |
| `Dialog` | Header, Body, Footer | Header, Body, Footer | Confirmation prompts |
| `Alert` | Header, Body | Body | System messages |
| `Panel` | Header, Body, Footer | Body | Sidebar content |
| `Sheet` | Header, Body, Footer | Body | Bottom/side drawers |

```csharp
new Card {
    Header = new Header { "Title", new Badge { "New" } },
    Media = new Media { new Image(src) },
    Body = new Body { new Text { "Description" } },
    Footer = new Footer { new Button(Importance.Primary) { "Action" } }
}
```

---

## Slots

Typed regions within containers.

| Slot | Purpose | Accepts |
|------|---------|---------|
| `Header` | Top region, title/identity | Text, Icon, Badge, Button |
| `Media` | Visual content region | Image, Video, Avatar |
| `Body` | Main content area | Flow content, layout primitives |
| `Footer` | Bottom region, actions | Button, Link, Text |

Slots are universal — `Header` is `Header` everywhere. The theme controls how `Header` appears in each container context.

---

## Layout Primitives

Spatial arrangement with semantic purpose.

| Element | Purpose | Parameters |
|---------|---------|------------|
| `Stack` | Vertical arrangement | `For` (purpose) |
| `Row` | Horizontal arrangement | `For` (purpose) |
| `Grid` | Two-dimensional layout | `For` (purpose), `Columns` |

### The `For` Enum

Layout spacing is purpose-based, not size-based.

| Value | Purpose | Typical Use |
|-------|---------|-------------|
| `For.Actions` | Tight grouping | Button groups, toolbars |
| `For.Fields` | Comfortable spacing | Form inputs |
| `For.Content` | Readable flow | Paragraphs, prose |
| `For.Items` | Consistent rhythm | List items, repeated elements |
| `For.Sections` | Generous separation | Major page divisions |
| `For.Inline` | Minimal gap | Icon + label pairs |

```csharp
new Stack(For.Fields) {
    new Input(InputType.Text, Placeholder: "Name"),
    new Input(InputType.Email, Placeholder: "Email"),
    new Button(Importance.Primary) { "Submit" }
}
```

### Grid Columns

| Value | Meaning |
|-------|---------|
| `Columns.One` | Single column |
| `Columns.Two` | Two columns |
| `Columns.Three` | Three columns |
| `Columns.Four` | Four columns |
| `Columns.Auto` | Responsive, as many as fit |

```csharp
new Grid(For.Cards, Columns.Three) {
    new Card { ... },
    new Card { ... },
    new Card { ... }
}
```

---

## Text Elements

Typography with semantic meaning.

| Element | Purpose | Parameters |
|---------|---------|------------|
| `PageTitle` | Page-level heading | — |
| `SectionTitle` | Section-level heading | — |
| `ItemTitle` | Title within repeated items | — |
| `Text` | Body text | `Tone`, `Weight` |
| `Caption` | Supporting/secondary text | `Tone` |
| `Label` | Form field labels | `Required` |
| `Code` | Monospace/code text | — |
| `Small` | De-emphasized text | — |

### Text Tone

| Value | Meaning |
|-------|---------|
| `Tone.Default` | Standard text |
| `Tone.Muted` | De-emphasized |
| `Tone.Accent` | Highlighted |
| `Tone.Positive` | Success/good |
| `Tone.Warning` | Caution |
| `Tone.Critical` | Error/danger |

### Text Weight

| Value | Meaning |
|-------|---------|
| `Weight.Normal` | Regular weight |
| `Weight.Medium` | Slightly emphasized |
| `Weight.Semibold` | Moderately emphasized |
| `Weight.Bold` | Strongly emphasized |

---

## Interactive Elements

Elements users act upon.

### Button

```csharp
public record Button(
    Importance Importance = Importance.Secondary,
    Tone Tone = Tone.Default,
    bool Disabled = false,
    bool Loading = false
);
```

| `Importance` | Meaning |
|--------------|---------|
| `Primary` | Main action |
| `Secondary` | Supporting action |
| `Tertiary` | Minor action |
| `Ghost` | De-emphasized action |

Button children: text, or Icon + text.

```csharp
new Button(Importance.Primary) { "Save" }
new Button(Importance.Secondary) { new Icon(Icons.Download), "Export" }
```

### Link

```csharp
public record Link(string Href, bool External = false);
```

```csharp
new Link("/settings") { "Settings" }
new Link("https://example.com", External: true) { "Learn more" }
```

### Input

```csharp
public record Input(
    InputType Type = InputType.Text,
    string? Placeholder = null,
    bool Disabled = false,
    bool Required = false,
    ValidationState Validation = ValidationState.None
);
```

| `InputType` | Purpose |
|-------------|---------|
| `Text` | General text |
| `Email` | Email addresses |
| `Password` | Hidden input |
| `Number` | Numeric values |
| `Search` | Search queries |
| `Tel` | Phone numbers |
| `Url` | Web addresses |

| `ValidationState` | Meaning |
|-------------------|---------|
| `None` | Not validated |
| `Valid` | Passed validation |
| `Invalid` | Failed validation |

### Select

```csharp
public record Select(bool Disabled = false, bool Required = false);
public record Option(string Value, bool Selected = false);
```

```csharp
new Select {
    new Option("us") { "United States" },
    new Option("ca") { "Canada" },
    new Option("mx") { "Mexico" }
}
```

### Toggle, Checkbox, Radio

```csharp
public record Toggle(bool Checked = false, bool Disabled = false);
public record Checkbox(bool Checked = false, bool Disabled = false);
public record Radio(string Group, bool Checked = false, bool Disabled = false);
```

---

## Feedback Elements

System communication to users.

| Element | Purpose | Parameters |
|---------|---------|------------|
| `Badge` | Status indicators | `Tone` |
| `Progress` | Completion status | `Value`, `Max` |
| `Spinner` | Loading state | — |
| `Skeleton` | Content placeholder | — |

```csharp
new Badge(Tone.Positive) { "Active" }
new Progress(Value: 75, Max: 100)
new Spinner()
new Skeleton()
```

---

## Media Elements

Visual content.

| Element | Purpose | Parameters |
|---------|---------|------------|
| `Image` | Pictures | `Src`, `Alt` |
| `Avatar` | User/entity images | `Src`, `Alt`, `Size` |
| `Icon` | Symbolic icons | `Type` |

Avatar Size:

| Value | Meaning |
|-------|---------|
| `AvatarSize.Sm` | Small (lists, inline) |
| `AvatarSize.Md` | Medium (cards) |
| `AvatarSize.Lg` | Large (profiles) |

```csharp
new Image("/photo.jpg", Alt: "Description")
new Avatar("/user.jpg", Alt: "Jane Doe", Size: AvatarSize.Md)
new Icon(Icons.Settings)
```

---

## Navigation Elements

Wayfinding patterns.

| Element | Purpose |
|---------|---------|
| `Nav` | Navigation container |
| `NavItem` | Navigation link |
| `Tabs` | Tab container |
| `Tab` | Individual tab |
| `Breadcrumb` | Path trail |
| `BreadcrumbItem` | Path segment |

```csharp
new Nav {
    new NavItem("/dashboard", Active: true) { "Dashboard" },
    new NavItem("/settings") { "Settings" },
    new NavItem("/help") { "Help" }
}

new Tabs {
    new Tab("overview", Active: true) { "Overview" },
    new Tab("details") { "Details" },
    new Tab("history") { "History" }
}
```

---

## What Does NOT Exist

The following concepts intentionally do not exist in the vocabulary:

| Concept | Why Not |
|---------|---------|
| `className` | Escape hatch |
| `style` | Escape hatch |
| `padding`, `margin` | Appearance property |
| `fontSize`, `color` | Appearance property |
| `Size.Sm/Md/Lg` on most elements | Context-derived |
| Generic `<div>`, `<span>` | Non-semantic |
| Arbitrary wrapper | Escape hatch |

If a developer needs something not in this vocabulary, the answer is vocabulary extension through a controlled process — not an escape hatch.
