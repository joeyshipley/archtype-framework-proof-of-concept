using FluentValidation;
using PagePlay.Site.Infrastructure.Core.Application;

namespace PagePlay.Site.Application.Todos.Performers.UpdateTodo;

public class UpdateTodoResponse : IPerformerResponse
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}

public class UpdateTodoRequest : IPerformerRequest
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
}

public class UpdateTodoRequestValidator : AbstractValidator<UpdateTodoRequest>
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
