namespace OrganisationRegistry.ArchitectureTests;

using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

/// <summary>Waarom: [OrganisationRegistryAuthorize(Role = ...)] en RoleMapping zijn verboden migratieoverblijfselen (FR-006).</summary>
public class ObsoleteRoleAttributeTests : ArchitectureTestBase
{
    private static readonly IObjectProvider<IType> EdgeTranslationLayer =
        Types().That()
            .ResideInNamespace("OrganisationRegistry.Infrastructure.Authorization")
            .Or().ResideInNamespace("OrganisationRegistry.Api.Security")
            .Or().ResideInNamespace("OrganisationRegistry.Api.Infrastructure.Security")
            .Or().ResideInNamespace("OrganisationRegistry.Api.Auth")
            .Or().ResideInNamespace("OrganisationRegistry.Api.Auth.Models")
            .As("edge translation layer");

    [Fact]
    public void RoleMappingIsOnlyUsedInsideTheEdgeTranslationLayer()
    {
        var roleMappingType = Architecture.GetITypeOfType(typeof(Infrastructure.Authorization.Role));

        IArchRule rule = Types().That().AreNot(EdgeTranslationLayer)
            .Should().NotDependOnAny(Types().That().Are(roleMappingType))
            .Because("Role mapping only exists to feed the edge translation tables");

        rule.Check(Architecture);
    }
}
