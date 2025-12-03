namespace PagePlay.Site.Infrastructure.Web.Html;

/// <summary>
/// Utilities for managing HTML fragments with OOB (Out-Of-Band) attributes.
/// Supports both single-fragment and multi-fragment OOB responses for component-first architecture.
/// </summary>
public static class HtmlFragment
{
    /// <summary>
    /// Wraps HTML content in a div with the specified ID.
    /// Use when you need a targetable container but don't need OOB updates.
    /// </summary>
    /// <param name="id">Element ID for targeting</param>
    /// <param name="content">Inner HTML content</param>
    /// <returns>HTML string with id attribute</returns>
    public static string WithId(string id, string content) =>
        $$"""
        <div id="{{id}}">
            {{content}}
        </div>
        """;

    /// <summary>
    /// Wraps HTML content in a div with both id and hx-swap-oob attributes.
    /// Use for explicit OOB updates when BuildHtmlFragmentResult() isn't sufficient.
    /// </summary>
    /// <param name="id">Element ID for OOB targeting</param>
    /// <param name="content">Inner HTML content</param>
    /// <param name="swapStrategy">OOB swap strategy (defaults to "true" which uses innerHTML)</param>
    /// <returns>HTML string with id and hx-swap-oob attributes</returns>
    public static string WithOob(string id, string content, string swapStrategy = "true") =>
        $$"""
        <div id="{{id}}" hx-swap-oob="{{swapStrategy}}">
            {{content}}
        </div>
        """;

    /// <summary>
    /// Injects hx-swap-oob attribute into existing HTML at the first element's opening tag.
    /// Use when you have complete HTML with an id and need to add OOB attribute.
    /// </summary>
    /// <param name="html">HTML fragment with an id attribute</param>
    /// <param name="swapStrategy">OOB swap strategy (defaults to "true")</param>
    /// <returns>HTML with hx-swap-oob attribute injected</returns>
    /// <exception cref="InvalidOperationException">Thrown when HTML fragment does not contain an id attribute</exception>
    public static string InjectOob(string html, string swapStrategy = "true")
    {
        // Find first id="..." and inject hx-swap-oob after it
        var idPattern = @"id=""([^""]+)""";
        var match = System.Text.RegularExpressions.Regex.Match(html, idPattern);

        if (!match.Success)
            throw new InvalidOperationException(
                "Cannot inject OOB attribute: HTML fragment does not contain an id attribute");

        return html.Replace(
            match.Value,
            $"{match.Value} hx-swap-oob=\"{swapStrategy}\""
        );
    }
}
