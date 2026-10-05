namespace OrganisationRegistry.UnitTests.Backoffice.Organisation;

using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation;
using OrganisationRegistry.Tests.Shared;
using Xunit;

/// <summary>
/// <see cref="OrganisationPermissions.CanViewFunctions"/> / <c>CanViewCapacities</c>
/// / <c>CanViewKbo</c> / <c>CanViewVlimpers</c> across every backoffice role,
/// per the "R"/"CRUD"/"–" cells of <c>ui-permission-matrix.md</c>:
///
/// | Scherm       | VO | Algemeen | Decentraal (own org) | Vlimpers | Orgaan | Regelgeving |
/// |--------------|----|----------|-----------------------|----------|--------|--------------|
/// | Functies     | R  | CRUD     | CRUD                  | R        | R      | R            |
/// | KBO          | –  | CRUD     | –                     | –        | –      | R            |
/// | Vlimpers     | –  | CRUD     | –                     | –        | –      | R            |
///
/// Only the unauthenticated "Publiek" caller sees none of these — every
/// backoffice role gets at least read access to Functies/Hoedanigheden.
/// </summary>
public class OrganisationPermissionsViewFlagsAcrossRolesTests
{
    private const string OwnOvoNumber = "OVO000003";

    [Fact]
    public void User_cannot_view_functions_and_capacities()
    {
        var permissions = OrganisationPermissions.For(
            TestUser.User, OwnOvoNumber, isUnderVlimpersManagement: false);

        permissions.CanViewFunctions.Should().BeFalse();
        permissions.CanViewCapacities.Should().BeFalse();


        var permissions2 = OrganisationPermissions.For(
            TestUser.User, OwnOvoNumber, isUnderVlimpersManagement: true);

        permissions2.CanViewFunctions.Should().BeFalse();
        permissions2.CanViewCapacities.Should().BeFalse();
    }

    [Fact]
    public void AlgemeenBeheerder_can_view_everything_via_manage_implies_view()
    {
        var permissions = OrganisationPermissions.For(
            TestUser.AlgemeenBeheerder, OwnOvoNumber, isUnderVlimpersManagement: false);

        permissions.CanViewFunctions.Should().BeTrue();
        permissions.CanViewCapacities.Should().BeTrue();
        permissions.CanViewKbo.Should().BeTrue();
        permissions.CanViewVlimpers.Should().BeTrue();
    }

    [Fact]
    public void VlimpersBeheerder_can_view_functions_and_capacities_but_not_kbo_or_vlimpers()
    {
        var permissions = OrganisationPermissions.For(
            TestUser.VlimpersBeheerder, OwnOvoNumber, isUnderVlimpersManagement: true);

        permissions.CanViewFunctions.Should().BeTrue();
        permissions.CanViewCapacities.Should().BeTrue();
        permissions.CanViewKbo.Should().BeFalse();
        permissions.CanViewVlimpers.Should().BeFalse();
    }

    [Fact]
    public void OrgaanBeheerder_can_view_functions_and_capacities_but_not_kbo_or_vlimpers()
    {
        var permissions = OrganisationPermissions.For(
            TestUser.OrgaanBeheerder, OwnOvoNumber, isUnderVlimpersManagement: false);

        permissions.CanViewFunctions.Should().BeTrue();
        permissions.CanViewCapacities.Should().BeTrue();
        permissions.CanViewKbo.Should().BeFalse();
        permissions.CanViewVlimpers.Should().BeFalse();
    }

    [Fact]
    public void RegelgevingBeheerder_can_view_functions_kbo_and_vlimpers()
    {
        var permissions = OrganisationPermissions.For(
            TestUser.RegelgevingBeheerder, OwnOvoNumber, isUnderVlimpersManagement: false);

        permissions.CanViewFunctions.Should().BeTrue();
        permissions.CanViewKbo.Should().BeTrue();
        permissions.CanViewVlimpers.Should().BeTrue();
    }
}
