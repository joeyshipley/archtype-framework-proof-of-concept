using AwesomeAssertions;
using PagePlay.Site.Infrastructure.Web.Components;

namespace PagePlay.Tests.Infrastructure.Web.Components;

public class ComponentContextParserUnitTests
{
    [Fact]
    public void Parse_WithNullInput_ReturnsEmptyList()
    {
        // Arrange
        var parser = new ComponentContextParser();

        // Act
        var result = parser.Parse(null);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(0);
    }

    [Fact]
    public void Parse_WithEmptyString_ReturnsEmptyList()
    {
        // Arrange
        var parser = new ComponentContextParser();

        // Act
        var result = parser.Parse("");

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(0);
    }

    [Fact]
    public void Parse_WithWhitespace_ReturnsEmptyList()
    {
        // Arrange
        var parser = new ComponentContextParser();

        // Act
        var result = parser.Parse("   ");

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(0);
    }

    [Fact]
    public void Parse_WithValidSingleComponent_ReturnsComponentInfo()
    {
        // Arrange
        var parser = new ComponentContextParser();
        var json = """
            [
                {
                    "Id": "welcome-widget",
                    "ComponentType": "WelcomeWidget",
                    "Domain": "todos"
                }
            ]
            """;

        // Act
        var result = parser.Parse(json);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(1);
        result[0].Id.Should().Be("welcome-widget");
        result[0].ComponentType.Should().Be("WelcomeWidget");
        result[0].Domain.Should().Be("todos");
    }

    [Fact]
    public void Parse_WithMultipleComponents_ReturnsAllComponentInfo()
    {
        // Arrange
        var parser = new ComponentContextParser();
        var json = """
            [
                {
                    "Id": "welcome-widget",
                    "ComponentType": "WelcomeWidget",
                    "Domain": "todos"
                },
                {
                    "Id": "todo-list",
                    "ComponentType": "TodoList",
                    "Domain": "todos"
                },
                {
                    "Id": "notification-bell",
                    "ComponentType": "NotificationBell",
                    "Domain": "notifications"
                }
            ]
            """;

        // Act
        var result = parser.Parse(json);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(3);

        result[0].Id.Should().Be("welcome-widget");
        result[0].ComponentType.Should().Be("WelcomeWidget");
        result[0].Domain.Should().Be("todos");

        result[1].Id.Should().Be("todo-list");
        result[1].ComponentType.Should().Be("TodoList");
        result[1].Domain.Should().Be("todos");

        result[2].Id.Should().Be("notification-bell");
        result[2].ComponentType.Should().Be("NotificationBell");
        result[2].Domain.Should().Be("notifications");
    }

    [Fact]
    public void Parse_WithInvalidJson_ReturnsEmptyList()
    {
        // Arrange
        var parser = new ComponentContextParser();
        var invalidJson = "{ this is not valid json }";

        // Act
        var result = parser.Parse(invalidJson);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(0);
    }

    [Fact]
    public void Parse_WithMalformedJson_ReturnsEmptyList()
    {
        // Arrange
        var parser = new ComponentContextParser();
        var malformedJson = "[{\"Id\": \"test\", }]"; // Trailing comma

        // Act
        var result = parser.Parse(malformedJson);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(0);
    }

    [Fact]
    public void Parse_WithEmptyArray_ReturnsEmptyList()
    {
        // Arrange
        var parser = new ComponentContextParser();
        var json = "[]";

        // Act
        var result = parser.Parse(json);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(0);
    }

    [Fact]
    public void Parse_WithPartialData_ReturnsComponentWithEmptyStrings()
    {
        // Arrange
        var parser = new ComponentContextParser();
        var json = """
            [
                {
                    "Id": "test-widget"
                }
            ]
            """;

        // Act
        var result = parser.Parse(json);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(1);
        result[0].Id.Should().Be("test-widget");
        result[0].ComponentType.Should().Be("");
        result[0].Domain.Should().Be("");
    }
}
