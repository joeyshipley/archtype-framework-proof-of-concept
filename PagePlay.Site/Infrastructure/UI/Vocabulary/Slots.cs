namespace PagePlay.Site.Infrastructure.UI.Vocabulary;

/// <summary>
/// Header slot - appears at the top of containers.
/// Universal slot type - containers define how it appears in their context.
/// </summary>
public record Header : ComponentBase
{
    public Header() { }

    public Header(params IHeaderContent[] content)
    {
        foreach (var item in content)
            Add(item);
    }
}

/// <summary>
/// Body slot - main content area of containers.
/// Universal slot type - containers define how it appears in their context.
/// </summary>
public record Body : ComponentBase
{
    public Body() { }

    public Body(params IBodyContent[] content)
    {
        foreach (var item in content)
            Add(item);
    }
}

/// <summary>
/// Footer slot - appears at the bottom of containers.
/// Universal slot type - containers define how it appears in their context.
/// </summary>
public record Footer : ComponentBase
{
    public Footer() { }

    public Footer(params IFooterContent[] content)
    {
        foreach (var item in content)
            Add(item);
    }
}
