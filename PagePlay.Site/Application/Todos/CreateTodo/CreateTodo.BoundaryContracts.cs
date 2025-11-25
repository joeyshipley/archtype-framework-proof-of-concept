using FluentValidation;
using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Application.Todos.CreateTodo;

public class CreateTodoResponse : IResponse
{
    public required TodoListEntry Todo { get; set; }
}

public class CreateTodoRequest : IRequest
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
