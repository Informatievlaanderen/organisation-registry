// test/OrganisationRegistry.UnitTests/Authorization/OrafinKeyRulesBeforeReworkTests.cs
//
// BUSINESS RULES for organisation keys on the Edit API, as they were BEFORE the permission rework
// (KeyPolicy at commit dd0c81ea^, 2026-09-22), executed against TODAY'S code.
//
//   A test that is RED here = a rule that no longer holds today (trait Status=regression-since-2026-09-22).
//   A test that is GREEN here = a rule that survived the rework.
//
// Source of the rules (old KeyPolicy.Check):
//   - AlgemeenBeheerder (= scope testclient) and CjmBeheerder: every key type
//   - Orafin (and CJM): only key types in KeyIdsAllowedOnlyForOrafin (1e3611a7-... on dev/stg/prd)
//   - everybody else: never the Orafin key type
//   - old integration test CanCreateAndUpdateAs: Orafin + CJM M2M clients -> POST 201 then PUT 200 on the Orafin key type
//
// Not covered (follow-up): CJM (role removed 2026-10-19, or-3114), Vlimpers and Decentraal beheerder
// (they need organisation context), key types listed in KeyIdsAllowedForVlimpers.
//
// Flow under test: token scope claim -> SecurityService.GetRequiredUser (real) -> KeyPolicy (real)
// plus the controller's CanManageKeys gate that was added on 2026-09-24 (40e4a42e), printed for information.
// Run:  dotnet test --filter "FullyQualifiedName~OrafinKeyRulesBeforeReworkTests"
// Hide the known regressions:  --filter "Status!=regression-since-2026-09-22"
namespace OrganisationRegistry.UnitTests.Authorization;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrganisationRegistry.Api.Infrastructure.Security;
using OrganisationRegistry.Api.Security;
using OrganisationRegistry.Handling.Authorization;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Authorization.Cache;
using OrganisationRegistry.Infrastructure.Configuration;
using OrganisationRegistry.SqlServer;
using Xunit;
using Xunit.Abstractions;

public class OrafinKeyRulesBeforeReworkTests
{
    private const string Regression = "regression-since-2026-09-22";

    // KeyIdsAllowedOnlyForOrafin in appsettings.staging/production/development.json
    private static readonly Guid OrafinKeyType = Guid.Parse("1e3611a7-7914-411a-a0c9-84fcd6218e67");
    // any other key type (the keyTypeId of the POST example in or-auth.http)
    private static readonly Guid OtherKeyType = Guid.Parse("95dfef4a-ba38-47d7-bd76-05151aec8141");

    private const string Orafin = AcmIdmConstants.Scopes.OrafinBeheerder;
    private const string Testclient = AcmIdmConstants.Scopes.TestClient;
    private const string Info = AcmIdmConstants.Scopes.Info;

    private readonly ITestOutputHelper _out;

    public OrafinKeyRulesBeforeReworkTests(ITestOutputHelper output) => _out = output;

    // ------------------------------------------------------------------ the rules, one per fact

    [Fact, Trait("Status", Regression)]
    public async Task R1_Orafin_may_add_and_update_keys_of_the_orafin_key_type()
        => (await KeyPolicyAllows(Orafin, OrafinKeyType)).Should().BeTrue(
            "old test CanCreateAndUpdateAs expected 201/200 for the Orafin client on this key type");

    [Fact, Trait("Status", Regression)]
    public async Task R2_Orafin_keeps_that_right_when_the_token_also_carries_info()
        => (await KeyPolicyAllows(Orafin + " " + Info, OrafinKeyType)).Should().BeTrue();

    [Fact]
    public async Task R3_Orafin_may_NOT_use_any_other_key_type()
        => (await KeyPolicyAllows(Orafin, OtherKeyType)).Should().BeFalse(
            "old KeyPolicy: Orafin only passed when the key type is in KeyIdsAllowedOnlyForOrafin");

    [Fact]
    public async Task R4_Admin_testclient_may_use_every_key_type()
    {
        using var _ = new AssertionScope();
        (await KeyPolicyAllows(Testclient, OrafinKeyType)).Should().BeTrue();
        (await KeyPolicyAllows(Testclient, OtherKeyType)).Should().BeTrue();
    }

    [Fact]
    public async Task R5_Admin_testclient_combined_with_orafin_still_may_use_every_key_type()
    {
        using var _ = new AssertionScope();
        (await KeyPolicyAllows(Testclient + " " + Orafin, OrafinKeyType)).Should().BeTrue();
        (await KeyPolicyAllows(Testclient + " " + Orafin, OtherKeyType)).Should().BeTrue();
    }

    [Fact]
    public async Task R6_A_client_with_only_the_info_scope_may_use_no_key_type()
    {
        using var _ = new AssertionScope();
        (await KeyPolicyAllows(Info, OrafinKeyType)).Should().BeFalse();
        (await KeyPolicyAllows(Info, OtherKeyType)).Should().BeFalse();
    }

    // ------------------------------------------------------------------ all rules in one table

    private sealed record Rule(string Description, string Scopes, Guid KeyType, bool AllowedBefore);

    private static readonly Rule[] Rules =
    [
        new("orafin            -> orafin key type", Orafin, OrafinKeyType, true),
        new("orafin + info     -> orafin key type", Orafin + " " + Info, OrafinKeyType, true),
        new("orafin            -> other key type", Orafin, OtherKeyType, false),
        new("testclient        -> orafin key type", Testclient, OrafinKeyType, true),
        new("testclient        -> other key type", Testclient, OtherKeyType, true),
        new("testclient+orafin -> orafin key type", Testclient + " " + Orafin, OrafinKeyType, true),
        new("testclient+orafin -> other key type", Testclient + " " + Orafin, OtherKeyType, true),
        new("info only         -> orafin key type", Info, OrafinKeyType, false),
        new("info only         -> other key type", Info, OtherKeyType, false),
    ];

    [Fact]
    public async Task Rule_table_before_rework_vs_today()
    {
        var rows = new List<string>
        {
            $"{"case",-40} {"before",-8} {"today",-8} {"ctrl gate",-10} verdict",
            new string('-', 90),
        };
        var regressions = 0;

        foreach (var r in Rules)
        {
            var user = await UserFor(r.Scopes);
            var today = KeyPolicyAllows(user, r.KeyType);
            var gate = await ControllerGateAllows(user);
            var verdict = today == r.AllowedBefore ? "same" : "CHANGED";
            if (today != r.AllowedBefore) regressions++;
            rows.Add($"{r.Description,-40} {(r.AllowedBefore ? "allow" : "deny"),-8} {(today ? "allow" : "deny"),-8} {(gate ? "allow" : "deny"),-10} {verdict}");
        }

        rows.Add($"{regressions} rule(s) behave differently today than before 2026-09-22");
        rows.ForEach(_out.WriteLine);
    }

    // ------------------------------------------------------------------ helpers

    // KeyIdsAllowedOnlyForOrafin is configuration; a fix that restores the Orafin grant reads it from here.
    private static SecurityService NewSecurityService()
    {
        var configuration = new Mock<IOrganisationRegistryConfiguration>();
        configuration.SetupGet(c => c.Authorization.KeyIdsAllowedOnlyForOrafin).Returns([OrafinKeyType]);

        return new SecurityService(
            Mock.Of<IContextFactory>(),
            configuration.Object,
            Mock.Of<ICache<OrganisationSecurityInformation>>());
    }

    // Real production path for an M2M token: scope claim -> GetRequiredUser
    // A token without testclient/orafin scope has no usable identity: GetRequiredUser throws
    // ("Could not determine current user"), the request never gets past authorization. Modelled as Nobody (no permissions).
    private static async Task<IUser> UserFor(string scopes)
    {
        try
        {
            return await NewSecurityService().GetRequiredUser(
                new ClaimsPrincipal(new ClaimsIdentity(
                    [new Claim(AcmIdmConstants.Claims.Scope, scopes)], "test")));
        }
        catch (Exception e) when (e.Message.StartsWith("Could not determine current user"))
        {
            return WellknownUsers.Nobody;   // any other exception is a real error and must surface
        }
    }

    private static bool KeyPolicyAllows(IUser user, Guid keyType)
        => new KeyPolicy(isUnderVlimpersManagement: false, keyType).Check(user).IsSuccessful;

    private static async Task<bool> KeyPolicyAllows(string scopes, Guid keyType)
        => KeyPolicyAllows(await UserFor(scopes), keyType);

    // The [OrganisationRegistryAuthorize(RequiredPermissions = [CanManageKeys])] gate on OrganisationKeyController
    // (added 2026-09-24). Informational: it did not exist before the rework.
    private static async Task<bool> ControllerGateAllows(IUser user)
    {
        var security = new Mock<ISecurityService>();
        security.Setup(s => s.GetRequiredUser(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        var services = new ServiceCollection().AddSingleton(security.Object).BuildServiceProvider();

        var http = new DefaultHttpContext { RequestServices = services };
        var context = new AuthorizationFilterContext(
            new ActionContext(http, new RouteData(), new ActionDescriptor()),
            new List<IFilterMetadata>());

        await new OrganisationRegistryAuthorizeAttribute { RequiredPermissions = [Permission.CanManageKeys] }
            .OnAuthorizationAsync(context);

        return context.Result is null;
    }
}
