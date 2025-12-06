# Experiment: Complete YAML Theme Control

**Status:** 🟡 In Progress
**Started:** 2025-12-06
**Goal:** Enable designers to change UI appearance completely via YAML without touching .cs or .css files

---

## Problem Statement

The ThemeCompiler currently has significant hardcoded values that prevent designers from having complete control over the UI appearance through YAML alone. Analysis reveals ~50+ hardcoded CSS properties across 9 components and the entire base layer.

### Current State
- ✅ Token layer: Fully YAML-driven (spacing, colors, typography, etc.)
- ❌ Base layer: 100% hardcoded structural CSS
- ⚠️ Components layer: Partially YAML-driven with many hardcoded values
- ❌ Some YAML definitions exist but are completely ignored by compiler

---

## Research Findings

### Components with YAML Definitions That Are IGNORED

1. **text** component (YAML lines 117-121)
   - Compiler: `ThemeCompiler.cs:386-395` - Completely ignores YAML
   - Hardcoded: `line-height: 1.5`

2. **alert** component (YAML lines 181-205)
   - Compiler: `ThemeCompiler.cs:496-538` - Completely ignores YAML
   - Has complete YAML definition but uses hardcoded values for padding, border-radius, border-width, etc.

3. **empty-state** component (YAML lines 207-225)
   - Compiler: `ThemeCompiler.cs:540-583` - Completely ignores YAML
   - Has complete YAML definition but uses hardcoded text-align, display, padding, font-sizes

4. **list** component (YAML lines 228-240)
   - Compiler: `ThemeCompiler.cs:585-644` - Partially ignores YAML
   - Only reads `list-item`, completely ignores `list` base and style variants

### Components with Incomplete Helper Method Usage

5. **input** - Reads some properties, ignores:
   - `display: block`, `width: 100%`, `font-family: inherit`, `line-height: 1.5`
   - Focus state: `outline: 2px solid`, `outline-offset: 2px`
   - Disabled state: `cursor: not-allowed`

6. **label** - Reads some properties, ignores:
   - `display: block`

7. **field** - YAML exists (lines 143-161) but completely ignored
   - Hardcoded: `display: block`, `margin-bottom: var(--spacing-4)`
   - Error state colors hardcoded

8. **checkbox** - Reads some properties, ignores:
   - `cursor: pointer` (base state)

9. **list-item** - Reads some properties, ignores:
   - `text-decoration: line-through` (completed state)
   - `cursor: not-allowed` (disabled state)
   - Border and background colors (error state)

10. **page-title / section-title** - Reads some properties, ignores:
    - Margin properties

### Base Layer Issues (100% Hardcoded)

**Location:** `ThemeCompiler.cs:153-226`

Critical hardcoded values:
- `.page { max-width: 1200px; margin: 0 auto; padding: 0 var(--spacing-lg); }`
- `.section { display: block; }`
- `.stack { display: flex; flex-direction: column; }`
- `.row { display: flex; flex-direction: row; align-items: center; }`
- `.grid { display: grid; }`
- `.card { display: flex; flex-direction: column; }`
- `.footer { display: flex; align-items: center; }`
- `.button { display: inline-flex; align-items: center; justify-content: center; border: none; cursor: pointer; font-family: inherit; transition: all var(--duration-fast) ease; }`
- `.text { margin: 0; }`

### Layout Spacing Issues (100% Hardcoded)

**Location:** `ThemeCompiler.cs:677-750`

All semantic spacing mappings are hardcoded:
```csharp
.stack--actions { gap: var(--spacing-sm); }
.stack--fields { gap: var(--spacing-lg); }
.stack--content { gap: var(--spacing-md); }
// etc...
```

Grid column configurations hardcoded:
- `.grid--cols-auto { grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); }`
- The `300px` breakpoint is not themeable

---

## Implementation Plan

### Phase 1: Fix Existing YAML Definitions (High Priority) ✅ COMPLETED
**Goal:** Make all existing YAML definitions actually work

#### 1.1 Text Component ✅
- [x] Update `generateTextStyles()` to read from theme
- [x] Add support for `line-height` property via `getLineHeightValue()`
- [x] Use `getPropertyOrDefault()` for all text properties

#### 1.2 Alert Component ✅
- [x] Update `generateFeedbackStyles()` - Alert section
- [x] Read `alert.base.padding-x`, `alert.base.padding-y`
- [x] Read `alert.base.radius`
- [x] Read `alert.base.border-width`
- [x] Read `alert.message.size`, `alert.message.margin`
- [x] Read all tone variant properties from YAML

#### 1.3 Empty State Component ✅
- [x] Update `generateFeedbackStyles()` - EmptyState section
- [x] Read `empty-state.base.text-align`, `empty-state.base.color`
- [x] Read size variant properties from YAML
- [x] Read action properties from YAML
- [x] Add support for `text-align`, `text-decoration` via helper methods

#### 1.4 List Component ✅
- [x] Update `generateListStyles()` to read `list` component
- [x] Read `list.base.margin`, `list.base.padding`
- [x] Read `list.style-unordered`, `list.style-ordered`, `list.style-plain` properties
- [x] Add support for `list-style` property via `getListStyleValue()`

#### 1.5 Complete Partial Implementations ✅
- [x] **Field**: Read all properties from `field` YAML (lines 143-161)
- [x] **Input**: Add missing properties (width, disabled cursor)
- [x] **Label**: No changes needed (display is structural, Phase 4)
- [x] **Checkbox**: Add missing cursor property
- [x] **ListItem**: Add missing text-decoration, cursor, border properties
- [x] **Page/Section Titles**: Already complete (margin properties already read)

#### New Helper Methods Added
- [x] `getLineHeightValue()` - Unitless/unit line-height values
- [x] `getBorderWidthValue()` - Numeric to px conversion
- [x] `getTextAlignValue()` - Raw CSS text-align
- [x] `getTextDecorationValue()` - Raw CSS text-decoration
- [x] `getListStyleValue()` - Raw CSS list-style
- [x] `getCursorValue()` - Raw CSS cursor
- [x] `getWidthValue()` - Raw CSS width

**Actual Effort:** 1 session
**Files Modified:** `ThemeCompiler.cs` (generateFeedbackStyles, generateTextStyles, generateListStyles, generateFormStyles)
**Build Status:** ✅ 0 warnings, 0 errors

---

### Phase 2: Extend resolvePropertyValue() (Medium Priority) ✅ COMPLETED
**Goal:** Support all CSS properties needed for complete theme control

#### 2.1 Add CSS Property Mappings ✅
- [x] `display` (block, flex, inline-flex, grid, inline-block, none)
- [x] `flex-direction` (row, column, row-reverse, column-reverse)
- [x] `align-items` (center, flex-start, flex-end, stretch, baseline)
- [x] `justify-content` (center, flex-start, flex-end, space-between, space-around)
- [x] `text-align` (left, center, right, justify) - Phase 1
- [x] `text-decoration` (none, underline, line-through) - Phase 1
- [x] `cursor` (pointer, not-allowed, default, text) - Phase 1
- [x] `line-height` (numeric or unit values) - Phase 1
- [x] `list-style` (none, disc, decimal, circle, square) - Phase 1
- [x] `border-style` (solid, dashed, dotted, none)
- [x] `border-width` (pixel values) - Phase 1
- [x] `grid-template-columns` (fr units, repeat, minmax)
- [x] `max-width` (pixel or unit values)

#### 2.2 Handle Special Cases
- [x] Compound properties - Not needed (hardcoded where appropriate, e.g. `outline: 2px solid`)
- [x] Multiple value properties - Handled by helper methods (e.g. `padding: {paddingY} {paddingX}`)
- [x] Conditional token resolution - Already implemented in `resolvePropertyValue()`

#### New Helper Methods Added
- [x] `getDisplayValue()` - Display modes (block, flex, grid, etc.)
- [x] `getFlexDirectionValue()` - Flex direction (row, column, etc.)
- [x] `getAlignItemsValue()` - Flex alignment (center, flex-start, etc.)
- [x] `getJustifyContentValue()` - Flex justification (center, space-between, etc.)
- [x] `getBorderStyleValue()` - Border styles (solid, dashed, etc.)
- [x] `getMaxWidthValue()` - Max-width with px conversion
- [x] `getGridTemplateColumnsValue()` - Grid column definitions

**Actual Effort:** 1 session
**Files Modified:** `ThemeCompiler.cs` (added 7 new helper methods)
**Build Status:** ✅ 0 warnings, 0 errors

---

### Phase 3: Add Missing YAML Structures (Medium Priority)
**Goal:** Define YAML structures for all currently hardcoded values

#### 3.1 Page & Section Structure
Add to `default.theme.yaml`:
```yaml
page:
  base:
    max-width: 1200px
    margin: auto
    padding-x: lg
    padding-y: 3xl
    display: block

section:
  base:
    display: block
    margin-bottom: 3xl
```

#### 3.2 Layout Semantic Spacing
Add to `default.theme.yaml`:
```yaml
layout:
  stack:
    purpose-actions: sm
    purpose-fields: lg
    purpose-content: md
    purpose-items: md
    purpose-sections: 3xl
    purpose-inline: xs
    purpose-cards: 2xl
  row:
    purpose-actions: sm
    purpose-fields: lg
    purpose-inline: xs
  grid:
    purpose-cards: 2xl
    purpose-items: md
    cols-auto-minwidth: 300px
```

#### 3.3 Base Layer Properties
Add component-specific base properties:
```yaml
# Under existing components, add base layer properties

stack:
  base:
    display: flex
    flex-direction: column

row:
  base:
    display: flex
    flex-direction: row
    align-items: center

grid:
  base:
    display: grid

card:
  base:
    display: flex
    flex-direction: column
    # ... existing properties

footer:
  base:
    display: flex
    align-items: center

button:
  base:
    display: inline-flex
    align-items: center
    justify-content: center
    border: none
    cursor: pointer
    font-family: inherit
    transition-property: all
    transition-duration: fast
    transition-timing: ease
    # ... existing properties

text:
  base:
    margin: 0
    # ... existing properties
```

#### 3.4 Update ThemeCompiler to Read New Structures
- [ ] Create `generatePageStructureBase()` method or integrate into existing
- [ ] Create `generateLayoutBase()` method or integrate into existing
- [ ] Update all component generators to read base properties

**Estimated Effort:** 2-3 sessions
**Files Modified:** `default.theme.yaml`, `ThemeCompiler.cs`

---

### Phase 4: Refactor Base Layer (High Priority)
**Goal:** Make base layer read from theme instead of being hardcoded

#### 4.1 Decision: Approach Selection
Choose one approach:

**Option A: Move to Components Layer**
- Merge base layer properties into component definitions
- Eliminate separate base layer generation
- Pro: Simpler, all in one place
- Con: May blur separation of concerns

**Option B: Make Base Layer Theme-Aware**
- Keep base layer separate but read from theme
- Add `base:` section to YAML for structural defaults
- Pro: Maintains clean separation
- Con: More YAML structure complexity

**Recommended:** Option B - maintains architecture clarity

#### 4.2 Implement Base Layer Theme Integration
- [ ] Add `base:` section to YAML schema
- [ ] Update `generateBaseLayer()` to accept theme parameter
- [ ] Read all structural properties from theme
- [ ] Maintain backward compatibility with defaults

#### 4.3 Test All Components
- [ ] Verify page layout
- [ ] Verify all layout primitives (stack, row, grid)
- [ ] Verify containers (card, slots)
- [ ] Verify interactive elements (button, input, checkbox)
- [ ] Verify text elements

**Estimated Effort:** 2-3 sessions
**Files Modified:** `ThemeCompiler.cs` (generateBaseLayer), `default.theme.yaml`

---

### Phase 5: Validation & Testing (Critical)
**Goal:** Ensure complete designer control and no regressions

#### 5.1 Create Test Themes
- [ ] Create `minimal.theme.yaml` - Bare minimum styling
- [ ] Create `brutalist.theme.yaml` - Completely different aesthetic
- [ ] Create `compact.theme.yaml` - Tight spacing, small text
- [ ] Create `spacious.theme.yaml` - Generous spacing, large text

#### 5.2 Compile & Verify Test Themes
- [ ] Each theme should produce valid CSS
- [ ] Each theme should create visually distinct UIs
- [ ] No C# changes required between theme switches

#### 5.3 Documentation
- [ ] Document all YAML properties and their effects
- [ ] Create theme authoring guide
- [ ] Document property resolution rules
- [ ] Document token reference system

#### 5.4 Regression Testing
- [ ] Verify default.theme.yaml produces same output as before
- [ ] Check all component renders correctly
- [ ] Verify HTMX interactions still work
- [ ] Test responsive behavior

**Estimated Effort:** 2-3 sessions
**Files Created:** Multiple test theme files, documentation

---

### Phase 6: Advanced Features (Future Enhancement)
**Goal:** Enable even more designer control

#### 6.1 Responsive Design Tokens
- [ ] Add breakpoint definitions to YAML
- [ ] Support responsive spacing scales
- [ ] Media query generation from theme

#### 6.2 Component Variants
- [ ] Allow themes to define custom component variants
- [ ] Support modifier combinations

#### 6.3 Dark Mode Support
- [ ] Add color scheme switching
- [ ] Support prefers-color-scheme
- [ ] Multiple color palettes per theme

**Estimated Effort:** 3-5 sessions (optional/future)

---

## Progress Tracking

### Session 1 (2025-12-06)
- ✅ Deep architecture analysis completed
- ✅ Identified all hardcoded values
- ✅ Documented gaps in component mapping
- ✅ Created comprehensive implementation plan

### Session 2 (2025-12-06)
- ✅ **Phase 1 COMPLETED**: Fixed all existing YAML definitions
- ✅ Fixed text component (line-height)
- ✅ Fixed alert component (all base and tone variant properties)
- ✅ Fixed empty-state component (text-align, color, all size variants)
- ✅ Fixed list component (base and all style variants)
- ✅ Fixed field component (completely ignored before)
- ✅ Fixed input component (width, cursor)
- ✅ Fixed checkbox component (cursor)
- ✅ Fixed list-item component (text-decoration, cursor, border properties)
- ✅ Added 7 new helper methods for CSS property handling
- ✅ Build verified: 0 warnings, 0 errors

### Session 3 (2025-12-06)
- ✅ **Phase 2 COMPLETED**: Extended helper methods for structural CSS properties
- ✅ Added `getDisplayValue()` helper method
- ✅ Added `getFlexDirectionValue()` helper method
- ✅ Added `getAlignItemsValue()` helper method
- ✅ Added `getJustifyContentValue()` helper method
- ✅ Added `getBorderStyleValue()` helper method
- ✅ Added `getMaxWidthValue()` helper method (with px conversion)
- ✅ Added `getGridTemplateColumnsValue()` helper method
- ✅ Build verified: 0 warnings, 0 errors
- **Next:** Phase 3 or Phase 4 - Add YAML structures and/or refactor base layer

---

## Success Criteria

- [ ] A designer can create a new theme YAML file
- [ ] The designer never needs to touch .cs or .css files
- [ ] Compiling the new theme produces a complete, working stylesheet
- [ ] All visual aspects are controlled through YAML properties
- [ ] The UI renders correctly with the new theme
- [ ] No hardcoded CSS values remain in ThemeCompiler.cs

---

## Open Questions

1. **Base Layer Architecture**: Should base layer remain separate or merge into components layer?
   - **Decision:** TBD - needs discussion

2. **Property Naming Convention**: How should compound properties be represented in YAML?
   - Example: `transition: all 150ms ease` → separate properties or compound?
   - **Decision:** TBD

3. **Backward Compatibility**: Should we maintain exact CSS output for default theme?
   - **Decision:** Yes - regression testing required

4. **Token vs Raw Values**: When should properties resolve to tokens vs accept raw values?
   - Example: `opacity: 1` vs `opacity: disabled`
   - **Decision:** Support both, prefer tokens

5. **Grid Auto Columns**: Should minwidth be globally configured or per-grid?
   - Current: `minmax(300px, 1fr)` hardcoded
   - **Decision:** TBD - global config vs per-component

---

## Related Files

### Primary Files to Modify
- `/PagePlay.Site/Infrastructure/UI/Rendering/ThemeCompiler.cs` - Main compiler logic
- `/PagePlay.Site/Infrastructure/UI/Themes/default.theme.yaml` - Theme definition

### Reference Files
- `/PagePlay.Site/Infrastructure/UI/IComponent.cs` - Component interfaces
- `/PagePlay.Site/Infrastructure/UI/Rendering/HtmlRenderer.cs` - HTML generation
- All files in `/PagePlay.Site/Infrastructure/UI/Vocabulary/` - Component definitions

### Testing Files (to be created)
- `/PagePlay.Site/Infrastructure/UI/Themes/minimal.theme.yaml`
- `/PagePlay.Site/Infrastructure/UI/Themes/brutalist.theme.yaml`
- `/PagePlay.Site/Infrastructure/UI/Themes/compact.theme.yaml`

---

## Notes & Observations

### Architecture Insights
- The helper methods (`getComponent`, `getComponentProperty`, `resolvePropertyValue`, `getPropertyOrDefault`) are well-designed and ready to use
- The YAML structure is already well-organized with semantic naming
- The token system is solid - issue is purely in the compiler implementation
- The @layer architecture (tokens, base, components) is a good separation

### Risk Areas
- Changing base layer could affect layout behavior across entire app
- Property resolution logic needs careful testing for edge cases
- Some CSS properties may not map cleanly to token system
- Need to ensure HTMX attributes aren't affected by changes

### Performance Considerations
- Theme compilation happens at build time, not runtime
- No performance impact on generated CSS
- Consider caching compiled themes in production

---

## References

- CSS @layer specification: https://developer.mozilla.org/en-US/docs/Web/CSS/@layer
- YamlDotNet documentation: https://github.com/aaubry/YamlDotNet/wiki
- CSS custom properties: https://developer.mozilla.org/en-US/docs/Web/CSS/--*
