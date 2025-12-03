using AwesomeAssertions;
using PagePlay.Site.Infrastructure.UI.Rendering;
using PagePlay.Site.Infrastructure.UI.Vocabulary;
using ListVocabulary = PagePlay.Site.Infrastructure.UI.Vocabulary.List;

namespace PagePlay.Tests.Infrastructure.UI;

public class HtmlRendererListElementsTests
{
    private readonly IHtmlRenderer _renderer = new HtmlRenderer();

    [Fact]
    public void RenderList_WithUnorderedStyle_RendersUlTag()
    {
        // Arrange
        var list = new ListVocabulary(
            new ListItem(new Text("Item 1")),
            new ListItem(new Text("Item 2"))
        )
        {
            Style = ListStyle.Unordered
        };

        // Act
        var html = _renderer.Render(list);

        // Assert
        html.Should().Contain("<ul class=\"list list--unordered\">");
        html.Should().Contain("</ul>");
        html.Should().NotContain("<ol");
    }

    [Fact]
    public void RenderList_WithOrderedStyle_RendersOlTag()
    {
        // Arrange
        var list = new ListVocabulary(
            new ListItem(new Text("First")),
            new ListItem(new Text("Second"))
        )
        {
            Style = ListStyle.Ordered
        };

        // Act
        var html = _renderer.Render(list);

        // Assert
        html.Should().Contain("<ol class=\"list list--ordered\">");
        html.Should().Contain("</ol>");
        html.Should().NotContain("<ul");
    }

    [Fact]
    public void RenderList_WithPlainStyle_RendersUlTagWithPlainClass()
    {
        // Arrange
        var list = new ListVocabulary(
            new ListItem(new Text("Todo 1")),
            new ListItem(new Text("Todo 2"))
        )
        {
            Style = ListStyle.Plain
        };

        // Act
        var html = _renderer.Render(list);

        // Assert
        html.Should().Contain("<ul class=\"list list--plain\">");
        html.Should().Contain("</ul>");
    }

    [Fact]
    public void RenderList_WithId_IncludesIdAttribute()
    {
        // Arrange
        var list = new ListVocabulary(
            new ListItem(new Text("Item"))
        )
        {
            Id = "todo-list"
        };

        // Act
        var html = _renderer.Render(list);

        // Assert
        html.Should().Contain("id=\"todo-list\"");
    }

    [Fact]
    public void RenderList_WithMultipleItems_RendersAllItems()
    {
        // Arrange
        var list = new ListVocabulary(
            new ListItem(new Text("Item 1")),
            new ListItem(new Text("Item 2")),
            new ListItem(new Text("Item 3"))
        )
        {
            Style = ListStyle.Plain
        };

        // Act
        var html = _renderer.Render(list);

        // Assert
        html.Should().Contain("<li class=\"list-item list-item--normal\">");
        html.Should().Contain("<p class=\"text\">Item 1</p>");
        html.Should().Contain("<p class=\"text\">Item 2</p>");
        html.Should().Contain("<p class=\"text\">Item 3</p>");
    }

    [Fact]
    public void RenderListItem_WithNormalState_RendersCorrectClass()
    {
        // Arrange
        var listItem = new ListItem(new Text("Normal item"))
        {
            State = ListItemState.Normal
        };

        // Act
        var html = _renderer.Render(listItem);

        // Assert
        html.Should().Contain("<li class=\"list-item list-item--normal\">");
        html.Should().Contain("<p class=\"text\">Normal item</p>");
        html.Should().Contain("</li>");
    }

    [Fact]
    public void RenderListItem_WithCompletedState_RendersCorrectClass()
    {
        // Arrange
        var listItem = new ListItem(new Text("Completed task"))
        {
            State = ListItemState.Completed
        };

        // Act
        var html = _renderer.Render(listItem);

        // Assert
        html.Should().Contain("<li class=\"list-item list-item--completed\">");
        html.Should().Contain("<p class=\"text\">Completed task</p>");
    }

    [Fact]
    public void RenderListItem_WithDisabledState_RendersCorrectClass()
    {
        // Arrange
        var listItem = new ListItem(new Text("Disabled item"))
        {
            State = ListItemState.Disabled
        };

        // Act
        var html = _renderer.Render(listItem);

        // Assert
        html.Should().Contain("<li class=\"list-item list-item--disabled\">");
        html.Should().Contain("<p class=\"text\">Disabled item</p>");
    }

    [Fact]
    public void RenderListItem_WithErrorState_RendersCorrectClass()
    {
        // Arrange
        var listItem = new ListItem(new Text("Error item"))
        {
            State = ListItemState.Error
        };

        // Act
        var html = _renderer.Render(listItem);

        // Assert
        html.Should().Contain("<li class=\"list-item list-item--error\">");
        html.Should().Contain("<p class=\"text\">Error item</p>");
    }

    [Fact]
    public void RenderListItem_WithId_IncludesIdAttribute()
    {
        // Arrange
        var listItem = new ListItem(new Text("Todo item"))
        {
            Id = "todo-123"
        };

        // Act
        var html = _renderer.Render(listItem);

        // Assert
        html.Should().Contain("id=\"todo-123\"");
    }

    [Fact]
    public void RenderListItem_WithComplexContent_RendersAllChildren()
    {
        // Arrange
        var listItem = new ListItem(
            new Row(For.Items,
                new Checkbox { Name = "id", Checked = false },
                new Text("Buy milk"),
                new Button(Importance.Ghost, "Delete")
            )
        )
        {
            State = ListItemState.Normal,
            Id = "todo-1"
        };

        // Act
        var html = _renderer.Render(listItem);

        // Assert
        html.Should().Contain("<li class=\"list-item list-item--normal\" id=\"todo-1\">");
        html.Should().Contain("<div class=\"row row--items\">");
        html.Should().Contain("<input type=\"checkbox\"");
        html.Should().Contain("<p class=\"text\">Buy milk</p>");
        html.Should().Contain("<button");
        html.Should().Contain("Delete");
        html.Should().Contain("</li>");
    }

    [Fact]
    public void RenderList_WithEmptyList_RendersEmptyListTag()
    {
        // Arrange
        var list = new ListVocabulary
        {
            Style = ListStyle.Plain,
            Id = "empty-list"
        };

        // Act
        var html = _renderer.Render(list);

        // Assert
        html.Should().Contain("<ul class=\"list list--plain\" id=\"empty-list\"></ul>");
    }

    [Fact]
    public void RenderList_TodoListScenario_RendersCompleteStructure()
    {
        // Arrange
        var list = new ListVocabulary(
            new ListItem(
                new Row(For.Items,
                    new Checkbox { Name = "id", Checked = false },
                    new Text("Buy groceries")
                )
            )
            {
                State = ListItemState.Normal,
                Id = "todo-1"
            },
            new ListItem(
                new Row(For.Items,
                    new Checkbox { Name = "id", Checked = true },
                    new Text("Finish report")
                )
            )
            {
                State = ListItemState.Completed,
                Id = "todo-2"
            }
        )
        {
            Style = ListStyle.Plain,
            Id = "todo-list"
        };

        // Act
        var html = _renderer.Render(list);

        // Assert
        html.Should().Contain("<ul class=\"list list--plain\" id=\"todo-list\">");
        html.Should().Contain("<li class=\"list-item list-item--normal\" id=\"todo-1\">");
        html.Should().Contain("<li class=\"list-item list-item--completed\" id=\"todo-2\">");
        html.Should().Contain("Buy groceries");
        html.Should().Contain("Finish report");
        html.Should().Contain("</ul>");
    }

    [Fact]
    public void RenderList_DefaultStyle_RendersUnordered()
    {
        // Arrange - No explicit style set, should default to Unordered
        var list = new ListVocabulary(
            new ListItem(new Text("Default item"))
        );

        // Act
        var html = _renderer.Render(list);

        // Assert
        html.Should().Contain("<ul class=\"list list--unordered\">");
    }

    [Fact]
    public void RenderListItem_DefaultState_RendersNormal()
    {
        // Arrange - No explicit state set, should default to Normal
        var listItem = new ListItem(new Text("Default state item"));

        // Act
        var html = _renderer.Render(listItem);

        // Assert
        html.Should().Contain("<li class=\"list-item list-item--normal\">");
    }
}
