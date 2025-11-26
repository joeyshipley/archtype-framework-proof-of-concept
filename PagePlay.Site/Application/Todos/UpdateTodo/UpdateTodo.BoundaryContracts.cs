using FluentValidation;
using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Application.Todos.UpdateTodo;

public class UpdateTodoWorkflowResponse : IWorkflowResponse
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}

public class UpdateTodoWorkflowRequest : IWorkflowRequest
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
}

public class UpdateTodoRequestValidator : AbstractValidator<UpdateTodoWorkflowRequest>
{
    public UpdateTodoRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Todo ID must be greater than 0.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(500).WithMessage("Title must be 500 characters or less.");
    }
}
