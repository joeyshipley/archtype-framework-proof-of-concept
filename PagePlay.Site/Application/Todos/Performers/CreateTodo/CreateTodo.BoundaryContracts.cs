using FluentValidation;
using PagePlay.Site.Application.Todos.Models;
using PagePlay.Site.Infrastructure.Core.Application;

namespace PagePlay.Site.Application.Todos.Performers.CreateTodo;

public class CreateTodoResponse : IPerformerResponse
{
    public long CreatedId { get; set; }
    // Metadata only - query data comes from TodosListProvider
}

public class CreateTodoRequest : IPerformerRequest
{
    public string Title { get; set; } = string.Empty;
}

public class CreateTodoRequestValidator : AbstractValidator<CreateTodoRequest>
{
    public CreateTodoRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must be 200 characters or less.");
    }
}
