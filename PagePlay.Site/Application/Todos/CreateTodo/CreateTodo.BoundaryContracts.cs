using FluentValidation;
using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Infrastructure.Core.Application;

namespace PagePlay.Site.Application.Todos.CreateTodo;

public class CreateTodoWorkflowResponse : IWorkflowResponse
{
    public required TodoListEntry Todo { get; set; }
}

public class CreateTodoWorkflowRequest : IWorkflowRequest
{
    public string Title { get; set; } = string.Empty;
}

public class CreateTodoRequestValidator : AbstractValidator<CreateTodoWorkflowRequest>
{
    public CreateTodoRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must be 200 characters or less.");
    }
}
