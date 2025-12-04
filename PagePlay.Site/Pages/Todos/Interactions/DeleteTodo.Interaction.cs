using PagePlay.Site.Application.Todos.Perspectives;
using PagePlay.Site.Application.Todos.Perspectives.List;
using PagePlay.Site.Application.Todos.Workflows.DeleteTodo;
using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Web.Pages;
using PagePlay.Site.Infrastructure.Web.Framework;
using PagePlay.Site.Infrastructure.Web.Mutations;
using PagePlay.Site.Infrastructure.Web.Html;

namespace PagePlay.Site.Pages.Todos.Interactions;

public class DeleteTodoInteraction(
    ITodosPageView page,
    IFrameworkOrchestrator framework
) : PageInteractionBase<DeleteTodoWorkflowRequest, DeleteTodoWorkflowResponse, ITodosPageView>(page, framework),
      ITodosPageInteraction
{
    protected override string RouteBase => TodosPageEndpoints.PAGE_ROUTE;
    protected override string RouteAction => "delete";
    protected override DataMutations Mutates => DataMutations.For(TodosListDomainView.DomainName);

    protected override async Task<IResult> OnSuccess(DeleteTodoWorkflowResponse response)
    {
        return await BuildHtmlFragmentResult();
    }

    // TODO: fetch the task from DB and send back a row with error state instead of using OH NOES!!! in html
    // This will be needed for better UX in other places.
    protected override IResult OnError(IEnumerable<ResponseErrorEntry> errors)
    {
        // DeleteTodo needs the request ID to render the error state properly
        // For now, we'll use the generic error handling until the TODO above is addressed
        var errorMessage = errors.FirstOrDefault()?.Message ?? "Failed to delete todo";
        var errorHtml = Page.RenderErrorNotification(errorMessage);
        return Results.Content(HtmlFragment.InjectOob(errorHtml), "text/html");
    }

    protected override IResult RenderError(string message)
    {
        var errorHtml = Page.RenderErrorNotification(message);
        return Results.Content(HtmlFragment.InjectOob(errorHtml), "text/html");
    }
}
