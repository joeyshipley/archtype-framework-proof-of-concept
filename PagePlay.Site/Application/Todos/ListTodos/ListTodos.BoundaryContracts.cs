using FluentValidation;
using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Application.Todos.ListTodos;

public class ListTodosResponse : IResponse
{
    public List<TodoListEntry> Todos { get; set; } = new();
}

public class ListTodosRequest : IRequest
{
    // No request parameters needed - will list all todos for authenticated user
}

public class ListTodosRequestValidator : AbstractValidator<ListTodosRequest>
{
    public ListTodosRequestValidator()
    {
        // No validation rules needed for empty request
    }
}
