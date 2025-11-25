namespace PagePlay.Site.Application.Todos.Domain.Models;

public class TodoListEntry
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }

    public static TodoListEntry FromTodo(Todo todo)
    {
        return new TodoListEntry
        {
            Id = todo.Id,
            Title = todo.Title,
            IsCompleted = todo.IsCompleted
        };
    }
}
