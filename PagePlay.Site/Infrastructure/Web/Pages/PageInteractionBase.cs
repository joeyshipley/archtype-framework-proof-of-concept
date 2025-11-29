using Microsoft.AspNetCore.Mvc;
using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Web.Routing;

namespace PagePlay.Site.Infrastructure.Web.Pages;

/// <summary>
/// Base class for page interactions that handles common boilerplate for workflow execution,
/// routing, authentication, and error handling. Junior developers only need to specify
/// route information and success/error rendering logic.
/// </summary>
public abstract class PageInteractionBase<TRequest, TResponse, TView> : IEndpoint
    where TRequest : IWorkflowRequest
    where TResponse : IWorkflowResponse
    where TView : class
{
    protected readonly TView Page;

    /// <summary>
    /// The base route for the page (e.g., "todos", "login")
    /// </summary>
    protected abstract string RouteBase { get; }

    /// <summary>
    /// The action name for this interaction (e.g., "create", "delete", "toggle")
    /// </summary>
    protected abstract string Action { get; }

    /// <summary>
    /// Whether this interaction requires an authenticated user. Defaults to true.
    /// </summary>
    protected virtual bool RequireAuth => true;

    protected PageInteractionBase(TView page) => Page = page;

    public void Map(IEndpointRouteBuilder endpoints)
    {
        var builder = endpoints.MapPost(
            PageInteraction.GetRoute(RouteBase, Action),
            Handle
        );

        if (RequireAuth)
            builder.RequireAuthenticatedUser();
    }

    private async Task<IResult> Handle(
        [FromForm] TRequest request,
        IWorkflow<TRequest, TResponse> workflow
    )
    {
        var result = await workflow.Perform(request);

        return result.Success
            ? OnSuccess(result.Model)
            : OnError(result.Errors);
    }

    /// <summary>
    /// Override this to define what HTML should be returned on successful workflow execution.
    /// </summary>
    protected abstract IResult OnSuccess(TResponse response);

    /// <summary>
    /// Override this to customize error handling. Default implementation uses the first error message
    /// and calls RenderError. Can be overridden for more sophisticated error handling.
    /// </summary>
    // TODO: handle the collection of errors and not just the first one.
    protected virtual IResult OnError(IEnumerable<ResponseErrorEntry> errors)
    {
        var errorMessage = errors.FirstOrDefault()?.Message ?? "An error occurred";
        return RenderError(errorMessage);
    }

    /// <summary>
    /// Override this to define how errors should be rendered as HTML.
    /// </summary>
    protected abstract IResult RenderError(string message);
}
