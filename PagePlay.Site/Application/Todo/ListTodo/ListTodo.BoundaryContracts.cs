using FluentValidation;
using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Application.Todo.ListTodo;

public class ListTodoResponse : IResponse
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

public class ListTodoRequest : IRequest
{
    // No request parameters needed - will list all todos for authenticated user
}

public class ListTodoRequestValidator : AbstractValidator<ListTodoRequest>
{
    public ListTodoRequestValidator()
    {
        // No validation rules needed for empty request
    }
}
