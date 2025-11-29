using PagePlay.Site.Application.Todos.CreateTodo;
using PagePlay.Site.Infrastructure.Web.Pages;
using PagePlay.Site.Infrastructure.Web.Framework;
using PagePlay.Site.Infrastructure.Web.Mutations;

namespace PagePlay.Site.Pages.Todos.Interactions;

public class CreateTodoInteraction(
    ITodosPageView page,
    IFrameworkOrchestrator _framework
) : PageInteractionBase<CreateTodoWorkflowRequest, CreateTodoWorkflowResponse, ITodosPageView>(page),
      ITodosPageInteraction
{
    protected override string RouteBase => TodosPageEndpoints.PAGE_ROUTE;
    protected override string Action => "create";

    // Declare what this interaction mutates
    protected virtual DataMutations Mutates => DataMutations.For("todos");

    protected override async Task<IResult> OnSuccess(CreateTodoWorkflowResponse response)
    {
        // Get component context from request header
        var contextHeader = HttpContext.Request.Headers["X-Component-Context"].ToString();

        // Render the new todo item
        var todoHtml = Page.RenderSuccessfulTodoCreation(response.Todo);

        // Framework handles re-rendering affected components
        var oobResult = await _framework.RenderMutationResponseAsync(Mutates, contextHeader);

        // Extract OOB HTML from result
        var oobHtml = await getResponseContent(oobResult);

        // Combine todo HTML with OOB updates
        var combinedHtml = todoHtml + "\n" + oobHtml;

        return Results.Content(combinedHtml, "text/html");
    }

    protected override IResult RenderError(string message) =>
        Results.Content(Page.RenderErrorNotification(message), "text/html");

    // Helper to extract HTML content from IResult
    private async Task<string> getResponseContent(IResult result)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        await result.ExecuteAsync(httpContext);

        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(httpContext.Response.Body);
        return await reader.ReadToEndAsync();
    }
}
