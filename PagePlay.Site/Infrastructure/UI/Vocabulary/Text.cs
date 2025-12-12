namespace PagePlay.Site.Infrastructure.UI.Vocabulary;

/// <summary>
/// Text - Standard body text element.
/// Can appear in Header, Body, or Footer slots.
/// </summary>
public record Text : IHeaderContent, IBodyContent, IFooterContent
{
    private readonly string _content;

    public IEnumerable<IElement> Children => Enumerable.Empty<IElement>();

    public string Content => _content;

    public Text(string content)
    {
        _content = content;
    }
}
