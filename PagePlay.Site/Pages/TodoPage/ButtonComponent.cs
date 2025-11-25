namespace PagePlay.Site.Pages.TodoPage;

public class Button
{
    // language=html
    public static string Render(
        string content,
        string endpoint,
        string antiforgeryToken,
        string targetSelector,
        string httpMethod = "post",
        string swapStrategy = "morph:innerHTML",
        string? cssClass = null,
        string? title = null,
        Dictionary<string, object>? additionalData = null
    )
    {
        var classAttr = !string.IsNullOrEmpty(cssClass) ? $"class=\"{cssClass}\"" : "";
        var titleAttr = !string.IsNullOrEmpty(title) ? $"title=\"{title}\"" : "";

        // Automatically include antiforgery token
        var data = new Dictionary<string, object>
        {
            { "__RequestVerificationToken", antiforgeryToken }
        };

        if (additionalData != null)
        {
            foreach (var kvp in additionalData)
                data[kvp.Key] = kvp.Value;
        }

        // Build hx-vals JSON
        var jsonPairs = data.Select(kvp =>
            kvp.Value is string
                ? $"\"{kvp.Key}\": \"{kvp.Value}\""
                : $"\"{kvp.Key}\": {kvp.Value}");
        var json = string.Join(", ", jsonPairs);
        var hxValsAttr = $"hx-vals='{{ {json} }}'";

        return $$"""
        <button hx-{{httpMethod}}="{{endpoint}}"
                hx-target="{{targetSelector}}"
                hx-swap="{{swapStrategy}}"
                {{hxValsAttr}}
                {{classAttr}}
                {{titleAttr}}>
            {{content}}
        </button>
        """;
    }
}