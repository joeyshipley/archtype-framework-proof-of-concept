# Closed-World UI Specification

> **HISTORICAL DOCUMENTS**
>
> This specification (B-00 through B-05) was written during the initial design phase and has been **consolidated** into the main styling documentation.
>
> **Current documentation:** See `.claude/docs/README.DESIGN_STYLING.md`
>
> These files are preserved for historical context showing the design evolution:
> - B-00_README.md - This overview
> - B-01_philosophy.md - Core beliefs (consolidated)
> - B-02_vocabulary.md - Semantic types (consolidated)
> - B-03_type-system.md - C# enforcement (consolidated)
> - B-04_theme-authoring.md - Designer control (consolidated)
> - B-05_rendering.md - HTML/CSS output (consolidated)
>
> **Key insight preserved:** "Invalid output is impossible, not discouraged."

A styling framework where developers declare what things ARE, and the system derives how they look.

---

## Documents

| # | Document | Purpose |
|---|----------|---------|
| 00 | [Philosophy](00-philosophy.md) | Core beliefs, the inversion, why this exists |
| 01 | [Vocabulary](01-vocabulary.md) | Complete set of semantic types |
| 02 | [Type System](02-type-system.md) | How C# enforces valid combinations |
| 03 | [Theme Authoring](03-theme-authoring.md) | Designer-owned appearance control |
| 04 | [Rendering](04-rendering.md) | How types become HTML + CSS |

---

## The One-Sentence Summary

> **Invalid output is impossible, not discouraged.**

---

## Key Decisions Made

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Escape hatches | None | The whole point |
| Sizing parameters | Context-derived | Developers don't choose sizes |
| Layout spacing | Purpose-based (`For.Fields`) | Semantic, not pixel values |
| Container titles | Slot-based | `Header` is universal, containers define slots |
| Theme organization | Container-centric | Units that change together live together |
| CSS context | Descendant selectors | Clean HTML, CSS handles context |
| Hot reloading | Not in scope | Server-rendered, rebuild to update |
| Extension process | Not in scope | Vocabulary is complete for experiment |

---

## What Developers Write

```csharp
new Page {
    new PageTitle { "Dashboard" },
    new Section {
        new SectionTitle { "Recent Activity" },
        new Card {
            Header = new Header { "Updates", new Badge(Tone.Positive) { "3" } },
            Body = new Body {
                new Stack(For.Items) {
                    new Text { "New comment on your post" },
                    new Text { "Profile view from Seattle" },
                    new Text { "Download completed" }
                }
            },
            Footer = new Footer {
                new Button(Importance.Secondary) { "View All" }
            }
        }
    }
}
```

## What Developers Never Write

```csharp
// None of these exist
className: "my-class"
style: "padding: 16px"
Size: ButtonSize.Large
Gap: Spacing.Md
padding: 4
color: "#333"
```

---

## What Designers Control

```yaml
card:
  base:
    background: surface
    shadow: sm
    radius: md
  header:
    size: md
    padding: 4
  body:
    padding: 4
  footer:
    padding: 4
    gap: 2
  button:
    size: sm
```

Complete visual control. Zero code involvement.

---

## Status

This specification represents the conceptual model. Implementation requires:

1. **C# type definitions** for the vocabulary
2. **Rendering engine** that outputs HTML
3. **Theme compiler** that generates CSS
4. **Build integration** for the target framework

---

## Open Questions for Implementation

- Composition syntax (constructors vs fluent vs collection initializers)
- Theme file format (YAML vs custom DSL vs visual editor)
- CSS generation strategy (pre-compiled vs usage-analyzed)
- Accessibility defaults (ARIA attributes, focus management)
