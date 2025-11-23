using PagePlay.Site.Infrastructure.Domain;

namespace PagePlay.Site.Application.Todo.Domain.Models;

public class Todo : IEntity
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static Todo Create(long userId, string title)
    {
        return new Todo
        {
            UserId = userId,
            Title = title,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Toggle()
    {
        IsCompleted = !IsCompleted;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTitle(string title)
    {
        Title = title;
        UpdatedAt = DateTime.UtcNow;
    }
}
