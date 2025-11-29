using AwesomeAssertions;
using PagePlay.Site.Infrastructure.Web.Data;

namespace PagePlay.Tests.Infrastructure.Web.Data;

public class DomainDataContextUnitTests
{
    [Fact]
    public void IndexerSet_StoresValue()
    {
        // Arrange
        var context = new DomainDataContext();

        // Act
        context["openCount"] = 5;

        // Assert
        context["openCount"].Should().Be(5);
    }

    [Fact]
    public void Get_WithValidKey_ReturnsTypedValue()
    {
        // Arrange
        var context = new DomainDataContext
        {
            ["openCount"] = 42,
            ["completionRate"] = 0.75
        };

        // Act
        var openCount = context.Get<int>("openCount");
        var completionRate = context.Get<double>("completionRate");

        // Assert
        openCount.Should().Be(42);
        completionRate.Should().Be(0.75);
    }

    [Fact]
    public void Get_WithComplexType_ReturnsTypedValue()
    {
        // Arrange
        var todos = new List<string> { "Todo 1", "Todo 2" };
        var context = new DomainDataContext
        {
            ["list"] = todos
        };

        // Act
        var result = context.Get<List<string>>("list");

        // Assert
        result.Should().BeSameAs(todos);
        result.Count.Should().Be(2);
    }

    [Fact]
    public void Contains_WithExistingKey_ReturnsTrue()
    {
        // Arrange
        var context = new DomainDataContext
        {
            ["openCount"] = 5
        };

        // Act & Assert
        context.Contains("openCount").Should().BeTrue();
    }

    [Fact]
    public void Contains_WithNonExistentKey_ReturnsFalse()
    {
        // Arrange
        var context = new DomainDataContext();

        // Act & Assert
        context.Contains("nonExistent").Should().BeFalse();
    }

    [Fact]
    public void Get_WithNonExistentKey_ThrowsException()
    {
        // Arrange
        var context = new DomainDataContext();

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => context.Get<int>("nonExistent"));
    }
}
