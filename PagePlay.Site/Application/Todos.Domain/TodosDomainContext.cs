using PagePlay.Site.Application.Todos.Domain.Models;

namespace PagePlay.Site.Application.Todos.Domain;

/// <summary>
/// Strongly-typed context for the Todos domain.
/// Provides compile-time safe access to todo data.
/// </summary>
public class TodosDomainContext
{
    /// <summary>
    /// List of todos for the current user, ordered by completion status and creation date.
    /// </summary>
    public List<TodoListEntry> List { get; set; } = new();

    /// <summary>
    /// Count of incomplete todos.
    /// </summary>
    public int OpenCount { get; set; }

    /// <summary>
    /// Total count of all todos (completed + incomplete).
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Completion rate as a decimal (0.0 to 1.0).
    /// 0.75 means 75% of todos are completed.
    /// </summary>
    public double CompletionRate { get; set; }
}
