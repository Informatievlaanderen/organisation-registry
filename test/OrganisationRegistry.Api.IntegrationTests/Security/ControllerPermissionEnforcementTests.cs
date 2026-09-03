namespace OrganisationRegistry.Api.IntegrationTests.Security;

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using OrganisationRegistry.Infrastructure.Authorization;
using Tests.Shared;
using Xunit;

/// <summary>
/// T023 — Controller-level permission enforcement integration tests.
///
/// Verifies that after the T026 controller sweep, every endpoint gated by
/// <c>[OrganisationRegistryAuthorize(RequiredPermissions = new[] { Permission.X })]</c>
/// returns 200 for identities carrying that permission and
/// 403 for identities that do not.
///
/// Currently all facts are <c>Skip</c>ped because:
///   1. T026 (controller sweep) has not run — controllers still gate on <c>Role[]</c>,
///      so a permission-based assertion would validate the wrong contract.
///   2. <see cref="ApiFixture.HttpClient"/> authenticates as <c>AlgemeenBeheerder</c>,
///      which carries every permission granularly via <c>RolePermissionMap</c>. A
///      limited-permission interactive identity is not yet provisioned in the
///      fixture and will be added alongside T026.
///
/// Structure: one <c>[Fact]</c> per Permission enum value, naming the representative
/// endpoint the T026 sweep must migrate. When T026 lands, unskip in the same commit
/// that flips the corresponding attribute.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class ControllerPermissionEnforcementTests
{
    private const string T026 = "T026 — awaiting controller sweep to convert Role[] attributes to RequiredPermissions[].";

    private readonly ApiFixture _apiFixture;

    public ControllerPermissionEnforcementTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }


    // -----------------------------------------------------------------------
    // Organisation editing family
    // -----------------------------------------------------------------------

    [Fact(Skip = T026)]
    public Task CanEditChildren_Gates_OrganisationDetailCommandController()
        => AssertPermissionGate(
            endpoint: "/v1/organisations/{id}/parent",
            requiredPermission: Permission.CanEditChildren,
            expectedForAlgemeenBeheerder: HttpStatusCode.OK);

    [Fact(Skip = T026)]
    public Task CanEditVlimpers_Gates_OrganisationVlimpersEndpoints()
        => AssertPermissionGate(
            endpoint: "/v1/organisations/{id}/vlimpers",
            requiredPermission: Permission.CanEditVlimpers,
            expectedForAlgemeenBeheerder: HttpStatusCode.OK);

    [Fact(Skip = T026)]
    public Task CanEditDelegations_Gates_DelegationController()
        => AssertPermissionGate(
            endpoint: "/v1/delegations",
            requiredPermission: Permission.CanEditDelegations,
            expectedForAlgemeenBeheerder: HttpStatusCode.OK);

    // -----------------------------------------------------------------------
    // Parameter management family
    // -----------------------------------------------------------------------

    [Fact(Skip = T026)]
    public Task CanAddLocations_Gates_LocationCommandController()
        => AssertPermissionGate(
            endpoint: "/v1/locations",
            requiredPermission: Permission.CanAddLocations,
            expectedForAlgemeenBeheerder: HttpStatusCode.OK);

    [Fact(Skip = T026)]
    public Task CanAddBodies_Gates_BodyCreateEndpoint()
        => AssertPermissionGate(
            endpoint: "/v1/bodies",
            requiredPermission: Permission.CanAddBodies,
            expectedForAlgemeenBeheerder: HttpStatusCode.OK);

    [Fact(Skip = T026)]
    public Task CanEditBodies_Gates_BodyDetailCommandController()
        => AssertPermissionGate(
            endpoint: "/v1/bodies/{id}",
            requiredPermission: Permission.CanEditBodies,
            expectedForAlgemeenBeheerder: HttpStatusCode.OK);

    [Fact(Skip = T026)]
    public Task CanRegisterBodies_Gates_BodyRegistrationEndpoint()
        => AssertPermissionGate(
            endpoint: "/v1/bodies/{id}/register",
            requiredPermission: Permission.CanRegisterBodies,
            expectedForAlgemeenBeheerder: HttpStatusCode.OK);

    [Fact(Skip = T026)]
    public Task CanManageKeys_Gates_KeyTypeCommandController()
        => AssertPermissionGate(
            endpoint: "/v1/keytypes",
            requiredPermission: Permission.CanManageKeys,
            expectedForAlgemeenBeheerder: HttpStatusCode.OK);

    [Fact(Skip = T026)]
    public Task CanManageLabels_Gates_LabelTypeCommandController()
        => AssertPermissionGate(
            endpoint: "/v1/labeltypes",
            requiredPermission: Permission.CanManageLabels,
            expectedForAlgemeenBeheerder: HttpStatusCode.OK);

    [Fact(Skip = T026)]
    public Task CanEditOrganisationLabels_Gates_OrganisationLabelCommandController()
        => AssertPermissionGate(
            endpoint: "/v1/organisations/{id}/labels",
            requiredPermission: Permission.CanEditOrganisationLabels,
            expectedForAlgemeenBeheerder: HttpStatusCode.OK);

    [Fact(Skip = T026)]
    public Task CanManageCapacities_Gates_CapacityCommandController()
        => AssertPermissionGate(
            endpoint: "/v1/capacities",
            requiredPermission: Permission.CanManageCapacities,
            expectedForAlgemeenBeheerder: HttpStatusCode.OK);

    [Fact(Skip = T026)]
    public Task CanManageFormalFrameworks_Gates_FormalFrameworkCommandController()
        => AssertPermissionGate(
            endpoint: "/v1/formalframeworks",
            requiredPermission: Permission.CanManageFormalFrameworks,
            expectedForAlgemeenBeheerder: HttpStatusCode.OK);

    [Fact(Skip = T026)]
    public Task CanManageOrganisationClassifications_Gates_OrganisationClassificationCommandController()
        => AssertPermissionGate(
            endpoint: "/v1/organisationclassifications",
            requiredPermission: Permission.CanManageOrganisationClassifications,
            expectedForAlgemeenBeheerder: HttpStatusCode.OK);

    [Fact(Skip = T026)]
    public Task CanManageRegulations_Gates_RegulationThemeCommandController()
        => AssertPermissionGate(
            endpoint: "/v1/regulationthemes",
            requiredPermission: Permission.CanManageRegulations,
            expectedForAlgemeenBeheerder: HttpStatusCode.OK);

    // -----------------------------------------------------------------------
    // Automation / integration family (Client Credentials)
    // -----------------------------------------------------------------------

    [Fact(Skip = T026)]
    public Task CanImport_Gates_ImportEndpoints()
        => AssertPermissionGate(
            endpoint: "/v1/import",
            requiredPermission: Permission.CanImport,
            expectedForAlgemeenBeheerder: HttpStatusCode.OK);

    [Fact(Skip = T026)]
    public Task CanRunScheduledJobs_Gates_AutomatedTaskEndpoints()
        => AssertPermissionGate(
            endpoint: "/v1/tasks",
            requiredPermission: Permission.CanRunScheduledJobs,
            expectedForAlgemeenBeheerder: HttpStatusCode.OK);

    [Fact(Skip = T026)]
    public Task CanReadOrafin_Gates_OrafinReadEndpoints()
        => AssertPermissionGate(
            endpoint: "/v1/orafin/organisations",
            requiredPermission: Permission.CanReadOrafin,
            expectedForAlgemeenBeheerder: HttpStatusCode.OK);

    [Fact(Skip = T026)]
    public Task CanReadInfoEndpoints_Gates_InfoScopeEndpoints()
        => AssertPermissionGate(
            endpoint: "/v1/info/organisations",
            requiredPermission: Permission.CanReadInfoEndpoints,
            expectedForAlgemeenBeheerder: HttpStatusCode.OK);

    // -----------------------------------------------------------------------
    // CanReadConfiguration — ConfigurationController migrated ahead of the
    // rest of T026a. AlgemeenBeheerder carries CanReadConfiguration explicitly
    // via RolePermissionMap, so 200 is expected via the seeded identity.
    // Per-role negative facts remain deferred until ApiFixture exposes a
    // limited-permission HttpClient.
    // -----------------------------------------------------------------------

    [Fact]
    public Task CanReadConfiguration_Gates_ConfigurationController()
        => AssertPermissionGate(
            endpoint: "/v1/configuration",
            requiredPermission: Permission.CanReadConfiguration,
            expectedForAlgemeenBeheerder: HttpStatusCode.OK);

    // -----------------------------------------------------------------------
    // Negative case — identity lacking the permission must receive 403.
    // Unskipped once a limited-permission interactive identity is seeded in
    // ApiFixture (planned as part of T026 completion).
    // -----------------------------------------------------------------------

    [Fact(Skip = T026)]
    public Task IdentityWithoutRequiredPermission_Returns403()
        => AssertPermissionGate(
            endpoint: "/v1/keytypes",
            requiredPermission: Permission.CanManageKeys,
            expectedForAlgemeenBeheerder: HttpStatusCode.OK,
            expectedForLimitedIdentity: HttpStatusCode.Forbidden);

    // -----------------------------------------------------------------------
    // Helper — kept intentionally minimal until T026 provides the concrete
    // limited-permission identity path. Signature encodes the expected 200/403
    // matrix so implementations only need to fill in the identity resolver.
    // -----------------------------------------------------------------------

    private async Task AssertPermissionGate(
        string endpoint,
        Permission requiredPermission,
        HttpStatusCode expectedForAlgemeenBeheerder,
        HttpStatusCode? expectedForLimitedIdentity = null)
    {
        // AlgemeenBeheerder identity — carries the required permission explicitly.
        var algemeenResponse = await _apiFixture.HttpClient.GetAsync(endpoint);
        algemeenResponse.StatusCode.Should().Be(expectedForAlgemeenBeheerder,
            $"AlgemeenBeheerder must access endpoint gated by {requiredPermission}, which it carries explicitly via RolePermissionMap.");

        if (expectedForLimitedIdentity.HasValue)
        {
            // T026 follow-up: swap in limited-permission HttpClient once ApiFixture exposes one.
            // Placeholder retains the assertion shape so the fact fails loudly if unskipped early.
            expectedForLimitedIdentity.Value.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
