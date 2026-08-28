namespace OrganisationRegistry.UnitTests.Authorization;

using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;

/// <summary>
/// T022 — verifies unknown role / scope translation is fail-closed and that
/// the throttled warning fires exactly once per distinct unknown key per
/// process (per-key isolation).
/// </summary>
[Collection("PermissionMapThrottleState")]
public class PermissionMapThrottleTests
{
    public PermissionMapThrottleTests()
    {
        RolePermissionMap.ResetThrottleState();
        ScopePermissionMap.ResetThrottleState();
    }

    // ---- Role side -------------------------------------------------------

    [Fact]
    public void Unknown_role_yields_empty_permission_set()
    {
        var unknown = (Role)9999;

        var set = RolePermissionMap.For(unknown);

        set.Count.Should().Be(0);
    }

    [Fact]
    public void Unknown_role_logs_exactly_once_across_many_lookups()
    {
        var unknown = (Role)9999;
        var logger = new Mock<ILogger>();

        for (var i = 0; i < 100; i++)
            RolePermissionMap.For(unknown, logger.Object);

        VerifyWarnCount(logger, 1);
    }

    [Fact]
    public void Two_distinct_unknown_roles_log_two_separate_warnings()
    {
        var unknownA = (Role)9001;
        var unknownB = (Role)9002;
        var logger = new Mock<ILogger>();

        RolePermissionMap.For(unknownA, logger.Object);
        RolePermissionMap.For(unknownA, logger.Object);
        RolePermissionMap.For(unknownB, logger.Object);
        RolePermissionMap.For(unknownB, logger.Object);

        VerifyWarnCount(logger, 2);
    }

    // ---- Scope side ------------------------------------------------------

    [Fact]
    public void Unknown_scope_yields_empty_permission_set()
    {
        var set = ScopePermissionMap.For(new[] { "dv_organisatieregister_ghost" });

        set.Count.Should().Be(0);
    }

    [Fact]
    public void Unknown_orgregister_scope_logs_exactly_once_across_many_lookups()
    {
        const string unknown = "dv_organisatieregister_ghost";
        var logger = new Mock<ILogger>();

        for (var i = 0; i < 100; i++)
            ScopePermissionMap.For(new[] { unknown }, logger.Object);

        VerifyWarnCount(logger, 1);
    }

    [Fact]
    public void Two_distinct_unknown_orgregister_scopes_log_two_separate_warnings()
    {
        const string unknownA = "dv_organisatieregister_ghost_a";
        const string unknownB = "dv_organisatieregister_ghost_b";
        var logger = new Mock<ILogger>();

        ScopePermissionMap.For(new[] { unknownA }, logger.Object);
        ScopePermissionMap.For(new[] { unknownA }, logger.Object);
        ScopePermissionMap.For(new[] { unknownB }, logger.Object);
        ScopePermissionMap.For(new[] { unknownB }, logger.Object);

        VerifyWarnCount(logger, 2);
    }

    [Fact]
    public void Unknown_scope_without_orgregister_prefix_does_not_log()
    {
        var logger = new Mock<ILogger>();

        ScopePermissionMap.For(new[] { "some_other_resource_server_scope" }, logger.Object);
        ScopePermissionMap.For(new[] { "openid", "profile", "email" }, logger.Object);

        VerifyWarnCount(logger, 0);
    }

    // ---- helpers ---------------------------------------------------------

    private static void VerifyWarnCount(Mock<ILogger> logger, int expected)
        => logger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Exactly(expected));
}
