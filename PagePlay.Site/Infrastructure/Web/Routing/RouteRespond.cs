using PagePlay.Site.Infrastructure.Core.Application;

namespace PagePlay.Site.Infrastructure.Web.Routing;

public static class Respond
{
    public static IResult With<T>(IApplicationResult<T> result) =>
        Results.Ok(result);
}