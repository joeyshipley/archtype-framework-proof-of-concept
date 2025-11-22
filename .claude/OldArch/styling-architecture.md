# Styling Architecture: C# Style Functions + Responsive Tokens

## Philosophy

This styling approach leverages our server-authoritative architecture to solve CSS's fundamental challenges in a novel way. Rather than fighting CSS's limitations with build tools or utility frameworks, we split responsibilities between what C# does well (type safety, logic, refactoring) and what CSS does well (responsiveness, interactions, animations).

## Core Principles

### 1. Server Controls Structure, CSS Controls Adaptation
- **C# functions** generate component structure styles (layout, spacing, colors, typography)
- **CSS** handles environmental adaptation (media queries, hover states, animations)
- Clear separation prevents overlap and confusion

### 2. Type Safety Over String Literals
- All styling goes through C# functions, never string literals in views
- Compile-time validation catches breaking changes
- IDE refactoring tools work across entire codebase

### 3. Single Point of Change
- Changing all 267 buttons: edit one C# method
- Changing responsive breakpoints: edit tokens.css
- Changing animations: edit animations.css
- No hunting through files

### 4. Validation Over Discipline
- Build-time validation ensures CSS and C# stay in sync
- Token usage validated automatically
- Contract violations fail builds, not production

## Architecture Layers

### Layer 1: Design Tokens (CSS Custom Properties)

**File:** `wwwroot/css/01-tokens.css`

Design tokens are CSS custom properties that define all design values. Tokens can be static or responsive.

```css
:root {
    /* Color Tokens - Static */
    --color-primary: #667eea;
    --color-secondary: #764ba2;
    --color-success: #28a745;
    --color-danger: #dc3545;
    --color-warning: #ffc107;

    /* Surface Colors */
    --surface-primary: #ffffff;
    --surface-secondary: #f8f9fa;

    /* Text Colors */
    --text-primary: #212529;
    --text-secondary: #6c757d;

    /* Shadows */
    --shadow-sm: 0 1px 2px rgba(0,0,0,0.05);
    --shadow-md: 0 4px 6px rgba(0,0,0,0.1);
    --shadow-lg: 0 10px 15px rgba(0,0,0,0.1);

    /* Border Radius */
    --radius-sm: 4px;
    --radius-md: 6px;
    --radius-lg: 12px;

    /* Responsive Tokens - Mobile First (default) */
    --spacing-xs: 0.25rem;
    --spacing-sm: 0.5rem;
    --spacing-md: 0.75rem;
    --spacing-lg: 1rem;
    --spacing-xl: 1.5rem;

    --font-size-sm: 0.875rem;
    --font-size-base: 1rem;
    --font-size-lg: 1.125rem;
    --font-size-xl: 1.25rem;

    /* Component-Specific Responsive Tokens */
    --spacing-btn: 0.75rem;
    --font-size-btn: 0.875rem;
}

/* Tablet and up */
@media (min-width: 768px) {
    :root {
        --spacing-md: 1rem;
        --spacing-lg: 1.5rem;
        --spacing-xl: 2rem;

        --spacing-btn: 1rem;
        --font-size-btn: 1rem;
    }
}

/* Desktop and up */
@media (min-width: 1024px) {
    :root {
        --spacing-lg: 2rem;
        --spacing-xl: 3rem;
    }
}
```

**Key Insight:** Responsive tokens make media queries work WITH inline styles instead of against them.

---

### Layer 2: Interactions & Pseudo-Classes

**File:** `wwwroot/css/02-interactions.css`

This minimal CSS file handles only what inline styles cannot: pseudo-classes, pseudo-elements, and interaction states.

```css
/* Button Interactions */
.btn {
    cursor: pointer;
    transition: filter 0.2s ease;
    border: none;
    font-family: inherit;
}

.btn:hover {
    filter: brightness(1.1);
}

.btn:active {
    filter: brightness(0.9);
}

.btn:focus-visible {
    outline: 2px solid var(--color-primary);
    outline-offset: 2px;
}

.btn:disabled {
    opacity: 0.5;
    cursor: not-allowed;
}

/* Input Interactions */
input[type="text"],
input[type="checkbox"] {
    transition: border-color 0.2s ease;
}

input[type="text"]:focus {
    outline: none;
    border-color: var(--color-primary);
}

/* Checkbox Styling */
input[type="checkbox"] {
    cursor: pointer;
    accent-color: var(--color-primary);
}

/* Link Interactions */
a {
    color: var(--color-primary);
    text-decoration: none;
    transition: color 0.2s ease;
}

a:hover {
    color: var(--color-secondary);
}
```

**Rule:** This file should ONLY contain pseudo-classes, pseudo-elements, and transitions. No structural styles.

---

### Layer 3: Animations

**File:** `wwwroot/css/03-animations.css`

Defines all keyframe animations and HTMX integration.

```css
/* Keyframe Definitions */
@keyframes fadeIn {
    from {
        opacity: 0;
        transform: translateY(-10px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

@keyframes slideIn {
    from {
        transform: translateX(var(--slide-distance, -100%));
    }
    to {
        transform: translateX(0);
    }
}

@keyframes pulse {
    0%, 100% { opacity: 1; }
    50% { opacity: 0.5; }
}

@keyframes completePulse {
    0% { transform: scale(1); }
    50% { transform: scale(1.05); }
    100% { transform: scale(1); }
}

/* HTMX Swap Animations */
.htmx-swapping {
    animation: fadeIn 0.2s ease-out;
}

.htmx-added {
    animation: slideIn 0.3s ease-out;
}

/* Accessibility: Respect User Preferences */
@media (prefers-reduced-motion: reduce) {
    *,
    *::before,
    *::after {
        animation-duration: 0.01ms !important;
        animation-iteration-count: 1 !important;
        transition-duration: 0.01ms !important;
    }
}

/* Responsive Animation Adjustments */
@media (max-width: 640px) {
    .htmx-swapping,
    .htmx-added {
        animation-duration: 0.15s;
    }
}
```

**Key Feature:** Animations can be parameterized via CSS custom properties set in inline styles.

---

### Layer 4: Component-Specific Responsive CSS

**Files:** `wwwroot/css/components/*.css` (as needed)

For complex responsive layouts that can't be handled by responsive tokens alone.

```css
/* components/todo-item.css */
.todo-item {
    display: flex;
    align-items: center;
    gap: var(--spacing-md);
}

.todo-item__actions {
    display: flex;
    gap: var(--spacing-sm);
}

/* Complex responsive layout changes */
@media (max-width: 640px) {
    .todo-item {
        flex-direction: column;
        align-items: stretch;
    }

    .todo-item__actions {
        width: 100%;
        justify-content: space-between;
    }
}

/* State-based animations */
[data-state="completed"] {
    animation: completePulse 0.5s ease-out;
}
```

**Rule:** Use this layer sparingly. Most components should work with just responsive tokens.

---

### Layer 5: C# Style Functions

**Files:** `Styles/*.cs`

C# classes that generate inline styles and CSS class references.

#### Example: ButtonStyles.cs

```csharp
namespace TurnBasedTodo.Styles;

public enum ButtonSize
{
    Small,
    Medium,
    Large
}

public enum ButtonVariant
{
    Primary,
    Secondary,
    Danger
}

public static class ButtonStyles
{
    public static string Button(
        ButtonVariant variant = ButtonVariant.Primary,
        ButtonSize size = ButtonSize.Medium,
        bool fullWidth = false
    ) =>
        $"class='btn' style='" +
        $"display: inline-flex; " +
        $"align-items: center; " +
        $"justify-content: center; " +
        $"gap: var(--spacing-sm); " +
        $"padding: {PaddingFor(size)}; " +
        $"font-size: {FontSizeFor(size)}; " +
        $"font-weight: 600; " +
        $"border-radius: var(--radius-md); " +
        $"{ColorStylesFor(variant)} " +
        $"{(fullWidth ? "width: 100%;" : "")}" +
        $"'";

    private static string PaddingFor(ButtonSize size) => size switch
    {
        ButtonSize.Small => "calc(var(--spacing-btn) * 0.75) calc(var(--spacing-btn) * 1.5)",
        ButtonSize.Medium => "var(--spacing-btn) calc(var(--spacing-btn) * 2)",
        ButtonSize.Large => "calc(var(--spacing-btn) * 1.25) calc(var(--spacing-btn) * 2.5)",
        _ => "var(--spacing-btn) calc(var(--spacing-btn) * 2)"
    };

    private static string FontSizeFor(ButtonSize size) => size switch
    {
        ButtonSize.Small => "calc(var(--font-size-btn) * 0.875)",
        ButtonSize.Medium => "var(--font-size-btn)",
        ButtonSize.Large => "calc(var(--font-size-btn) * 1.125)",
        _ => "var(--font-size-btn)"
    };

    private static string ColorStylesFor(ButtonVariant variant) => variant switch
    {
        ButtonVariant.Primary =>
            "background: var(--color-primary); " +
            "color: white; " +
            "box-shadow: var(--shadow-md);",
        ButtonVariant.Secondary =>
            "background: var(--surface-secondary); " +
            "color: var(--text-primary); " +
            "box-shadow: var(--shadow-sm);",
        ButtonVariant.Danger =>
            "background: var(--color-danger); " +
            "color: white; " +
            "box-shadow: var(--shadow-md);",
        _ => ""
    };
}
```

#### Example: TodoItemStyles.cs

```csharp
namespace TurnBasedTodo.Styles;

public enum AnimationDirection
{
    None,
    FromLeft,
    FromRight
}

public static class TodoItemStyles
{
    public static string Container(
        bool completed,
        bool isNew = false,
        AnimationDirection animation = AnimationDirection.None
    ) =>
        $"class='todo-item' " +
        $"data-state='{(completed ? "completed" : "active")}' " +
        $"style='" +
        $"padding: var(--spacing-md); " +
        $"border: 2px solid {(completed ? "var(--color-success)" : "var(--surface-secondary)")}; " +
        $"border-radius: var(--radius-md); " +
        $"margin-bottom: var(--spacing-md); " +
        $"background: var(--surface-primary); " +
        $"opacity: {(completed ? "0.6" : "1")}; " +
        $"{AnimationStyles(isNew, animation)}" +
        $"'";

    public static string Title(bool completed) =>
        $"style='" +
        $"flex: 1; " +
        $"color: var(--text-primary); " +
        $"{(completed ? "text-decoration: line-through; color: var(--text-secondary);" : "")}" +
        $"'";

    public static string Checkbox() =>
        $"style='" +
        $"width: 20px; " +
        $"height: 20px;" +
        $"'";

    private static string AnimationStyles(bool isNew, AnimationDirection direction)
    {
        if (!isNew) return "";

        return direction switch
        {
            AnimationDirection.FromLeft => "--slide-distance: -100%; animation: slideIn 0.3s ease-out;",
            AnimationDirection.FromRight => "--slide-distance: 100%; animation: slideIn 0.3s ease-out;",
            _ => "animation: fadeIn 0.3s ease-out;"
        };
    }
}
```

#### Example: LayoutStyles.cs

```csharp
namespace TurnBasedTodo.Styles;

public static class LayoutStyles
{
    public static string FlexRow(
        string gap = "var(--spacing-md)",
        string align = "center",
        string justify = "flex-start"
    ) =>
        $"style='" +
        $"display: flex; " +
        $"flex-direction: row; " +
        $"align-items: {align}; " +
        $"justify-content: {justify}; " +
        $"gap: {gap};" +
        $"'";

    public static string FlexColumn(
        string gap = "var(--spacing-md)",
        string align = "stretch"
    ) =>
        $"style='" +
        $"display: flex; " +
        $"flex-direction: column; " +
        $"align-items: {align}; " +
        $"gap: {gap};" +
        $"'";

    public static string Grid(
        int columns,
        string gap = "var(--spacing-md)"
    ) =>
        $"style='" +
        $"display: grid; " +
        $"grid-template-columns: repeat({columns}, 1fr); " +
        $"gap: {gap};" +
        $"'";

    public static string Container(
        string maxWidth = "800px"
    ) =>
        $"style='" +
        $"max-width: {maxWidth}; " +
        $"margin: 0 auto; " +
        $"padding: 0 var(--spacing-md);" +
        $"'";
}
```

---

## Usage in Razor Views

### Example: Updated Board.cshtml

```cshtml
@model List<TurnBasedTodo.Models.Todo>
@using TurnBasedTodo.Styles
<!DOCTYPE html>
<html>
<head>
    <title>Turn-Based Todo Game</title>
    <script src="https://unpkg.com/htmx.org@1.9.10"></script>
    <link rel="stylesheet" href="/css/01-tokens.css">
    <link rel="stylesheet" href="/css/02-interactions.css">
    <link rel="stylesheet" href="/css/03-animations.css">
    <link rel="stylesheet" href="/css/components/todo-item.css">
</head>
<body style="
    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
    background: linear-gradient(135deg, var(--color-primary) 0%, var(--color-secondary) 100%);
    min-height: 100vh;
    padding: var(--spacing-xl) var(--spacing-md);
    margin: 0;
">
    <div style="
        max-width: 800px;
        margin: 0 auto;
        background: var(--surface-primary);
        border-radius: var(--radius-lg);
        box-shadow: var(--shadow-lg);
        overflow: hidden;
    ">
        <div style="
            background: linear-gradient(135deg, var(--color-primary) 0%, var(--color-secondary) 100%);
            color: white;
            padding: var(--spacing-xl);
            text-align: center;
        ">
            <h1 style="font-size: 32px; margin-bottom: var(--spacing-sm);">
                ⚔️ Turn-Based Todo Game
            </h1>
            <p style="opacity: 0.9; font-size: 14px; margin: 0;">
                Every action is a turn. Server is the game master.
            </p>
        </div>

        <div id="stats-panel" hx-get="/turn/stats" hx-trigger="load, turnComplete from:body">
            @await Html.PartialAsync("_Stats")
        </div>

        <div style="padding: var(--spacing-xl);">
            <div style="
                background: var(--surface-secondary);
                padding: var(--spacing-lg);
                border-radius: var(--radius-md);
                margin-bottom: var(--spacing-xl);
            ">
                <h3 style="margin-bottom: var(--spacing-md); color: var(--text-primary); font-size: 18px;">
                    🎮 Your Turn: Add Todo
                </h3>
                <form hx-post="/turn/add"
                      hx-target="#todo-list"
                      hx-on::after-request="this.reset(); htmx.trigger('body', 'turnComplete')"
                      @Html.Raw(LayoutStyles.FlexRow(justify: "space-between"))>
                    <input type="text"
                           name="title"
                           placeholder="What needs to be done?"
                           required
                           autofocus
                           style="
                               flex: 1;
                               padding: var(--spacing-md);
                               border: 2px solid var(--surface-secondary);
                               border-radius: var(--radius-md);
                               font-size: var(--font-size-base);
                           ">
                    <button type="submit" @Html.Raw(ButtonStyles.Button())>
                        Add<span class="htmx-indicator"></span>
                    </button>
                </form>
            </div>
            <div id="todo-list">
                @await Html.PartialAsync("_TodoList", Model)
            </div>
        </div>
    </div>
</body>
</html>
```

### Example: Updated _TodoItem.cshtml

```cshtml
@model TurnBasedTodo.Models.Todo
@using TurnBasedTodo.Styles

<li id="todo-@Model.Id" @Html.Raw(TodoItemStyles.Container(Model.Completed))>
    <div class="todo-item__content" @Html.Raw(LayoutStyles.FlexRow(align: "center", justify: "space-between"))>
        <div @Html.Raw(LayoutStyles.FlexRow(gap: "var(--spacing-md)"))>
            <input type="checkbox"
                   checked="@Model.Completed"
                   @Html.Raw(TodoItemStyles.Checkbox())
                   hx-put="/turn/toggle/@Model.Id"
                   hx-target="#todo-@Model.Id"
                   hx-swap="outerHTML"
                   hx-on::after-request="htmx.trigger('body', 'turnComplete')" />
            <span @Html.Raw(TodoItemStyles.Title(Model.Completed))>
                @(Model.Completed ? "✅" : "⚪") @Model.Title
            </span>
        </div>
        <div class="todo-item__actions">
            <button hx-get="/turn/edit/@Model.Id"
                    hx-target="#todo-@Model.Id"
                    hx-swap="outerHTML"
                    @Html.Raw(ButtonStyles.Button(ButtonVariant.Secondary, ButtonSize.Small))>
                ✏️ Edit
            </button>
            <button hx-delete="/turn/delete/@Model.Id"
                    hx-target="#todo-@Model.Id"
                    hx-swap="outerHTML swap:1s"
                    hx-confirm="Delete this quest?"
                    hx-on::after-request="htmx.trigger('body', 'turnComplete')"
                    @Html.Raw(ButtonStyles.Button(ButtonVariant.Danger, ButtonSize.Small))>
                🗑️
            </button>
        </div>
    </div>
</li>
```

---

## Build-Time Validation

### StyleContractValidator.cs

Located in a test project or build task:

```csharp
using System.Text.RegularExpressions;
using Xunit;

namespace TurnBasedTodo.Tests.Styles;

public class StyleContractValidator
{
    private const string TokensPath = "wwwroot/css/01-tokens.css";
    private const string InteractionsPath = "wwwroot/css/02-interactions.css";

    [Fact]
    public void AllUsedTokensAreDefined()
    {
        // Get all defined tokens from tokens.css
        var tokensContent = File.ReadAllText(TokensPath);
        var definedTokens = ExtractDefinedTokens(tokensContent);

        // Get all style function files
        var styleFiles = Directory.GetFiles("Styles", "*.cs");

        foreach (var file in styleFiles)
        {
            var content = File.ReadAllText(file);
            var usedTokens = ExtractUsedTokens(content);

            foreach (var token in usedTokens)
            {
                Assert.Contains(token, definedTokens,
                    $"Token '{token}' used in {Path.GetFileName(file)} but not defined in tokens.css");
            }
        }
    }

    [Fact]
    public void InteractionsCssOnlyContainsPseudoClassesAndMediaQueries()
    {
        var content = File.ReadAllText(InteractionsPath);

        // Parse CSS and ensure all rules either:
        // 1. Contain pseudo-classes (:hover, :focus, etc.)
        // 2. Are inside @media queries
        // 3. Only set transition or cursor properties

        var rules = ParseCssRules(content);

        foreach (var rule in rules)
        {
            var isValid =
                rule.Selector.Contains(':') ||  // Pseudo-class
                rule.IsInMediaQuery ||
                rule.OnlySetsAllowedProperties(new[] { "transition", "cursor", "accent-color" });

            Assert.True(isValid,
                $"Rule '{rule.Selector}' in interactions.css violates layer constraints");
        }
    }

    [Fact]
    public void AllButtonVariantsHaveColorStyles()
    {
        var variants = Enum.GetValues<ButtonVariant>();

        foreach (var variant in variants)
        {
            var result = ButtonStyles.Button(variant);

            Assert.Contains("background:", result);
            Assert.Contains("color:", result);
        }
    }

    private static HashSet<string> ExtractDefinedTokens(string css)
    {
        var tokens = new HashSet<string>();
        var regex = new Regex(@"--[\w-]+(?=\s*:)");

        foreach (Match match in regex.Matches(css))
        {
            tokens.Add(match.Value);
        }

        return tokens;
    }

    private static HashSet<string> ExtractUsedTokens(string csharp)
    {
        var tokens = new HashSet<string>();
        var regex = new Regex(@"var\((--[\w-]+)\)");

        foreach (Match match in regex.Matches(csharp))
        {
            tokens.Add(match.Groups[1].Value);
        }

        return tokens;
    }

    private static List<CssRule> ParseCssRules(string css)
    {
        // Simplified CSS parser - in production use a proper CSS parser library
        var rules = new List<CssRule>();

        // Remove comments
        css = Regex.Replace(css, @"/\*.*?\*/", "", RegexOptions.Singleline);

        // Extract rules
        var rulePattern = new Regex(@"([^{]+)\{([^}]+)\}", RegexOptions.Singleline);
        var mediaPattern = new Regex(@"@media[^{]+\{");

        var matches = rulePattern.Matches(css);
        var inMediaQuery = false;

        foreach (Match match in matches)
        {
            var selector = match.Groups[1].Value.Trim();
            var properties = match.Groups[2].Value.Trim();

            if (mediaPattern.IsMatch(selector))
            {
                inMediaQuery = true;
                continue;
            }

            rules.Add(new CssRule
            {
                Selector = selector,
                Properties = properties,
                IsInMediaQuery = inMediaQuery
            });
        }

        return rules;
    }

    private class CssRule
    {
        public string Selector { get; set; } = "";
        public string Properties { get; set; } = "";
        public bool IsInMediaQuery { get; set; }

        public bool OnlySetsAllowedProperties(string[] allowed)
        {
            var propertyNames = Regex.Matches(Properties, @"([\w-]+)\s*:")
                .Select(m => m.Groups[1].Value.Trim())
                .ToList();

            return propertyNames.All(prop => allowed.Contains(prop));
        }
    }
}
```

---

## Migration Strategy

### Phase 1: Setup Infrastructure
1. Create `/Styles` directory for C# style functions
2. Create CSS layer files (tokens, interactions, animations)
3. Add build-time validation tests
4. Update `_ViewImports.cshtml` to include style namespaces

### Phase 2: Create Style Functions
1. Start with layout primitives (`LayoutStyles`)
2. Add button styles (`ButtonStyles`)
3. Create component-specific styles (`TodoItemStyles`, `StatsStyles`)

### Phase 3: Migrate Views
1. Update one partial view at a time
2. Replace inline styles with C# function calls
3. Add appropriate CSS classes where needed
4. Test responsiveness and interactions

### Phase 4: Validation & Refinement
1. Run validation tests
2. Fix any contract violations
3. Optimize token usage
4. Document patterns for team

---

## Benefits Summary

### Solves the "267 Button Problem"
**Change all buttons:** Edit one C# method
**Change mobile padding:** Edit tokens.css
**Change hover effects:** Edit interactions.css

### Type Safety & Refactoring
- Compile-time errors for breaking changes
- IDE refactoring support
- No string literal hunting

### Maintainability
- Clear separation of concerns
- Minimal CSS files (only what CSS must do)
- Single source of truth per concern
- Build validation prevents drift

### Team Scalability
- New developers use C# functions (type-safe, discoverable)
- Design changes happen in predictable places
- No CSS specificity wars
- No naming convention discipline required

### Performance
- No runtime CSS-in-JS overhead
- No large utility framework downloads
- Browser caching works normally
- Minimal CSS footprint

---

## Trade-offs Accepted

### What We Give Up
- Pure separation of HTML/CSS (we mix them intentionally)
- CSS-only workflow (designers need C# knowledge for structure)
- Inline style debugging in DevTools shows computed values, not source

### What We Gain
- Server-authoritative styling (matches our architecture)
- Type safety across entire styling system
- Refactoring tools work for styles
- Single point of change for design updates
- Build-time validation prevents errors

---

## Future Enhancements

### Potential Additions
1. **Visual Studio Extension**: Syntax highlighting for style strings
2. **Style Inspector Tool**: Shows all usages of a particular style function
3. **CSS Generator**: Generate static CSS from C# functions for email templates
4. **Design Token Sync**: Sync tokens with Figma or other design tools
5. **Performance Monitoring**: Track inline style size per page

### Open Questions
1. Should we cache style function outputs? (probably not needed)
2. Could we generate TypeScript definitions for frontend tooling?
3. Should component-specific CSS live in `/Styles/Components/` or `/wwwroot/css/components/`?

---

## Conclusion

This styling architecture leverages our server-authoritative design to solve CSS's fundamental problems in a novel way. By splitting responsibilities between C# (structure, logic, type safety) and CSS (adaptation, interaction, animation), we get:

- **Maintainability**: Single point of change
- **Type Safety**: Compile-time validation
- **Scalability**: Works for teams, not just individuals
- **Performance**: No runtime overhead
- **Simplicity**: Clear rules about what goes where

This is not just "inline styles with extra steps." It's a principled approach that uses each technology for what it does best, validated by build-time tooling that prevents the discipline failures that plague other CSS methodologies.
