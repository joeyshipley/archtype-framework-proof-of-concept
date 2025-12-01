namespace PagePlay.Site.Application.Todos.Perspectives.Analytics;

/// <summary>
/// Strongly-typed context for the TodoAnalytics domain.
/// Provides compile-time safe access to analytics data.
/// </summary>
public class TodoAnalyticsDomainView
{
    /// <summary>
    /// Completion trend over the last 7 days.
    /// Key: Date string (e.g., "Nov 29"), Value: Number of todos completed on that day.
    /// </summary>
    public Dictionary<string, int> CompletionTrend { get; set; } = new();

    /// <summary>
    /// Longest streak of consecutive days with completed todos.
    /// </summary>
    public int LongestStreak { get; set; }

    /// <summary>
    /// Average time from todo creation to completion (in hours).
    /// </summary>
    public double AverageCompletionTime { get; set; }

    /// <summary>
    /// Productivity score (0-100) based on completion rate.
    /// </summary>
    public int ProductivityScore { get; set; }
}
