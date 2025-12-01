using FluentValidation;
using PagePlay.Site.Infrastructure.Core.Application;

namespace PagePlay.Site.Application.Todos.Workflows.DeleteTodo;

public class DeleteTodoWorkflowResponse : IWorkflowResponse
{
    public long Id { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class DeleteTodoWorkflowRequest : IWorkflowRequest
{
    public long Id { get; set; }
}

public class DeleteTodoRequestValidator : AbstractValidator<DeleteTodoWorkflowRequest>
{
    public DeleteTodoRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Todo ID must be greater than 0.");
    }
}
