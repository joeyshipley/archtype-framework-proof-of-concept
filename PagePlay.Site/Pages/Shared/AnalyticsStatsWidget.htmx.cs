using PagePlay.Site.Application.Todos.Perspectives;
using PagePlay.Site.Application.Todos.Perspectives.Analytics;
using PagePlay.Site.Infrastructure.Web.Components;

namespace PagePlay.Site.Pages.Shared;

/// <summary>
/// Analytics widget demonstrating todoAnalytics domain usage.
///
/// Pattern: This component depends on "todoAnalytics" domain, NOT "todos" domain.
/// - When todos are created/updated/deleted, "todos" domain is mutated
/// - Framework checks: does any component depend on "todos"? (Yes: WelcomeWidget)
/// - Framework does NOT re-render this component (depends on "todoAnalytics")
///
/// To trigger this component's update, an interaction must declare:
///   DataMutations.For("todos", "todoAnalytics")
///
/// This separation allows:
/// - Expensive analytics calculations only when needed
/// - Simple CRUD pages don't pay analytics cost
/// - Independent domain evolution
/// </summary>
public interface IAnalyticsStatsWidget : IServerComponent {}

public class AnalyticsStatsWidget : IAnalyticsStatsWidget
{
    public string ComponentId => "analytics-stats-widget";

    public DataDependencies Dependencies => DataDependencies
        .From<TodoAnalyticsDomainView>();

    public string Render(IDataContext data)
    {
        var analytics = data.Get<TodoAnalyticsDomainView>();
        var productivityScore = analytics.ProductivityScore;
        var longestStreak = analytics.LongestStreak;
        var avgCompletionTime = analytics.AverageCompletionTime;

        // language=html
        return $$"""
        <div id="{{ComponentId}}"
             class="analytics-stats-widget">
            <div class="stats-grid">
                <div class="stat-card">
                    <h3>Productivity Score</h3>
                    <p class="stat-value">{{productivityScore}}%</p>
                </div>
                <div class="stat-card">
                    <h3>Longest Streak</h3>
                    <p class="stat-value">{{longestStreak}} days</p>
                </div>
                <div class="stat-card">
                    <h3>Avg Completion Time</h3>
                    <p class="stat-value">{{avgCompletionTime}} hrs</p>
                </div>
            </div>
        </div>
        """;
    }
}
