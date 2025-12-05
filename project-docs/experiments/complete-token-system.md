# EXP-003: Complete Token System Implementation

**Status:** 🟡 In Progress
**Started:** 2025-12-05
**Target Completion:** TBD
**Owner:** Team

---

## Objective

Implement a complete token system that gives designers full control over all appearance decisions without requiring code changes. This experiment validates the Closed-World UI principle: **"Developers declare WHAT things ARE. Designers control HOW they look."**

---

## Success Criteria

- ✅ Designer can rebrand the application by editing only `default.theme.yaml`
- ✅ Designer can adjust all font weights without touching code
- ✅ Designer can change transition/animation speeds globally
- ✅ Designer can modify disabled/subtle state opacity values
- ✅ Generated CSS uses tokens for all previously hardcoded values
- ✅ No visual regressions (output matches current appearance by default)

---

## Current State vs. Target State

### Token Gaps

| Category | Current | Required | Gap |
|----------|---------|----------|-----|
| `text` sizes | sm, md, lg | xs, sm, md, lg, xl, 2xl | Missing 3 |
| `font` weights | ❌ None | normal, medium, semibold, bold | Missing all |
| `color` | 16 colors | + white | Missing 1 |
| `shadow` | sm, md | sm, md, lg | Missing 1 |
| `radius` | sm, md, lg | sm, md, lg, full | Missing 1 |
| `duration` | ❌ None | fast, normal, slow | Missing all |
| `opacity` | ❌ None | subdued, disabled, subtle | Missing all |

### Hardcoded Values in ThemeCompiler.cs

**Font Weights:**
- Line 146: `font-weight: 600;` (card header)
- Line 264: `font-weight: 500;` (button)
- Line 384: `font-weight: 500;` (label)
- Line 601: `font-weight: 700;` (page-title)
- Line 608: `font-weight: 600;` (section-title)

**Opacity Values:**
- Line 312: `opacity: 0.5;` (button disabled)
- Line 566: `opacity: 1;` (list-item normal)
- Line 570: `opacity: 0.6;` (list-item completed)
- Line 574: `opacity: 0.4;` (list-item disabled)

**Duration Values:**
- Line 184: `transition: all 150ms ease;` (button)
- htmx-lib.css line 11: `transition: opacity 500ms ease-in;`

---

## Implementation Plan

### Phase 1: Token Definitions (Designer-Facing)

**Goal:** Add all missing token categories to `default.theme.yaml`

**Files to Modify:**
- `PagePlay.Site/Infrastructure/UI/Themes/default.theme.yaml`

**Tasks:**

#### 1.1 Add Missing Text Sizes
```yaml
text:
  xs: 0.75rem      # NEW - captions, fine print
  sm: 0.875rem     # EXISTING
  md: 1rem         # EXISTING
  lg: 1.125rem     # EXISTING
  xl: 1.25rem      # NEW - section headings
  2xl: 1.5rem      # NEW - page titles
```

**Status:** ✅ Complete
**Validation:** ✅ Can reference `--text-xs`, `--text-xl`, `--text-2xl` in generated CSS

---

#### 1.2 Add Font Weight Tokens
```yaml
font:
  weight-normal: 400
  weight-medium: 500
  weight-semibold: 600
  weight-bold: 700
```

**Status:** ✅ Complete
**Validation:** ✅ Can reference `--weight-*` tokens in generated CSS (note: prefix is `weight-` not `font-weight-`)

---

#### 1.3 Add Duration Tokens
```yaml
duration:
  fast: 150ms
  normal: 300ms
  slow: 500ms
```

**Status:** ✅ Complete
**Validation:** ✅ Can reference `--duration-*` tokens in generated CSS

---

#### 1.4 Add Opacity Tokens
```yaml
opacity:
  subdued: 0.4
  disabled: 0.5
  subtle: 0.6
```

**Status:** ✅ Complete
**Validation:** ✅ Can reference `--opacity-*` tokens in generated CSS

---

#### 1.5 Add Missing Color/Shadow/Radius Tokens
```yaml
color:
  white: "#ffffff"  # Explicit for button text

shadow:
  lg: "0 10px 15px rgba(0,0,0,0.1)"

radius:
  full: "9999px"
```

**Status:** ✅ Complete
**Validation:** ✅ Can reference these tokens in generated CSS

---

### Phase 2: Theme Compiler Updates (Token Generation)
**Status:** ✅ Complete

**Goal:** Generate CSS custom properties for all new token categories

**Files to Modify:**
- `PagePlay.Site/Infrastructure/UI/Rendering/ThemeCompiler.cs`

**Tasks:**

#### 2.1 Update `generateTokensLayer()` - Font Weights

**Current Location:** Line 49-118

**Changes Needed:**
```csharp
// After text tokens section (around line 80)
// Add font tokens
if (tokens.TryGetValue("font", out var fontObj) && fontObj is Dictionary<object, object> font)
{
    css.AppendLine("    /* Font Weights */");
    foreach (var kvp in font)
    {
        css.AppendLine($"    --{kvp.Key}: {kvp.Value};");
    }
    css.AppendLine();
}
```

**Status:** ✅ Complete
**Validation:** ✅ Generated CSS includes `--weight-normal: 400;` etc.

---

#### 2.2 Update `generateTokensLayer()` - Duration

**Changes Needed:**
```csharp
// After opacity tokens
if (tokens.TryGetValue("duration", out var durationObj) && durationObj is Dictionary<object, object> durations)
{
    css.AppendLine("    /* Duration */");
    foreach (var kvp in durations)
    {
        css.AppendLine($"    --duration-{kvp.Key}: {kvp.Value};");
    }
    css.AppendLine();
}
```

**Status:** ✅ Complete
**Validation:** ✅ Generated CSS includes `--duration-fast: 150ms;` etc.

---

#### 2.3 Update `generateTokensLayer()` - Opacity

**Changes Needed:**
```csharp
// After duration tokens
if (tokens.TryGetValue("opacity", out var opacityObj) && opacityObj is Dictionary<object, object> opacities)
{
    css.AppendLine("    /* Opacity */");
    foreach (var kvp in opacities)
    {
        css.AppendLine($"    --opacity-{kvp.Key}: {kvp.Value};");
    }
    css.AppendLine();
}
```

**Status:** ✅ Complete
**Validation:** ✅ Generated CSS includes `--opacity-disabled: 0.5;` etc.

---

### Phase 3: Theme Compiler Updates (Component Generation)
**Status:** ⬜ Not Started

**Goal:** Replace all hardcoded values with token references

**Files to Modify:**
- `PagePlay.Site/Infrastructure/UI/Rendering/ThemeCompiler.cs`

**Tasks:**

#### 3.1 Replace Hardcoded Font Weights

**Locations:**
- `generateCardStyles()` - line 146
- `generateButtonStyles()` - line 264
- `generateFormStyles()` - line 384
- `generatePageStructureStyles()` - lines 601, 608

**Changes:**
```csharp
// Before:
css.AppendLine("    font-weight: 600;");

// After:
css.AppendLine("    font-weight: var(--font-weight-semibold);");
```

**Mapping:**
- `500` → `var(--font-weight-medium)`
- `600` → `var(--font-weight-semibold)`
- `700` → `var(--font-weight-bold)`

**Status:** ⬜ Not Started
**Validation:** No hardcoded font-weight values in generated CSS

---

#### 3.2 Replace Hardcoded Opacity Values

**Locations:**
- `generateButtonStyles()` - line 312 (disabled: 0.5)
- `generateListStyles()` - lines 566, 570, 574

**Changes:**
```csharp
// Before:
css.AppendLine("    opacity: 0.5;");

// After:
css.AppendLine("    opacity: var(--opacity-disabled);");
```

**Mapping:**
- `0.4` → `var(--opacity-subdued)` (list-item disabled)
- `0.5` → `var(--opacity-disabled)` (button disabled)
- `0.6` → `var(--opacity-subtle)` (list-item completed)
- `1` → `1` (can stay hardcoded - normal state)

**Status:** ⬜ Not Started
**Validation:** No hardcoded opacity values (except `1`) in generated CSS

---

#### 3.3 Replace Hardcoded Duration Values

**Locations:**
- `generateButtonStyles()` - line 184 (150ms)

**Changes:**
```csharp
// Before:
css.AppendLine("    transition: all 150ms ease;");

// After:
css.AppendLine("    transition: all var(--duration-fast) ease;");
```

**Status:** ⬜ Not Started
**Validation:** No hardcoded duration values in generated CSS

---

#### 3.4 Use New Text Sizes for Headings

**Locations:**
- `generatePageStructureStyles()` - lines 600, 607

**Changes:**
```csharp
// Before:
css.AppendLine("    font-size: 2rem;");

// After:
css.AppendLine("    font-size: var(--text-2xl);");
```

**Mapping:**
- `2rem` → `var(--text-2xl)` (page-title)
- `1.5rem` → `var(--text-xl)` (section-title)

**Status:** ⬜ Not Started
**Validation:** Headings use text scale tokens

---

### Phase 4: External CSS Files

**Goal:** Update non-generated CSS to use tokens

**Files to Modify:**
- `PagePlay.Site/wwwroot/css/htmx-lib.css`

**Tasks:**

#### 4.1 Update htmx-lib.css

**Current (line 11):**
```css
transition: opacity 500ms ease-in;
```

**After:**
```css
transition: opacity var(--duration-slow) ease-in;
```

**Status:** ⬜ Not Started
**Validation:** htmx animations use duration tokens

---

### Phase 5: Component Mapping Enhancement

**Goal:** Add semantic property mappings to theme.yaml

**Files to Modify:**
- `PagePlay.Site/Infrastructure/UI/Themes/default.theme.yaml`

**Tasks:**

#### 5.1 Add Weight Properties to Component Mappings

**Example:**
```yaml
button:
  base:
    padding-x: 4
    padding-y: 2
    radius: md
    weight: medium      # NEW - maps to font-weight-medium
    size: md
```

**Note:** This is future work for component-level theme mappings. Not required for Phase 1-4 to succeed.

**Status:** ⬜ Not Started (Optional)
**Validation:** Theme compiler can read and apply component-level weight/duration/opacity

---

### Phase 6: Testing & Validation

**Goal:** Verify no regressions and designer control is complete

**Tasks:**

#### 6.1 Visual Regression Testing

- [ ] Build with new tokens
- [ ] Generate CSS: `dotnet run compile-theme ...`
- [ ] Compare output to current `closed-world.css`
- [ ] All visual properties should match exactly (using same values)
- [ ] Run app, verify no visual changes

**Status:** ⬜ Not Started

---

#### 6.2 Designer Control Testing

**Test 1: Change All Font Weights**
- [ ] Edit `default.theme.yaml`: Change `weight-semibold: 600` to `500`
- [ ] Regenerate CSS
- [ ] Verify all headers, labels, buttons change weight
- [ ] No code changes required

**Test 2: Change Transition Speed**
- [ ] Edit `default.theme.yaml`: Change `fast: 150ms` to `100ms`
- [ ] Regenerate CSS
- [ ] Verify all transitions feel snappier
- [ ] No code changes required

**Test 3: Change Disabled Opacity**
- [ ] Edit `default.theme.yaml`: Change `disabled: 0.5` to `0.3`
- [ ] Regenerate CSS
- [ ] Verify all disabled states are more subtle
- [ ] No code changes required

**Status:** ⬜ Not Started

---

#### 6.3 Token Coverage Audit

Run through every hardcoded value in ThemeCompiler.cs:
- [ ] All font-weight values use tokens
- [ ] All opacity values use tokens (except `1`)
- [ ] All duration values use tokens
- [ ] All font-size values use tokens
- [ ] All spacing values use tokens (already complete)
- [ ] All color values use tokens (already complete)

**Status:** ⬜ Not Started

---

## Session Log

### Session 1: 2025-12-05 (Analysis & Planning)
**Participants:** Claude, User
**Duration:** ~2 hours

**Completed:**
- ✅ Deep analysis of current token system
- ✅ Read all research documents (B-00 through B-05, A-01 through A-04)
- ✅ Identified token gaps and hardcoded values
- ✅ Created unified specification document
- ✅ Created this experiment plan

**Decisions Made:**
- Font weights, duration, and opacity MUST be tokens (designer control test)
- Line-height stays hardcoded (universal constant)
- Border widths stay hardcoded (universal constants)
- Context-aware styling is first-class (container-centric theme organization)

**Next Session Goals:**
- Begin Phase 1: Add token definitions to `default.theme.yaml`
- Test token generation in ThemeCompiler

---

### Session 2: 2025-12-05 (Phase 1 & 2 Implementation)
**Status:** ✅ Complete
**Participants:** Claude, User
**Duration:** ~30 minutes

**Completed:**
- ✅ Phase 1: All token definitions added to `default.theme.yaml`
  - Text sizes: xs, xl, 2xl
  - Font weights: weight-normal, weight-medium, weight-semibold, weight-bold
  - Duration: fast, normal, slow
  - Opacity: subdued, disabled, subtle
  - Additional: color.white, shadow.lg, radius.full
- ✅ Phase 2: ThemeCompiler.cs updated to generate all new token categories
  - Added font weight generation
  - Added duration generation
  - Added opacity generation
- ✅ Verified CSS generation: All tokens output correctly

**Files Modified:**
- `default.theme.yaml` (22 additions)
- `ThemeCompiler.cs` (3 new token generation blocks)
- `closed-world.css` (regenerated with new tokens)

**Commits:**
- `65fb457` - Complete Phase 1: Add complete token system to default.theme.yaml

**Next Session Goals:**
- Begin Phase 3: Replace hardcoded values in component generation

---

### Session 3: [Date] (Phase 3 Implementation)
**Status:** ⬜ Not Started

**Goals:**
- Complete Phase 3 (component generation updates)
- Replace hardcoded font-weight, opacity, duration values

**Notes:**
- [To be filled during session]

---

### Session 4: [Date] (Phase 4-6 Validation)
**Status:** ⬜ Not Started

**Goals:**
- Complete Phase 4 (external CSS)
- Complete Phase 5 (optional component mappings)
- Complete Phase 6 (testing & validation)

**Notes:**
- [To be filled during session]

---

## Risks & Mitigations

| Risk | Impact | Mitigation |
|------|--------|------------|
| Visual regression during token swap | HIGH | Compare generated CSS line-by-line before/after |
| Designer confusion about token names | MEDIUM | Document token naming conventions clearly |
| Theme compiler bugs with new token types | MEDIUM | Add unit tests for token generation |
| Performance impact of more CSS variables | LOW | CSS custom properties are performant |

---

## Success Metrics

### Quantitative
- **0** hardcoded font-weight values in generated CSS
- **0** hardcoded opacity values (except `1`) in generated CSS
- **0** hardcoded duration values in generated CSS
- **100%** token coverage for designer-controlled properties

### Qualitative
- Designer can perform complete rebrand without developer
- New designers can understand token system in < 30 minutes
- Theme changes feel predictable and safe

---

## Related Documents

- [UNIFIED_STYLING_SPECIFICATION.md](./styles/UNIFIED_STYLING_SPECIFICATION.md) - Complete specification
- [default.theme.yaml](../../PagePlay.Site/Infrastructure/UI/Themes/default.theme.yaml) - Token definitions
- [ThemeCompiler.cs](../../PagePlay.Site/Infrastructure/UI/Rendering/ThemeCompiler.cs) - CSS generation

---

## Experiment Outcomes

### What We Learned
- [To be filled after completion]

### What Changed
- [To be filled after completion]

### Next Experiments
- [To be filled after completion]

---

**Last Updated:** 2025-12-05
**Status Summary:** Planning complete, ready for implementation
