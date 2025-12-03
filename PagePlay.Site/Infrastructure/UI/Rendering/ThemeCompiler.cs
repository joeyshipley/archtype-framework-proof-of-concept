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
        css.AppendLine("    padding: 0 var(--spacing-4);");
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
        css.AppendLine("    transition: all 150ms ease;");
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

        // Page structure styling
        generatePageStructureStyles(css);

        // Layout primitive styling
        generateLayoutStyles(css);

        css.AppendLine("}");
    }

    private static void generateCardStyles(Dictionary<string, object> theme, StringBuilder css)
    {
        css.AppendLine("  /* Card structure */");
        css.AppendLine("  .card {");
        css.AppendLine("    background: var(--color-surface);");
        css.AppendLine("    border-radius: var(--radius-md);");
        css.AppendLine("    box-shadow: var(--shadow-sm);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .card > .header {");
        css.AppendLine("    font-size: var(--text-md);");
        css.AppendLine("    font-weight: 600;");
        css.AppendLine("    padding: var(--spacing-4);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .card > .body {");
        css.AppendLine("    padding: var(--spacing-4);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .card > .footer {");
        css.AppendLine("    padding: var(--spacing-4);");
        css.AppendLine("    gap: var(--spacing-2);");
        css.AppendLine("    justify-content: flex-end;");
        css.AppendLine("  }");
        css.AppendLine();
    }

    private static void generateButtonStyles(Dictionary<string, object> theme, StringBuilder css)
    {
        css.AppendLine("  /* Button base */");
        css.AppendLine("  .button {");
        css.AppendLine("    padding: var(--spacing-2) var(--spacing-4);");
        css.AppendLine("    border-radius: var(--radius-md);");
        css.AppendLine("    font-weight: 500;");
        css.AppendLine("    font-size: var(--text-md);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  /* Button: Primary importance */");
        css.AppendLine("  .button--primary {");
        css.AppendLine("    background: var(--color-accent);");
        css.AppendLine("    color: white;");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .button--primary:hover:not(:disabled) {");
        css.AppendLine("    background: var(--color-accent-hover);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  /* Button: Secondary importance */");
        css.AppendLine("  .button--secondary {");
        css.AppendLine("    background: transparent;");
        css.AppendLine("    border: 1px solid var(--color-border);");
        css.AppendLine("    color: var(--color-text-primary);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .button--secondary:hover:not(:disabled) {");
        css.AppendLine("    background: var(--color-surface-raised);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  /* Button: Tertiary importance */");
        css.AppendLine("  .button--tertiary {");
        css.AppendLine("    background: transparent;");
        css.AppendLine("    color: var(--color-text-secondary);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .button--tertiary:hover:not(:disabled) {");
        css.AppendLine("    color: var(--color-text-primary);");
        css.AppendLine("    background: var(--color-surface-raised);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  /* Button: Ghost importance */");
        css.AppendLine("  .button--ghost {");
        css.AppendLine("    background: transparent;");
        css.AppendLine("    color: var(--color-text-secondary);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .button--ghost:hover:not(:disabled) {");
        css.AppendLine("    color: var(--color-accent);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  /* Button states */");
        css.AppendLine("  .button:disabled,");
        css.AppendLine("  .button--disabled {");
        css.AppendLine("    opacity: 0.5;");
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

    private static void generatePageStructureStyles(StringBuilder css)
    {
        css.AppendLine("  /* Page structure */");
        css.AppendLine("  .page {");
        css.AppendLine("    padding-top: var(--spacing-8);");
        css.AppendLine("    padding-bottom: var(--spacing-8);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .section {");
        css.AppendLine("    margin-bottom: var(--spacing-8);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .page-title {");
        css.AppendLine("    font-size: 2rem;");
        css.AppendLine("    font-weight: 700;");
        css.AppendLine("    color: var(--color-text-primary);");
        css.AppendLine("    margin-bottom: var(--spacing-4);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .section-title {");
        css.AppendLine("    font-size: 1.5rem;");
        css.AppendLine("    font-weight: 600;");
        css.AppendLine("    color: var(--color-text-primary);");
        css.AppendLine("    margin-bottom: var(--spacing-3);");
        css.AppendLine("  }");
        css.AppendLine();
    }

    private static void generateLayoutStyles(StringBuilder css)
    {
        css.AppendLine("  /* Stack - purpose-based spacing */");
        css.AppendLine("  .stack--actions {");
        css.AppendLine("    gap: var(--spacing-2);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .stack--fields {");
        css.AppendLine("    gap: var(--spacing-4);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .stack--content {");
        css.AppendLine("    gap: var(--spacing-3);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .stack--items {");
        css.AppendLine("    gap: var(--spacing-3);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .stack--sections {");
        css.AppendLine("    gap: var(--spacing-8);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .stack--inline {");
        css.AppendLine("    gap: var(--spacing-1);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .stack--cards {");
        css.AppendLine("    gap: var(--spacing-6);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  /* Row - purpose-based spacing */");
        css.AppendLine("  .row--actions {");
        css.AppendLine("    gap: var(--spacing-2);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .row--fields {");
        css.AppendLine("    gap: var(--spacing-4);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .row--inline {");
        css.AppendLine("    gap: var(--spacing-1);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  /* Grid - purpose-based spacing and columns */");
        css.AppendLine("  .grid--cards {");
        css.AppendLine("    gap: var(--spacing-6);");
        css.AppendLine("  }");
        css.AppendLine();
        css.AppendLine("  .grid--items {");
        css.AppendLine("    gap: var(--spacing-3);");
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
}
