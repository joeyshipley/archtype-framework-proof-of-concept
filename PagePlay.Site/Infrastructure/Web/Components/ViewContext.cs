namespace PagePlay.Site.Infrastructure.Web.Components;

using System.Text.Json;

/// <summary>
/// Represents client-side view information sent with each request.
/// Tells server which views are currently on the page.
/// </summary>
public class ViewInfo
{
    public string Id { get; set; } = string.Empty;
    public string ViewType { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
}

/// <summary>
/// Parses view context from client HTTP headers.
/// </summary>
public interface IViewContextParser
{
    /// <summary>
    /// Parses X-Component-Context header into view info list.
    /// Returns empty list if header missing or invalid.
    /// </summary>
    List<ViewInfo> Parse(string contextJson);
}

public class ViewContextParser : IViewContextParser
{
    public List<ViewInfo> Parse(string contextJson)
    {
        if (string.IsNullOrWhiteSpace(contextJson))
            return new List<ViewInfo>();

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var views = JsonSerializer.Deserialize<List<ViewInfo>>(contextJson, options);
            return views ?? new List<ViewInfo>();
        }
        catch (JsonException)
        {
            // Log warning? For now, return empty list
            return new List<ViewInfo>();
        }
    }
}
