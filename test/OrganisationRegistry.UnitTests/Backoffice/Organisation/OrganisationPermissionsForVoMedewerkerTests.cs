namespace OrganisationRegistry.UnitTests.Backoffice.Organisation;

using System;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Tests.Shared;
using Xunit;

/// <summary>
/// <see cref="OrganisationPermissions"/> for the read-only VO medewerker
/// role: only <see cref="OrganisationPermissions.CanViewFunctions"/> and
/// <see cref="OrganisationPermissions.CanViewCapacities"/> should be true;
/// every manage/edit/delete flag, and <c>CanViewKbo</c>/<c>CanViewVlimpers</c>,
/// must be false.
/// </summary>
public class OrganisationPermissionsForVoMedewerkerTests
{
    private const string OvoNumber = "OVO000001";

    [Fact]
    public void Can_view_functions_and_capacities_but_nothing_else_for_a_regular_organisation()
    {
        var permissions = OrganisationPermissions.For(
            TestUser.VoMedewerker, OvoNumber, isUnderVlimpersManagement: false);

        permissions.CanViewFunctions.Should().BeTrue();
        permissions.CanViewCapacities.Should().BeTrue();

        permissions.CanManageFunctions.Should().BeFalse();
        permissions.CanManageCapacities.Should().BeFalse();
        permissions.CanEdit.Should().BeFalse();
        permissions.CanDelete.Should().BeFalse();
        permissions.CanManageChildren.Should().BeFalse();
        permissions.CanManageContacts.Should().BeFalse();
        permissions.CanManageLocations.Should().BeFalse();
        permissions.CanManageBuildings.Should().BeFalse();
        permissions.CanManageNames.Should().BeFalse();
        permissions.CanManageClassifications.Should().BeFalse();
        permissions.CanManageFormalFrameworks.Should().BeFalse();
        permissions.CanManageKeys.Should().BeFalse();
        permissions.CanManageRelations.Should().BeFalse();
        permissions.CanManageLabels.Should().BeFalse();
        permissions.CanManageRegulations.Should().BeFalse();
        permissions.CanManageKbo.Should().BeFalse();
        permissions.CanManageVlimpers.Should().BeFalse();
        permissions.CanViewKbo.Should().BeFalse();
        permissions.CanViewVlimpers.Should().BeFalse();
    }

    [Fact]
    public void Nothing_changes_when_the_organisation_is_under_Vlimpers_management()
    {
        // VO medewerker has no scoping at all: Vlimpers-management status of
        // the organisation must not affect any of its (read-only) rights.
        var permissions = OrganisationPermissions.For(
            TestUser.VoMedewerker, OvoNumber, isUnderVlimpersManagement: true);

        permissions.CanViewFunctions.Should().BeTrue();
        permissions.CanViewCapacities.Should().BeTrue();
        permissions.CanEdit.Should().BeFalse();
        permissions.CanManageLabels.Should().BeFalse();
        permissions.CanManageKeys.Should().BeFalse();
    }
}
