namespace OrganisationRegistry.UnitTests.Authorization;

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
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
    [InlineData(Role.RegelgevingBeheerder, Permission.CanManageRegulations)]
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
        // TODO: check this
        // union.Contains(Permission.CanAddBodies).Should().BeTrue();
        // union.Contains(Permission.CanEditBodies).Should().BeTrue();
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
}
