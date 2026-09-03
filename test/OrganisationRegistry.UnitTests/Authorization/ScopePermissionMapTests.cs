namespace OrganisationRegistry.UnitTests.Authorization;

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;

[Collection("PermissionMapThrottleState")]
public class ScopePermissionMapTests
{
    public ScopePermissionMapTests() => ScopePermissionMap.ResetThrottleState();

    [Theory]
    // [InlineData(AcmIdmConstants.Scopes.CjmBeheerder, Permission.CanAddBodies)]
    // [InlineData(AcmIdmConstants.Scopes.CjmBeheerder, Permission.CanEditBodies)]
    [InlineData(AcmIdmConstants.Scopes.OrafinBeheerder, Permission.CanReadOrafin)]
    [InlineData(AcmIdmConstants.Scopes.Info, Permission.CanReadInfoEndpoints)]
    [InlineData(AcmIdmConstants.Scopes.TestClient, Permission.CanReadConfiguration)]
    [InlineData(AcmIdmConstants.Scopes.TestClient, Permission.CanEditOrganisationLabels)]
    public void Every_registered_scope_grants_expected_permission(string scope, Permission expected)
    {
        ScopePermissionMap.For(scope).Contains(expected).Should().BeTrue();
    }

    [Fact]
    public void TestClient_scope_matches_AlgemeenBeheerder_role_permission_set()
    {
        ((object)ScopePermissionMap.For(AcmIdmConstants.Scopes.TestClient))
            .Should().Be(RolePermissionMap.For(Role.AlgemeenBeheerder));
    }

    [Fact]
    public void Info_scope_only_grants_CanReadInfoEndpoints()
    {
        var set = ScopePermissionMap.For(AcmIdmConstants.Scopes.Info);
        set.Count.Should().Be(1);
        set.Contains(Permission.CanReadInfoEndpoints).Should().BeTrue();
    }

    [Fact]
    public void OrafinBeheerder_scope_only_grants_CanReadOrafin()
    {
        var set = ScopePermissionMap.For(AcmIdmConstants.Scopes.OrafinBeheerder);
        set.Count.Should().Be(1);
        set.Contains(Permission.CanReadOrafin).Should().BeTrue();
    }

    [Fact]
    public void For_null_or_empty_scope_returns_Empty()
    {
        ScopePermissionMap.For((string?)null).Should().BeSameAs(PermissionSet.Empty);
        ScopePermissionMap.For(string.Empty).Should().BeSameAs(PermissionSet.Empty);
    }

    [Fact]
    public void For_scopes_null_returns_Empty()
    {
        ScopePermissionMap.For((IEnumerable<string>?)null).Should().BeSameAs(PermissionSet.Empty);
    }

    [Fact]
    public void For_scopes_empty_returns_Empty()
    {
        ScopePermissionMap.For(Array.Empty<string>()).Should().BeSameAs(PermissionSet.Empty);
    }

    [Fact]
    public void For_scopes_unions_all_permissions()
    {
        var union = ScopePermissionMap.For(new[]
        {
            AcmIdmConstants.Scopes.CjmBeheerder,
            AcmIdmConstants.Scopes.OrafinBeheerder,
            AcmIdmConstants.Scopes.Info,
        });

        union.Contains(Permission.CanReadOrafin).Should().BeTrue();
        union.Contains(Permission.CanReadInfoEndpoints).Should().BeTrue();
    }

    [Fact]
    public void For_scopes_ignores_unrelated_prefix_silently()
    {
        var logger = new Mock<ILogger>();
        var union = ScopePermissionMap.For(
            new[] { "openid", "profile", "email", "some.other.api" },
            logger.Object);

        union.Should().BeSameAs(PermissionSet.Empty);
        logger.Verify(
            l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Fact]
    public void For_scopes_logs_once_for_unknown_orgreg_scope()
    {
        var logger = new Mock<ILogger>();
        var unknown = "dv_organisatieregister_bogus";

        ScopePermissionMap.For(new[] { unknown }, logger.Object);
        ScopePermissionMap.For(new[] { unknown }, logger.Object);

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
    public void For_scopes_recognises_valid_alongside_unknown()
    {
        var logger = new Mock<ILogger>();
        var union = ScopePermissionMap.For(
            new[]
            {
                AcmIdmConstants.Scopes.Info,
                "openid",
                "dv_organisatieregister_ghost",
            },
            logger.Object);

        union.Contains(Permission.CanReadInfoEndpoints).Should().BeTrue();
        union.Count.Should().Be(1);
    }

    [Fact]
    public void ResetThrottleState_allows_warning_to_fire_again()
    {
        var logger = new Mock<ILogger>();
        var unknown = "dv_organisatieregister_phantom";

        ScopePermissionMap.For(new[] { unknown }, logger.Object);
        ScopePermissionMap.ResetThrottleState();
        ScopePermissionMap.For(new[] { unknown }, logger.Object);

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
