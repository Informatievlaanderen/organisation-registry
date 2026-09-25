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

        result.Should().Contain(new[]
        {
            "organisations:create",
            "bodies:create",
            "bodies",
            "delegations",
            "delegations:read",
            "delegations:write",
            "delegations:delete",
            "reports:read",
            "reports",
            "imports",
            "system",
            "system.statistics:read",
            "system.events:read",
            "system.kbo-terminated:read",
            "parameters",
            "parameters.organisation-classification-types:write",
            "parameters.information-systems:delete",
            "people",
            "people:write",
            "people.functions:read",
            "people.capacities:read",
        });

        // There is no Parameters*Read permission: reading a master-data list is
        // open to any authenticated backoffice user (see GlobalPermissionTranslator),
        // so it carries no per-role signal and is never surfaced on /v1/me.
        result.Should().NotContain("parameters:read");
        result.Should().NotContain("parameters.organisation-classification-types:read");
    }

    [Fact]
    public void Developer_has_the_expected_global_permissions()
    {
        var result = RolePermissions.Resolve(Role.Developer, PermissionsFor(Role.Developer)).ToList();

        result.Should().Contain(new[]
        {
            "organisations:create",
            "bodies:create",
            "bodies",
            "delegations",
            "delegations:read",
            "delegations:write",
            "delegations:delete",
            "reports:read",
            "reports",
            "imports",
            "system",
            "system.statistics:read",
            "system.events:read",
            "system.kbo-terminated:read",
            "parameters",
            "people",
            "people:write",
            "people.functions:read",
            "people.capacities:read",
        });
    }

    [Fact]
    public void DecentraalBeheerder_has_the_expected_global_permissions()
    {
        var result = RolePermissions.Resolve(Role.DecentraalBeheerder, PermissionsFor(Role.DecentraalBeheerder)).ToList();

        result.Should().Contain("organisations:create");
        result.Should().Contain("bodies:create");
        result.Should().Contain("bodies");
        result.Should().Contain("reports:read");
        result.Should().Contain("reports");
        result.Should().Contain("people");
        result.Should().Contain("people.functions:read");
        result.Should().Contain("people.capacities:read");

        result.Should().NotContain("delegations");
        result.Should().NotContain("delegations:read");
        result.Should().NotContain("imports");
        result.Should().NotContain("parameters");
        result.Should().NotContain("parameters:read");
        result.Should().NotContain("people:write");
        result.Should().NotContain("system");
        result.Should().NotContain("system.statistics:read");
    }

    [Fact]
    public void VlimpersBeheerder_has_the_expected_global_permissions()
    {
        var result = RolePermissions.Resolve(Role.VlimpersBeheerder, PermissionsFor(Role.VlimpersBeheerder)).ToList();

        result.Should().Contain("organisations:create");
        result.Should().Contain("reports:read");
        result.Should().Contain("reports");
        result.Should().Contain("imports");
        result.Should().Contain("people");

        result.Should().NotContain("bodies:create");
        result.Should().NotContain("bodies");
        result.Should().NotContain("delegations");
        result.Should().NotContain("delegations:read");
        result.Should().NotContain("parameters");
        result.Should().NotContain("system");
    }

    [Fact]
    public void OrgaanBeheerder_has_the_expected_global_permissions()
    {
        var result = RolePermissions.Resolve(Role.OrgaanBeheerder, PermissionsFor(Role.OrgaanBeheerder)).ToList();

        result.Should().Contain("bodies:create");
        result.Should().Contain("bodies");
        result.Should().NotContain("bodies:read");
        result.Should().NotContain("bodies:write");
        result.Should().NotContain(p => p.StartsWith("bodies."));
        result.Should().Contain("reports:read");
        result.Should().Contain("reports");
        result.Should().Contain("people");

        result.Should().NotContain("organisations:create");
        result.Should().NotContain("imports");
        result.Should().NotContain("delegations");
        result.Should().NotContain("parameters");
        result.Should().NotContain("system");
        result.Should().NotContain("people:write");
    }

    [Fact]
    public void RegelgevingBeheerder_has_the_expected_global_permissions()
    {
        var result = RolePermissions.Resolve(Role.RegelgevingBeheerder, PermissionsFor(Role.RegelgevingBeheerder)).ToList();

        result.Should().Contain("reports:read");
        result.Should().Contain("reports");
        result.Should().Contain("people");

        result.Should().NotContain("organisations:create");
        result.Should().NotContain("bodies:create");
        result.Should().NotContain("bodies");
        result.Should().NotContain("imports");
        result.Should().NotContain("delegations");
        result.Should().NotContain("parameters");
        result.Should().NotContain("system");
    }
}
