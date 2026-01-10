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
| `Tabs` | ✅ Complete | Tab container with Underline/Boxed/Pill styles |
| `Tab` | ✅ Complete | Individual tab items with content slots |
| `Badge` | ⬜ Not started | Small labels (counts, status indicators) |
| `TrendIndicator` | ⬜ Not started | ↑12.5% style positive/negative indicators |
| `Sidebar` / `Nav` | ⬜ Not started | Left navigation with icons, expandable sections |
| `NavItem` | ⬜ Not started | Navigation items with icon + label + chevron |
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

### Task 1.5: List Styling ✅

**Elements:** `List`, `ListItem`

**Target (Flowbite observations):**
- Product list items have consistent spacing
- Subtle dividers between items
- Completed/disabled states subdued with muted text color

**Acceptance Criteria:**
- ✅ List items have appropriate padding (md instead of sm)
- ✅ States (completed, disabled, error) visible with proper colors
- ✅ Plain list style works for dashboard lists with dividers

**Changes Made:**
1. **default.theme.yaml** - Updated list-item section:
   - Increased `padding-y` from `sm` to `md` for Flowbite density
   - Added explicit `color: text-primary` for base list items
   - Added `border-bottom: border` for subtle dividers between items
   - Added `last-child.border-bottom: none` to remove border on last item
   - Updated `state-completed` to use `color: text-secondary` (not just opacity)
   - Updated `state-disabled` to use `color: text-secondary` + opacity

2. **ThemeCompiler.cs** - Updated generateListStyles():
   - Added support for `color` property on list items
   - Added support for `border-bottom` property for dividers
   - Added `:last-child` pseudo-selector for removing bottom border
   - Updated state variants to use explicit color properties

3. **Home.Page.cs** - Added List showcase section:
   - Plain list (dashboard style) with dividers
   - List item states (normal, completed, disabled, error)
   - Unordered list with bullets
   - Ordered list with numbers

---

### Task 1.6: Alert Styling ✅

**Element:** `Alert`

**Target:**
- Matches Flowbite notification/alert patterns
- Four tones clearly distinguished
- Appropriate icon space (even if no icons yet)

**Acceptance Criteria:**
- ✅ All four tones styled appropriately
- ✅ Positive/Warning/Critical use correct colors
- ✅ Neutral is subtle but visible

**Changes Made:**
1. **default.theme.yaml** - Updated alert section:
   - Changed `radius` from `md` to `lg` (8px) to match cards/buttons
   - Added `size: sm` to message for slightly smaller text (Flowbite style)
   - Added `weight: medium` to message for better readability

2. **ThemeCompiler.cs** - Updated generateFeedbackStyles():
   - Added `font-weight` generation for alert messages

3. **Home.Page.cs** - Added Alert showcase section:
   - All four tones: Neutral, Positive, Warning, Critical
   - Displayed in a Stack with appropriate spacing

---

### Task 1.7: Layout Spacing ✅

**Elements:** `Stack`, `Row`, `Grid`, `Page`, `Section`

**Target:**
- Purpose-based gaps feel right for Flowbite density
- Page max-width and padding appropriate
- Grid columns work for dashboard layouts

**Acceptance Criteria:**
- ✅ `For.Actions` creates tight button groups
- ✅ `For.Fields` creates comfortable form spacing
- ✅ `For.Cards` creates appropriate card grid gaps
- ✅ Page container width/padding feels right

**Changes Made:**
1. Simplified Home.Page.cs layout showcase to use Cards as visual blocks
2. Removed verbose descriptive text - now uses single-word labels (Inline, Actions, Items, etc.)
3. Visual progression clearly shows gap sizes increasing: Inline → Actions → Items → Fields → Cards
4. Grid showcase simplified to show Two/Three/Four/Auto column layouts with card blocks

---

## Phase 2: New Vocabulary Elements

### Task 2.1: Tabs/Tab ✅

**Elements:** `Tabs`, `Tab`, `TabStyle` enum

**Target (Flowbite observations):**
- Underline style most common (colored bottom border on active tab)
- Triggers in horizontal row, content below
- Clear active/inactive visual distinction
- Subtle hover states

**Files Created/Modified:**
1. `Infrastructure/UI/Vocabulary/TabElements.cs` - NEW
2. `Infrastructure/UI/Rendering/HtmlRenderer.cs` - Added tab rendering
3. `Infrastructure/UI/Themes/default.theme.yaml` - Added tabs configuration
4. `Infrastructure/UI/Rendering/ThemeCompiler.cs` - Added generateTabsStyles()
5. `Pages/Home/Home.Page.cs` - Added Tabs showcase

**Implementation Details:**

**C# Vocabulary:**
```csharp
public enum TabStyle { Underline, Boxed, Pill }

public record Tabs : ElementBase, IBodyContent
{
    public TabStyle ElementStyle { get; init; } = TabStyle.Underline;
    public string ElementId { get; init; }
    internal List<Tab> _tabs { get; init; } = new();

    public Tabs Style(TabStyle style) => this with { ElementStyle = style };
    public Tabs Id(string id) => this with { ElementId = id };
    public Tabs Tab(Tab tab) => this with { _tabs = _tabs.Append(tab).ToList() };
}

public record Tab : IElement
{
    public string Label { get; }
    public bool ElementActive { get; init; }
    public string ElementId { get; init; }
    // HTMX support
    public string ElementAction { get; init; }
    public string ElementTarget { get; init; }
    public SwapStrategy ElementSwap { get; init; } = SwapStrategy.OuterHTML;
    internal TabContent _content { get; init; }

    public Tab Active(bool active = true) => this with { ElementActive = active };
    public Tab Content(params IBodyContent[] content) => ...
}
```

**HTML Output:**
```html
<div class="tabs tabs--underline" id="...">
  <div class="tabs__triggers" role="tablist">
    <button class="tabs__trigger tabs__trigger--active" role="tab" aria-selected="true">...</button>
    <button class="tabs__trigger" role="tab" aria-selected="false">...</button>
  </div>
  <div class="tabs__panels">
    <div class="tabs__panel tabs__panel--active" role="tabpanel">...</div>
    <div class="tabs__panel" role="tabpanel" hidden>...</div>
  </div>
</div>
```

**CSS Classes (BEM):**
- `.tabs` - Container
- `.tabs--underline` / `.tabs--boxed` / `.tabs--pill` - Style variants
- `.tabs__triggers` - Trigger button container
- `.tabs__trigger` - Individual trigger
- `.tabs__trigger--active` - Active state
- `.tabs__panels` - Content panels container
- `.tabs__panel` / `.tabs__panel--active` - Content panels

**Features:**
- Three style variants (Underline, Boxed, Pill)
- Server-authoritative active state (no client-side JS)
- HTMX-ready for server-driven tab switching
- Accessible (ARIA roles: tablist, tab, tabpanel, aria-selected)
- Theme-controlled appearance via YAML

**Acceptance Criteria:**
- ✅ Underline tabs match Flowbite style (blue underline on active)
- ✅ Boxed and Pill variants provide alternatives
- ✅ Inactive tabs muted, hover shows interaction
- ✅ Focus ring consistent with buttons
- ✅ Content panels show/hide correctly

---

### Task 2.2: Badge

*Not started*

Priority order (remaining):
1. `Badge` - Counts, status indicators
2. `TrendIndicator` - ↑12.5% patterns
3. `Sidebar` / `NavItem` - Navigation structure
4. `Avatar` - User profile
5. `Icon` - Icon system

---

## Current Status

**Active Phase:** Phase 2 - New Vocabulary Elements
**Next Task:** Task 2.2 - Badge
**Blockers:** None
**Completed:** All Phase 1 tasks (1.1-1.7), Task 2.1 (Tabs/Tab)

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

### Session 6 (2026-01-10)

**Completed:** Task 1.5 - List Styling

**Changes made:**
1. **default.theme.yaml** - Updated list-item section:
   - Increased `padding-y` from `sm` to `md` for better Flowbite-style density
   - Added explicit `color: text-primary` for base list items
   - Added `border-bottom: border` for subtle dividers between items (Flowbite pattern)
   - Added `last-child.border-bottom: none` to remove border on last item
   - Updated `state-completed` to use `color: text-secondary` (clearer than just opacity)
   - Updated `state-disabled` to use `color: text-secondary` + `opacity: subdued`

2. **ThemeCompiler.cs** - Updated generateListStyles():
   - Added support for `color` property on list items (base and state variants)
   - Added support for `border-bottom` property for dividers
   - Added `:last-child` pseudo-selector support for removing bottom border
   - Updated state variant generation to use explicit color properties

3. **Home.Page.cs** - Added List showcase section:
   - Plain list (dashboard style) showing dividers
   - List item states (normal, completed, disabled, error)
   - Unordered list with bullets
   - Ordered list with numbers

**Key insight:**
Flowbite dashboard lists use subtle dividers between items rather than relying purely on spacing. The `:last-child` pseudo-selector removes the bottom border to prevent a double-border when lists are inside cards. Using explicit `color` for states (rather than just `opacity`) provides better visual distinction - completed items are muted gray rather than just transparent.

**Generated CSS result:**
```css
.list-item {
  padding: var(--spacing-md) 0px;
  color: var(--color-text-primary);
  border-bottom: 1px solid var(--color-border);
}
.list-item:last-child {
  border-bottom: none;
}
.list-item--completed {
  color: var(--color-text-secondary);
  text-decoration: line-through;
}
```

### Session 7 (2026-01-10)

**Completed:** Task 1.6 - Alert Styling

**Analysis:**
Alert styling was already mostly in place with good Flowbite-aligned colors (subtle backgrounds, tone-matched borders, dark text). The main refinements were:
- Increasing border-radius from `md` (6px) to `lg` (8px) for consistency with cards/buttons
- Using smaller text (`sm` = 14px) which is more typical for Flowbite alerts
- Adding medium font weight for better readability

**Changes made:**
1. **default.theme.yaml** - Updated alert configuration:
   - `radius: lg` (matches cards/buttons)
   - `message.size: sm` (14px - Flowbite style)
   - `message.weight: medium` (500 - better readability)

2. **ThemeCompiler.cs** - Added font-weight support to alert message generation

3. **Home.Page.cs** - Added Alert showcase section with all four tones

**Key insight:**
The existing theme tokens (positive-subtle, warning-subtle, critical-subtle for backgrounds; positive-dark, warning-dark, critical-dark for text) already followed Flowbite patterns. The alert system was well-designed from the start - just needed minor tweaks for consistency with other components (radius) and Flowbite density (smaller text).

**Generated CSS result:**
```css
.alert {
  padding: var(--spacing-md) var(--spacing-lg);
  border-radius: var(--radius-lg);
  border-width: 1px;
  border-style: solid;
}
.alert__message {
  margin: 0px;
  font-size: var(--text-sm);
  font-weight: var(--weight-medium);
}
.alert--positive {
  background: var(--color-positive-subtle);
  border-color: var(--color-positive);
  color: var(--color-positive-dark);
}
/* ... similar for warning, critical, neutral */
```

### Session 8 (2026-01-10)

**Completed:** Task 1.7 - Layout Spacing (Phase 1 Complete!)

**Changes made:**
1. **Home.Page.cs** - Simplified layout spacing showcase:
   - Replaced verbose text descriptions with single-word labels
   - Used Cards as visual blocks to make gaps immediately apparent
   - Row Gaps section shows progression: Inline → Actions → Items → Fields → Cards
   - Grid Columns section shows Two → Three → Four → Auto layouts
   - Much easier to visually verify spacing at a glance

**Key insight:**
The original showcase had too much explanatory text (e.g., "For.Actions (sm = 8px) - Button toolbar:") which made it hard to quickly assess spacing visually. Using Cards with borders as placeholder blocks and minimal labels makes the gaps between elements immediately obvious. This is a better pattern for visual QA of spacing/layout work.

**Phase 1 Complete:**
All existing vocabulary elements now styled to match Flowbite aesthetic:
- Cards: Border-based (not shadow), lg radius
- Buttons: Focus rings, proper hover states
- Forms: Placeholder styling, checkbox sizing
- Typography: Line-height tokens, proper hierarchy
- Lists: Dividers, state colors
- Alerts: Consistent radius, proper tone colors
- Layout: Purpose-based gaps verified visually

### Session 9 (2026-01-10)

**Completed:** Task 2.1 - Tabs/Tab (First Phase 2 element!)

**Files created/modified:**
1. `Infrastructure/UI/Vocabulary/TabElements.cs` - NEW file with `Tabs`, `Tab`, `TabContent` records and `TabStyle` enum
2. `Infrastructure/UI/Rendering/HtmlRenderer.cs` - Added `renderTabs()`, `renderTabTrigger()`, `renderTabPanel()` methods
3. `Infrastructure/UI/Themes/default.theme.yaml` - Added `tabs`, `tabs-underline`, `tabs-boxed`, `tabs-pill` configurations
4. `Infrastructure/UI/Rendering/ThemeCompiler.cs` - Added `generateTabsStyles()` method (~150 lines)
5. `Pages/Home/Home.Page.cs` - Added Tabs showcase section with all three styles

**Design decisions:**
1. **Server-authoritative state:** Active tab is controlled via `.Active()` method on `Tab`. No client-side JavaScript for tab switching - follows PagePlay's HTTP-first philosophy.

2. **HTMX-ready:** Tabs support optional `.Action()`, `.Target()`, `.Swap()` for server-driven tab switching. When HTMX action is set, only the active panel is rendered (reduces payload).

3. **Three style variants:**
   - `Underline` (default) - Flowbite common style, colored bottom border on active
   - `Boxed` - Bordered tab triggers, white background on active
   - `Pill` - Rounded pill shape, colored background on active

4. **BEM class naming:** `.tabs`, `.tabs__trigger`, `.tabs__trigger--active`, `.tabs__panel` - consistent with existing patterns.

5. **Accessibility:** ARIA roles (`tablist`, `tab`, `tabpanel`) and `aria-selected` attribute for screen readers.

6. **Internal content slot:** `TabContent` is an internal record to prevent orphan content elements. Content is set via fluent `.Content()` method.

**Key insight:**
The implementation follows the same patterns established in Phase 1:
- C# records with `init` properties and fluent methods
- Internal slots for composition (like Card's `_headerSlot`)
- Enum variants for style variations
- Theme YAML controls all appearance
- ThemeCompiler generates CSS with proper token resolution

**Generated CSS includes:**
- Base container (flex column)
- Trigger container (flex row with border-bottom)
- Trigger buttons (transparent, muted color, hover/focus states)
- Active trigger (accent color, underline/background depending on style)
- Style variant overrides (boxed borders, pill radius)
- Panel visibility (hidden attribute support)

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
- ✅ All existing elements visually match Flowbite aesthetic
- ✅ Token values refined if needed
- ✅ Component mappings produce clean, consistent output
- ✅ Site looks professional, not broken

**Phase 2:**
- ✅ Tabs/Tab functional with three style variants
- ⬜ Badge component added
- ⬜ TrendIndicator component added
- ⬜ Navigation/Sidebar components added
- ⬜ Full dashboard layout achievable

**Overall:**
- ✅ Designer can adjust appearance via YAML only
- ✅ No escape hatches needed
- ✅ Closed-World UI principles maintained

---

**Last Updated:** 2026-01-10
**Document Version:** 1.3
