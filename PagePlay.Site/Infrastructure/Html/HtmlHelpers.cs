using System.Web;

namespace PagePlay.Site.Infrastructure.Html;

public static class HtmlHelpers
{
    public static string Safe(this string value) =>
        string.IsNullOrEmpty(value) ? string.Empty : HttpUtility.HtmlEncode(value);
}
