using PagePlay.Site.Application.Todos.Models;
using PagePlay.Site.Infrastructure.Data.Repositories;
using PagePlay.Site.Infrastructure.Web.Data;

namespace PagePlay.Site.Application.Todos.Perspectives.Analytics;

public class TodoAnalyticsProvider(IRepository _repository) : IDataProvider<TodoAnalyticsDomainView>
{
    public const string DomainName = "todoAnalytics";

    public async Task<TodoAnalyticsDomainView> FetchTyped(long userId)
    {
        var todos = await fetchData(userId);

        return new TodoAnalyticsDomainView
        {
            CompletionTrend = calculateCompletionTrend(todos),
            LongestStreak = calculateLongestStreak(todos),
            AverageCompletionTime = calculateAverageCompletionTime(todos),
            ProductivityScore = calculateProductivityScore(todos)
        };
    }

    private async Task<List<Todo>> fetchData(long userId) =>
        await _repository.List(Todo.ByUserId(userId));

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

    private double calculateAverageCompletionTime(List<Todo> todos)
    {
        var completedTodos = todos.Where(t => t.IsCompleted).ToList();

        if (completedTodos.Count == 0) return 0.0;

        var totalHours = completedTodos.Sum(t =>
            (t.UpdatedAt - t.CreatedAt).TotalHours
        );

        return Math.Round(totalHours / completedTodos.Count, 1);
    }

    private int calculateProductivityScore(List<Todo> todos)
    {
        if (todos.Count == 0) return 0;

        var completedCount = todos.Count(t => t.IsCompleted);
        return (int)Math.Round((double)completedCount / todos.Count * 100);
    }
}
