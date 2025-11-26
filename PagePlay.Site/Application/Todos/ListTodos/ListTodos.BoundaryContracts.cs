using FluentValidation;
using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Infrastructure.Core.Application;

namespace PagePlay.Site.Application.Todos.ListTodos;

public class ListTodosWorkflowResponse : IWorkflowResponse
{
    public List<TodoListEntry> Todos { get; set; } = new();
}

public class ListTodosWorkflowRequest : IWorkflowRequest
{
    // No request parameters needed - will list all todos for authenticated user
}

public class ListTodosRequestValidator : AbstractValidator<ListTodosWorkflowRequest>
{
    public ListTodosRequestValidator()
    {
        // No validation rules needed for empty request
    }
}
