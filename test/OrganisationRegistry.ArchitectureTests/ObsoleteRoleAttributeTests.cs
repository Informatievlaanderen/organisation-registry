namespace OrganisationRegistry.ArchitectureTests;

using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

/// <summary>Waarom: obsolete role-attributen en RoleMapping horen enkel thuis in de rand — verwijderen bij T035/T036.</summary>
public class ObsoleteRoleAttributeTests : ArchitectureTestBase
{
    [Fact]
    public void NobodyUsesOrProtectedAttribute()
    {
        var orProtected = Architecture.GetAttributeOfType(typeof(Api.Infrastructure.Security.OrProtectedAttribute));
        if (orProtected is null)
            return; // already removed from the codebase

        IArchRule rule = Classes().That()
            .AreNot(typeof(Api.Infrastructure.Security.OrProtectedAttribute))
            .Should().NotHaveAnyAttributes(Attributes().That().Are(typeof(Api.Infrastructure.Security.OrProtectedAttribute)))
            .Because("OrProtectedAttribute is the obsolete role-based catch-all (feature 009)");

        rule.Check(Architecture);
    }

    [Fact]
    public void RoleMappingIsOnlyUsedInsideTheEdgeTranslationLayer()
    {
        // RoleMapping maps ACM/IDM role names to claim values. After the rework it is
        // only invoked inside the edge translation layer (token builder, token exchange
        // claims transformation, the obsolete role attribute). Every other usage fails.
        IArchRule rule = Classes().That()
            .DoNotResideInNamespace("OrganisationRegistry.Api.Security")
            .And().DoNotResideInNamespace("OrganisationRegistry.Api.Infrastructure.Security")
            .Should().NotDependOnAny(Types().That().Are(typeof(Api.Security.RoleMapping)))
            .Because("role mapping must only be invoked inside the edge translation layer (SC-006)");

        rule.Check(Architecture);
    }
}
