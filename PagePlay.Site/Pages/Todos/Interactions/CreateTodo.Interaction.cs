using PagePlay.Site.Application.Todos.Perspectives.List;
using PagePlay.Site.Application.Todos.Performers.CreateTodo;
using PagePlay.Site.Infrastructure.Web.Pages;
using PagePlay.Site.Infrastructure.Web.Framework;
using PagePlay.Site.Infrastructure.Web.Mutations;
using PagePlay.Site.Infrastructure.Web.Html;

namespace PagePlay.Site.Pages.Todos.Interactions;

public class CreateTodoInteraction(
    ITodosPageView page,
    IFrameworkOrchestrator _framework
) : PageInteractionBase<CreateTodoRequest, CreateTodoResponse, ITodosPageView>(page, _framework),
      ITodosPageInteraction
{
    protected override string RouteBase => TodosPageEndpoints.PAGE_ROUTE;
    protected override string RouteAction => "create";

    protected override DataMutations Mutates => DataMutations.For(TodosListDomainView.DomainName);

    protected override async Task<IResult> OnSuccess(CreateTodoResponse response)
    {
        // Framework OOB updates component, plus manual form reset OOB
        var formReset = HtmlFragment.InjectOob(Page.RenderCreateForm());
        return await BuildOobResultWith(formReset);
    }

    protected override IResult RenderError(string message)
    {
        // Only return error notification OOB - form stays as-is with user's values
        var errorHtml = Page.RenderErrorNotification(message);
        return BuildOobOnly(HtmlFragment.InjectOob(errorHtml));
    }
}