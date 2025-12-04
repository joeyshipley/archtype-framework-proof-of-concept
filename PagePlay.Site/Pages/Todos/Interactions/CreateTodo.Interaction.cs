using PagePlay.Site.Application.Todos.Perspectives.List;
using PagePlay.Site.Application.Todos.Workflows.CreateTodo;
using PagePlay.Site.Infrastructure.Web.Pages;
using PagePlay.Site.Infrastructure.Web.Framework;
using PagePlay.Site.Infrastructure.Web.Mutations;
using PagePlay.Site.Infrastructure.Web.Html;

namespace PagePlay.Site.Pages.Todos.Interactions;

public class CreateTodoInteraction(
    ITodosPageView page,
    IFrameworkOrchestrator _framework
) : PageInteractionBase<CreateTodoWorkflowRequest, CreateTodoWorkflowResponse, ITodosPageView>(page, _framework),
      ITodosPageInteraction
{
    protected override string RouteBase => TodosPageEndpoints.PAGE_ROUTE;
    protected override string RouteAction => "create";

    protected override DataMutations Mutates => DataMutations.For(TodosListDomainView.DomainName);

    protected override async Task<IResult> OnSuccess(CreateTodoWorkflowResponse response)
    {
        // BuildHtmlFragmentResult combines main content + OOB component updates
        // We pass the form reset as main content, framework adds component OOB automatically
        var formReset = HtmlFragment.InjectOob(Page.RenderCreateForm());
        return await BuildHtmlFragmentResult(formReset);
    }

    protected override IResult RenderError(string message)
    {
        var errorHtml = Page.RenderErrorNotification(message);
        return Results.Content(HtmlFragment.InjectOob(errorHtml), "text/html");
    }
}