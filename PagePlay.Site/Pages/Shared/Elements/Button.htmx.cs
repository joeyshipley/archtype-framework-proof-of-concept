namespace PagePlay.Site.Pages.Shared.Elements;

public class RouteData
{
    public required string Endpoint { get; init; }
    public long? ModelId { get; init; }
    public required string Target { get; init; }
    public string HttpMethod { get; init; } = "post";
    public string SwapStrategy { get; init; } = "innerHTML";
    public string? HxExt { get; init; }
    public List<(string name, object value)>? AdditionalValues { get; init; }
}

public class HtmlData
{
    public required string ElementId { get; init; }
    public string? CssClass { get; init; }
    public string? Title { get; init; }
    public List<(string attribute, object value)>? DataAttributes { get; init; }
}

public class Button
{
    // language=html
    public static string Render(RouteData route, HtmlData html, string content)
    {
        var classAttr = !string.IsNullOrEmpty(html.CssClass) ? $"class=\"{html.CssClass}\"" : "";
        var titleAttr = !string.IsNullOrEmpty(html.Title) ? $"title=\"{html.Title}\"" : "";

        // Build hx-vals with entity id and any additional values
        var values = new List<(string, object)>();
        if (route.ModelId.HasValue)
        {
            values.Add(("id", route.ModelId.Value));
        }
        if (route.AdditionalValues != null)
        {
            values.AddRange(route.AdditionalValues);
        }

        var hxValsAttr = "";
        if (values.Count > 0)
        {
            var jsonPairs = values.Select(v =>
                v.Item2 is string
                    ? $"\"{v.Item1}\": \"{v.Item2}\""
                    : $"\"{v.Item1}\": {v.Item2}");
            var json = string.Join(", ", jsonPairs);
            hxValsAttr = $"hx-vals='{{ {json} }}'";
        }

        // Build data attributes
        var dataAttrs = "";
        if (html.DataAttributes != null && html.DataAttributes.Count > 0)
        {
            dataAttrs = string.Join(" ", html.DataAttributes.Select(attr =>
                $"data-{attr.attribute}=\"{attr.value}\""));
        }

        var hxExtAttr = !string.IsNullOrEmpty(route.HxExt) ? $"hx-ext=\"{route.HxExt}\"" : "";

        return $$"""
        <button id="{{html.ElementId}}"
                hx-{{route.HttpMethod}}="{{route.Endpoint}}"
                hx-target="{{route.Target}}"
                hx-swap="{{route.SwapStrategy}}"
                {{hxExtAttr}}
                {{hxValsAttr}}
                {{classAttr}}
                {{titleAttr}}
                {{dataAttrs}}>
            {{content}}
        </button>
        """;
    }
}