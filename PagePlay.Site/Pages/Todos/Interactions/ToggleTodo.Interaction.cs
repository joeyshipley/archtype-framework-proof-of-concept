using PagePlay.Site.Application.Todos.ToggleTodo;
using PagePlay.Site.Infrastructure.Web.Pages;
using PagePlay.Site.Infrastructure.Web.Framework;
using PagePlay.Site.Infrastructure.Web.Mutations;

namespace PagePlay.Site.Pages.Todos.Interactions;

public class ToggleTodoInteraction(
    ITodosPageView page,
    IFrameworkOrchestrator _framework
) : PageInteractionBase<ToggleTodoWorkflowRequest, ToggleTodoWorkflowResponse, ITodosPageView>(page),
      ITodosPageInteraction
{
    protected override string RouteBase => TodosPageEndpoints.PAGE_ROUTE;
    protected override string Action => "toggle";

    // Declare what this interaction mutates
    protected virtual DataMutations Mutates => DataMutations.For("todos");

    protected override async Task<IResult> OnSuccess(ToggleTodoWorkflowResponse response)
    {
        // Get component context from request header
        var contextHeader = HttpContext.Request.Headers["X-Component-Context"].ToString();

        // Render the updated todo list
        var todoListHtml = Page.RenderTodoList(response.Todos);

        // Framework handles re-rendering affected components (returns HTML string)
        var oobHtml = await _framework.RenderMutationResponseAsync(Mutates, contextHeader);

        // Combine todo list HTML with OOB updates
        var combinedHtml = todoListHtml + "\n" + oobHtml;

        return Results.Content(combinedHtml, "text/html");
    }

    protected override IResult RenderError(string message) =>
        Results.Content(Page.RenderErrorNotification(message), "text/html");
}
