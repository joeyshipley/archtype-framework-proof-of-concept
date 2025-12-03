namespace PagePlay.Site.Infrastructure.Web.Html;

/// <summary>
/// Configuration for HTMX-enabled forms. Provides a consistent, declarative way
/// to create forms with HTMX attributes without needing to remember attribute syntax.
/// </summary>
public class HtmxFormData
{
    /// <summary>
    /// The endpoint URL to POST to (required)
    /// </summary>
    public required string Action { get; init; }

    /// <summary>
    /// The CSS selector for the target element (optional - omit for OOB-only responses)
    /// </summary>
    public string Target { get; init; }

    /// <summary>
    /// How to swap the response into the target. Defaults to "innerHTML".
    /// Common values: "innerHTML", "outerHTML", "afterbegin", "beforeend"
    /// Note: "morph:innerHTML" requires idiomorph extension and client-side sorting for
    /// server-authoritative list ordering. Use standard swap strategies for simplicity.
    /// </summary>
    public string SwapStrategy { get; init; } = "innerHTML";

    /// <summary>
    /// HTMX extensions to enable (e.g., "morph" for morphdom support)
    /// </summary>
    public string HxExt { get; init; }

    /// <summary>
    /// The form's HTML id attribute
    /// </summary>
    public string Id { get; init; }

    /// <summary>
    /// CSS class(es) to apply to the form
    /// </summary>
    public string CssClass { get; init; }
}

/// <summary>
/// Helper for rendering HTMX-enabled forms with consistent attribute patterns.
/// Reduces complexity for junior developers by abstracting HTMX syntax.
/// </summary>
public static class HtmxForm
{
    /// <summary>
    /// Renders an HTMX-enabled form with POST method.
    /// </summary>
    /// <param name="data">Form configuration with HTMX attributes</param>
    /// <param name="content">The HTML content inside the form (inputs, buttons, etc.)</param>
    /// <returns>HTML string for the complete form element</returns>
    // language=html
    public static string Render(HtmxFormData data, string content)
    {
        var idAttr = !string.IsNullOrEmpty(data.Id) ? $"id=\"{data.Id}\"" : "";
        var classAttr = !string.IsNullOrEmpty(data.CssClass) ? $"class=\"{data.CssClass}\"" : "";
        var extAttr = !string.IsNullOrEmpty(data.HxExt) ? $"hx-ext=\"{data.HxExt}\"" : "";

        // Conditionally render target and swap - omit for OOB-only responses
        var targetAttr = !string.IsNullOrEmpty(data.Target)
            ? $"hx-target=\"{data.Target}\""
            : "";
        var swapAttr = !string.IsNullOrEmpty(data.Target)
            ? $"hx-swap=\"{data.SwapStrategy}\""
            : "";

        return $$"""
        <form {{idAttr}}
              {{classAttr}}
              hx-post="{{data.Action}}"
              {{targetAttr}}
              {{swapAttr}}
              {{extAttr}}>
            {{content}}
        </form>
        """;
    }
}
