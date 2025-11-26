namespace PagePlay.Site.Infrastructure.Web.Pages;

public static class PageInteraction
{
    private const string ROUTE_PREFIX = "interaction";

    public static string GetRoute(string page, string route) =>
        $"{ ROUTE_PREFIX }/{ page }/{ route }";
}
