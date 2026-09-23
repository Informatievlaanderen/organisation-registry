namespace OrganisationRegistry.ArchitectureTests;

using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

/// <summary>Waarom: rollen/scopes mogen na vertaling aan de rand nergens meer voorkomen — enkel in RolePermissionMap/ScopePermissionMap (SC-006).</summary>
public class RoleAndScopeIsolationTests : ArchitectureTestBase
{
    private static readonly IObjectProvider<IType> RoleEnum =
        Types().That().Are(typeof(Infrastructure.Authorization.Role)).As("Role enum");

    private static readonly IObjectProvider<IType> EdgeTranslationLayer =
        Types().That()
            .ResideInNamespace("OrganisationRegistry.Infrastructure.Authorization")
            .Or().ResideInNamespace("OrganisationRegistry.Api.Security")
            .Or().ResideInNamespace("OrganisationRegistry.Api.Infrastructure.Security")
            .As("edge translation layer");

    [Fact]
    public void RoleEnumIsOnlyReferencedFromTheEdgeTranslationLayer()
    {
        IArchRule rule = Types().That().AreNot(RoleEnum)
            .And().AreNot(EdgeTranslationLayer)
            .Should().NotDependOnAny(RoleEnum)
            .Because("roles must be translated to permissions at the edge (FR-015, SC-006)");

        rule.Check(Architecture);
    }
}
