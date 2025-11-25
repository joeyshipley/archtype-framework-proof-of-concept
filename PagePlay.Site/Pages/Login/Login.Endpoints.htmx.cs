using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using PagePlay.Site.Application.Accounts.Login;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Pages.Shared;

namespace PagePlay.Site.Pages.Login;

public interface IHtmxPage
{
    string RenderPage(string antiforgeryToken = "");
}

public interface IHtmxPage<TPageData>
{
    string RenderPage(string antiforgeryToken, TPageData data);
}

public interface IPageDataLoader<TPageData>
{
    Task<TPageData> Load();
}

public interface IHtmxFragment<TResponse> where TResponse : IResponse
{
    string RenderSuccess(TResponse model);
    string RenderError(IEnumerable<ResponseErrorEntry> errors);
}

public static class PlumbingExplorations
{
    // Generic GET endpoint for any page that implements IHtmxPage
    public static void MapHtmxPage<TPageInterface>(
        this IEndpointRouteBuilder endpoints,
        string route,
        string pageTitle
    ) where TPageInterface : IHtmxPage
    {
        endpoints.MapGet(route, (
            HttpContext context,
            [FromServices] IAntiforgery antiforgery,
            [FromServices] TPageInterface page
        ) =>
        {
            var tokens = antiforgery.GetAndStoreTokens(context);
            var bodyContent = page.RenderPage(tokens.RequestToken);
            return RenderFullPage(bodyContent, pageTitle);
        });
    }

    // Generic GET endpoint for pages with preloaded data
    public static void MapHtmxPage<TPageInterface, TPageData>(
        this IEndpointRouteBuilder endpoints,
        string route,
        string pageTitle
    ) where TPageInterface : IHtmxPage<TPageData>
    {
        endpoints.MapGet(route, async (
            HttpContext context,
            [FromServices] IAntiforgery antiforgery,
            [FromServices] TPageInterface page,
            [FromServices] IPageDataLoader<TPageData> loader
        ) =>
        {
            var tokens = antiforgery.GetAndStoreTokens(context);
            var data = await loader.Load();
            var bodyContent = page.RenderPage(tokens.RequestToken, data);
            return RenderFullPage(bodyContent, pageTitle);
        });
    }

    // Generic POST endpoint for HTMX workflow fragments
    public static void MapHtmxFragment<TFragment, TRequest, TResponse>(
        this IEndpointRouteBuilder endpoints,
        string route
    )
        where TFragment : IHtmxFragment<TResponse>
        where TRequest : IRequest, new()
        where TResponse : IResponse
    {
        endpoints.MapPost(route, async (
            HttpContext context,
            [FromServices] TFragment fragment,
            [FromServices] IWorkflow<TRequest, TResponse> workflow
        ) =>
        {
            var form = context.Request.Form;
            var request = new TRequest();

            // Bind form data to request properties
            foreach (var property in typeof(TRequest).GetProperties())
            {
                if (form.ContainsKey(property.Name.ToLowerInvariant()))
                {
                    var value = form[property.Name.ToLowerInvariant()].ToString();
                    property.SetValue(request, value);
                }
                else if (form.ContainsKey(property.Name))
                {
                    var value = form[property.Name].ToString();
                    property.SetValue(request, value);
                }
            }

            var result = await workflow.Perform(request);

            if (!result.Success)
            {
                return Results.Content(
                    fragment.RenderError(result.Errors),
                    "text/html"
                );
            }

            return Results.Content(
                fragment.RenderSuccess(result.Model),
                "text/html"
            );
        });
    }

    private static IResult RenderFullPage(string bodyContent, string pageTitle)
    {
        var fullPage = Layout.Render(bodyContent, pageTitle);
        return Results.Content(fullPage, "text/html");
    }
}
