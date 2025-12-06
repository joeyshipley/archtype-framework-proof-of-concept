# Theme Comparison Guide

This document demonstrates complete YAML control over UI appearance through four test themes.

## Overview

All themes use **identical C# component code**. Every visual difference is controlled purely through YAML configuration.

---

## Test Themes

### 1. Default Theme (`default.theme.yaml`)
**Goal:** Professional, modern, comfortable design

**Characteristics:**
- Text: 16px body, 24px page titles
- Spacing: Comfortable (8px-32px range)
- Colors: Blue accent (#2563eb), subtle grays
- Shadows: Subtle (0 1px 2px rgba(0,0,0,0.05))
- Radius: Gentle curves (0.375rem)
- Transitions: Smooth (150ms-500ms)

**Use Case:** Production applications, professional tools

---

### 2. Minimal Theme (`minimal.theme.yaml`)
**Goal:** Bare essentials, maximum simplicity

**Characteristics:**
- Text: 16px body, 20px page titles
- Spacing: Tight (2px-24px range)
- Colors: Pure colors (#0000ff blue, #008000 green, #ff0000 red)
- Shadows: **None**
- Radius: **No curves** (all 0)
- Transitions: **Instant** (0ms)
- Borders: Simple gray (#cccccc)
- Page width: Narrower (900px)

**Key Differences:**
```yaml
shadow:
  sm: "none"    # vs default "0 1px 2px rgba(0,0,0,0.05)"

radius:
  md: "0"       # vs default "0.375rem"

duration:
  fast: 0ms     # vs default 150ms

spacing:
  xs: 0.125rem  # vs default 0.25rem (half the size)
```

**Use Case:** Prototype validation, testing, minimalist aesthetic

---

### 3. Brutalist Theme (`brutalist.theme.yaml`)
**Goal:** Bold, dramatic, high-contrast, geometric

**Characteristics:**
- Text: 18px body, 48px page titles (massive)
- Spacing: Generous (8px-64px range)
- Colors: Pure primaries (#ff0000 red, #00ff00 lime, #ffff00 yellow)
- Shadows: **Hard drop shadows** (8px 8px 0 #000000)
- Radius: **Square** (all 0)
- Transitions: **Instant** (0ms)
- Borders: Pure black (#000000)
- Font weights: Extra bold (700-900)
- Page width: Wide (1400px)

**Key Differences:**
```yaml
shadow:
  md: "8px 8px 0 #000000"  # vs default "0 4px 6px rgba(0,0,0,0.1)"

color:
  accent: "#ff0000"        # vs default "#2563eb"
  border: "#000000"        # vs default "#e5e5e5"
  positive: "#00ff00"      # vs default "#16a34a"

font:
  weight-bold: 900         # vs default 700
  weight-medium: 700       # vs default 500

text:
  2xl: 3rem                # vs default 1.5rem (double size!)

spacing:
  3xl: 4rem                # vs default 2rem (double spacing!)
```

**Use Case:** Art projects, portfolio sites, making a statement

---

### 4. Compact Theme (`compact.theme.yaml`)
**Goal:** Dense information, maximize screen space

**Characteristics:**
- Text: 14px body, 20px page titles (small)
- Spacing: Very tight (2px-24px range)
- Colors: Subtle borders (#d4d4d4), muted tones
- Shadows: Barely visible (0 1px 2px rgba(0,0,0,0.03))
- Radius: Minimal curves (0.125rem-0.375rem)
- Transitions: Quick (100ms-300ms)
- Checkboxes: Smaller (12px)
- Page width: Narrow (1000px)

**Key Differences:**
```yaml
text:
  md: 0.875rem             # vs default 1rem (14px vs 16px)
  xs: 0.625rem             # vs default 0.75rem (10px!)

spacing:
  xs: 0.125rem             # vs default 0.25rem (half size)
  md: 0.5rem               # vs default 0.75rem

checkbox:
  base:
    size: 3                # vs default 4 (12px vs 16px)

button:
  base:
    padding-x: 2           # vs default 4 (half padding)
    size: sm               # vs default md (smaller text)
```

**Use Case:** Data-heavy dashboards, admin panels, power users

---

### 5. Spacious Theme (`spacious.theme.yaml`)
**Goal:** Maximum readability, generous breathing room

**Characteristics:**
- Text: 18px body, 36px page titles (large)
- Spacing: Generous (8px-64px range)
- Colors: Soft colors (#3b82f6 blue), gentle grays
- Shadows: Noticeable (0 4px 8px rgba(0,0,0,0.12))
- Radius: Comfortable curves (0.5rem-0.75rem)
- Transitions: Smooth (200ms-600ms)
- Line height: 1.7 (comfortable reading)
- Page width: Wide (1400px)

**Key Differences:**
```yaml
text:
  md: 1.125rem             # vs default 1rem (18px vs 16px)
  2xl: 2.25rem             # vs default 1.5rem (36px vs 24px)

spacing:
  xl: 2rem                 # vs default 1.25rem
  3xl: 4rem                # vs default 2rem (double!)

text:
  base:
    line-height: 1.7       # vs default 1.5 (more space)

button:
  base:
    padding-x: 6           # vs default 4 (50% more)
    size: lg               # vs default md (larger text)
```

**Use Case:** Content-focused sites, reading apps, accessibility focus

---

## Token Comparison Table

| Token | Default | Minimal | Brutalist | Compact | Spacious |
|-------|---------|---------|-----------|---------|----------|
| **Body Text** | 16px | 16px | 18px | 14px | 18px |
| **Page Title** | 24px | 20px | 48px | 20px | 36px |
| **Button Padding** | 16px × 8px | 8px × 4px | 24px × 12px | 8px × 4px | 24px × 12px |
| **Card Shadow** | Subtle | None | Hard 8px | Barely visible | Noticeable |
| **Border Radius** | 0.375rem | 0 | 0 | 0.125rem | 0.5rem |
| **Accent Color** | Blue #2563eb | Blue #0000ff | Red #ff0000 | Blue #2563eb | Blue #3b82f6 |
| **Transition** | 150ms | 0ms | 0ms | 100ms | 200ms |
| **Page Width** | 1200px | 900px | 1400px | 1000px | 1400px |
| **Section Gap** | 32px | 16px | 64px | 24px | 64px |

---

## Compilation Results

All themes compiled successfully:

```bash
dotnet run compile-theme Infrastructure/UI/Themes/minimal.theme.yaml wwwroot/css/minimal.css
# ✓ Theme compiled successfully!

dotnet run compile-theme Infrastructure/UI/Themes/brutalist.theme.yaml wwwroot/css/brutalist.css
# ✓ Theme compiled successfully!

dotnet run compile-theme Infrastructure/UI/Themes/compact.theme.yaml wwwroot/css/compact.css
# ✓ Theme compiled successfully!

dotnet run compile-theme Infrastructure/UI/Themes/spacious.theme.yaml wwwroot/css/spacious.css
# ✓ Theme compiled successfully!
```

**Generated Files:**
- `wwwroot/css/minimal.css` - 11KB
- `wwwroot/css/brutalist.css` - 11KB
- `wwwroot/css/compact.css` - 11KB
- `wwwroot/css/spacious.css` - 11KB

**Zero C# code changes required** - all visual differences controlled through YAML.

---

## Key Insights

### 1. Complete Designer Control
Designers can now:
- Change any color, shadow, spacing, or typography
- Create entirely new visual identities
- Never touch C# or generated CSS files
- Compile themes independently

### 2. Consistent Output Size
All themes produce ~11KB CSS files, regardless of styling choices. The token system ensures efficient CSS generation.

### 3. Token-Driven Architecture
Every visual property resolves through the token system:
```yaml
card:
  header:
    size: md           # Resolves to --text-md (theme-specific)
    weight: semibold   # Resolves to --weight-semibold (theme-specific)
    padding: 4         # Resolves to --spacing-4 (theme-specific)
```

### 4. No Hardcoded Values
The ThemeCompiler reads **all** properties from YAML:
- Base layer structural CSS
- Layout semantic spacing
- Component properties
- State variations
- Tone mappings

---

## Next Steps

1. **Browser Testing**: View each theme in a browser with real components
2. **Documentation**: Complete theme authoring guide
3. **Regression Testing**: Verify HTMX interactions work with all themes
4. **Theme Switching**: Build UI to switch themes at runtime

---

**Conclusion:** The experiment successfully demonstrates complete YAML control over UI appearance. Designers can create dramatically different visual identities without touching code.
