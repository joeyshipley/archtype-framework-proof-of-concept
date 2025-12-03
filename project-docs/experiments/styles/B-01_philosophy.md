# Closed-World UI: Philosophy

The foundational beliefs and principles behind the framework.

---

## The Core Problem

CSS was designed for styling documents. We build applications.

| Document Model | Application Model |
|----------------|-------------------|
| Single author | Multiple teams |
| Linear structure | Dynamic, nested hierarchies |
| Written once | Constantly changing |
| Global scope works | Global scope is dangerous |

Every CSS methodology attempts to retrofit predictability onto a language that was never designed for it.

---

## The Wrong Question

Every CSS approach asks:

> "How do we write CSS at scale without chaos?"

- BEM: Write CSS with disciplined naming
- CSS Modules: Write CSS but scope it automatically
- CSS-in-JS: Write CSS but in JavaScript
- Tailwind: Write CSS but with constraints

They're all still **writing CSS**. Developers making appearance decisions.

---

## The Right Question

> "Why are developers making appearance decisions at all?"

A developer's job is to build features that let users complete tasks. Their job is NOT:
- Deciding padding values
- Choosing colors
- Balancing visual hierarchy
- Ensuring consistency

We've conflated "building UI" with "styling UI." They're not the same.

---

## The Inversion

**htmx insight:** Don't write JavaScript to do what HTML does. Declare intent, let the platform handle it.

**Closed-World UI:** Don't write CSS to describe appearance. Declare what something IS, let the system derive how it looks.

---

## The Principle

> **Declare what things ARE, not how they look.**

Developer writes:
```csharp
new Card {
    Header = new Header { "Settings" },
    Body = new Body { "Configure your preferences" },
    Footer = new Footer { new Button(Importance.Primary) { "Save" } }
}
```

Developer does NOT write:
- `className: "..."`
- `style: "..."`
- `padding: 16`
- `fontSize: "lg"`

---

## The Three Requirements

### 1. The Vocabulary Must Be Complete

The system must name everything a developer might need:
- Every layout pattern
- Every container type
- Every text treatment
- Every interactive element

If something isn't in the vocabulary, developers can't build it.

### 2. The Variant Space Must Be Finite and Semantic

Developers don't choose `padding: 16px`. They choose `Density.Comfortable`.
They don't choose `background: blue`. They choose `Importance.Primary`.

A developer says `new Button(Importance.Primary)`. Never `padding="16px"`.

### 3. There Must Be No Escape Hatch

This is where every existing system fails.

- Component libraries fail: you can pass `className` or `style`
- Tailwind fails: you can write arbitrary values
- Design tokens fail: you can use raw values
- CSS Modules fail: you can write any CSS

For the inversion to work, arbitrary styling must be **impossible**, not discouraged.

---

## Why C# Is Ideal

In dynamic languages, "no escape hatches" requires runtime checks, linting, external tooling.

In C#: **The compiler is the enforcement layer.**

- If it doesn't compile, it doesn't ship
- No convincing developers to follow conventions
- No linting rules they can disable
- The type system IS the closed world

---

## The Equivalence

| Characteristic | 1000 Junior Devs | AI Code Generation |
|----------------|------------------|-------------------|
| Follows instructions precisely | ✅ | ✅ |
| Makes good judgment calls | ❌ | ❌ |
| Finds every escape hatch | ✅ | ✅ |
| Output is locally impressive | ✅ | ✅ |
| Output is globally naive | ✅ | ✅ |

The solution for junior developers and AI is the same:
- They can only use what exists
- They can't deviate if deviation is impossible
- They don't need judgment if the system made the decisions

---

## The Separation

| Domain | Owner | Concerns |
|--------|-------|----------|
| **Structure** | Developer | What IS this? What does it contain? |
| **Appearance** | Designer | How does it look? How does it feel? |

Developers own the vocabulary (semantic types).
Designers own the theme (visual mappings).
Neither crosses into the other's domain.

---

## The Bet

UI design is more constrained than people think. The patterns are finite:
- Cards, modals, forms, lists, tables, navigation
- 90% of applications use the same 50 patterns

The remaining 10% can be handled by vocabulary extension — a controlled process, not an escape hatch.

---

## Summary

| Concept | Description |
|---------|-------------|
| Core insight | Stop writing CSS. Declare what things ARE. |
| Key requirement | No escape hatches. Invalid output is impossible. |
| The mechanism | Type system enforces vocabulary |
| The separation | Developers: structure. Designers: appearance. |
| The goal | A space where all possible outputs are valid. |
