using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Infrastructure.Data.Repositories;
using PagePlay.Site.Infrastructure.Web.Data;

namespace PagePlay.Site.Application.Todos.Domain;

/// <summary>
/// Analytics domain for todo data - demonstrates domain separation pattern.
/// This domain provides expensive analytics calculations separate from basic CRUD operations.
///
/// Pattern: Split domains by computational cost and usage frequency
/// - TodosDomain: Fast queries for CRUD operations (list, counts, basic stats)
/// - TodoAnalyticsDomain: Expensive analytics for dashboard/reporting pages
///
/// Components declare which domain they need, avoiding unnecessary computation.
/// </summary>
public class TodoAnalyticsDomain(IRepository _repository) : IDataDomain
{
    public string Name => "todoAnalytics";

    // Entry point reveals analytics data structure - readable without implementation details
    public async Task<DomainDataContext> FetchAllAsync(long userId)
    {
        var todos = await fetchTodos(userId);
        var completionTrend = calculateCompletionTrend(todos);
        var longestStreak = calculateLongestStreak(todos);
        var averageCompletionTime = calculateAverageCompletionTime(todos);
        var productivityScore = calculateProductivityScore(todos);
        var weeklyBreakdown = calculateWeeklyBreakdown(todos);

        return buildContext(completionTrend, longestStreak, averageCompletionTime, productivityScore, weeklyBreakdown);
    }

    // Data fetching - single query to prevent N+1
    private async Task<List<Todo>> fetchTodos(long userId) =>
        await _repository.List(Todo.ByUserId(userId));

    // Example analytics calculation: completion trend over last 7 days
    private Dictionary<string, int> calculateCompletionTrend(List<Todo> todos)
    {
        var last7Days = Enumerable.Range(0, 7)
            .Select(i => DateTime.UtcNow.Date.AddDays(-i))
            .Reverse()
            .ToList();

        return last7Days.ToDictionary(
            date => date.ToString("MMM dd"),
            date => todos.Count(t =>
                t.IsCompleted &&
                t.UpdatedAt.Date == date
            )
        );
    }

    // Example: Calculate longest completion streak
    private int calculateLongestStreak(List<Todo> todos)
    {
        var completedTodos = todos
            .Where(t => t.IsCompleted)
            .OrderBy(t => t.UpdatedAt)
            .ToList();

        if (completedTodos.Count == 0) return 0;

        int maxStreak = 1;
        int currentStreak = 1;

        for (int i = 1; i < completedTodos.Count; i++)
        {
            var daysDiff = (completedTodos[i].UpdatedAt.Date - completedTodos[i - 1].UpdatedAt.Date).Days;

            if (daysDiff <= 1)
            {
                currentStreak++;
                maxStreak = Math.Max(maxStreak, currentStreak);
            }
            else
            {
                currentStreak = 1;
            }
        }

        return maxStreak;
    }

    // Example: Average time from creation to completion (in hours)
    private double calculateAverageCompletionTime(List<Todo> todos)
    {
        var completedTodos = todos.Where(t => t.IsCompleted).ToList();

        if (completedTodos.Count == 0) return 0.0;

        var totalHours = completedTodos.Sum(t =>
            (t.UpdatedAt - t.CreatedAt).TotalHours
        );

        return Math.Round(totalHours / completedTodos.Count, 1);
    }

    // Example: Simple productivity score (completed / total * 100)
    private int calculateProductivityScore(List<Todo> todos)
    {
        if (todos.Count == 0) return 0;

        var completedCount = todos.Count(t => t.IsCompleted);
        return (int)Math.Round((double)completedCount / todos.Count * 100);
    }

    // Example: Weekly breakdown showing created vs completed
    private Dictionary<string, object> calculateWeeklyBreakdown(List<Todo> todos)
    {
        var weekStart = DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
        var weekEnd = weekStart.AddDays(7);

        var createdThisWeek = todos.Count(t =>
            t.CreatedAt >= weekStart && t.CreatedAt < weekEnd
        );

        var completedThisWeek = todos.Count(t =>
            t.IsCompleted &&
            t.UpdatedAt >= weekStart &&
            t.UpdatedAt < weekEnd
        );

        return new Dictionary<string, object>
        {
            ["created"] = createdThisWeek,
            ["completed"] = completedThisWeek,
            ["weekStart"] = weekStart.ToString("MMM dd"),
            ["weekEnd"] = weekEnd.AddDays(-1).ToString("MMM dd")
        };
    }

    // Context assembly - defines analytics domain data shape
    private DomainDataContext buildContext(
        Dictionary<string, int> completionTrend,
        int longestStreak,
        double averageCompletionTime,
        int productivityScore,
        Dictionary<string, object> weeklyBreakdown)
    {
        var context = new DomainDataContext();
        context["completionTrend"] = completionTrend;
        context["longestStreak"] = longestStreak;
        context["averageCompletionTime"] = averageCompletionTime;
        context["productivityScore"] = productivityScore;
        context["weeklyBreakdown"] = weeklyBreakdown;
        return context;
    }
}
