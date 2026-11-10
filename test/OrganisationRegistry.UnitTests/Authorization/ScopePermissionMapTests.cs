namespace OrganisationRegistry.UnitTests.Authorization;

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;
using OrganisationRegistry.Infrastructure.Authorization.Restrictions;
using OrganisationRegistry.Infrastructure.Configuration;

[Collection("PermissionMapThrottleState")]
public class ScopePermissionMapTests
{
    public ScopePermissionMapTests() => ScopePermissionMap.ResetThrottleState();
    private static readonly Guid OrafinKeyType = Guid.Parse("1e3611a7-7914-411a-a0c9-84fcd6218e67");
    private static readonly Guid OtherKeyType = Guid.Parse("95dfef4a-ba38-47d7-bd76-05151aec8141");

    private static IOrganisationRegistryConfiguration ConfigWithOrafinKeyTypes(params Guid[] ids)
    {
        var configuration = new Mock<IOrganisationRegistryConfiguration>();
        configuration.SetupGet(c => c.Authorization.KeyIdsAllowedOnlyForOrafin).Returns(ids);
        return configuration.Object;
    }

    [Theory]
    [InlineData(AcmIdmConstants.Scopes.OrafinBeheerder, Permission.CanReadOrafin)]
    [InlineData(AcmIdmConstants.Scopes.Info, Permission.CanReadInfoEndpoints)]
    [InlineData(AcmIdmConstants.Scopes.TestClient, Permission.CanReadConfiguration)]
    [InlineData(AcmIdmConstants.Scopes.TestClient, Permission.CanManageLabels)]
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
            AcmIdmConstants.Scopes.OrafinBeheerder,
            AcmIdmConstants.Scopes.Info,
            AcmIdmConstants.Scopes.TestClient,
        });

        union.Contains(Permission.CanReadOrafin).Should().BeTrue();
        union.Contains(Permission.CanReadInfoEndpoints).Should().BeTrue();
        union.Contains(Permission.CanReadConfiguration).Should().BeTrue();
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

    [Fact]
    public void Client_credentials_scope_string_with_retired_cjmbeheerder_still_maps()
    {
        const string tokenScope =
            "dv_organisatieregister_orafinbeheerder dv_organisatieregister_cjmbeheerder  " +
            "dv_organisatieregister_info dv_organisatieregister_testclient";

        // same split as SecurityService.GetRequiredUser
        var scopes = tokenScope.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var logger = new Mock<ILogger>();

        var union = ScopePermissionMap.For(scopes, logger.Object);

        union.Contains(Permission.CanManageKeys).Should().BeTrue();         // from testclient
        union.Contains(Permission.CanReadOrafin).Should().BeTrue();         // from orafinbeheerder
        union.Contains(Permission.CanReadInfoEndpoints).Should().BeTrue();  // from info
        union.Count.Should().Be(ScopePermissionMap.For(AcmIdmConstants.Scopes.TestClient).Count + 2);

        // only the retired scope warns, once
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
    public void OrafinBeheerder_with_configuration_may_manage_keys_of_the_orafin_key_types_only()
    {
        var set = ScopePermissionMap.For(
            [AcmIdmConstants.Scopes.OrafinBeheerder],
            ConfigWithOrafinKeyTypes(OrafinKeyType));

        set.Contains(Permission.CanManageKeys).Should().BeTrue("the controller gate only checks presence");
        set.IsSatisfiedFor(Permission.CanManageKeys, new KeyContext(false, OrafinKeyType)).Should().BeTrue();
        set.IsSatisfiedFor(Permission.CanManageKeys, new KeyContext(false, OtherKeyType)).Should().BeFalse();
        set.Contains(Permission.CanReadOrafin).Should().BeTrue("the original grant is kept");
    }

    [Fact]
    public void OrafinBeheerder_with_configuration_gets_no_key_rights_when_no_orafin_key_types_are_configured()
    {
        var set = ScopePermissionMap.For(
            [AcmIdmConstants.Scopes.OrafinBeheerder],
            ConfigWithOrafinKeyTypes());

        set.IsSatisfiedFor(Permission.CanManageKeys, new KeyContext(false, OrafinKeyType)).Should().BeFalse();
    }

    [Fact]
    public void Other_scopes_are_not_affected_by_the_configuration_overload()
    {
        var config = ConfigWithOrafinKeyTypes(OrafinKeyType);

        ((object)ScopePermissionMap.For([AcmIdmConstants.Scopes.Info], config))
            .Should().Be(ScopePermissionMap.For([AcmIdmConstants.Scopes.Info]));

        ((object)ScopePermissionMap.For([AcmIdmConstants.Scopes.TestClient], config))
            .Should().Be(ScopePermissionMap.For([AcmIdmConstants.Scopes.TestClient]));
    }

    [Fact]
    public void TestClient_plus_orafin_keeps_the_unrestricted_key_right()
    {
        var set = ScopePermissionMap.For(
            [AcmIdmConstants.Scopes.TestClient, AcmIdmConstants.Scopes.OrafinBeheerder],
            ConfigWithOrafinKeyTypes(OrafinKeyType));

        set.IsSatisfiedFor(Permission.CanManageKeys, new KeyContext(false, OtherKeyType)).Should().BeTrue();
    }

    [Fact]
    public void OrafinBeheerder_with_empty_configuration_gets_no_CanManageKeys_at_all()
        => ScopePermissionMap.For(
                [AcmIdmConstants.Scopes.OrafinBeheerder], ConfigWithOrafinKeyTypes())
            .Contains(Permission.CanManageKeys).Should().BeFalse();

    [Fact]
    public void OrafinBeheerder_with_null_configuration_does_not_throw_and_gets_no_CanManageKeys()
    {
        var set = ScopePermissionMap.For([AcmIdmConstants.Scopes.OrafinBeheerder], null);

        set.Contains(Permission.CanManageKeys).Should().BeFalse();
        set.Contains(Permission.CanReadOrafin).Should().BeTrue();
    }

}
