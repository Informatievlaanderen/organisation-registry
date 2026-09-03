namespace OrganisationRegistry.UnitTests.Authorization;

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Authorization.Restrictions;
using Tests.Shared;
using Tests.Shared.Stubs;
using Xunit;

[Collection("PermissionMapThrottleState")]
public class RolePermissionMapTests
{
    public RolePermissionMapTests() => RolePermissionMap.ResetThrottleState();

    [Theory]
    [InlineData(Role.AlgemeenBeheerder, Permission.CanReadConfiguration)]
    [InlineData(Role.AlgemeenBeheerder, Permission.CanEditOrganisationLabels)]
    [InlineData(Role.VlimpersBeheerder, Permission.CanEditVlimpers)]
    [InlineData(Role.VlimpersBeheerder, Permission.CanEditOrganisationLabels)]
    [InlineData(Role.DecentraalBeheerder, Permission.CanEditChildren)]
    [InlineData(Role.DecentraalBeheerder, Permission.CanEditOrganisationLabels)]
    [InlineData(Role.OrgaanBeheerder, Permission.CanRegisterBodies)]
    [InlineData(Role.RegelgevingBeheerder, Permission.CanManageRegulations)]
    [InlineData(Role.CjmBeheerder, Permission.CanEditBodies)]
    [InlineData(Role.CjmBeheerder, Permission.CanEditOrganisationLabels)]
    [InlineData(Role.Orafin, Permission.CanReadOrafin)]
    [InlineData(Role.Developer, Permission.CanReadConfiguration)]
    [InlineData(Role.Developer, Permission.CanEditOrganisationLabels)]
    [InlineData(Role.AutomatedTask, Permission.CanRunScheduledJobs)]
    public void Every_role_maps_to_a_non_empty_permission_set_containing_expected_permission(
        Role role, Permission expected)
    {
        var set = RolePermissionMap.For(role);
        set.Count.Should().BeGreaterThan(0);
        set.Contains(expected).Should().BeTrue();
    }

    [Theory]
    [InlineData(Role.OrgaanBeheerder)]
    [InlineData(Role.RegelgevingBeheerder)]
    [InlineData(Role.Orafin)]
    [InlineData(Role.AutomatedTask)]
    public void Roles_without_org_label_editing_do_not_grant_CanEditOrganisationLabels(Role role)
    {
        RolePermissionMap.For(role).Contains(Permission.CanEditOrganisationLabels).Should().BeFalse();
    }

    [Fact]
    public void Developer_is_AlgemeenBeheerder_superset_by_CanRunScheduledJobs()
    {
        // Developer intentionally has all AlgemeenBeheerder permissions PLUS CanRunScheduledJobs
        // (preserves current Developer access to /backoffice/tasks after T026a conversion).
        var ab = RolePermissionMap.For(Role.AlgemeenBeheerder);
        var dev = RolePermissionMap.For(Role.Developer);

        ((object)dev).Should().Be(ab.Union(PermissionSet.Of(Permission.CanRunScheduledJobs)));
    }

    [Fact]
    public void AlgemeenBeheerder_does_not_grant_orafin_or_info_or_scheduled()
    {
        var set = RolePermissionMap.For(Role.AlgemeenBeheerder);
        set.Contains(Permission.CanReadOrafin).Should().BeFalse();
        set.Contains(Permission.CanReadInfoEndpoints).Should().BeFalse();
        set.Contains(Permission.CanRunScheduledJobs).Should().BeFalse();
    }

    [Theory]
    [InlineData(Role.VlimpersBeheerder)]
    [InlineData(Role.DecentraalBeheerder)]
    [InlineData(Role.OrgaanBeheerder)]
    [InlineData(Role.RegelgevingBeheerder)]
    [InlineData(Role.CjmBeheerder)]
    [InlineData(Role.Orafin)]
    [InlineData(Role.AutomatedTask)]
    public void Non_admin_roles_do_not_grant_CanReadConfiguration(Role role)
    {
        RolePermissionMap.For(role).Contains(Permission.CanReadConfiguration).Should().BeFalse();
    }

    [Fact]
    public void For_roles_unions_all_permissions()
    {
        var union = RolePermissionMap.For(new[] { Role.Orafin, Role.CjmBeheerder });

        union.Contains(Permission.CanReadOrafin).Should().BeTrue();
        union.Contains(Permission.CanAddBodies).Should().BeTrue();
        union.Contains(Permission.CanEditBodies).Should().BeTrue();
    }

    [Fact]
    public void For_roles_null_returns_Empty()
    {
        RolePermissionMap.For((IEnumerable<Role>?)null).Should().BeSameAs(PermissionSet.Empty);
    }

    [Fact]
    public void For_roles_empty_returns_Empty()
    {
        RolePermissionMap.For(Array.Empty<Role>()).Should().BeSameAs(PermissionSet.Empty);
    }

    [Fact]
    public void Unknown_role_fails_closed_and_logs_once()
    {
        var unknown = (Role)9999;
        var logger = new Mock<ILogger>();

        var first = RolePermissionMap.For(unknown, logger.Object);
        var second = RolePermissionMap.For(unknown, logger.Object);

        first.Should().BeSameAs(PermissionSet.Empty);
        second.Should().BeSameAs(PermissionSet.Empty);

        logger.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Unknown_role_without_logger_still_returns_Empty()
    {
        RolePermissionMap.For((Role)8888).Should().BeSameAs(PermissionSet.Empty);
    }

    [Fact]
    public void ResetThrottleState_allows_warning_to_fire_again()
    {
        var unknown = (Role)7777;
        var logger = new Mock<ILogger>();

        RolePermissionMap.For(unknown, logger.Object);
        RolePermissionMap.ResetThrottleState();
        RolePermissionMap.For(unknown, logger.Object);

        logger.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(2));
    }

    [Fact]
    public void Static_For_VlimpersBeheerder_does_not_grant_CanManageKeys()
    {
        // Regression guard: the static map must not grant unrestricted CanManageKeys
        // to VlimpersBeheerder — that grant is only added by the config-aware overload
        // (data-driven, restricted to Vlimpers-allowed keytypes on Vlimpers-managed orgs).
        RolePermissionMap.For(Role.VlimpersBeheerder)
            .Contains(Permission.CanManageKeys).Should().BeFalse();
    }

    [Fact]
    public void For_config_null_roles_returns_Empty()
    {
        var config = new OrganisationRegistryConfigurationStub();

        RolePermissionMap.For((IEnumerable<Role>?)null, config)
            .Should().BeSameAs(PermissionSet.Empty);
    }

    [Fact]
    public void For_config_VlimpersBeheerder_grants_restricted_CanManageKeys()
    {
        var allowedKeyType = Guid.NewGuid();
        var otherKeyType = Guid.NewGuid();
        var config = new OrganisationRegistryConfigurationStub();
        ((AuthorizationConfigurationStub)config.Authorization).KeyIdsAllowedForVlimpers
            = new[] { allowedKeyType };

        var set = RolePermissionMap.For(new[] { Role.VlimpersBeheerder }, config);

        set.IsSatisfiedFor(
                Permission.CanManageKeys,
                new KeyContext(isUnderVlimpersManagement: true, allowedKeyType))
            .Should().BeTrue();

        set.IsSatisfiedFor(
                Permission.CanManageKeys,
                new KeyContext(isUnderVlimpersManagement: true, otherKeyType))
            .Should().BeFalse();

        set.IsSatisfiedFor(
                Permission.CanManageKeys,
                new KeyContext(isUnderVlimpersManagement: false, allowedKeyType))
            .Should().BeFalse();
    }

    [Fact]
    public void For_config_unions_across_roles_and_unrestricted_absorbs_restricted()
    {
        var allowedKeyType = Guid.NewGuid();
        var otherKeyType = Guid.NewGuid();
        var config = new OrganisationRegistryConfigurationStub();
        ((AuthorizationConfigurationStub)config.Authorization).KeyIdsAllowedForVlimpers
            = new[] { allowedKeyType };

        var set = RolePermissionMap.For(
            new[] { Role.VlimpersBeheerder, Role.AlgemeenBeheerder },
            config);

        // AlgemeenBeheerder holds unrestricted CanManageKeys; must absorb the
        // VlimpersBeheerder restricted grant regardless of context.
        set.IsSatisfiedFor(
                Permission.CanManageKeys,
                new KeyContext(isUnderVlimpersManagement: false, otherKeyType))
            .Should().BeTrue();
    }

    [Fact]
    public void For_config_non_vlimpers_role_matches_static_For()
    {
        var config = new OrganisationRegistryConfigurationStub();

        var staticSet = RolePermissionMap.For(Role.CjmBeheerder);
        var configSet = RolePermissionMap.For(new[] { Role.CjmBeheerder }, config);

        ((object)configSet).Should().Be(staticSet);
    }

    [Fact]
    public void For_config_VlimpersBeheerder_grants_restricted_CanManageFormalFrameworks()
    {
        var vlimpersFormalFrameworkId = Guid.NewGuid();
        var otherFormalFrameworkId = Guid.NewGuid();
        var config = new OrganisationRegistryConfigurationStub();
        ((AuthorizationConfigurationStub)config.Authorization).FormalFrameworkIdsOwnedByVlimpers
            = new[] { vlimpersFormalFrameworkId };

        var set = RolePermissionMap.For(new[] { Role.VlimpersBeheerder }, config);

        set.IsSatisfiedFor(
                Permission.CanManageFormalFrameworks,
                new FormalFrameworkContext(vlimpersFormalFrameworkId))
            .Should().BeTrue();

        set.IsSatisfiedFor(
                Permission.CanManageFormalFrameworks,
                new FormalFrameworkContext(otherFormalFrameworkId))
            .Should().BeFalse();
    }

    [Fact]
    public void For_config_DecentraalBeheerder_grants_restricted_CanManageFormalFrameworks()
    {
        var ovoNumber = "OVO123456";
        var vlimpersFormalFrameworkId = Guid.NewGuid();
        var otherFormalFrameworkId = Guid.NewGuid();
        var config = new OrganisationRegistryConfigurationStub();
        ((AuthorizationConfigurationStub)config.Authorization).FormalFrameworkIdsOwnedByVlimpers
            = new[] { vlimpersFormalFrameworkId };

        var user = new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisations(ovoNumber)
            .Build();

        var set = RolePermissionMap.For(new[] { Role.DecentraalBeheerder }, config);

        set.IsSatisfiedFor(
                Permission.CanManageFormalFrameworks,
                new UserContext(user),
                new OrganisationContext(ovoNumber),
                new FormalFrameworkContext(otherFormalFrameworkId))
            .Should().BeTrue();

        set.IsSatisfiedFor(
                Permission.CanManageFormalFrameworks,
                new UserContext(user),
                new OrganisationContext(ovoNumber),
                new FormalFrameworkContext(vlimpersFormalFrameworkId))
            .Should().BeFalse();
    }

    [Fact]
    public void For_config_RegelgevingBeheerder_grants_restricted_CanManageFormalFrameworks()
    {
        var regelgevingDbFormalFrameworkId = Guid.NewGuid();
        var otherFormalFrameworkId = Guid.NewGuid();
        var config = new OrganisationRegistryConfigurationStub();
        ((AuthorizationConfigurationStub)config.Authorization).FormalFrameworkIdsOwnedByRegelgevingDbBeheerder
            = new[] { regelgevingDbFormalFrameworkId };

        var set = RolePermissionMap.For(new[] { Role.RegelgevingBeheerder }, config);

        set.IsSatisfiedFor(
                Permission.CanManageFormalFrameworks,
                new FormalFrameworkContext(regelgevingDbFormalFrameworkId))
            .Should().BeTrue();

        set.IsSatisfiedFor(
                Permission.CanManageFormalFrameworks,
                new FormalFrameworkContext(otherFormalFrameworkId))
            .Should().BeFalse();
    }
}
