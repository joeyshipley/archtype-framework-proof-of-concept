using Microsoft.AspNetCore.Mvc;
using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Web.Framework;
using PagePlay.Site.Infrastructure.Web.Mutations;
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
    /// The HttpContext for the current request. Available in OnSuccess and OnError methods.
    /// </summary>
    protected HttpContext HttpContext { get; private set; } = null!;

    /// <summary>
    /// The base route for the page (e.g., "todos", "login")
    /// </summary>
    protected abstract string RouteBase { get; }

    /// <summary>
    /// The action name for this interaction (e.g., "create", "delete", "toggle")
    /// </summary>
    protected abstract string RouteAction { get; }

    /// <summary>
    /// Whether this interaction requires an authenticated user. Defaults to true.
    /// </summary>
    protected virtual bool RequireAuth => true;
    
    protected virtual DataMutations Mutates => null;
    
    private readonly IFrameworkOrchestrator _framework;
    
    protected PageInteractionBase(
        TView page,
        IFrameworkOrchestrator framework
    ) 
    {
        Page = page;
        _framework = framework;
    }

    /// <summary>
    /// Builds a result containing only OOB component updates based on declared mutations.
    /// Use this for component-first interactions that don't return targeted HTML.
    /// The framework automatically re-renders all components affected by your Mutates declaration.
    /// </summary>
    protected async Task<IResult> BuildOobResult()
    {
        var oobHtml = await OobHtml();
        return Results.Content(oobHtml, "text/html");
    }

    /// <summary>
    /// Builds a result containing only OOB content (no target swap).
    /// Hides the empty string hack - HTMX needs empty main content when only OOB updates are sent.
    /// Use this for error responses that show notifications via OOB but don't update the triggering element.
    /// </summary>
    /// <param name="oobHtml">HTML with hx-swap-oob="true" attributes</param>
    protected IResult BuildOobOnly(string oobHtml)
    {
        // Empty main content tells HTMX to ignore target swap (when SwapStrategy.None is set)
        // Only the OOB updates are processed
        var mainContent = "";
        return Results.Content(mainContent + oobHtml, "text/html");
    }

    /// <summary>
    /// Builds a result combining framework component OOB updates with additional manual OOB fragments.
    /// Use this when you need both automatic component updates (from Mutates) and custom OOB content.
    /// Example: Form reset after successful creation, where the created item is handled by framework OOB.
    /// </summary>
    /// <param name="oobFragments">Additional HTML fragments with hx-swap-oob="true" attributes</param>
    protected async Task<IResult> BuildOobResultWith(params string[] oobFragments)
    {
        var frameworkOob = await OobHtml();
        var manualOob = string.Join("\n", oobFragments);
        var combinedOob = string.IsNullOrEmpty(frameworkOob)
            ? manualOob
            : frameworkOob + "\n" + manualOob;
        return Results.Content(combinedOob, "text/html");
    }

    public void Map(IEndpointRouteBuilder endpoints)
    {
        var builder = endpoints.MapPost(
            PageInteraction.GetRoute(RouteBase, RouteAction),
            Handle
        );

        if (RequireAuth)
            builder.RequireAuthenticatedUser();
    }

    private async Task<IResult> Handle(
        [FromForm] TRequest request,
        IWorkflow<TRequest, TResponse> workflow,
        HttpContext httpContext
    )
    {
        HttpContext = httpContext;

        var result = await workflow.Perform(request);

        return result.Success
            ? await OnSuccess(result.Model)
            : OnError(result.Errors);
    }

    private async Task<string> OobHtml()
    {
        var contextHeader = HttpContext.Request.Headers["X-Component-Context"].ToString();
        return await _framework.RenderMutationResponseAsync(Mutates, contextHeader);
    }
    
    /// <summary>
    /// Override this to define what HTML should be returned on successful workflow execution.
    /// </summary>
    protected abstract Task<IResult> OnSuccess(TResponse response);

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
