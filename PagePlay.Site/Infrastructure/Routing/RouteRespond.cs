using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Infrastructure.Routing;

public static class Respond
{
    public static IResult With<T>(IApplicationResult<T> result) =>
        Results.Ok(result);
}