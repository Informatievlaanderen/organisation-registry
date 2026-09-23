namespace OrganisationRegistry.ArchitectureTests;

using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

/// <summary>Waarom: policies checken enkel scope-restricties, nooit rollen (SC-003/FR-008).</summary>
public class PolicyRoleIsolationTests : ArchitectureTestBase
{
    private static readonly IObjectProvider<Class> SecurityPolicies =
        Classes().That().ImplementInterface(typeof(Handling.Authorization.ISecurityPolicy))
            .As("security policies");

    [Fact]
    public void PoliciesMustNotDependOnTheRoleEnum()
    {
        IArchRule rule = Classes().That().Are(SecurityPolicies)
            .Should().NotDependOnAny(typeof(Infrastructure.Authorization.Role))
            .Because("policies check only scope restrictions after the permission rework (FR-008, SC-003)");

        rule.Check(Architecture);
    }
}
