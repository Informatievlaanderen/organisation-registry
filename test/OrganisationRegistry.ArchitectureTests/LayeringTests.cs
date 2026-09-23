namespace OrganisationRegistry.ArchitectureTests;

using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

/// <summary>Waarom: domein blijft onafhankelijk van de API; policies lezen restricties uit SQL, nooit ElasticSearch (FR-018).</summary>
public class LayeringTests : ArchitectureTestBase
{
    [Fact]
    public void DomainDoesNotDependOnApiLayer()
    {
        IArchRule rule = Types().That().ResideInAssembly(DomainAssembly)
            .Should().NotDependOnAny(Types().That().ResideInAssembly(ApiAssembly))
            .Because("the domain assembly must stay independent of the API host");

        rule.Check(Architecture);
    }

    [Fact]
    public void AuthorizationPoliciesDoNotDependOnElasticSearch()
    {
        IArchRule rule = Classes().That()
            .ResideInNamespace("OrganisationRegistry.Handling.Authorization")
            .Should().NotDependOnAnyTypesThat()
            .ResideInNamespace("OrganisationRegistry.ElasticSearch")
            .Because("JIT scope restrictions come from SQL Server projections, not ElasticSearch (FR-018)");

        rule.Check(Architecture);
    }
}
