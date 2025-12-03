using FluentValidation;
using PagePlay.Site.Application.Todos.Models;
using PagePlay.Site.Infrastructure.Core.Application;

namespace PagePlay.Site.Application.Todos.Workflows.ToggleTodo;

public class ToggleTodoWorkflowResponse : IWorkflowResponse
{
    // Empty response - no metadata needed for toggle
    // Query data comes from TodosListProvider via component update
}

public class ToggleTodoWorkflowRequest : IWorkflowRequest
{
    public long Id { get; set; }
}

public class ToggleTodoRequestValidator : AbstractValidator<ToggleTodoWorkflowRequest>
{
    public ToggleTodoRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Todo ID must be greater than 0.");
    }
}
