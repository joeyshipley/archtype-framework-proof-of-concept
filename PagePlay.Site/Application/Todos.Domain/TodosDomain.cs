using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Infrastructure.Data.Repositories;
using PagePlay.Site.Infrastructure.Web.Data;

namespace PagePlay.Site.Application.Todos.Domain;

/// <summary>
/// Data domain for todo-related data.
/// Fetches all todo data in one query and provides it to components.
/// </summary>
public class TodosDomain(IRepository _repository) : IDataDomain
{
    public string Name => "todos";

    public async Task<DomainDataContext> FetchAllAsync(long userId)
    {
        // Fetch todos once using existing repository pattern
        var todos = await _repository.List(Todo.ByUserId(userId));

        // Calculate all derived data
        var openCount = todos.Count(t => !t.IsCompleted);
        var totalCount = todos.Count;
        var completionRate = totalCount > 0
            ? (double)todos.Count(t => t.IsCompleted) / totalCount
            : 0.0;

        // Transform todos to list entries (same as ListTodos workflow)
        var todoList = todos
            .OrderBy(t => t.IsCompleted)
            .ThenByDescending(t => t.CreatedAt)
            .Select(TodoListEntry.FromTodo)
            .ToList();

        // Return everything this domain provides
        var context = new DomainDataContext();
        context["list"] = todoList;
        context["openCount"] = openCount;
        context["totalCount"] = totalCount;
        context["completionRate"] = completionRate;

        return context;
    }
}
