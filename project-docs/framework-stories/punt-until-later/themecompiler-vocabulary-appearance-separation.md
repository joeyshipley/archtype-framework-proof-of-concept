# Experiment: ThemeCompiler Vocabulary vs Appearance Separation

**Status:** Not Started
**Started:** 2026-01-10
**Goal:** Refactor ThemeCompiler to cleanly separate structural CSS (vocabulary) from appearance CSS (theme-controlled)

---

## Problem Statement

The ThemeCompiler currently has 131+ hardcoded fallback values mixed throughout. This violates our core principle:

> "If a designer needs to edit C# code to change how something looks, that's a bug."

**Current state:**
```csharp
getPropertyOrDefault(button, "base.padding-y", "padding-y", "var(--spacing-sm)")
```

If the theme doesn't specify `padding-y`, C# silently provides `var(--spacing-sm)`. This means:
- Theme isn't the single source of truth
- Designer doesn't know what's customizable
- Two places define the same value

---

## Design Principle

Not all CSS is appearance. Some CSS defines **what an element IS** (vocabulary), not how it looks.

| Category | Description | Owner | Example |
|----------|-------------|-------|---------|
| **Vocabulary** | Structural CSS that defines element behavior | C# (hardcoded OK) | `display: flex` for Stack |
| **Appearance** | Visual CSS that defines how it looks | Theme YAML | `gap: var(--spacing-md)` |

**Examples:**

| Property | Category | Reasoning |
|----------|----------|-----------|
| `display: flex` on Stack | Vocabulary | A Stack IS a flex column - that's its definition |
| `flex-direction: column` on Stack | Vocabulary | Structural behavior |
| `gap: var(--spacing-md)` on Stack | Appearance | How much space is visual choice |
| `background: var(--color-surface)` | Appearance | Color is always visual |
| `border-radius: var(--radius-lg)` | Appearance | Shape is always visual |
| `padding: var(--spacing-lg)` | Appearance | Spacing is always visual |
| `font-size: var(--text-md)` | Appearance | Typography is always visual |

---

## Scope

### Files to Modify

| File | Changes |
|------|---------|
| `ThemeCompiler.cs` | Categorize and clean up all property calls |
| `default.theme.yaml` | Ensure all appearance values are present |

### Generator Methods to Audit

| Method | Est. Structural | Est. Appearance | Priority |
|--------|-----------------|-----------------|----------|
| `generatePageStructureStyles` | High | Low | 1 |
| `generateLayoutStyles` | High | Medium | 2 |
| `generateCardStyles` | Low | High | 3 |
| `generateButtonStyles` | Low | High | 4 |
| `generateTextStyles` | Low | High | 5 |
| `generateFormStyles` | Low | High | 6 |
| `generateFeedbackStyles` | Low | High | 7 |
| `generateListStyles` | Medium | High | 8 |
| `generateTokensLayer` | N/A | N/A | Skip |
| `generateBaseLayer` | Medium | Low | 9 |
| `generateHtmxStyles` | High | Low | 10 |
| `generateComponentsLayer` | N/A | N/A | Skip (routing) |

---

## Tasks

### Task 1: generatePageStructureStyles

**Current property calls:** ~TBD
**Expected outcome:** Structural properties hardcoded, appearance from theme

**Structural (keep in C#):**
- `display` values
- `flex-direction`
- `box-sizing`

**Appearance (must come from theme):**
- `max-width`
- `padding`
- `margin`
- `background`

**Acceptance Criteria:**
- [ ] All structural CSS is explicit in C# (no fallback mechanism needed)
- [ ] All appearance CSS reads from theme (fails if missing, or theme is complete)
- [ ] Generated CSS unchanged (refactor, not behavior change)

---

### Task 2: generateLayoutStyles

**Elements:** Stack, Row, Grid

**Structural (keep in C#):**
- Stack: `display: flex; flex-direction: column`
- Row: `display: flex; flex-direction: row; align-items: center`
- Grid: `display: grid`

**Appearance (must come from theme):**
- `gap` values for each purpose
- Grid `grid-template-columns`

**Acceptance Criteria:**
- [ ] Stack/Row/Grid structural CSS hardcoded
- [ ] Gap values come from theme for each `For.*` purpose
- [ ] Generated CSS unchanged

---

### Task 3: generateCardStyles

**Structural:**
- `display: flex; flex-direction: column` (card is a flex container)

**Appearance:**
- `background`, `border`, `border-radius`, `box-shadow`
- Header/body/footer padding
- Header/footer borders

**Acceptance Criteria:**
- [ ] Card flex structure hardcoded
- [ ] All visual properties from theme
- [ ] Generated CSS unchanged

---

### Task 4: generateButtonStyles

**Structural:**
- `display: inline-flex; align-items: center; justify-content: center`
- `cursor: pointer`

**Appearance:**
- Colors (background, text, border)
- Spacing (padding)
- Typography (font-size, font-weight)
- Effects (border-radius, transitions, focus ring)

**Acceptance Criteria:**
- [ ] Button flex/cursor hardcoded
- [ ] All visual properties from theme
- [ ] Generated CSS unchanged

---

### Task 5: generateTextStyles

**Structural:**
- Minimal (text is just a `<p>`)

**Appearance:**
- `font-size`, `color`, `line-height`

**Acceptance Criteria:**
- [ ] All properties from theme
- [ ] Generated CSS unchanged

---

### Task 6: generateFormStyles

**Structural:**
- Input: `display: block`
- Label: `display: block`
- Checkbox: `appearance: none` (for custom styling)

**Appearance:**
- All spacing, colors, typography, focus rings

**Acceptance Criteria:**
- [ ] Structural CSS hardcoded
- [ ] All visual properties from theme
- [ ] Generated CSS unchanged

---

### Task 7: generateFeedbackStyles

**Elements:** Alert, EmptyState

**Structural:**
- `role="alert"` is HTML, not CSS
- Minimal structural CSS

**Appearance:**
- All colors, spacing, typography per tone

**Acceptance Criteria:**
- [ ] All properties from theme
- [ ] Generated CSS unchanged

---

### Task 8: generateListStyles

**Structural:**
- List types (ul vs ol based on style)

**Appearance:**
- Spacing, colors, state indicators

**Acceptance Criteria:**
- [ ] List structure in C# (vocabulary)
- [ ] Visual properties from theme
- [ ] Generated CSS unchanged

---

## Implementation Approach

For each method:

1. **Audit** - List all property calls
2. **Categorize** - Mark each as Structural or Appearance
3. **For Structural** - Replace `getPropertyOrDefault` with hardcoded value
4. **For Appearance** - Verify theme has the value, simplify to `getProperty` (no fallback)
5. **Test** - Build, compare generated CSS (should be identical)
6. **Commit** - One commit per method for easy review

---

## Current Status

**Active Task:** Not started
**Blockers:** None
**Completed:** None

---

## Notes & Decisions

### Session 1 (2026-01-10)

**Context:**
- Discovered during Flowbite styling work that ThemeCompiler has 131+ hardcoded fallbacks
- Discussed three options: (1) make theme comprehensive, (2) warning mode, (3) categorize
- Chose Option 3 - categorize as vocabulary vs appearance
- Fits belief system: 4/5 - recognizes that some CSS IS vocabulary (what a Stack is)

**Key insight:**
`display: flex` for a Stack isn't "appearance" - it's the definition of what a Stack IS. A designer shouldn't change that through theme - they'd be changing the vocabulary. But `gap` between items IS appearance - that's a visual choice.

---

## Success Criteria

This experiment is considered successful when:

- [ ] All generator methods audited and categorized
- [ ] Structural CSS is explicit in C# (no fallback mechanism)
- [ ] Appearance CSS comes from theme (theme is comprehensive)
- [ ] Generated CSS is identical before/after (pure refactor)
- [ ] Clear documentation of what's structural vs appearance
- [ ] Principle maintained: "Designer controls appearance via YAML only"

---

**Last Updated:** 2026-01-10
**Document Version:** 1.0
