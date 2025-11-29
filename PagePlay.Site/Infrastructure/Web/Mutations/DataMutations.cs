namespace PagePlay.Site.Infrastructure.Web.Mutations;

/// <summary>
/// Declares which domains are mutated by an interaction.
/// Framework uses this to determine which components to re-render.
/// </summary>
public class DataMutations
{
    public List<string> Domains { get; private set; } = new();

    /// <summary>
    /// Creates a mutation declaration for one or more domains.
    /// </summary>
    public static DataMutations For(params string[] domains)
    {
        return new DataMutations { Domains = domains.ToList() };
    }
}
