using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Infrastructure.Data.Repositories;
using PagePlay.Site.Infrastructure.Web.Data;

namespace PagePlay.Site.Application.Todos.Domain;

/// <summary>
/// Basic CRUD domain for todo-related data.
/// Provides fast queries for list operations and simple statistics.
///
/// Domain Separation Pattern:
/// - TodosDomain: Fast queries for CRUD pages (list, openCount, totalCount, completionRate)
/// - TodoAnalyticsDomain: Expensive analytics for dashboard/reporting pages (trends, streaks, history)
///
/// When to split domains:
/// 1. Computational cost differs significantly (simple count vs complex aggregation)
/// 2. Different pages need different data (CRUD page vs analytics dashboard)
/// 3. Independent mutation concerns (basic operations vs analytics recalculation)
///
/// Components declare domain dependency:
///   WelcomeWidget: DataDependencies.From("todos") - uses this domain
///   AnalyticsStatsWidget: DataDependencies.From("todoAnalytics") - uses analytics domain
///
/// Interactions declare which domains are affected:
///   CreateTodo: DataMutations.For("todos") - only WelcomeWidget updates
///   AnalyticsRefresh: DataMutations.For("todos", "todoAnalytics") - both domains update
/// </summary>
public class TodosDomain(IRepository _repository) : IDataDomain
{
    public string Name => "todos";

    // Entry point reveals domain data structure - readable without implementation details
    public async Task<DomainDataContext> FetchAllAsync(long userId)
    {
        var todos = await fetchTodos(userId);
        var todoList = transformToListEntries(todos);
        var openCount = calculateOpenCount(todos);
        var totalCount = calculateTotalCount(todos);
        var completionRate = calculateCompletionRate(todos);

        return buildContext(todoList, openCount, totalCount, completionRate);
    }

    // Data fetching - single query to prevent N+1
    private async Task<List<Todo>> fetchTodos(long userId) =>
        await _repository.List(Todo.ByUserId(userId));

    // Data transformation - domain model to presentation model
    private List<TodoListEntry> transformToListEntries(List<Todo> todos) =>
        todos
            .OrderBy(t => t.IsCompleted)
            .ThenByDescending(t => t.CreatedAt)
            .Select(TodoListEntry.FromTodo)
            .ToList();

    // Derived data calculations
    private int calculateOpenCount(List<Todo> todos) =>
        todos.Count(t => !t.IsCompleted);

    private int calculateTotalCount(List<Todo> todos) =>
        todos.Count;

    private double calculateCompletionRate(List<Todo> todos)
    {
        var totalCount = todos.Count;
        return totalCount > 0
            ? (double)todos.Count(t => t.IsCompleted) / totalCount
            : 0.0;
    }

    // Context assembly - defines domain data shape
    private DomainDataContext buildContext(
        List<TodoListEntry> todoList,
        int openCount,
        int totalCount,
        double completionRate)
    {
        var context = new DomainDataContext();
        context["list"] = todoList;
        context["openCount"] = openCount;
        context["totalCount"] = totalCount;
        context["completionRate"] = completionRate;
        return context;
    }
}
