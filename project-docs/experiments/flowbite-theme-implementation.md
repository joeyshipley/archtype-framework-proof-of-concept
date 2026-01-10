# Experiment: Flowbite Theme Implementation

**Status:** In Progress
**Started:** 2026-01-10
**Goal:** Style the Closed-World UI system to match Flowbite dashboard aesthetic

---

## Reference Design

**Source:** Flowbite Admin Dashboard
**Screenshot:** `/Users/jshipley/Desktop/Screenshot 2026-01-10 at 8.46.00 AM.png`

**Visual Characteristics Observed:**
- Very clean, minimal aesthetic
- White/light gray backgrounds with subtle borders
- Blue accent color (similar to current tokens)
- Green for positive trends, red for negative
- Small, consistent border radius (~6-8px)
- Extremely subtle shadows (almost flat)
- Clear typography hierarchy
- Tight spacing scale (4/8/12/16/24px)

---

## Element Inventory

### Phase 1: Existing Vocabulary (Style Only)

Elements we already have that need styling to match Flowbite:

| Element | Status | Notes |
|---------|--------|-------|
| `Page` | ⬜ Not started | Max-width container, padding |
| `Section` | ⬜ Not started | Content grouping |
| `PageTitle` | ⬜ Not started | Large bold heading |
| `SectionTitle` | ⬜ Not started | Medium semibold heading |
| `Card` | ⬜ Not started | White bg, subtle border/shadow, rounded |
| `Header`/`Body`/`Footer` | ⬜ Not started | Card slot styling |
| `Stack` | ⬜ Not started | Vertical layout with purpose-based gaps |
| `Row` | ⬜ Not started | Horizontal layout with purpose-based gaps |
| `Grid` | ⬜ Not started | Multi-column layouts |
| `Text` | ⬜ Not started | Body text styling |
| `Button` | ⬜ Not started | Primary/Secondary/Tertiary/Ghost variants |
| `Input` | ⬜ Not started | Text inputs with focus states |
| `Label` | ⬜ Not started | Form labels |
| `Field` | ⬜ Not started | Label + Input + error grouping |
| `Form` | ⬜ Not started | Form container |
| `Checkbox` | ⬜ Not started | Boolean inputs |
| `Alert` | ⬜ Not started | Feedback messages (4 tones) |
| `EmptyState` | ⬜ Not started | No-content messaging |
| `List` | ⬜ Not started | Container for list items |
| `ListItem` | ⬜ Not started | Individual items with states |

### Phase 2: New Vocabulary (Requires C# + Styling)

Elements visible in Flowbite that we don't have yet:

| Element | Status | Notes |
|---------|--------|-------|
| `Sidebar` / `Nav` | ⬜ Not started | Left navigation with icons, expandable sections |
| `NavItem` | ⬜ Not started | Navigation items with icon + label + chevron |
| `Tabs` | ⬜ Not started | Tab container with active state |
| `Tab` | ⬜ Not started | Individual tab items |
| `Badge` | ⬜ Not started | Small labels (counts, status indicators) |
| `TrendIndicator` | ⬜ Not started | ↑12.5% style positive/negative indicators |
| `Avatar` | ⬜ Not started | User profile images (circular) |
| `Icon` | ⬜ Not started | Icon system integration |
| `SearchInput` | ⬜ Not started | Search-specific input with icon |
| `StatCard` | ⬜ Not started | Big number + label + trend (could be Card variant) |

---

## Phase 1: Style Existing Elements

### Token Adjustments

Current tokens vs Flowbite observations:

| Token | Current | Flowbite Observation | Action |
|-------|---------|---------------------|--------|
| `color.accent` | `#2563eb` | Similar blue | Keep |
| `color.surface` | `#ffffff` | White | Keep |
| `color.border` | `#e5e5e5` | Very light gray | Keep |
| `shadow.sm` | `0 1px 2px rgba(0,0,0,0.05)` | Even more subtle | Review |
| `radius.md` | `0.375rem` (~6px) | ~6-8px | Keep |
| `spacing.*` | Current scale | Appears similar | Keep |

**Observation:** Current tokens are already close to Flowbite. Main work is in component mappings.

---

### Task 1.1: Card Styling ✅

**File:** `PagePlay.Site/Infrastructure/UI/Themes/default.theme.yaml`

**Before:**
```yaml
card:
  base:
    background: surface
    shadow: sm
    radius: md
```

**After:**
```yaml
card:
  base:
    background: surface
    border: border          # Flowbite uses subtle border
    shadow: none            # Flowbite is nearly flat
    radius: lg              # Slightly more rounded (~8px)
  header:
    border-bottom: border   # Subtle separator
  footer:
    border-top: border      # Subtle separator
```

**Changes Made:**
1. Added `border` property support to ThemeCompiler for cards
2. Changed from shadow-based to border-based card styling
3. Added header/footer border support for slot separation
4. Increased radius from `md` (6px) to `lg` (8px) to match Flowbite

**Acceptance Criteria:**
- ✅ Cards match Flowbite visual style (border instead of shadow)
- ✅ Header has appropriate typography (md size, semibold weight)
- ✅ Footer actions right-aligned (justify: end)
- ✅ Consistent internal spacing (lg padding throughout)

---

### Task 1.2: Button Styling ✅

**File:** `PagePlay.Site/Infrastructure/UI/Themes/default.theme.yaml`

**Target (Flowbite observations):**
- Primary: Solid blue background, white text, rounded
- Secondary: White/transparent with border
- Hover states with subtle transitions
- Consistent padding and sizing

**Acceptance Criteria:**
- ✅ Primary button matches Flowbite blue style
- ✅ Secondary button has border styling
- ✅ Hover transitions feel smooth
- ✅ Disabled state clearly visible

**Changes Made:**
1. Added focus ring configuration (Flowbite-style blue ring on focus)
2. Made all hover backgrounds theme-controlled (removed hardcoded values)
3. Secondary button now has explicit `background: surface` (white)
4. Fixed ThemeCompiler to preserve CSS keywords like `transparent`

---

### Task 1.3: Form Elements Styling ✅

**Elements:** `Input`, `Label`, `Field`, `Checkbox`

**Target (Flowbite observations):**
- Inputs have subtle border, rounded corners
- Focus state with blue ring/border
- Labels are small, medium weight
- Clean error states

**Acceptance Criteria:**
- ✅ Input matches Flowbite text field style
- ✅ Focus states visible and consistent
- ✅ Error states use red appropriately
- ✅ Checkbox styling matches

---

### Task 1.4: Typography Styling ✅

**Elements:** `Text`, `PageTitle`, `SectionTitle`

**Target (Flowbite observations):**
- Clear hierarchy (title > section > body)
- Dark gray text, not pure black
- Appropriate weights at each level

**Acceptance Criteria:**
- ✅ PageTitle is large and bold
- ✅ SectionTitle is medium and semibold
- ✅ Body text is readable default size
- ✅ Colors match Flowbite's gray tones

**Changes Made:**
1. Added `line-height` tokens to the token system: `tight` (1.25), `normal` (1.5), `relaxed` (1.625)
2. Updated page-title and section-title to use `line-height: tight` token reference
3. Updated text to use `line-height: normal` token reference
4. Updated ThemeCompiler to generate `--line-height-*` CSS custom properties
5. Updated `getLineHeightValue()` to resolve token names to CSS variable references
6. Verified existing configuration already matched Flowbite patterns:
   - PageTitle: 2xl (24px), bold (700), text-primary (#171717), line-height: tight
   - SectionTitle: xl (20px), semibold (600), text-primary (#171717), line-height: tight
   - Text: md (16px), text-primary (#171717), line-height: normal

---

### Task 1.5: List Styling ⬜

**Elements:** `List`, `ListItem`

**Target (Flowbite observations):**
- Product list items have consistent spacing
- Hover states on interactive items
- Completed/disabled states subdued

**Acceptance Criteria:**
- ⬜ List items have appropriate padding
- ⬜ States (completed, disabled, error) visible
- ⬜ Plain list style works for dashboard lists

---

### Task 1.6: Alert Styling ⬜

**Element:** `Alert`

**Target:**
- Matches Flowbite notification/alert patterns
- Four tones clearly distinguished
- Appropriate icon space (even if no icons yet)

**Acceptance Criteria:**
- ⬜ All four tones styled appropriately
- ⬜ Positive/Warning/Critical use correct colors
- ⬜ Neutral is subtle but visible

---

### Task 1.7: Layout Spacing ⬜

**Elements:** `Stack`, `Row`, `Grid`, `Page`, `Section`

**Target:**
- Purpose-based gaps feel right for Flowbite density
- Page max-width and padding appropriate
- Grid columns work for dashboard layouts

**Acceptance Criteria:**
- ⬜ `For.Actions` creates tight button groups
- ⬜ `For.Fields` creates comfortable form spacing
- ⬜ `For.Cards` creates appropriate card grid gaps
- ⬜ Page container width/padding feels right

---

## Phase 2: New Vocabulary Elements

*To be expanded after Phase 1 is complete.*

Priority order (based on Flowbite usage):
1. `Tabs` / `Tab` - Seen in "Top products / Top Customers"
2. `Badge` - Counts, status indicators
3. `TrendIndicator` - ↑12.5% patterns
4. `Sidebar` / `NavItem` - Navigation structure
5. `Avatar` - User profile
6. `Icon` - Icon system

---

## Current Status

**Active Phase:** Phase 1 - Style Existing Elements
**Next Task:** Task 1.5 - List Styling
**Blockers:** None
**Completed:** Task 1.1 (Card Styling), Task 1.2 (Button Styling), Task 1.3 (Form Elements Styling), Task 1.4 (Typography Styling)

---

## Notes & Decisions

### Session 1 (2026-01-10)

**Context:**
- Reviewed HtmlRenderer.cs - renders semantic elements to HTML with class names
- Discussed whether to refactor to string interpolation (decided: no, keep StringBuilder)
- User chose Flowbite as reference design
- Created this tracking document

**Key observations from Flowbite screenshot:**
- Very minimal, clean aesthetic
- Almost "flat" design with subtle shadows
- Blue accent matches our current tokens well
- Tight spacing throughout
- Clear visual hierarchy

**Approach decision:**
- Focus on Phase 1 first (existing elements)
- Get the foundation right before adding new vocabulary
- Token values appear close already - main work is component mappings

### Session 2 (2026-01-10)

**Completed:** Task 1.1 - Card Styling

**Changes made:**
1. **ThemeCompiler.cs** - Added support for:
   - `border` property on cards (generates `border: 1px solid {color}`)
   - `shadow: none` to disable box-shadow
   - `border-bottom` on card headers
   - `border-top` on card footers

2. **default.theme.yaml** - Updated card section:
   - Changed from shadow-based to border-based styling
   - Added `border: border` (references `--color-border`)
   - Set `shadow: none` (Flowbite is nearly flat)
   - Changed `radius: md` to `radius: lg` (~8px vs ~6px)
   - Added header `border-bottom` and footer `border-top` for slot separation

**Key insight:**
Flowbite uses a very subtle 1px border approach rather than shadows for cards. This gives a cleaner, flatter look. The existing token values (`--color-border: #e5e5e5`) work well for this.

**Generated CSS result:**
```css
.card {
  background: var(--color-surface);
  border-radius: var(--radius-lg);
  border: 1px solid var(--color-border);
}
.card > .header {
  border-bottom: 1px solid var(--color-border);
}
.card > .footer {
  border-top: 1px solid var(--color-border);
}
```

### Session 3 (2026-01-10)

**Completed:** Task 1.2 - Button Styling

**Changes made:**
1. **default.theme.yaml** - Updated button section:
   - Added `focus` configuration with `ring-width: 4px`, `ring-color: accent`, `ring-opacity: 0.3`
   - Added explicit `background: surface` to secondary buttons (white background)
   - Added `background-hover` to secondary, tertiary, ghost variants for theme control
   - Ghost button hover background stays transparent (no visible change)

2. **ThemeCompiler.cs** - Updated button generation:
   - Added focus state CSS with Flowbite-style box-shadow ring
   - Made all hover backgrounds read from theme (removed hardcoded values)
   - Added CSS keywords preservation (`transparent`, `inherit`, `none`, etc.) to prevent them being converted to token references

**Key insight:**
Flowbite uses a blue focus ring (box-shadow) instead of browser default outline. This gives a more polished, consistent look across browsers. The ring uses the accent color with reduced opacity (0.3) for a subtle but visible effect.

**Generated CSS result:**
```css
.button:focus {
  outline: none;
  box-shadow: 0 0 0 4px rgba(37, 99, 235, 0.3);
}
.button--primary {
  background: var(--color-accent);
  color: var(--color-white);
}
.button--secondary {
  background: var(--color-surface);
  border: 1px solid var(--color-border);
  color: var(--color-text-primary);
}
.button--tertiary {
  background: transparent;
  color: var(--color-text-secondary);
}
.button--ghost {
  background: transparent;
  color: var(--color-text-secondary);
}
```

### Session 4 (2026-01-10)

**Completed:** Task 1.3 - Form Elements Styling

**Changes made:**
1. **default.theme.yaml** - Updated form elements:
   - Added `placeholder-color: text-secondary` to input base configuration
   - Checkbox size remains `lg` (semantic token)

2. **ThemeCompiler.cs** - Updated form styling generation:
   - Added placeholder styling generation (`.input::placeholder` with color and opacity: 1 for Firefox)
   - Fixed `resolvePropertyValue` to recognize `rem` and `em` values as raw CSS values (not token references)
   - Changed checkbox size to resolve to `--spacing-` tokens (for dimensions) instead of `--text-` tokens (for font-size)

**Key insight:**
The form elements were already well-styled from previous sessions. The main gaps were:
1. Missing placeholder text styling (Flowbite shows placeholder in gray)
2. Checkbox size semantic mismatch - `size` property was resolving to text tokens (`--text-lg` = 18px) when it should use spacing tokens (`--spacing-lg` = 16px) for dimensional sizing

**Design decision:** Checkbox `size` property now explicitly maps to spacing tokens since it represents width/height dimensions, not font-size. This keeps the YAML semantic (`size: lg`) while producing correct CSS.

**Generated CSS result:**
```css
.input::placeholder {
  color: var(--color-text-secondary);
  opacity: 1;
}

.checkbox {
  width: var(--spacing-lg);
  height: var(--spacing-lg);
  /* ... */
}
```

**Form elements now include:**
- Input: gray background (#fafafa), subtle border, lg radius (8px), blue focus ring + border
- Label: sm text (14px), medium weight, primary color
- Field: error state with red border + subtle red background, help/error text styling
- Checkbox: 16px size, custom appearance, blue checked state with SVG checkmark, focus ring

### Session 5 (2026-01-10)

**Completed:** Task 1.4 - Typography Styling

**Analysis:**
Typography was already well-configured from initial theme setup. The existing configuration matched Flowbite patterns:
- PageTitle: 2xl (24px), bold (700), text-primary (#171717)
- SectionTitle: xl (20px), semibold (600), text-primary (#171717)
- Text: md (16px), text-primary (#171717), line-height 1.5

**Changes made:**
1. **default.theme.yaml** - Added `line-height` token category with semantic values:
   - `tight: 1.25` - for headings
   - `normal: 1.5` - for body text
   - `relaxed: 1.625` - for spacious paragraphs
   - Updated page-title, section-title to use `line-height: tight`
   - Updated text to use `line-height: normal`

2. **ThemeCompiler.cs**:
   - Added line-height token generation in `generateTokensLayer()`
   - Updated `getLineHeightValue()` to resolve token names to CSS variables

**Key insight:**
Initially used hardcoded values (1.25), but this violated the Closed-World UI principle where designers should control all appearance through tokens. Refactored to use proper token references so designers can adjust line-height via YAML without touching code.

**Generated CSS result:**
```css
/* Tokens */
--line-height-tight: 1.25;
--line-height-normal: 1.5;
--line-height-relaxed: 1.625;

/* Components */
.text {
  font-size: var(--text-md);
  color: var(--color-text-primary);
  line-height: var(--line-height-normal);
}
.page-title {
  font-size: var(--text-2xl);
  font-weight: var(--weight-bold);
  color: var(--color-text-primary);
  line-height: var(--line-height-tight);
  margin-bottom: var(--spacing-lg);
}
.section-title {
  font-size: var(--text-xl);
  font-weight: var(--weight-semibold);
  color: var(--color-text-primary);
  line-height: var(--line-height-tight);
  margin-bottom: var(--spacing-md);
}
```

---

## Session Handoff Protocol

**When resuming work on this experiment:**

1. Read this document top-to-bottom
2. Check "Current Status" section for active phase/task
3. Review "Notes & Decisions" for context
4. Locate the next ⬜ unchecked task
5. Reference the Flowbite screenshot for visual target
6. Make changes to `default.theme.yaml`
7. Test visually in browser
8. Mark task ✅ when complete
9. Update "Current Status" section
10. Add any new decisions/learnings to "Notes & Decisions"

**Key Files:**
- Theme: `PagePlay.Site/Infrastructure/UI/Themes/default.theme.yaml`
- Renderer: `PagePlay.Site/Infrastructure/UI/Rendering/HtmlRenderer.cs`
- Compiler: `PagePlay.Site/Infrastructure/UI/Rendering/ThemeCompiler.cs`
- Generated CSS: `PagePlay.Site/wwwroot/css/closed-world.css`

---

## Success Criteria

This experiment is considered successful when:

**Phase 1:**
- ⬜ All existing elements visually match Flowbite aesthetic
- ⬜ Token values refined if needed
- ⬜ Component mappings produce clean, consistent output
- ⬜ Site looks professional, not broken

**Phase 2:**
- ⬜ New vocabulary elements added as needed
- ⬜ Navigation/Tabs/Badges functional
- ⬜ Full dashboard layout achievable

**Overall:**
- ⬜ Designer can adjust appearance via YAML only
- ⬜ No escape hatches needed
- ⬜ Closed-World UI principles maintained

---

**Last Updated:** 2026-01-10
**Document Version:** 1.2
