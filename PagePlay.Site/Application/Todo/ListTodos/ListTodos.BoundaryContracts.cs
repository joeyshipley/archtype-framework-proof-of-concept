using FluentValidation;
using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Application.Todo.ListTodos;

public class ListTodosResponse : IResponse
{
    public List<TodoItem> Todos { get; set; } = new();
}

public class TodoItem
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
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
