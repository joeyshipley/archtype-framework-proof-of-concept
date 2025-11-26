using FluentValidation;
using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Application.Todos.ToggleTodo;

public class ToggleTodoWorkflowResponse : IWorkflowResponse
{
    public List<TodoListEntry> Todos { get; set; } = new();
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
