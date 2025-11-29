namespace PagePlay.Site.Infrastructure.Web.Components;

using System.Text.Json;

/// <summary>
/// Represents client-side component information sent with each request.
/// Tells server which components are currently on the page.
/// </summary>
public class ComponentInfo
{
    public string Id { get; set; } = string.Empty;
    public string ComponentType { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
}

/// <summary>
/// Parses component context from client HTTP headers.
/// </summary>
public interface IComponentContextParser
{
    /// <summary>
    /// Parses X-Component-Context header into component info list.
    /// Returns empty list if header missing or invalid.
    /// </summary>
    List<ComponentInfo> Parse(string? contextJson);
}

public class ComponentContextParser : IComponentContextParser
{
    public List<ComponentInfo> Parse(string? contextJson)
    {
        if (string.IsNullOrWhiteSpace(contextJson))
            return new List<ComponentInfo>();

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var components = JsonSerializer.Deserialize<List<ComponentInfo>>(contextJson, options);
            return components ?? new List<ComponentInfo>();
        }
        catch (JsonException)
        {
            // Log warning? For now, return empty list
            return new List<ComponentInfo>();
        }
    }
}
