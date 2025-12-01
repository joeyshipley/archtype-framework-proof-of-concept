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
/// Components declare domain dependency (typed API):
///   WelcomeWidget: DataDependencies.From<TodosDomain, TodosDomainContext>()
///
/// Interactions declare which domains are affected:
///   CreateTodo: DataMutations.For(TodosDomain.DomainName) - only WelcomeWidget updates
///   AnalyticsRefresh: DataMutations.For(TodosDomain.DomainName, TodoAnalyticsDomain.DomainName) - both domains update
/// </summary>
public class TodosDomain(IRepository _repository) : IDataDomain<TodosDomainContext>
{
    public const string DomainName = "todos";

    // Typed API - compile-time safe access
    public async Task<TodosDomainContext> FetchTypedAsync(long userId)
    {
        var todos = await fetchData(userId);

        return new TodosDomainContext
        {
            List = transformToListEntries(todos),
            OpenCount = calculateOpenCount(todos),
            TotalCount = calculateTotalCount(todos),
            CompletionRate = calculateCompletionRate(todos)
        };
    }

    // Data fetching - single query to prevent N+1
    private async Task<List<Todo>> fetchData(long userId) =>
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

}
