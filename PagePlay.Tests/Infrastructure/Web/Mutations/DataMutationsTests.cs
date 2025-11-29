using AwesomeAssertions;
using PagePlay.Site.Infrastructure.Web.Mutations;

namespace PagePlay.Tests.Infrastructure.Web.Mutations;

public class DataMutationsTests
{
    [Fact]
    public void For_WithSingleDomain_ReturnsMutationsWithOneDomain()
    {
        var mutations = DataMutations.For("todos");

        mutations.Domains.Should().HaveCount(1);
        mutations.Domains[0].Should().Be("todos");
    }

    [Fact]
    public void For_WithMultipleDomains_ReturnsMutationsWithAllDomains()
    {
        var mutations = DataMutations.For("todos", "notifications");

        mutations.Domains.Should().HaveCount(2);
        mutations.Domains[0].Should().Be("todos");
        mutations.Domains[1].Should().Be("notifications");
    }

    [Fact]
    public void Domains_IsPubliclyAccessible()
    {
        var mutations = DataMutations.For("todos");

        var domains = mutations.Domains;

        domains.Should().NotBeNull();
        domains.Should().BeAssignableTo<List<string>>();
    }

    [Fact]
    public void For_WithNoDomains_ReturnsEmptyMutations()
    {
        var mutations = DataMutations.For();

        mutations.Domains.Should().BeEmpty();
    }

    [Fact]
    public void For_WithThreeDomains_ReturnsMutationsWithAllThree()
    {
        var mutations = DataMutations.For("todos", "notifications", "accounts");

        mutations.Domains.Should().HaveCount(3);
        mutations.Domains.Should().Contain("todos");
        mutations.Domains.Should().Contain("notifications");
        mutations.Domains.Should().Contain("accounts");
    }
}
