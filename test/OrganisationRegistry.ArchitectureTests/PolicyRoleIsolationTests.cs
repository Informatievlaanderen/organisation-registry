namespace OrganisationRegistry.ArchitectureTests;

using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

/// <summary>Waarom: policies mogen geen rolchecks meer bevatten, enkel scope-restricties (FR-008, SC-003).</summary>
public class PolicyRoleIsolationTests : ArchitectureTestBase
{
    private static readonly IObjectProvider<IType> PolicyNamespace =
        Types().That().ResideInNamespace("OrganisationRegistry.Handling.Authorization").As("policies");

    private static readonly IObjectProvider<IType> RoleEnum =
        Types().That().Are(typeof(Infrastructure.Authorization.Role)).As("Role enum");

    [Fact]
    public void PoliciesMustNotDependOnTheRoleEnum()
    {
        IArchRule rule = Types().That().Are(PolicyNamespace)
            .Should().NotDependOnAny(RoleEnum)
            .Because("policies must evaluate scope restrictions, not roles");

        rule.Check(Architecture);
    }
}
