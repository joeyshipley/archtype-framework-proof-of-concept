# Brief: Auth Page Navigation Links

**Status:** Complete
**Created:** 2026-01-10

---

## Goal

Add cross-navigation between Login and Register pages with secondary-styled link buttons and contextual text prompts.

## Problem

Users on the Login page have no easy way to navigate to Register (and vice versa). New users arriving at Login must manually find the registration path, and existing users on Register have no quick way back to Login.

## Success Signal

- [x] Login page shows "Need an account?" text with "Register" secondary button below the Login button
- [x] Register page shows "Already have an account?" text with "Login" secondary button below the Create Account button
- [x] Visual spacing separates the primary action from the navigation section
- [x] Links navigate to correct pages (`/register` and `/login`)
- [x] Secondary button style follows closed-world UI patterns (theme-controlled appearance)

## Constraints

- Must use existing Link vocabulary element (extend, not replace)
- Must follow closed-world UI principle (no escape hatches, theme controls appearance)
- CSS generation must go through ThemeCompiler (designer-controllable)

---

## Phases

| # | Goal | Status |
|---|------|--------|
| 1 | Add `ButtonSecondary` style to Link vocabulary | :white_check_mark: |
| 2 | Add navigation sections to Login and Register pages | :white_check_mark: |

---

## Phase 1: Link ButtonSecondary Style

**Goal:** Extend the Link element to support secondary button appearance

### Research
- [x] Verify LinkStyle enum location and current values
- [x] Verify HtmlRenderer link rendering logic
- [x] Verify ThemeCompiler link style generation

### Findings

**LinkElements.cs:44-49** - Current `LinkStyle` enum:
```csharp
public enum LinkStyle
{
    Default,    // Standard link
    Button,     // Primary button style
    Ghost       // Subtle link
}
```

**HtmlRenderer.cs:709-714** - Renders style class based on enum:
```csharp
var styleClass = link.ElementStyle switch
{
    LinkStyle.Default => "link",
    LinkStyle.Button => "link link--button",
    LinkStyle.Ghost => "link link--ghost",
    _ => "link"
};
```

**ThemeCompiler.cs:1446-1479** - Generates CSS for each link style variant. Pattern established for `link--button` (primary) with padding, radius, background, color.

### Adjustments
None needed - pattern is clear and consistent.

### TDD Plan

1. **Test:** LinkStyle enum has ButtonSecondary value
   **Implement:** Add `ButtonSecondary` to LinkStyle enum in `LinkElements.cs`

2. **Test:** HtmlRenderer outputs `link--button-secondary` class for ButtonSecondary style
   **Implement:** Add case to switch in `renderLink` method

3. **Test:** Generated CSS contains `.link--button-secondary` with secondary button styling
   **Implement:** Add CSS generation in `generateLinkStyles` method of ThemeCompiler

### Done
- Added `ButtonSecondary` to `LinkStyle` enum in `LinkElements.cs`
- Added `link--button-secondary` class mapping in `HtmlRenderer.cs`
- Added CSS generation for `.link--button-secondary` in `ThemeCompiler.cs` with:
  - Transparent background with border
  - Hover state with accent color border/text
- Regenerated `closed-world.css` with new styles

---

## Phase 2: Auth Page Navigation Sections

**Goal:** Add navigation sections to both Login and Register pages

### Research
- [x] Verify Login page form structure
- [x] Verify Register page form structure
- [x] Verify routes for both pages

### Findings

**Login.Page.cs:59-88** - Form in `renderLoginFormComponent()` method, Login button at line 84-85
**Register.Page.cs:59-96** - Form in `renderRegisterFormComponent()` method, Create Account button at line 92-93
**Routes:** `/login` and `/register` (confirmed in Route files)

Structure needed:
- Stack with spacing (`For.Sections` for generous separation) containing:
  - Text element with prompt
  - Link with ButtonSecondary style

### Adjustments
None needed - structure is straightforward.

### TDD Plan

1. **Test:** Login page renders "Need an account?" text
   **Implement:** Add Text element to Login page form

2. **Test:** Login page renders Register link with secondary button style pointing to /register
   **Implement:** Add Link element with ButtonSecondary style

3. **Test:** Register page renders "Already have an account?" text
   **Implement:** Add Text element to Register page form

4. **Test:** Register page renders Login link with secondary button style pointing to /login
   **Implement:** Add Link element with ButtonSecondary style

### Done
- Login page (`Login.Page.cs`): Added "Need an account?" text with Register secondary button link to `/register`
- Register page (`Register.Page.cs`): Added "Already have an account?" text with Login secondary button link to `/login`
- Both use `Stack(For.Sections)` wrapper for generous spacing between form and navigation
- Navigation sections use `Stack(For.Fields)` for text + link grouping

---

## Open Questions

None currently - scope is well-defined.

---

## Session Log

### 2026-01-10 - Session 1
- Explored existing Login/Register page structure
- Identified Link element with existing Button style
- Clarified UX: Text prompt + secondary button, with spacing from primary action
- Created brief with 2 phases

### 2026-01-10 - Session 2
- Implemented Phase 1: Added `ButtonSecondary` to LinkStyle enum, HtmlRenderer, and ThemeCompiler
- Implemented Phase 2: Added navigation sections to Login and Register pages
- Regenerated CSS with `dotnet run compile-theme` to include new styles
- All 19 tests passing, build successful
- Brief complete
