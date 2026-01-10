using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace PagePlay.Site.Infrastructure.UI.Rendering;

/// <summary>
/// Compiles theme YAML files into CSS.
/// Transforms semantic theme definitions into generated stylesheets.
/// </summary>
public class ThemeCompiler
{
    public static string CompileTheme(string yamlFilePath)
    {
        if (!File.Exists(yamlFilePath))
            throw new FileNotFoundException($"Theme file not found: {yamlFilePath}");

        var yamlContent = File.ReadAllText(yamlFilePath);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(HyphenatedNamingConvention.Instance)
            .Build();

        var theme = deserializer.Deserialize<Dictionary<string, object>>(yamlContent);

        var css = new StringBuilder();
        css.AppendLine("/*");
        css.AppendLine(" * Closed-World UI Stylesheet");
        css.AppendLine(" *");
        css.AppendLine(" * AUTO-GENERATED from theme file - DO NOT EDIT");
        css.AppendLine($" * Source: {Path.GetFileName(yamlFilePath)}");
        css.AppendLine(" * Generated: " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " UTC");
        css.AppendLine(" */");
        css.AppendLine();
        css.AppendLine("@layer tokens, base, components;");
        css.AppendLine();

        // Generate tokens layer
        generateTokensLayer(theme, css);

        // Generate base layer
        generateBaseLayer(theme, css);

        // Generate components layer
        generateComponentsLayer(theme, css);

        return css.ToString();
    }

    private static void generateTokensLayer(Dictionary<string, object> theme, StringBuilder css)
    {
        css.AppendLine("/* ============================================================================");
        css.AppendLine("   TOKENS - Design primitives");
        css.AppendLine("   ============================================================================ */");
        css.AppendLine();
        css.AppendLine("@layer tokens {");
        css.AppendLine("  :root {");

        if (theme.TryGetValue("tokens", out var tokensObj) && tokensObj is Dictionary<object, object> tokens)
        {
            // Spacing tokens
            if (tokens.TryGetValue("spacing", out var spacingObj) && spacingObj is Dictionary<object, object> spacing)
            {
                css.AppendLine("    /* Spacing */");
                foreach (var kvp in spacing.OrderBy(k => k.Key.ToString()))
                {
                    css.AppendLine($"    --spacing-{kvp.Key}: {kvp.Value};");
                }
                css.AppendLine();
            }

            // Text tokens
            if (tokens.TryGetValue("text", out var textObj) && textObj is Dictionary<object, object> text)
            {
                css.AppendLine("    /* Typography */");
                foreach (var kvp in text)
                {
                    css.AppendLine($"    --text-{kvp.Key}: {kvp.Value};");
                }
                css.AppendLine();
            }

            // Font tokens
            if (tokens.TryGetValue("font", out var fontObj) && fontObj is Dictionary<object, object> font)
            {
                css.AppendLine("    /* Font Weights */");
                foreach (var kvp in font)
                {
                    css.AppendLine($"    --{kvp.Key}: {kvp.Value};");
                }
                css.AppendLine();
            }

            // Color tokens
            if (tokens.TryGetValue("color", out var colorObj) && colorObj is Dictionary<object, object> colors)
            {
                css.AppendLine("    /* Colors */");
                foreach (var kvp in colors)
                {
                    css.AppendLine($"    --color-{kvp.Key}: {kvp.Value};");
                }
                css.AppendLine();
            }

            // Shadow tokens
            if (tokens.TryGetValue("shadow", out var shadowObj) && shadowObj is Dictionary<object, object> shadows)
            {
                css.AppendLine("    /* Shadows */");
                foreach (var kvp in shadows)
                {
                    css.AppendLine($"    --shadow-{kvp.Key}: {kvp.Value};");
                }
                css.AppendLine();
            }

            // Radius tokens
            if (tokens.TryGetValue("radius", out var radiusObj) && radiusObj is Dictionary<object, object> radius)
            {
                css.AppendLine("    /* Radius */");
                foreach (var kvp in radius)
                {
                    css.AppendLine($"    --radius-{kvp.Key}: {kvp.Value};");
                }
                css.AppendLine();
            }

            // Duration tokens
            if (tokens.TryGetValue("duration", out var durationObj) && durationObj is Dictionary<object, object> durations)
            {
                css.AppendLine("    /* Duration */");
                foreach (var kvp in durations)
                {
                    css.AppendLine($"    --duration-{kvp.Key}: {kvp.Value};");
                }
                css.AppendLine();
            }

            // Opacity tokens
            if (tokens.TryGetValue("opacity", out var opacityObj) && opacityObj is Dictionary<object, object> opacities)
            {
                css.AppendLine("    /* Opacity */");
                foreach (var kvp in opacities)
                {
                    css.AppendLine($"    --opacity-{kvp.Key}: {kvp.Value};");
                }
            }
        }

        css.AppendLine("  }");
        css.AppendLine("}");
        css.AppendLine();
    }

    private static void generateBaseLayer(Dictionary<string, object> theme, StringBuilder css)
    {
        css.AppendLine("/* ============================================================================");
        css.AppendLine("   BASE - Structural defaults");
        css.AppendLine("   ============================================================================ */");
        css.AppendLine();
        css.AppendLine("@layer base {");

        // Page structure
        var page = getComponent(theme, "page");
        css.AppendLine("  /* Page structure */");
        css.AppendLine("  .page {");
        css.AppendLine($"    max-width: {getMaxWidthValue(page, "base.max-width", "1200px")};");

        // Margin handling - "auto" should remain literal, not resolve to token
        var marginValue = page != null ? getComponentProperty(page, "base.margin") : null;
        var margin = marginValue?.ToString() == "auto" ? "0 auto" : getPropertyOrDefault(page, "base.margin", "margin", "0 auto");
        css.AppendLine($"    margin: {margin};");

        var paddingX = getPropertyOrDefault(page, "base.padding-x", "padding-x", "var(--spacing-lg)");
        var paddingY = page != null ? getComponentProperty(page, "base.padding-y")?.ToString() : null;
        paddingY = paddingY == "0" ? "0" : getPropertyOrDefault(page, "base.padding-y", "padding-y", "0");
        css.AppendLine($"    padding: {paddingY} {paddingX};");
        css.AppendLine("  }");
        css.AppendLine();

        var section = getComponent(theme, "section");
        css.AppendLine("  .section {");
        css.AppendLine($"    display: {getDisplayValue(section, "base.display", "block")};");

        var sectionMarginBottomRaw = section != null ? getComponentProperty(section, "base.margin-bottom") : null;
        var sectionMarginBottom = sectionMarginBottomRaw?.ToString() == "0" ? "0" : getPropertyOrDefault(section, "base.margin-bottom", "margin-bottom", "0");
        if (sectionMarginBottom != "0") {
            css.AppendLine($"    margin-bottom: {sectionMarginBottom};");
        }
        css.AppendLine("  }");
        css.AppendLine();

        // Page title and section title margins
        var pageTitle = getComponent(theme, "page-title");
        var sectionTitle = getComponent(theme, "section-title");
        css.AppendLine("  .page-title,");
        css.AppendLine("  .section-title {");
        var titleMarginRaw = pageTitle != null ? getComponentProperty(pageTitle, "base.margin") : null;
        var titleMargin = titleMarginRaw?.ToString() == "0" ? "0" : getPropertyOrDefault(pageTitle, "base.margin", "margin", "0");
        css.AppendLine($"    margin: {titleMargin};");
        css.AppendLine("  }");
        css.AppendLine();

        // Layout primitives
        var stack = getComponent(theme, "stack");
        css.AppendLine("  /* Layout primitives */");
        css.AppendLine("  .stack {");
        css.AppendLine($"    display: {getDisplayValue(stack, "base.display", "flex")};");
        css.AppendLine($"    flex-direction: {getFlexDirectionValue(stack, "base.flex-direction", "column")};");
        css.AppendLine("  }");
        css.AppendLine();

        var row = getComponent(theme, "row");
        css.AppendLine("  .row {");
        css.AppendLine($"    display: {getDisplayValue(row, "base.display", "flex")};");
        css.AppendLine($"    flex-direction: {getFlexDirectionValue(row, "base.flex-direction", "row")};");
        css.AppendLine($"    align-items: {getAlignItemsValue(row, "base.align-items", "center")};");
        css.AppendLine("  }");
        css.AppendLine();

        var grid = getComponent(theme, "grid");
        css.AppendLine("  .grid {");
        css.AppendLine($"    display: {getDisplayValue(grid, "base.display", "grid")};");
        css.AppendLine("  }");
        css.AppendLine();

        // Containers
        var card = getComponent(theme, "card");
        css.AppendLine("  /* Containers */");
        css.AppendLine("  .card {");
        css.AppendLine($"    display: {getDisplayValue(card, "base.display", "flex")};");
        css.AppendLine($"    flex-direction: {getFlexDirectionValue(card, "base.flex-direction", "column")};");
        css.AppendLine("  }");
        css.AppendLine();

        var header = getComponent(theme, "header");
        var body = getComponent(theme, "body");
        var footer = getComponent(theme, "footer");
        css.AppendLine("  .header,");
        css.AppendLine("  .body,");
        css.AppendLine("  .footer {");
        css.AppendLine($"    display: {getDisplayValue(header, "base.display", "block")};");
        css.AppendLine("  }");
        css.AppendLine();

        css.AppendLine("  .footer {");
        css.AppendLine($"    display: {getDisplayValue(footer, "base.display", "flex")};");
        css.AppendLine($"    align-items: {getAlignItemsValue(footer, "base.align-items", "center")};");
        css.AppendLine("  }");
        css.AppendLine();

        // Interactive elements
        var button = getComponent(theme, "button");
        css.AppendLine("  /* Interactive elements */");
        css.AppendLine("  .button {");
        css.AppendLine($"    display: {getDisplayValue(button, "base.display", "inline-flex")};");
        css.AppendLine($"    align-items: {getAlignItemsValue(button, "base.align-items", "center")};");
        css.AppendLine($"    justify-content: {getJustifyContentValue(button, "base.justify-content", "center")};");

        // Border handling - "none" is a literal value
        var borderRaw = button != null ? getComponentProperty(button, "base.border") : null;
        var border = borderRaw?.ToString() == "none" ? "none" : getPropertyOrDefault(button, "base.border", "border", "none");
        css.AppendLine($"    border: {border};");

        css.AppendLine($"    cursor: {getCursorValue(button, "base.cursor", "pointer")};");

        var fontFamily = button != null && getComponentProperty(button, "base.font-family") != null
            ? getComponentProperty(button, "base.font-family").ToString()
            : "inherit";
        css.AppendLine($"    font-family: {fontFamily};");

        // Transition handling - support separate properties or combined
        var transitionProp = (button != null ? getComponentProperty(button, "base.transition-property")?.ToString() : null) ?? "all";
        var transitionDuration = getPropertyOrDefault(button, "base.transition-duration", "transition-duration", "var(--duration-fast)");
        var transitionTiming = (button != null ? getComponentProperty(button, "base.transition-timing")?.ToString() : null) ?? "ease";
        css.AppendLine($"    transition: {transitionProp} {transitionDuration} {transitionTiming};");
        css.AppendLine("  }");
        css.AppendLine();

        // Text elements
        var text = getComponent(theme, "text");
        css.AppendLine("  /* Text elements */");
        css.AppendLine("  .text {");
        var textMarginRaw = text != null ? getComponentProperty(text, "base.margin") : null;
        var textMargin = textMarginRaw?.ToString() == "0" ? "0" : getPropertyOrDefault(text, "base.margin", "margin", "0");
        css.AppendLine($"    margin: {textMargin};");
        css.AppendLine("  }");
        css.AppendLine();

        // HTMX framework styles
        generateHtmxStyles(theme, css);

        css.AppendLine("}");
        css.AppendLine();
    }

    private static void generateHtmxStyles(Dictionary<string, object> theme, StringBuilder css)
    {
        css.AppendLine("  /* HTMX framework styles */");
        css.AppendLine("  .htmx-indicator {");
        css.AppendLine("    opacity: 0;");
        css.AppendLine("    transition: opacity var(--duration-slow) ease-in;");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .htmx-request .htmx-indicator {");
        css.AppendLine("    opacity: 1;");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .htmx-request.htmx-indicator {");
        css.AppendLine("    opacity: 1;");
        css.AppendLine("  }");
    }

    private static void generateComponentsLayer(Dictionary<string, object> theme, StringBuilder css)
    {
        css.AppendLine("/* ============================================================================");
        css.AppendLine("   COMPONENTS - Theme-controlled appearance");
        css.AppendLine("   ============================================================================ */");
        css.AppendLine();
        css.AppendLine("@layer components {");

        // Card styling
        generateCardStyles(theme, css);

        // Button styling
        generateButtonStyles(theme, css);

        // Text styling
        generateTextStyles(theme, css);

        // Form element styling
        generateFormStyles(theme, css);

        // Feedback element styling
        generateFeedbackStyles(theme, css);

        // List element styling
        generateListStyles(theme, css);

        // Page structure styling
        generatePageStructureStyles(theme, css);

        // Layout primitive styling
        generateLayoutStyles(theme, css);

        css.AppendLine("}");
    }

    private static void generateCardStyles(Dictionary<string, object> theme, StringBuilder css)
    {
        var card = getComponent(theme, "card");

        css.AppendLine("  /* Card structure */");
        css.AppendLine("  .card {");
        css.AppendLine($"    background: {getPropertyOrDefault(card, "base.background", "background", "var(--color-surface)")};");
        css.AppendLine($"    border-radius: {getPropertyOrDefault(card, "base.radius", "radius", "var(--radius-md)")};");

        // Border support - Flowbite uses subtle border instead of heavy shadows
        var borderValue = card != null ? getComponentProperty(card, "base.border") : null;
        if (borderValue != null)
        {
            var borderColor = resolvePropertyValue("border", borderValue);
            css.AppendLine($"    border: 1px solid {borderColor};");
        }

        // Shadow - can be "none" to disable, or a shadow token
        var shadowValue = card != null ? getComponentProperty(card, "base.shadow") : null;
        if (shadowValue?.ToString() != "none")
        {
            css.AppendLine($"    box-shadow: {getPropertyOrDefault(card, "base.shadow", "shadow", "var(--shadow-sm)")};");
        }

        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .card > .header {");
        css.AppendLine($"    font-size: {getPropertyOrDefault(card, "header.size", "size", "var(--text-md)")};");
        css.AppendLine($"    font-weight: {getPropertyOrDefault(card, "header.weight", "weight", "var(--weight-semibold)")};");
        css.AppendLine($"    padding: {getPropertyOrDefault(card, "header.padding", "padding", "var(--spacing-lg)")};");

        // Header border-bottom for slot separation
        var headerBorderBottom = card != null ? getComponentProperty(card, "header.border-bottom") : null;
        if (headerBorderBottom != null)
        {
            var borderColor = resolvePropertyValue("border", headerBorderBottom);
            css.AppendLine($"    border-bottom: 1px solid {borderColor};");
        }

        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .card > .body {");
        css.AppendLine($"    padding: {getPropertyOrDefault(card, "body.padding", "padding", "var(--spacing-lg)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .card > .footer {");
        css.AppendLine($"    padding: {getPropertyOrDefault(card, "footer.padding", "padding", "var(--spacing-lg)")};");
        css.AppendLine($"    gap: {getPropertyOrDefault(card, "footer.gap", "gap", "var(--spacing-sm)")};");
        css.AppendLine("    justify-content: flex-end;");

        // Footer border-top for slot separation
        var footerBorderTop = card != null ? getComponentProperty(card, "footer.border-top") : null;
        if (footerBorderTop != null)
        {
            var borderColor = resolvePropertyValue("border", footerBorderTop);
            css.AppendLine($"    border-top: 1px solid {borderColor};");
        }

        css.AppendLine("  }");
        css.AppendLine();
    }

    private static void generateButtonStyles(Dictionary<string, object> theme, StringBuilder css)
    {
        var button = getComponent(theme, "button");

        css.AppendLine("  /* Button base */");
        css.AppendLine("  .button {");

        var paddingY = getPropertyOrDefault(button, "base.padding-y", "padding-y", "var(--spacing-sm)");
        var paddingX = getPropertyOrDefault(button, "base.padding-x", "padding-x", "var(--spacing-lg)");
        css.AppendLine($"    padding: {paddingY} {paddingX};");

        css.AppendLine($"    border-radius: {getPropertyOrDefault(button, "base.radius", "radius", "var(--radius-md)")};");
        css.AppendLine($"    font-weight: {getPropertyOrDefault(button, "base.weight", "weight", "var(--weight-medium)")};");
        css.AppendLine($"    font-size: {getPropertyOrDefault(button, "base.size", "size", "var(--text-md)")};");
        css.AppendLine("  }");
        css.AppendLine();

        // Focus state - Flowbite-style focus ring
        css.AppendLine("  /* Button focus state */");
        css.AppendLine("  .button:focus {");
        css.AppendLine("    outline: none;");

        // Get focus ring configuration from theme
        var ringWidth = button != null ? getComponentProperty(button, "focus.ring-width")?.ToString() : null;
        var ringColor = button != null ? getComponentProperty(button, "focus.ring-color")?.ToString() : null;
        var ringOpacity = button != null ? getComponentProperty(button, "focus.ring-opacity")?.ToString() : null;

        ringWidth ??= "4px";
        var ringColorVar = ringColor != null ? $"var(--color-{ringColor})" : "var(--color-accent)";
        ringOpacity ??= "0.3";

        // Use box-shadow for the focus ring (Flowbite pattern)
        css.AppendLine($"    box-shadow: 0 0 0 {ringWidth} rgba(37, 99, 235, {ringOpacity});");
        css.AppendLine("  }");
        css.AppendLine();

        css.AppendLine("  /* Button: Primary importance */");
        css.AppendLine("  .button--primary {");
        css.AppendLine($"    background: {getPropertyOrDefault(button, "importance-primary.background", "background", "var(--color-accent)")};");
        css.AppendLine($"    color: {getPropertyOrDefault(button, "importance-primary.text", "text", "white")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .button--primary:hover:not(:disabled) {");
        css.AppendLine($"    background: {getPropertyOrDefault(button, "importance-primary.background-hover", "background", "var(--color-accent-hover)")};");
        css.AppendLine("  }");
        css.AppendLine();

        css.AppendLine("  /* Button: Secondary importance */");
        css.AppendLine("  .button--secondary {");
        css.AppendLine($"    background: {getPropertyOrDefault(button, "importance-secondary.background", "background", "var(--color-surface)")};");
        css.AppendLine($"    border: 1px solid {getPropertyOrDefault(button, "importance-secondary.border", "border", "var(--color-border)")};");
        css.AppendLine($"    color: {getPropertyOrDefault(button, "importance-secondary.text", "text", "var(--color-text-primary)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .button--secondary:hover:not(:disabled) {");
        css.AppendLine($"    background: {getPropertyOrDefault(button, "importance-secondary.background-hover", "background", "var(--color-surface-raised)")};");
        css.AppendLine("  }");
        css.AppendLine();

        css.AppendLine("  /* Button: Tertiary importance */");
        css.AppendLine("  .button--tertiary {");
        css.AppendLine($"    background: {getPropertyOrDefault(button, "importance-tertiary.background", "background", "transparent")};");
        css.AppendLine($"    color: {getPropertyOrDefault(button, "importance-tertiary.text", "text", "var(--color-text-secondary)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .button--tertiary:hover:not(:disabled) {");
        css.AppendLine($"    color: {getPropertyOrDefault(button, "importance-tertiary.text-hover", "text", "var(--color-text-primary)")};");
        css.AppendLine($"    background: {getPropertyOrDefault(button, "importance-tertiary.background-hover", "background", "var(--color-surface-raised)")};");
        css.AppendLine("  }");
        css.AppendLine();

        css.AppendLine("  /* Button: Ghost importance */");
        css.AppendLine("  .button--ghost {");
        css.AppendLine($"    background: {getPropertyOrDefault(button, "importance-ghost.background", "background", "transparent")};");
        css.AppendLine($"    color: {getPropertyOrDefault(button, "importance-ghost.text", "text", "var(--color-text-secondary)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .button--ghost:hover:not(:disabled) {");
        css.AppendLine($"    color: {getPropertyOrDefault(button, "importance-ghost.text-hover", "text", "var(--color-accent)")};");
        var ghostHoverBg = getPropertyOrDefault(button, "importance-ghost.background-hover", "background", "transparent");
        if (ghostHoverBg != "transparent")
        {
            css.AppendLine($"    background: {ghostHoverBg};");
        }
        css.AppendLine("  }");
        css.AppendLine();

        css.AppendLine("  /* Button states */");
        css.AppendLine("  .button:disabled,");
        css.AppendLine("  .button--disabled {");
        css.AppendLine("    opacity: var(--opacity-disabled);");
        css.AppendLine("    cursor: not-allowed;");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .button--loading {");
        css.AppendLine("    position: relative;");
        css.AppendLine("    color: transparent;");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .button--loading::after {");
        css.AppendLine("    content: \"\";");
        css.AppendLine("    position: absolute;");
        css.AppendLine("    width: 1rem;");
        css.AppendLine("    height: 1rem;");
        css.AppendLine("    border: 2px solid currentColor;");
        css.AppendLine("    border-radius: 50%;");
        css.AppendLine("    border-top-color: transparent;");
        css.AppendLine("    animation: button-spinner 600ms linear infinite;");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  @keyframes button-spinner {");
        css.AppendLine("    to {");
        css.AppendLine("      transform: rotate(360deg);");
        css.AppendLine("    }");
        css.AppendLine("  }");
        css.AppendLine();
    }

    private static void generateTextStyles(Dictionary<string, object> theme, StringBuilder css)
    {
        var text = getComponent(theme, "text");

        css.AppendLine("  /* Text */");
        css.AppendLine("  .text {");
        css.AppendLine($"    font-size: {getPropertyOrDefault(text, "base.size", "size", "var(--text-md)")};");
        css.AppendLine($"    color: {getPropertyOrDefault(text, "base.color", "color", "var(--color-text-primary)")};");
        css.AppendLine($"    line-height: {getLineHeightValue(text, "base.line-height", "1.5")};");
        css.AppendLine("  }");
        css.AppendLine();
    }

    private static void generateFormStyles(Dictionary<string, object> theme, StringBuilder css)
    {
        var input = getComponent(theme, "input");
        var label = getComponent(theme, "label");
        var field = getComponent(theme, "field");
        var checkbox = getComponent(theme, "checkbox");

        css.AppendLine("  /* Form elements */");

        // Input
        css.AppendLine("  .input {");
        css.AppendLine("    display: block;");
        css.AppendLine($"    width: {getWidthValue(input, "base.width", "100%")};");

        var inputPaddingY = getPropertyOrDefault(input, "base.padding-y", "padding-y", "var(--spacing-sm)");
        var inputPaddingX = getPropertyOrDefault(input, "base.padding-x", "padding-x", "var(--spacing-md)");
        css.AppendLine($"    padding: {inputPaddingY} {inputPaddingX};");

        css.AppendLine($"    border: 1px solid {getPropertyOrDefault(input, "base.border", "border", "var(--color-border)")};");
        css.AppendLine($"    border-radius: {getPropertyOrDefault(input, "base.radius", "radius", "var(--radius-md)")};");
        css.AppendLine($"    font-size: {getPropertyOrDefault(input, "base.size", "size", "var(--text-md)")};");
        css.AppendLine("    font-family: inherit;");
        css.AppendLine("    line-height: 1.5;");
        css.AppendLine("    color: var(--color-text-primary);");
        css.AppendLine("    background: var(--color-surface);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .input:focus {");
        css.AppendLine("    outline: 2px solid var(--color-accent);");
        css.AppendLine("    outline-offset: 2px;");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .input:disabled {");
        css.AppendLine($"    opacity: {getPropertyOrDefault(input, "state-disabled.opacity", "opacity", "var(--opacity-disabled)")};");
        css.AppendLine($"    cursor: {getCursorValue(input, "state-disabled.cursor", "not-allowed")};");
        css.AppendLine("  }");
        css.AppendLine();

        // Label
        css.AppendLine("  .label {");
        css.AppendLine("    display: block;");
        css.AppendLine($"    font-size: {getPropertyOrDefault(label, "base.size", "size", "var(--text-sm)")};");
        css.AppendLine($"    font-weight: {getPropertyOrDefault(label, "base.weight", "weight", "var(--weight-medium)")};");
        css.AppendLine($"    color: {getPropertyOrDefault(label, "base.color", "color", "var(--color-text-primary)")};");
        css.AppendLine($"    margin-bottom: {getPropertyOrDefault(label, "base.margin-bottom", "margin-bottom", "var(--spacing-xs)")};");
        css.AppendLine("  }");
        css.AppendLine();

        // Field
        css.AppendLine("  .field {");
        css.AppendLine("    display: block;");
        css.AppendLine($"    gap: {getPropertyOrDefault(field, "base.gap", "gap", "var(--spacing-xs)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .field--error .input {");
        css.AppendLine($"    border-color: {getPropertyOrDefault(field, "state-error.input.border-color", "border", "var(--color-critical)")};");
        css.AppendLine($"    background: {getPropertyOrDefault(field, "state-error.input.background", "background", "var(--color-critical-subtle)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .field__help {");
        css.AppendLine($"    margin: {getPropertyOrDefault(field, "help-text.margin-top", "margin-top", "var(--spacing-xs)")} 0 0 0;");
        css.AppendLine($"    font-size: {getPropertyOrDefault(field, "help-text.size", "size", "var(--text-sm)")};");
        css.AppendLine($"    color: {getPropertyOrDefault(field, "help-text.color", "color", "var(--color-text-secondary)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .field__error {");
        css.AppendLine($"    margin: {getPropertyOrDefault(field, "error-message.margin-top", "margin-top", "var(--spacing-xs)")} 0 0 0;");
        css.AppendLine($"    font-size: {getPropertyOrDefault(field, "error-message.size", "size", "var(--text-sm)")};");
        css.AppendLine($"    color: {getPropertyOrDefault(field, "error-message.color", "color", "var(--color-critical)")};");
        css.AppendLine("  }");
        css.AppendLine();

        // Form
        css.AppendLine("  .form {");
        css.AppendLine("    display: block;");
        css.AppendLine("  }");
        css.AppendLine();

        // Checkbox
        css.AppendLine("  .checkbox {");
        var checkboxSize = getPropertyOrDefault(checkbox, "base.size", "size", "var(--spacing-lg)");
        css.AppendLine($"    width: {checkboxSize};");
        css.AppendLine($"    height: {checkboxSize};");
        css.AppendLine($"    border: 1px solid {getPropertyOrDefault(checkbox, "base.border", "border", "var(--color-border)")};");
        css.AppendLine($"    border-radius: {getPropertyOrDefault(checkbox, "base.radius", "radius", "var(--radius-sm)")};");
        css.AppendLine($"    cursor: {getCursorValue(checkbox, "base.cursor", "pointer")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .checkbox:checked {");
        css.AppendLine($"    background: {getPropertyOrDefault(checkbox, "checked.background", "background", "var(--color-accent)")};");
        css.AppendLine($"    border-color: {getPropertyOrDefault(checkbox, "checked.border-color", "border", "var(--color-accent)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .checkbox:disabled {");
        css.AppendLine($"    opacity: {getPropertyOrDefault(checkbox, "disabled.opacity", "opacity", "var(--opacity-disabled)")};");
        css.AppendLine($"    cursor: {getCursorValue(checkbox, "disabled.cursor", "not-allowed")};");
        css.AppendLine("  }");
        css.AppendLine();
    }

    private static void generateFeedbackStyles(Dictionary<string, object> theme, StringBuilder css)
    {
        var alert = getComponent(theme, "alert");

        css.AppendLine("  /* Feedback elements */");

        // Alert base
        css.AppendLine("  .alert {");
        var alertPaddingY = getPropertyOrDefault(alert, "base.padding-y", "padding-y", "var(--spacing-md)");
        var alertPaddingX = getPropertyOrDefault(alert, "base.padding-x", "padding-x", "var(--spacing-lg)");
        css.AppendLine($"    padding: {alertPaddingY} {alertPaddingX};");
        css.AppendLine($"    border-radius: {getPropertyOrDefault(alert, "base.radius", "radius", "var(--radius-md)")};");
        css.AppendLine($"    border-width: {getBorderWidthValue(alert, "base.border-width", "1px")};");
        css.AppendLine("    border-style: solid;");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .alert__message {");
        css.AppendLine($"    margin: {getPropertyOrDefault(alert, "message.margin", "margin", "0")};");
        css.AppendLine($"    font-size: {getPropertyOrDefault(alert, "message.size", "size", "var(--text-md)")};");
        css.AppendLine("  }");
        css.AppendLine();

        // Alert tone variants
        css.AppendLine("  .alert--neutral {");
        css.AppendLine($"    background: {getPropertyOrDefault(alert, "tone-neutral.background", "background", "var(--color-surface-raised)")};");
        css.AppendLine($"    border-color: {getPropertyOrDefault(alert, "tone-neutral.border-color", "border", "var(--color-border)")};");
        css.AppendLine($"    color: {getPropertyOrDefault(alert, "tone-neutral.text", "text", "var(--color-text-primary)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .alert--positive {");
        css.AppendLine($"    background: {getPropertyOrDefault(alert, "tone-positive.background", "background", "var(--color-positive-subtle)")};");
        css.AppendLine($"    border-color: {getPropertyOrDefault(alert, "tone-positive.border-color", "border", "var(--color-positive)")};");
        css.AppendLine($"    color: {getPropertyOrDefault(alert, "tone-positive.text", "text", "var(--color-positive-dark)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .alert--warning {");
        css.AppendLine($"    background: {getPropertyOrDefault(alert, "tone-warning.background", "background", "var(--color-warning-subtle)")};");
        css.AppendLine($"    border-color: {getPropertyOrDefault(alert, "tone-warning.border-color", "border", "var(--color-warning)")};");
        css.AppendLine($"    color: {getPropertyOrDefault(alert, "tone-warning.text", "text", "var(--color-warning-dark)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .alert--critical {");
        css.AppendLine($"    background: {getPropertyOrDefault(alert, "tone-critical.background", "background", "var(--color-critical-subtle)")};");
        css.AppendLine($"    border-color: {getPropertyOrDefault(alert, "tone-critical.border-color", "border", "var(--color-critical)")};");
        css.AppendLine($"    color: {getPropertyOrDefault(alert, "tone-critical.text", "text", "var(--color-critical-dark)")};");
        css.AppendLine("  }");
        css.AppendLine();

        // EmptyState base
        var emptyState = getComponent(theme, "empty-state");

        css.AppendLine("  .empty-state {");
        css.AppendLine($"    text-align: {getTextAlignValue(emptyState, "base.text-align", "center")};");
        css.AppendLine($"    color: {getPropertyOrDefault(emptyState, "base.color", "color", "var(--color-text-secondary)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .empty-state__message {");
        css.AppendLine($"    margin: {getPropertyOrDefault(emptyState, "message.margin-bottom", "margin", "0")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .empty-state__action {");
        css.AppendLine("    display: inline-block;");
        css.AppendLine($"    margin-top: {getPropertyOrDefault(emptyState, "action.margin-top", "margin-top", "var(--spacing-md)")};");
        css.AppendLine($"    color: {getPropertyOrDefault(emptyState, "action.color", "color", "var(--color-accent)")};");
        css.AppendLine($"    text-decoration: {getTextDecorationValue(emptyState, "action.text-decoration", "underline")};");
        css.AppendLine("  }");
        css.AppendLine();

        // EmptyState size variants
        css.AppendLine("  .empty-state--small {");
        css.AppendLine($"    padding: {getPropertyOrDefault(emptyState, "size-small.padding", "padding", "var(--spacing-md)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .empty-state--small .empty-state__message {");
        css.AppendLine($"    font-size: {getPropertyOrDefault(emptyState, "size-small.text-size", "size", "var(--text-sm)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .empty-state--medium {");
        css.AppendLine($"    padding: {getPropertyOrDefault(emptyState, "size-medium.padding", "padding", "var(--spacing-2xl)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .empty-state--medium .empty-state__message {");
        css.AppendLine($"    font-size: {getPropertyOrDefault(emptyState, "size-medium.text-size", "size", "var(--text-md)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .empty-state--large {");
        css.AppendLine($"    padding: {getPropertyOrDefault(emptyState, "size-large.padding", "padding", "var(--spacing-3xl)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .empty-state--large .empty-state__message {");
        css.AppendLine($"    font-size: {getPropertyOrDefault(emptyState, "size-large.text-size", "size", "var(--text-lg)")};");
        css.AppendLine("  }");
        css.AppendLine();
    }

    private static void generateListStyles(Dictionary<string, object> theme, StringBuilder css)
    {
        var list = getComponent(theme, "list");
        var listItem = getComponent(theme, "list-item");

        css.AppendLine("  /* List elements */");

        // List base
        css.AppendLine("  .list {");
        css.AppendLine($"    margin: {getPropertyOrDefault(list, "base.margin", "margin", "0")};");
        css.AppendLine($"    padding: {getPropertyOrDefault(list, "base.padding", "padding", "0")};");
        css.AppendLine("  }");
        css.AppendLine();

        // List style variants
        css.AppendLine("  .list--unordered {");
        css.AppendLine($"    list-style: {getListStyleValue(list, "style-unordered.list-style", "disc")};");
        css.AppendLine($"    padding-left: {getPropertyOrDefault(list, "style-unordered.padding-left", "padding", "var(--spacing-xl)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .list--ordered {");
        css.AppendLine($"    list-style: {getListStyleValue(list, "style-ordered.list-style", "decimal")};");
        css.AppendLine($"    padding-left: {getPropertyOrDefault(list, "style-ordered.padding-left", "padding", "var(--spacing-xl)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .list--plain {");
        css.AppendLine($"    list-style: {getListStyleValue(list, "style-plain.list-style", "none")};");
        css.AppendLine($"    padding-left: {getPropertyOrDefault(list, "style-plain.padding-left", "padding", "0")};");
        css.AppendLine("  }");
        css.AppendLine();

        // ListItem base
        css.AppendLine("  .list-item {");
        var listItemPaddingY = getPropertyOrDefault(listItem, "base.padding-y", "padding-y", "var(--spacing-sm)");
        var listItemPaddingX = getPropertyOrDefault(listItem, "base.padding-x", "padding-x", "0");
        css.AppendLine($"    padding: {listItemPaddingY} {listItemPaddingX};");
        css.AppendLine("  }");
        css.AppendLine();

        // ListItem state variants
        css.AppendLine("  .list-item--normal {");
        css.AppendLine($"    opacity: {getPropertyOrDefault(listItem, "state-normal.opacity", "opacity", "1")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .list-item--completed {");
        css.AppendLine($"    opacity: {getPropertyOrDefault(listItem, "state-completed.opacity", "opacity", "var(--opacity-subtle)")};");
        css.AppendLine($"    text-decoration: {getTextDecorationValue(listItem, "state-completed.text-decoration", "line-through")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .list-item--disabled {");
        css.AppendLine($"    opacity: {getPropertyOrDefault(listItem, "state-disabled.opacity", "opacity", "var(--opacity-subdued)")};");
        css.AppendLine($"    cursor: {getCursorValue(listItem, "state-disabled.cursor", "not-allowed")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .list-item--error {");
        css.AppendLine($"    background: {getPropertyOrDefault(listItem, "state-error.background", "background", "var(--color-critical-subtle)")};");
        css.AppendLine($"    border-left: {getBorderWidthValue(listItem, "state-error.border-left-width", "2px")} solid {getPropertyOrDefault(listItem, "state-error.border-left-color", "border", "var(--color-critical)")};");
        css.AppendLine($"    padding-left: {getPropertyOrDefault(listItem, "state-error.padding-left", "padding", "var(--spacing-md)")};");
        css.AppendLine("  }");
        css.AppendLine();
    }

    private static void generatePageStructureStyles(Dictionary<string, object> theme, StringBuilder css)
    {
        var pageTitle = getComponent(theme, "page-title");
        var sectionTitle = getComponent(theme, "section-title");

        css.AppendLine("  /* Page structure */");
        css.AppendLine("  .page {");
        css.AppendLine("    padding-top: var(--spacing-3xl);");
        css.AppendLine("    padding-bottom: var(--spacing-3xl);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .section {");
        css.AppendLine("    margin-bottom: var(--spacing-3xl);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .page-title {");
        css.AppendLine($"    font-size: {getPropertyOrDefault(pageTitle, "base.size", "size", "var(--text-2xl)")};");
        css.AppendLine($"    font-weight: {getPropertyOrDefault(pageTitle, "base.weight", "weight", "var(--weight-bold)")};");
        css.AppendLine($"    color: {getPropertyOrDefault(pageTitle, "base.color", "color", "var(--color-text-primary)")};");
        css.AppendLine($"    margin-bottom: {getPropertyOrDefault(pageTitle, "base.margin-bottom", "margin-bottom", "var(--spacing-lg)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .section-title {");
        css.AppendLine($"    font-size: {getPropertyOrDefault(sectionTitle, "base.size", "size", "var(--text-xl)")};");
        css.AppendLine($"    font-weight: {getPropertyOrDefault(sectionTitle, "base.weight", "weight", "var(--weight-semibold)")};");
        css.AppendLine($"    color: {getPropertyOrDefault(sectionTitle, "base.color", "color", "var(--color-text-primary)")};");
        css.AppendLine($"    margin-bottom: {getPropertyOrDefault(sectionTitle, "base.margin-bottom", "margin-bottom", "var(--spacing-md)")};");
        css.AppendLine("  }");
        css.AppendLine();
    }

    private static void generateLayoutStyles(Dictionary<string, object> theme, StringBuilder css)
    {
        var layout = getComponent(theme, "layout");

        // Stack - purpose-based spacing
        css.AppendLine("  /* Stack - purpose-based spacing */");
        css.AppendLine("  .stack--actions {");
        css.AppendLine($"    gap: {getPropertyOrDefault(layout, "stack.purpose-actions", "gap", "var(--spacing-sm)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .stack--fields {");
        css.AppendLine($"    gap: {getPropertyOrDefault(layout, "stack.purpose-fields", "gap", "var(--spacing-lg)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .stack--content {");
        css.AppendLine($"    gap: {getPropertyOrDefault(layout, "stack.purpose-content", "gap", "var(--spacing-md)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .stack--items {");
        css.AppendLine($"    gap: {getPropertyOrDefault(layout, "stack.purpose-items", "gap", "var(--spacing-md)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .stack--sections {");
        css.AppendLine($"    gap: {getPropertyOrDefault(layout, "stack.purpose-sections", "gap", "var(--spacing-3xl)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .stack--inline {");
        css.AppendLine($"    gap: {getPropertyOrDefault(layout, "stack.purpose-inline", "gap", "var(--spacing-xs)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .stack--cards {");
        css.AppendLine($"    gap: {getPropertyOrDefault(layout, "stack.purpose-cards", "gap", "var(--spacing-2xl)")};");
        css.AppendLine("  }");
        css.AppendLine();

        // Row - purpose-based spacing
        css.AppendLine("  /* Row - purpose-based spacing */");
        css.AppendLine("  .row--actions {");
        css.AppendLine($"    gap: {getPropertyOrDefault(layout, "row.purpose-actions", "gap", "var(--spacing-sm)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .row--fields {");
        css.AppendLine($"    gap: {getPropertyOrDefault(layout, "row.purpose-fields", "gap", "var(--spacing-lg)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .row--inline {");
        css.AppendLine($"    gap: {getPropertyOrDefault(layout, "row.purpose-inline", "gap", "var(--spacing-xs)")};");
        css.AppendLine("  }");
        css.AppendLine();

        // Grid - purpose-based spacing and columns
        css.AppendLine("  /* Grid - purpose-based spacing and columns */");
        css.AppendLine("  .grid--cards {");
        css.AppendLine($"    gap: {getPropertyOrDefault(layout, "grid.purpose-cards", "gap", "var(--spacing-2xl)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .grid--items {");
        css.AppendLine($"    gap: {getPropertyOrDefault(layout, "grid.purpose-items", "gap", "var(--spacing-md)")};");
        css.AppendLine("  }");
        css.AppendLine();

        // Grid column configurations
        css.AppendLine("  .grid--cols-1 {");
        css.AppendLine("    grid-template-columns: 1fr;");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .grid--cols-2 {");
        css.AppendLine("    grid-template-columns: repeat(2, 1fr);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .grid--cols-3 {");
        css.AppendLine("    grid-template-columns: repeat(3, 1fr);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .grid--cols-4 {");
        css.AppendLine("    grid-template-columns: repeat(4, 1fr);");
        css.AppendLine("  }");
        css.AppendLine();

        // Grid auto columns with configurable minwidth
        var colsAutoMinwidth = getPropertyOrDefault(layout, "grid.cols-auto-minwidth", "cols-auto-minwidth", "300");
        css.AppendLine("  .grid--cols-auto {");
        css.AppendLine($"    grid-template-columns: repeat(auto-fit, minmax({colsAutoMinwidth}px, 1fr));");
        css.AppendLine("  }");
        css.AppendLine();
    }

    // ============================================================================
    // Component Mapping Helper Methods
    // ============================================================================

    /// <summary>
    /// Gets a component mapping from the theme dictionary.
    /// Returns null if the component doesn't exist.
    /// </summary>
    private static Dictionary<object, object> getComponent(Dictionary<string, object> theme, string componentName)
    {
        if (theme.TryGetValue(componentName, out var componentObj) && componentObj is Dictionary<object, object> component)
            return component;
        return null;
    }

    /// <summary>
    /// Gets a nested property from a component mapping (e.g., "base.weight").
    /// Returns null if the path doesn't exist.
    /// </summary>
    private static object getComponentProperty(Dictionary<object, object> component, string path)
    {
        var parts = path.Split('.');
        object current = component;

        foreach (var part in parts)
        {
            if (current is not Dictionary<object, object> dict)
                return null;

            if (!dict.TryGetValue(part, out var next))
                return null;

            current = next;
        }

        return current;
    }

    /// <summary>
    /// Resolves a property value to a CSS variable reference.
    /// Maps semantic names to CSS custom properties (e.g., "semibold" -> "var(--weight-semibold)").
    /// </summary>
    private static string resolvePropertyValue(string property, object value)
    {
        if (value == null)
            return "";

        var valueStr = value.ToString() ?? "";

        // Map property names to their CSS variable prefixes
        var cssVarMapping = new Dictionary<string, string>
        {
            ["weight"] = "--weight-",
            ["size"] = "--text-",
            ["padding"] = "--spacing-",
            ["padding-x"] = "--spacing-",
            ["padding-y"] = "--spacing-",
            ["gap"] = "--spacing-",
            ["margin"] = "--spacing-",
            ["margin-bottom"] = "--spacing-",
            ["margin-top"] = "--spacing-",
            ["radius"] = "--radius-",
            ["shadow"] = "--shadow-",
            ["duration"] = "--duration-",
            ["transition-duration"] = "--duration-",
            ["opacity"] = "--opacity-",
            ["background"] = "--color-",
            ["border"] = "--color-",
            ["color"] = "--color-",
            ["text"] = "--color-",
        };

        // CSS keywords that should stay as literal values (not converted to tokens)
        var cssKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "transparent", "inherit", "initial", "unset", "none", "auto", "currentColor"
        };

        // If the value is a CSS keyword, return it as-is
        if (cssKeywords.Contains(valueStr))
        {
            return valueStr;
        }

        // If this property maps to a CSS variable, resolve it
        if (cssVarMapping.TryGetValue(property, out var prefix))
        {
            // Check if the value looks like a token name (not a raw value like "1px" or "#fff")
            // Named tokens: xs, sm, md, lg, xl, 2xl, 3xl, semibold, disabled, fast, etc.
            if (!valueStr.Contains("px") && !valueStr.Contains("#") && !valueStr.Contains("rgba"))
            {
                // If it's a plain integer without units, assume it's a raw value (like opacity: 1 or border-width: 1)
                // But if it's spacing/padding/margin/gap, it should have been a named token in the YAML
                if (int.TryParse(valueStr, out var numValue))
                {
                    // For spacing properties, if someone still uses a number, treat it as pixels
                    if (property.Contains("padding") || property.Contains("margin") || property == "gap")
                    {
                        // This is likely an error - spacing should use named tokens (xs, sm, md, lg, etc.)
                        // But we'll convert to pixels as a fallback for backwards compatibility
                        return $"{valueStr}px";
                    }
                    // For other numeric values (opacity, border-width, etc.), return as-is
                    return valueStr;
                }

                // Named token - resolve to CSS variable
                return $"var({prefix}{valueStr})";
            }
        }

        // Return raw value for things like "1px", "#fff", "rgba(...)", etc.
        return valueStr;
    }

    /// <summary>
    /// Gets a CSS property value from component mapping, with fallback to default.
    /// </summary>
    private static string getPropertyOrDefault(Dictionary<object, object> component, string path, string property, string defaultValue)
    {
        if (component == null)
            return defaultValue;

        var value = getComponentProperty(component, path);
        if (value == null)
            return defaultValue;

        var resolved = resolvePropertyValue(property, value);
        return string.IsNullOrEmpty(resolved) ? defaultValue : resolved;
    }

    /// <summary>
    /// Gets a line-height value from component mapping, with fallback to default.
    /// Line-height is special because it can be a unitless number or a unit value, and doesn't map to tokens.
    /// </summary>
    private static string getLineHeightValue(Dictionary<object, object> component, string path, string defaultValue)
    {
        if (component == null)
            return defaultValue;

        var value = getComponentProperty(component, path);
        if (value == null)
            return defaultValue;

        return value.ToString() ?? defaultValue;
    }

    /// <summary>
    /// Gets a border-width value from component mapping, with fallback to default.
    /// Border-width is typically a unitless number that needs "px" appended.
    /// </summary>
    private static string getBorderWidthValue(Dictionary<object, object> component, string path, string defaultValue)
    {
        if (component == null)
            return defaultValue;

        var value = getComponentProperty(component, path);
        if (value == null)
            return defaultValue;

        var valueStr = value.ToString() ?? defaultValue;

        // If it's a plain number, append "px"
        if (int.TryParse(valueStr, out _))
            return $"{valueStr}px";

        return valueStr;
    }

    /// <summary>
    /// Gets a text-align value from component mapping, with fallback to default.
    /// Text-align doesn't map to tokens and returns raw values like "center", "left", "right", "justify".
    /// </summary>
    private static string getTextAlignValue(Dictionary<object, object> component, string path, string defaultValue)
    {
        if (component == null)
            return defaultValue;

        var value = getComponentProperty(component, path);
        if (value == null)
            return defaultValue;

        return value.ToString() ?? defaultValue;
    }

    /// <summary>
    /// Gets a text-decoration value from component mapping, with fallback to default.
    /// Text-decoration doesn't map to tokens and returns raw values like "none", "underline", "line-through".
    /// </summary>
    private static string getTextDecorationValue(Dictionary<object, object> component, string path, string defaultValue)
    {
        if (component == null)
            return defaultValue;

        var value = getComponentProperty(component, path);
        if (value == null)
            return defaultValue;

        return value.ToString() ?? defaultValue;
    }

    /// <summary>
    /// Gets a list-style value from component mapping, with fallback to default.
    /// List-style doesn't map to tokens and returns raw values like "none", "disc", "decimal", "circle", "square".
    /// </summary>
    private static string getListStyleValue(Dictionary<object, object> component, string path, string defaultValue)
    {
        if (component == null)
            return defaultValue;

        var value = getComponentProperty(component, path);
        if (value == null)
            return defaultValue;

        return value.ToString() ?? defaultValue;
    }

    /// <summary>
    /// Gets a cursor value from component mapping, with fallback to default.
    /// Cursor doesn't map to tokens and returns raw values like "pointer", "not-allowed", "default", "text".
    /// </summary>
    private static string getCursorValue(Dictionary<object, object> component, string path, string defaultValue)
    {
        if (component == null)
            return defaultValue;

        var value = getComponentProperty(component, path);
        if (value == null)
            return defaultValue;

        return value.ToString() ?? defaultValue;
    }

    /// <summary>
    /// Gets a width value from component mapping, with fallback to default.
    /// Width doesn't map to tokens and returns raw values like "100%", "50%", "200px", "auto".
    /// </summary>
    private static string getWidthValue(Dictionary<object, object> component, string path, string defaultValue)
    {
        if (component == null)
            return defaultValue;

        var value = getComponentProperty(component, path);
        if (value == null)
            return defaultValue;

        return value.ToString() ?? defaultValue;
    }

    /// <summary>
    /// Gets a display value from component mapping, with fallback to default.
    /// Display doesn't map to tokens and returns raw values like "block", "flex", "inline-flex", "grid", "none".
    /// </summary>
    private static string getDisplayValue(Dictionary<object, object> component, string path, string defaultValue)
    {
        if (component == null)
            return defaultValue;

        var value = getComponentProperty(component, path);
        if (value == null)
            return defaultValue;

        return value.ToString() ?? defaultValue;
    }

    /// <summary>
    /// Gets a flex-direction value from component mapping, with fallback to default.
    /// Flex-direction doesn't map to tokens and returns raw values like "row", "column", "row-reverse", "column-reverse".
    /// </summary>
    private static string getFlexDirectionValue(Dictionary<object, object> component, string path, string defaultValue)
    {
        if (component == null)
            return defaultValue;

        var value = getComponentProperty(component, path);
        if (value == null)
            return defaultValue;

        return value.ToString() ?? defaultValue;
    }

    /// <summary>
    /// Gets an align-items value from component mapping, with fallback to default.
    /// Align-items doesn't map to tokens and returns raw values like "center", "flex-start", "flex-end", "stretch", "baseline".
    /// </summary>
    private static string getAlignItemsValue(Dictionary<object, object> component, string path, string defaultValue)
    {
        if (component == null)
            return defaultValue;

        var value = getComponentProperty(component, path);
        if (value == null)
            return defaultValue;

        return value.ToString() ?? defaultValue;
    }

    /// <summary>
    /// Gets a justify-content value from component mapping, with fallback to default.
    /// Justify-content doesn't map to tokens and returns raw values like "center", "flex-start", "flex-end", "space-between", "space-around".
    /// </summary>
    private static string getJustifyContentValue(Dictionary<object, object> component, string path, string defaultValue)
    {
        if (component == null)
            return defaultValue;

        var value = getComponentProperty(component, path);
        if (value == null)
            return defaultValue;

        return value.ToString() ?? defaultValue;
    }

    /// <summary>
    /// Gets a border-style value from component mapping, with fallback to default.
    /// Border-style doesn't map to tokens and returns raw values like "solid", "dashed", "dotted", "none".
    /// </summary>
    private static string getBorderStyleValue(Dictionary<object, object> component, string path, string defaultValue)
    {
        if (component == null)
            return defaultValue;

        var value = getComponentProperty(component, path);
        if (value == null)
            return defaultValue;

        return value.ToString() ?? defaultValue;
    }

    /// <summary>
    /// Gets a max-width value from component mapping, with fallback to default.
    /// Max-width doesn't map to tokens and returns raw values like "1200px", "100%", "none".
    /// If the value is a plain number, it's treated as pixels and "px" is appended.
    /// </summary>
    private static string getMaxWidthValue(Dictionary<object, object> component, string path, string defaultValue)
    {
        if (component == null)
            return defaultValue;

        var value = getComponentProperty(component, path);
        if (value == null)
            return defaultValue;

        var valueStr = value.ToString() ?? defaultValue;

        // If it's a plain number, append "px"
        if (int.TryParse(valueStr, out _))
            return $"{valueStr}px";

        return valueStr;
    }

    /// <summary>
    /// Gets a grid-template-columns value from component mapping, with fallback to default.
    /// Grid-template-columns doesn't map to tokens and returns raw values like "repeat(2, 1fr)", "minmax(300px, 1fr)".
    /// </summary>
    private static string getGridTemplateColumnsValue(Dictionary<object, object> component, string path, string defaultValue)
    {
        if (component == null)
            return defaultValue;

        var value = getComponentProperty(component, path);
        if (value == null)
            return defaultValue;

        return value.ToString() ?? defaultValue;
    }
}
