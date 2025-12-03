using AwesomeAssertions;
using PagePlay.Site.Infrastructure.UI.Rendering;
using PagePlay.Site.Infrastructure.UI.Vocabulary;

namespace PagePlay.Tests.Infrastructure.UI;

public class HtmlRendererFeedbackElementsTests
{
    private readonly IHtmlRenderer _renderer = new HtmlRenderer();

    [Fact]
    public void RenderAlert_WithNeutralTone_RendersCorrectHtml()
    {
        // Arrange
        var alert = new Alert("This is an informational message", AlertTone.Neutral);

        // Act
        var html = _renderer.Render(alert);

        // Assert
        html.Should().Contain("<div class=\"alert alert--neutral\" role=\"alert\">");
        html.Should().Contain("<p class=\"alert__message\">This is an informational message</p>");
        html.Should().Contain("</div>");
    }

    [Fact]
    public void RenderAlert_WithPositiveTone_RendersCorrectHtml()
    {
        // Arrange
        var alert = new Alert("Operation completed successfully!", AlertTone.Positive);

        // Act
        var html = _renderer.Render(alert);

        // Assert
        html.Should().Contain("<div class=\"alert alert--positive\" role=\"alert\">");
        html.Should().Contain("<p class=\"alert__message\">Operation completed successfully!</p>");
    }

    [Fact]
    public void RenderAlert_WithWarningTone_RendersCorrectHtml()
    {
        // Arrange
        var alert = new Alert("Warning: This action cannot be undone", AlertTone.Warning);

        // Act
        var html = _renderer.Render(alert);

        // Assert
        html.Should().Contain("<div class=\"alert alert--warning\" role=\"alert\">");
        html.Should().Contain("<p class=\"alert__message\">Warning: This action cannot be undone</p>");
    }

    [Fact]
    public void RenderAlert_WithCriticalTone_RendersCorrectHtml()
    {
        // Arrange
        var alert = new Alert("Error: Invalid credentials", AlertTone.Critical);

        // Act
        var html = _renderer.Render(alert);

        // Assert
        html.Should().Contain("<div class=\"alert alert--critical\" role=\"alert\">");
        html.Should().Contain("<p class=\"alert__message\">Error: Invalid credentials</p>");
    }

    [Fact]
    public void RenderAlert_WithId_IncludesIdAttribute()
    {
        // Arrange
        var alert = new Alert("Test message")
        {
            Id = "error-alert"
        };

        // Act
        var html = _renderer.Render(alert);

        // Assert
        html.Should().Contain("id=\"error-alert\"");
    }

    [Fact]
    public void RenderAlert_EscapesHtmlInMessage()
    {
        // Arrange
        var alert = new Alert("<script>alert('xss')</script>");

        // Act
        var html = _renderer.Render(alert);

        // Assert
        html.Should().Contain("&lt;script&gt;alert(&#39;xss&#39;)&lt;/script&gt;");
        html.Should().NotContain("<script>");
    }

    [Fact]
    public void RenderEmptyState_WithSmallSize_RendersCorrectHtml()
    {
        // Arrange
        var emptyState = new EmptyState("No items found")
        {
            Size = EmptyStateSize.Small
        };

        // Act
        var html = _renderer.Render(emptyState);

        // Assert
        html.Should().Contain("<div class=\"empty-state empty-state--small\">");
        html.Should().Contain("<p class=\"empty-state__message\">No items found</p>");
        html.Should().Contain("</div>");
    }

    [Fact]
    public void RenderEmptyState_WithMediumSize_RendersCorrectHtml()
    {
        // Arrange
        var emptyState = new EmptyState("Your list is empty")
        {
            Size = EmptyStateSize.Medium
        };

        // Act
        var html = _renderer.Render(emptyState);

        // Assert
        html.Should().Contain("<div class=\"empty-state empty-state--medium\">");
        html.Should().Contain("<p class=\"empty-state__message\">Your list is empty</p>");
    }

    [Fact]
    public void RenderEmptyState_WithLargeSize_RendersCorrectHtml()
    {
        // Arrange
        var emptyState = new EmptyState("No data available")
        {
            Size = EmptyStateSize.Large
        };

        // Act
        var html = _renderer.Render(emptyState);

        // Assert
        html.Should().Contain("<div class=\"empty-state empty-state--large\">");
        html.Should().Contain("<p class=\"empty-state__message\">No data available</p>");
    }

    [Fact]
    public void RenderEmptyState_WithAction_RendersActionLink()
    {
        // Arrange
        var emptyState = new EmptyState("No todos yet")
        {
            ActionLabel = "Create your first todo",
            ActionUrl = "/todos/create"
        };

        // Act
        var html = _renderer.Render(emptyState);

        // Assert
        html.Should().Contain("<a class=\"empty-state__action\" href=\"/todos/create\">Create your first todo</a>");
    }

    [Fact]
    public void RenderEmptyState_WithoutAction_DoesNotRenderActionLink()
    {
        // Arrange
        var emptyState = new EmptyState("No items found");

        // Act
        var html = _renderer.Render(emptyState);

        // Assert
        html.Should().NotContain("<a");
        html.Should().NotContain("empty-state__action");
    }

    [Fact]
    public void RenderEmptyState_EscapesHtmlInMessage()
    {
        // Arrange
        var emptyState = new EmptyState("<img src=x onerror=alert('xss')>");

        // Act
        var html = _renderer.Render(emptyState);

        // Assert
        html.Should().Contain("&lt;img src=x onerror=alert(&#39;xss&#39;)&gt;");
        html.Should().NotContain("<img");
    }

    [Fact]
    public void RenderEmptyState_EscapesHtmlInActionLabelAndUrl()
    {
        // Arrange
        var emptyState = new EmptyState("Empty")
        {
            ActionLabel = "<script>alert('xss')</script>",
            ActionUrl = "javascript:alert('xss')"
        };

        // Act
        var html = _renderer.Render(emptyState);

        // Assert
        html.Should().Contain("href=\"javascript:alert(&#39;xss&#39;)\"");
        html.Should().Contain("&lt;script&gt;alert(&#39;xss&#39;)&lt;/script&gt;");
    }
}
