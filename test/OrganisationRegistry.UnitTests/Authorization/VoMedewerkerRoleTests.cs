namespace OrganisationRegistry.UnitTests.Authorization;

using System;
using System.Linq;
using FluentAssertions;
using OrganisationRegistry.Api.Security;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;

/// <summary>
/// Rol <b>VO medewerker</b> (<see cref="Role.VoMedewerker"/>): read-only rol,
/// conceptueel "Publiek + leesrecht op Functies en Hoedanigheden" (zowel op
/// organisaties als op personen). Geen enkel schrijf/aanmaak/verwijderrecht,
/// geen scoping. Zie <c>ui-permission-matrix.md</c> voor de volledige
/// rechtenmatrix.
/// </summary>
[Collection("PermissionMapThrottleState")]
public class VoMedewerkerRoleTests
{
    public VoMedewerkerRoleTests() => RolePermissionMap.ResetThrottleState();

    /// <summary>Every permission the closed set does NOT explicitly grant to VoMedewerker.</summary>
    private static readonly Permission[] AllPermissions =
        Enum.GetValues<Permission>();

    private static readonly Permission[] ExpectedGrants =
    {
        Permission.PeopleFunctionsRead,
        Permission.PeopleCapacitiesRead,
        Permission.CanViewOrganisationFunctions,
        Permission.CanViewOrganisationCapacities,
    };

    [Fact]
    public void Grants_exactly_the_four_read_only_permissions()
    {
        var set = RolePermissionMap.For(Role.VoMedewerker);

        foreach (var expected in ExpectedGrants)
            set.Contains(expected).Should().BeTrue($"VoMedewerker should hold {expected}");

        set.Count.Should().Be(ExpectedGrants.Length);
    }

    [Fact]
    public void Grants_nothing_beyond_the_four_read_only_permissions()
    {
        var set = RolePermissionMap.For(Role.VoMedewerker);

        var unexpected = AllPermissions
            .Except(ExpectedGrants)
            .Where(p => set.Contains(p))
            .ToList();

        unexpected.Should().BeEmpty(
            "VO medewerker is read-only and must not hold any other permission (fail-closed default)");
    }

    [Theory]
    [InlineData(Permission.CanManageOrganisation)]
    [InlineData(Permission.CanManageOrganisationInfoLimitedToVlimpers)]
    [InlineData(Permission.CanManageOrganisationInfoNotLimitedToVlimpers)]
    [InlineData(Permission.CanTerminateOrganisation)]
    [InlineData(Permission.CanCreateOrganisations)]
    [InlineData(Permission.CanManageChildren)]
    [InlineData(Permission.CanManageParent)]
    [InlineData(Permission.CanManageContacts)]
    [InlineData(Permission.CanManageFunctions)]
    [InlineData(Permission.CanManageCapacities)]
    [InlineData(Permission.CanManageLocations)]
    [InlineData(Permission.CanManageBuildings)]
    [InlineData(Permission.CanManageLabels)]
    [InlineData(Permission.CanManageOrganisationClassifications)]
    [InlineData(Permission.CanManageFormalFrameworks)]
    [InlineData(Permission.CanManageKeys)]
    [InlineData(Permission.CanManageRegulations)]
    [InlineData(Permission.CanManageBodies)]
    [InlineData(Permission.BodiesCanManageContacts)]
    [InlineData(Permission.BodiesCanManageSeats)]
    [InlineData(Permission.BodiesCanManageMandates)]
    [InlineData(Permission.BodiesCanManageLifecycles)]
    [InlineData(Permission.BodiesCanManageOrganisations)]
    [InlineData(Permission.BodiesCanManageClassifications)]
    [InlineData(Permission.BodiesCanManageFormalFrameworks)]
    [InlineData(Permission.BodiesCanManageMep)]
    [InlineData(Permission.CanManageRelations)]
    [InlineData(Permission.CanManageKbo)]
    [InlineData(Permission.CanManageVlimpers)]
    [InlineData(Permission.PeopleWrite)]
    [InlineData(Permission.CanImport)]
    [InlineData(Permission.CanRunScheduledJobs)]
    [InlineData(Permission.CanReadOrafin)]
    [InlineData(Permission.CanReadConfiguration)]
    [InlineData(Permission.System)]
    [InlineData(Permission.ParametersLocationsWrite)]
    [InlineData(Permission.ParametersBuildingsWrite)]
    [InlineData(Permission.ParametersInformationSystemsWrite)]
    [InlineData(Permission.ParametersInformationSystemsDelete)]
    [InlineData(Permission.ParametersOrganisationClassificationsWrite)]
    [InlineData(Permission.ParametersOrganisationClassificationTypesWrite)]
    [InlineData(Permission.ParametersBodyClassificationsWrite)]
    [InlineData(Permission.ParametersBodyClassificationTypesWrite)]
    [InlineData(Permission.ParametersOrganisationRelationTypesWrite)]
    [InlineData(Permission.ParametersFormalFrameworksWrite)]
    [InlineData(Permission.ParametersFormalFrameworkCategoriesWrite)]
    [InlineData(Permission.ParametersLifecyclePhaseTypesWrite)]
    [InlineData(Permission.ParametersCapacitiesWrite)]
    [InlineData(Permission.ParametersCapacitiesDelete)]
    [InlineData(Permission.ParametersFunctionTypesWrite)]
    [InlineData(Permission.ParametersContactTypesWrite)]
    [InlineData(Permission.ParametersLabelTypesWrite)]
    [InlineData(Permission.ParametersPurposesWrite)]
    [InlineData(Permission.ParametersSeatTypesWrite)]
    [InlineData(Permission.ParametersMandateRoleTypesWrite)]
    [InlineData(Permission.ParametersLocationTypesWrite)]
    [InlineData(Permission.ParametersRegulationThemesWrite)]
    [InlineData(Permission.ParametersRegulationSubThemesWrite)]
    [InlineData(Permission.DelegationsRead)]
    [InlineData(Permission.DelegationsWrite)]
    [InlineData(Permission.DelegationsDelete)]
    [InlineData(Permission.DelegationsCreate)]
    public void Every_write_create_delete_and_admin_permission_is_denied(Permission permission)
    {
        RolePermissionMap.For(Role.VoMedewerker).Contains(permission).Should().BeFalse();
    }

    [Fact]
    public void Config_aware_overload_grants_no_additional_restricted_permissions()
    {
        // VoMedewerker has no scoping/restrictions at all: the config-aware
        // overload (which layers data-driven restricted grants on top of the
        // static map) must be identical to the config-less static map for
        // this role.
        var config = new OrganisationRegistry.Tests.Shared.Stubs.OrganisationRegistryConfigurationStub();

        var staticSet = RolePermissionMap.For(Role.VoMedewerker);
        var configAwareSet = RolePermissionMap.For(new[] { Role.VoMedewerker }, config);

        configAwareSet.Count.Should().Be(staticSet.Count);
        foreach (var expected in ExpectedGrants)
            configAwareSet.Contains(expected).Should().BeTrue();
    }

    [Fact]
    public void Combined_with_a_write_role_adds_no_extra_write_rights_over_that_role_alone()
    {
        // OR-semantics across roles: union(VoMedewerker, DecentraalBeheerder)
        // must not contain any permission that DecentraalBeheerder alone
        // doesn't already have — VoMedewerker only contributes its own
        // (already-covered) read permissions.
        var decentraalAlone = RolePermissionMap.For(Role.DecentraalBeheerder);
        var combined = RolePermissionMap.For(new[] { Role.VoMedewerker, Role.DecentraalBeheerder });

        var extra = AllPermissions
            .Where(p => combined.Contains(p) && !decentraalAlone.Contains(p))
            .ToList();

        // The only permissions VoMedewerker may contribute on top of another
        // role are its own read-only grants; it must never add a write,
        // create or delete permission.
        extra.Should().OnlyContain(
            p => ExpectedGrants.Contains(p),
            "VoMedewerker must only ever add its own read-only permissions, never a write/create/delete right");
    }

    [Fact]
    public void RoleMapping_round_trips_the_VoMedewerker_claim_value()
    {
        var claim = RoleMapping.Map(Role.VoMedewerker);
        claim.Should().Be(AcmIdmConstants.Roles.VoMedewerker);
        RoleMapping.Exists(claim).Should().BeTrue();
        RoleMapping.Map(claim).Should().Be(Role.VoMedewerker);
    }
}
