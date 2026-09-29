namespace OrganisationRegistry.UnitTests.Auth;

using System.Linq;
using FluentAssertions;
using OrganisationRegistry.Api.Auth.Models;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using Tests.Shared.Stubs;
using Xunit;

/// <summary>
/// Verifies the "&lt;resource&gt;:&lt;operation&gt;" permission strings surfaced on
/// <c>/v1/me</c> for each role. <see cref="RolePermissions.Resolve"/> must be fed the
/// config-aware permission set (i.e. including restricted grants) or Vlimpers/Decentraal
/// would incorrectly look like they have no access at all.
/// </summary>
public class RolePermissionsTests
{
    private static readonly IOrganisationRegistryConfiguration Configuration =
        new OrganisationRegistryConfigurationStub();

    private static PermissionSet PermissionsFor(Role role)
        => RolePermissionMap.For(new[] { role }, Configuration);

    [Fact]
    public void AlgemeenBeheerder_has_the_expected_global_permissions()
    {
        var result = RolePermissions.Resolve(Role.AlgemeenBeheerder, PermissionsFor(Role.AlgemeenBeheerder)).ToList();

        result.Should().BeEquivalentTo(new[]
        {
            "organisations:create",
            "bodies:create",
            "delegations:read",
            "delegations:write",
            "delegations:delete",
            "parameters.locations:write",
            "parameters.buildings:write",
            "parameters.information-systems:write",
            "parameters.information-systems:delete",
            "parameters.organisation-classifications:write",
            "parameters.organisation-classification-types:write",
            "parameters.body-classifications:write",
            "parameters.body-classification-types:write",
            "parameters.organisation-relation-types:write",
            "parameters.formal-frameworks:write",
            "parameters.formal-framework-categories:write",
            "parameters.lifecycle-phase-types:write",
            "parameters.capacities:write",
            "parameters.capacities:delete",
            "parameters.function-types:write",
            "parameters.contact-types:write",
            "parameters.label-types:write",
            "parameters.purposes:write",
            "parameters.seat-types:write",
            "parameters.mandate-role-types:write",
            "parameters.location-types:write",
            "parameters.regulation-themes:write",
            "parameters.regulation-sub-themes:write",
            "people:write",
            "people.functions:read",
            "people.capacities:read",
            "reports:read",
            "system:read",
        });

        // There is no Parameters*Read permission: reading a master-data list is
        // open to any authenticated backoffice user (see GlobalPermissionTranslator),
        // so it carries no per-role signal and is never surfaced on /v1/me.
        result.Should().NotContain("parameters:read");
        result.Should().NotContain("parameters.organisation-classification-types:read");

        // Bare family/aggregate flags and "imports" are deliberately not
        // surfaced (see GlobalPermissionTranslator doc comment).
        result.Should().NotContain("bodies");
        result.Should().NotContain("parameters");
        result.Should().NotContain("people");
        result.Should().NotContain("delegations");
        result.Should().NotContain("reports");
        result.Should().NotContain("system");
        result.Should().NotContain("imports");
        result.Should().NotContain("system.statistics:read");
        result.Should().NotContain("system.events:read");
        result.Should().NotContain("system.kbo-terminated:read");
    }

    [Fact]
    public void Developer_has_the_expected_global_permissions()
    {
        var result = RolePermissions.Resolve(Role.Developer, PermissionsFor(Role.Developer)).ToList();

        result.Should().Contain(new[]
        {
            "organisations:create",
            "bodies:create",
            "delegations:read",
            "delegations:write",
            "delegations:delete",
            "reports:read",
            "system:read",
            "people:write",
            "people.functions:read",
            "people.capacities:read",
        });

        result.Should().NotContain("bodies");
        result.Should().NotContain("parameters");
        result.Should().NotContain("people");
        result.Should().NotContain("delegations");
        result.Should().NotContain("reports");
        result.Should().NotContain("system");
        result.Should().NotContain("imports");
        result.Should().NotContain("system.statistics:read");
    }

    [Fact]
    public void DecentraalBeheerder_has_the_expected_global_permissions()
    {
        var result = RolePermissions.Resolve(Role.DecentraalBeheerder, PermissionsFor(Role.DecentraalBeheerder)).ToList();

        result.Should().Contain("bodies:create");
        result.Should().Contain("reports:read");
        result.Should().Contain("people.functions:read");
        result.Should().Contain("people.capacities:read");

        result.Should().NotContain("organisations:create");
        result.Should().NotContain("bodies");
        result.Should().NotContain("reports");
        result.Should().NotContain("people");
        result.Should().NotContain("delegations");
        result.Should().NotContain("delegations:read");
        result.Should().NotContain("imports");
        result.Should().NotContain("parameters");
        result.Should().NotContain("parameters:read");
        result.Should().NotContain("people:write");
        result.Should().NotContain("system");
        result.Should().NotContain("system:read");
        result.Should().NotContain("system.statistics:read");
    }

    [Fact]
    public void VlimpersBeheerder_has_the_expected_global_permissions()
    {
        var result = RolePermissions.Resolve(Role.VlimpersBeheerder, PermissionsFor(Role.VlimpersBeheerder)).ToList();

        result.Should().Contain("reports:read");
        result.Should().Contain("people.functions:read");
        result.Should().Contain("people.capacities:read");

        result.Should().NotContain("organisations:create");
        result.Should().NotContain("bodies:create");
        result.Should().NotContain("bodies");
        result.Should().NotContain("delegations");
        result.Should().NotContain("delegations:read");
        result.Should().NotContain("parameters");
        result.Should().NotContain("people");
        result.Should().NotContain("reports");
        result.Should().NotContain("system");
        result.Should().NotContain("system:read");
        result.Should().NotContain("imports");
    }

    [Fact]
    public void OrgaanBeheerder_has_the_expected_global_permissions()
    {
        var result = RolePermissions.Resolve(Role.OrgaanBeheerder, PermissionsFor(Role.OrgaanBeheerder)).ToList();

        result.Should().Contain("bodies:create");
        result.Should().NotContain("bodies:read");
        result.Should().NotContain("bodies:write");
        result.Should().NotContain(p => p.StartsWith("bodies."));
        result.Should().Contain("reports:read");
        result.Should().Contain("people.functions:read");
        result.Should().Contain("people.capacities:read");

        result.Should().NotContain("bodies");
        result.Should().NotContain("people");
        result.Should().NotContain("reports");
        result.Should().NotContain("organisations:create");
        result.Should().NotContain("imports");
        result.Should().NotContain("delegations");
        result.Should().NotContain("parameters");
        result.Should().NotContain("system");
        result.Should().NotContain("system:read");
        result.Should().NotContain("people:write");
    }

    [Fact]
    public void RegelgevingBeheerder_has_the_expected_global_permissions()
    {
        var result = RolePermissions.Resolve(Role.RegelgevingBeheerder, PermissionsFor(Role.RegelgevingBeheerder)).ToList();

        result.Should().Contain("reports:read");
        result.Should().Contain("people.functions:read");
        result.Should().Contain("people.capacities:read");

        result.Should().NotContain("people");
        result.Should().NotContain("reports");
        result.Should().NotContain("organisations:create");
        result.Should().NotContain("bodies:create");
        result.Should().NotContain("bodies");
        result.Should().NotContain("imports");
        result.Should().NotContain("delegations");
        result.Should().NotContain("parameters");
        result.Should().NotContain("system");
        result.Should().NotContain("system:read");
    }
}
