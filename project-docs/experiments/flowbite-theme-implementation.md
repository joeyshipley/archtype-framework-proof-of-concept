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

### Task 1.1: Card Styling ⬜

**File:** `PagePlay.Site/Infrastructure/UI/Themes/default.theme.yaml`

**Current:**
```yaml
card:
  base:
    display: flex
    flex-direction: column
    background: surface
    shadow: sm
    radius: md
```

**Target (Flowbite):**
- White background
- Very subtle shadow OR just border
- Consistent padding in slots
- Clean separation between header/body/footer

**Acceptance Criteria:**
- ⬜ Cards match Flowbite visual style
- ⬜ Header has appropriate typography
- ⬜ Footer actions right-aligned
- ⬜ Consistent internal spacing

---

### Task 1.2: Button Styling ⬜

**File:** `PagePlay.Site/Infrastructure/UI/Themes/default.theme.yaml`

**Target (Flowbite observations):**
- Primary: Solid blue background, white text, rounded
- Secondary: White/transparent with border
- Hover states with subtle transitions
- Consistent padding and sizing

**Acceptance Criteria:**
- ⬜ Primary button matches Flowbite blue style
- ⬜ Secondary button has border styling
- ⬜ Hover transitions feel smooth
- ⬜ Disabled state clearly visible

---

### Task 1.3: Form Elements Styling ⬜

**Elements:** `Input`, `Label`, `Field`, `Checkbox`

**Target (Flowbite observations):**
- Inputs have subtle border, rounded corners
- Focus state with blue ring/border
- Labels are small, medium weight
- Clean error states

**Acceptance Criteria:**
- ⬜ Input matches Flowbite text field style
- ⬜ Focus states visible and consistent
- ⬜ Error states use red appropriately
- ⬜ Checkbox styling matches

---

### Task 1.4: Typography Styling ⬜

**Elements:** `Text`, `PageTitle`, `SectionTitle`

**Target (Flowbite observations):**
- Clear hierarchy (title > section > body)
- Dark gray text, not pure black
- Appropriate weights at each level

**Acceptance Criteria:**
- ⬜ PageTitle is large and bold
- ⬜ SectionTitle is medium and semibold
- ⬜ Body text is readable default size
- ⬜ Colors match Flowbite's gray tones

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
**Next Task:** Task 1.1 - Card Styling
**Blockers:** None

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
**Document Version:** 1.0
