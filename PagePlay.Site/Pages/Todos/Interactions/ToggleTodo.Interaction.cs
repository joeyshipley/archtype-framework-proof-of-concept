using PagePlay.Site.Application.Todos.Perspectives.List;
using PagePlay.Site.Application.Todos.Performers.ToggleTodo;
using PagePlay.Site.Infrastructure.Web.Pages;
using PagePlay.Site.Infrastructure.Web.Framework;
using PagePlay.Site.Infrastructure.Web.Mutations;
using PagePlay.Site.Infrastructure.Web.Html;

namespace PagePlay.Site.Pages.Todos.Interactions;

public class ToggleTodoInteraction(
    ITodosPageView page,
    IFrameworkOrchestrator framework
) : PageInteractionBase<ToggleTodoRequest, ToggleTodoResponse, ITodosPageView>(page, framework),
      ITodosPageInteraction
{
    protected override string RouteBase => TodosPageEndpoints.PAGE_ROUTE;
    protected override string RouteAction => "toggle";
    protected override DataMutations Mutates => DataMutations.For(TodosListDomainView.DomainName);

    protected override async Task<IResult> OnSuccess(ToggleTodoResponse response)
    {
        return await BuildOobResult();
    }

    protected override IResult RenderError(string message)
    {
        // Only return error notification OOB - checkbox stays as-is
        var errorHtml = Page.RenderErrorNotification(message);
        return BuildOobOnly(HtmlFragment.InjectOob(errorHtml));
    }
}
