namespace PagePlay.Site.Pages.Home;

public interface IHomePageView
{
    string RenderPage();
}

public class HomePage : IHomePageView
{
    // language=html
    public string RenderPage() =>
    """
    <h1>Hello Reader</h1>
    """;
}
