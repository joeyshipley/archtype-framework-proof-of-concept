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
        generateBaseLayer(css);

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

    private static void generateBaseLayer(StringBuilder css)
    {
        css.AppendLine("/* ============================================================================");
        css.AppendLine("   BASE - Structural defaults");
        css.AppendLine("   ============================================================================ */");
        css.AppendLine();
        css.AppendLine("@layer base {");
        css.AppendLine("  /* Page structure */");
        css.AppendLine("  .page {");
        css.AppendLine("    max-width: 1200px;");
        css.AppendLine("    margin: 0 auto;");
        css.AppendLine("    padding: 0 var(--spacing-lg);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .section {");
        css.AppendLine("    display: block;");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .page-title,");
        css.AppendLine("  .section-title {");
        css.AppendLine("    margin: 0;");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  /* Layout primitives */");
        css.AppendLine("  .stack {");
        css.AppendLine("    display: flex;");
        css.AppendLine("    flex-direction: column;");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .row {");
        css.AppendLine("    display: flex;");
        css.AppendLine("    flex-direction: row;");
        css.AppendLine("    align-items: center;");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .grid {");
        css.AppendLine("    display: grid;");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  /* Containers */");
        css.AppendLine("  .card {");
        css.AppendLine("    display: flex;");
        css.AppendLine("    flex-direction: column;");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .header,");
        css.AppendLine("  .body,");
        css.AppendLine("  .footer {");
        css.AppendLine("    display: block;");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .footer {");
        css.AppendLine("    display: flex;");
        css.AppendLine("    align-items: center;");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  /* Interactive elements */");
        css.AppendLine("  .button {");
        css.AppendLine("    display: inline-flex;");
        css.AppendLine("    align-items: center;");
        css.AppendLine("    justify-content: center;");
        css.AppendLine("    border: none;");
        css.AppendLine("    cursor: pointer;");
        css.AppendLine("    font-family: inherit;");
        css.AppendLine("    transition: all var(--duration-fast) ease;");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  /* Text elements */");
        css.AppendLine("  .text {");
        css.AppendLine("    margin: 0;");
        css.AppendLine("  }");
        css.AppendLine("}");
        css.AppendLine();
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
        generateLayoutStyles(css);

        css.AppendLine("}");
    }

    private static void generateCardStyles(Dictionary<string, object> theme, StringBuilder css)
    {
        var card = getComponent(theme, "card");

        css.AppendLine("  /* Card structure */");
        css.AppendLine("  .card {");
        css.AppendLine($"    background: {getPropertyOrDefault(card, "base.background", "background", "var(--color-surface)")};");
        css.AppendLine($"    border-radius: {getPropertyOrDefault(card, "base.radius", "radius", "var(--radius-md)")};");
        css.AppendLine($"    box-shadow: {getPropertyOrDefault(card, "base.shadow", "shadow", "var(--shadow-sm)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .card > .header {");
        css.AppendLine($"    font-size: {getPropertyOrDefault(card, "header.size", "size", "var(--text-md)")};");
        css.AppendLine($"    font-weight: {getPropertyOrDefault(card, "header.weight", "weight", "var(--weight-semibold)")};");
        css.AppendLine($"    padding: {getPropertyOrDefault(card, "header.padding", "padding", "var(--spacing-4)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .card > .body {");
        css.AppendLine($"    padding: {getPropertyOrDefault(card, "body.padding", "padding", "var(--spacing-4)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .card > .footer {");
        css.AppendLine($"    padding: {getPropertyOrDefault(card, "footer.padding", "padding", "var(--spacing-4)")};");
        css.AppendLine($"    gap: {getPropertyOrDefault(card, "footer.gap", "gap", "var(--spacing-2)")};");
        css.AppendLine("    justify-content: flex-end;");
        css.AppendLine("  }");
        css.AppendLine();
    }

    private static void generateButtonStyles(Dictionary<string, object> theme, StringBuilder css)
    {
        var button = getComponent(theme, "button");

        css.AppendLine("  /* Button base */");
        css.AppendLine("  .button {");

        var paddingY = getPropertyOrDefault(button, "base.padding-y", "padding-y", "var(--spacing-2)");
        var paddingX = getPropertyOrDefault(button, "base.padding-x", "padding-x", "var(--spacing-4)");
        css.AppendLine($"    padding: {paddingY} {paddingX};");

        css.AppendLine($"    border-radius: {getPropertyOrDefault(button, "base.radius", "radius", "var(--radius-md)")};");
        css.AppendLine($"    font-weight: {getPropertyOrDefault(button, "base.weight", "weight", "var(--weight-medium)")};");
        css.AppendLine($"    font-size: {getPropertyOrDefault(button, "base.size", "size", "var(--text-md)")};");
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
        css.AppendLine($"    background: {getPropertyOrDefault(button, "importance-secondary.background", "background", "transparent")};");
        css.AppendLine($"    border: 1px solid {getPropertyOrDefault(button, "importance-secondary.border", "border", "var(--color-border)")};");
        css.AppendLine($"    color: {getPropertyOrDefault(button, "importance-secondary.text", "text", "var(--color-text-primary)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .button--secondary:hover:not(:disabled) {");
        css.AppendLine("    background: var(--color-surface-raised);");
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
        css.AppendLine("    background: var(--color-surface-raised);");
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
        css.AppendLine("  /* Text */");
        css.AppendLine("  .text {");
        css.AppendLine("    font-size: var(--text-md);");
        css.AppendLine("    color: var(--color-text-primary);");
        css.AppendLine("    line-height: 1.5;");
        css.AppendLine("  }");
        css.AppendLine();
    }

    private static void generateFormStyles(Dictionary<string, object> theme, StringBuilder css)
    {
        var input = getComponent(theme, "input");
        var label = getComponent(theme, "label");
        var checkbox = getComponent(theme, "checkbox");

        css.AppendLine("  /* Form elements */");

        // Input
        css.AppendLine("  .input {");
        css.AppendLine("    display: block;");
        css.AppendLine("    width: 100%;");

        var inputPaddingY = getPropertyOrDefault(input, "base.padding-y", "padding-y", "var(--spacing-2)");
        var inputPaddingX = getPropertyOrDefault(input, "base.padding-x", "padding-x", "var(--spacing-3)");
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
        css.AppendLine("    cursor: not-allowed;");
        css.AppendLine("  }");
        css.AppendLine();

        // Label
        css.AppendLine("  .label {");
        css.AppendLine("    display: block;");
        css.AppendLine($"    font-size: {getPropertyOrDefault(label, "base.size", "size", "var(--text-sm)")};");
        css.AppendLine($"    font-weight: {getPropertyOrDefault(label, "base.weight", "weight", "var(--weight-medium)")};");
        css.AppendLine($"    color: {getPropertyOrDefault(label, "base.color", "color", "var(--color-text-primary)")};");
        css.AppendLine($"    margin-bottom: {getPropertyOrDefault(label, "base.margin-bottom", "margin-bottom", "var(--spacing-1)")};");
        css.AppendLine("  }");
        css.AppendLine();

        // Field
        css.AppendLine("  .field {");
        css.AppendLine("    display: block;");
        css.AppendLine("    margin-bottom: var(--spacing-4);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .field--error .input {");
        css.AppendLine("    border-color: var(--color-critical);");
        css.AppendLine("    background: var(--color-critical-subtle);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .field__help {");
        css.AppendLine("    margin: var(--spacing-1) 0 0 0;");
        css.AppendLine("    font-size: var(--text-sm);");
        css.AppendLine("    color: var(--color-text-secondary);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .field__error {");
        css.AppendLine("    margin: var(--spacing-1) 0 0 0;");
        css.AppendLine("    font-size: var(--text-sm);");
        css.AppendLine("    color: var(--color-critical);");
        css.AppendLine("  }");
        css.AppendLine();

        // Form
        css.AppendLine("  .form {");
        css.AppendLine("    display: block;");
        css.AppendLine("  }");
        css.AppendLine();

        // Checkbox
        css.AppendLine("  .checkbox {");
        var checkboxSize = getPropertyOrDefault(checkbox, "base.size", "size", "var(--spacing-4)");
        css.AppendLine($"    width: {checkboxSize};");
        css.AppendLine($"    height: {checkboxSize};");
        css.AppendLine($"    border: 1px solid {getPropertyOrDefault(checkbox, "base.border", "border", "var(--color-border)")};");
        css.AppendLine($"    border-radius: {getPropertyOrDefault(checkbox, "base.radius", "radius", "var(--radius-sm)")};");
        css.AppendLine("    cursor: pointer;");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .checkbox:checked {");
        css.AppendLine($"    background: {getPropertyOrDefault(checkbox, "checked.background", "background", "var(--color-accent)")};");
        css.AppendLine($"    border-color: {getPropertyOrDefault(checkbox, "checked.border-color", "border", "var(--color-accent)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .checkbox:disabled {");
        css.AppendLine($"    opacity: {getPropertyOrDefault(checkbox, "disabled.opacity", "opacity", "var(--opacity-disabled)")};");
        css.AppendLine("    cursor: not-allowed;");
        css.AppendLine("  }");
        css.AppendLine();
    }

    private static void generateFeedbackStyles(Dictionary<string, object> theme, StringBuilder css)
    {
        css.AppendLine("  /* Feedback elements */");

        // Alert base
        css.AppendLine("  .alert {");
        css.AppendLine("    padding: var(--spacing-3) var(--spacing-4);");
        css.AppendLine("    border-radius: var(--radius-md);");
        css.AppendLine("    border-width: 1px;");
        css.AppendLine("    border-style: solid;");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .alert__message {");
        css.AppendLine("    margin: 0;");
        css.AppendLine("    font-size: var(--text-md);");
        css.AppendLine("  }");
        css.AppendLine();

        // Alert tone variants
        css.AppendLine("  .alert--neutral {");
        css.AppendLine("    background: var(--color-surface-raised);");
        css.AppendLine("    border-color: var(--color-border);");
        css.AppendLine("    color: var(--color-text-primary);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .alert--positive {");
        css.AppendLine("    background: var(--color-positive-subtle);");
        css.AppendLine("    border-color: var(--color-positive);");
        css.AppendLine("    color: var(--color-positive-dark);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .alert--warning {");
        css.AppendLine("    background: var(--color-warning-subtle);");
        css.AppendLine("    border-color: var(--color-warning);");
        css.AppendLine("    color: var(--color-warning-dark);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .alert--critical {");
        css.AppendLine("    background: var(--color-critical-subtle);");
        css.AppendLine("    border-color: var(--color-critical);");
        css.AppendLine("    color: var(--color-critical-dark);");
        css.AppendLine("  }");
        css.AppendLine();

        // EmptyState base
        css.AppendLine("  .empty-state {");
        css.AppendLine("    text-align: center;");
        css.AppendLine("    color: var(--color-text-secondary);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .empty-state__message {");
        css.AppendLine("    margin: 0;");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .empty-state__action {");
        css.AppendLine("    display: inline-block;");
        css.AppendLine("    margin-top: var(--spacing-3);");
        css.AppendLine("    color: var(--color-accent);");
        css.AppendLine("    text-decoration: underline;");
        css.AppendLine("  }");
        css.AppendLine();

        // EmptyState size variants
        css.AppendLine("  .empty-state--small {");
        css.AppendLine("    padding: var(--spacing-3);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .empty-state--small .empty-state__message {");
        css.AppendLine("    font-size: var(--text-sm);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .empty-state--medium {");
        css.AppendLine("    padding: var(--spacing-6);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .empty-state--medium .empty-state__message {");
        css.AppendLine("    font-size: var(--text-md);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .empty-state--large {");
        css.AppendLine("    padding: var(--spacing-8);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .empty-state--large .empty-state__message {");
        css.AppendLine("    font-size: var(--text-lg);");
        css.AppendLine("  }");
        css.AppendLine();
    }

    private static void generateListStyles(Dictionary<string, object> theme, StringBuilder css)
    {
        var listItem = getComponent(theme, "list-item");

        css.AppendLine("  /* List elements */");

        // List base
        css.AppendLine("  .list {");
        css.AppendLine("    margin: 0;");
        css.AppendLine("    padding: 0;");
        css.AppendLine("  }");
        css.AppendLine();

        // List style variants
        css.AppendLine("  .list--unordered {");
        css.AppendLine("    list-style: disc;");
        css.AppendLine("    padding-left: var(--spacing-xl);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .list--ordered {");
        css.AppendLine("    list-style: decimal;");
        css.AppendLine("    padding-left: var(--spacing-xl);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .list--plain {");
        css.AppendLine("    list-style: none;");
        css.AppendLine("    padding-left: 0;");
        css.AppendLine("  }");
        css.AppendLine();

        // ListItem base
        css.AppendLine("  .list-item {");
        var listItemPaddingY = getPropertyOrDefault(listItem, "base.padding-y", "padding-y", "var(--spacing-2)");
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
        css.AppendLine("    text-decoration: line-through;");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .list-item--disabled {");
        css.AppendLine($"    opacity: {getPropertyOrDefault(listItem, "state-disabled.opacity", "opacity", "var(--opacity-subdued)")};");
        css.AppendLine("    cursor: not-allowed;");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .list-item--error {");
        css.AppendLine("    background: var(--color-critical-subtle);");
        css.AppendLine("    border-left: 2px solid var(--color-critical);");
        css.AppendLine("    padding-left: var(--spacing-md);");
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
        css.AppendLine($"    margin-bottom: {getPropertyOrDefault(pageTitle, "base.margin-bottom", "margin-bottom", "var(--spacing-4)")};");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .section-title {");
        css.AppendLine($"    font-size: {getPropertyOrDefault(sectionTitle, "base.size", "size", "var(--text-xl)")};");
        css.AppendLine($"    font-weight: {getPropertyOrDefault(sectionTitle, "base.weight", "weight", "var(--weight-semibold)")};");
        css.AppendLine($"    color: {getPropertyOrDefault(sectionTitle, "base.color", "color", "var(--color-text-primary)")};");
        css.AppendLine($"    margin-bottom: {getPropertyOrDefault(sectionTitle, "base.margin-bottom", "margin-bottom", "var(--spacing-3)")};");
        css.AppendLine("  }");
        css.AppendLine();
    }

    private static void generateLayoutStyles(StringBuilder css)
    {
        css.AppendLine("  /* Stack - purpose-based spacing */");
        css.AppendLine("  .stack--actions {");
        css.AppendLine("    gap: var(--spacing-sm);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .stack--fields {");
        css.AppendLine("    gap: var(--spacing-lg);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .stack--content {");
        css.AppendLine("    gap: var(--spacing-md);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .stack--items {");
        css.AppendLine("    gap: var(--spacing-md);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .stack--sections {");
        css.AppendLine("    gap: var(--spacing-3xl);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .stack--inline {");
        css.AppendLine("    gap: var(--spacing-xs);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .stack--cards {");
        css.AppendLine("    gap: var(--spacing-2xl);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  /* Row - purpose-based spacing */");
        css.AppendLine("  .row--actions {");
        css.AppendLine("    gap: var(--spacing-sm);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .row--fields {");
        css.AppendLine("    gap: var(--spacing-lg);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .row--inline {");
        css.AppendLine("    gap: var(--spacing-xs);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  /* Grid - purpose-based spacing and columns */");
        css.AppendLine("  .grid--cards {");
        css.AppendLine("    gap: var(--spacing-2xl);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .grid--items {");
        css.AppendLine("    gap: var(--spacing-md);");
        css.AppendLine("  }");
        css.AppendLine();
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
        css.AppendLine("  .grid--cols-auto {");
        css.AppendLine("    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));");
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
            ["opacity"] = "--opacity-",
            ["background"] = "--color-",
            ["border"] = "--color-",
            ["color"] = "--color-",
            ["text"] = "--color-",
        };

        // If this property maps to a CSS variable, resolve it
        if (cssVarMapping.TryGetValue(property, out var prefix))
        {
            // Check if the value looks like a token name (not a raw value like "1" or "1px")
            if (!valueStr.Contains("px") && !valueStr.Contains("#") && !valueStr.Contains("rgba") && !int.TryParse(valueStr, out _))
            {
                return $"var({prefix}{valueStr})";
            }
            // If it's a number and refers to spacing/padding/margin, treat as spacing token
            else if (int.TryParse(valueStr, out _) && (property.Contains("padding") || property.Contains("margin") || property == "gap"))
            {
                return $"var(--spacing-{valueStr})";
            }
        }

        // Return raw value for things like "1" (opacity: 1), URLs, etc.
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
}
