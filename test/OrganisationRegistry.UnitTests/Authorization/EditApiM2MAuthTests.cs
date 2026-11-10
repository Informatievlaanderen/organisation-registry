// test/OrganisationRegistry.UnitTests/Authorization/EditApiM2MAuthTests.cs
//
// Expected behaviour of the Edit API authentication/authorization, as hard asserts.
// A red test = production does not behave like this yet.
//   1. an M2M token is introspected once
//   2. an orafinbeheerder token may manage keys of the Orafin key type, and only those
//   3. Token_type_matrix: every token type -> authentication, policies, controller gate, KeyPolicy per key type
//
// The default selector is the real TokenExchangeSchemeSelector.SelectDefaultScheme; the two introspection schemes are a COPY of Startup.cs
// and of the policies at Startup.cs:369-400. If Startup changes, update the copies below.
namespace OrganisationRegistry.UnitTests.Authorization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrganisationRegistry.Api.Infrastructure;
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
using AuthenticationSchemes = OrganisationRegistry.Api.Infrastructure.Security.AuthenticationSchemes;

public class EditApiM2MAuthTests
{
    private const string BearerOrIntrospection = "BearerOrIntrospection";
    private const string EditPath = "/v1/edit/organisations/917f9854-a6c7-e304-d9b5-ddc7e5631737/keys/a864590c-05cf-4693-98b0-5bb4f0837cf7";

    private const string Testclient = AcmIdmConstants.Scopes.TestClient;
    private const string Orafin = AcmIdmConstants.Scopes.OrafinBeheerder;
    private const string Info = AcmIdmConstants.Scopes.Info;
    private const string RetiredCjm = "dv_organisatieregister_cjmbeheerder";   // no longer in the code

    // KeyIdsAllowedOnlyForOrafin (appsettings.*.json) and any other key type (POST example in or-auth.http)
    private static readonly Guid OrafinKeyType = Guid.Parse("1e3611a7-7914-411a-a0c9-84fcd6218e67");
    private static readonly Guid OtherKeyType = Guid.Parse("95dfef4a-ba38-47d7-bd76-05151aec8141");

    private readonly ITestOutputHelper _out;

    public EditApiM2MAuthTests(ITestOutputHelper output) => _out = output;

    // ---------------------------------------------------------------- 1. introspection

    [Fact]
    public async Task M2M_call_is_introspected_once()
    {
        var stub = new AcmStub(editApiScope: Orafin);
        await using var sp = BuildAuth(stub);
        var http = NewRequest(sp, EditPath);

        var viaMiddleware = await http.AuthenticateAsync();                                 // app.UseAuthentication()
        var viaEndpoint = await http.AuthenticateAsync(AuthenticationSchemes.EditApi);      // [OrganisationRegistryAuthorize(AuthenticationSchemes = EditApi)]

        _out.WriteLine("ACM introspection calls, in order: " + string.Join(" -> ", stub.CallsByClient));
        viaMiddleware.Succeeded.Should().BeTrue();
        viaEndpoint.Succeeded.Should().BeTrue();
        stub.CallsByClient.Should().Equal(AcmStub.EditApiClient);
    }

    // ---------------------------------------------------------------- 2. orafin and keys

    [Fact]
    public async Task Orafin_token_may_manage_keys_of_the_orafin_key_type_only()
    {
        var stub = new AcmStub(editApiScope: Orafin + " " + Info);
        await using var sp = BuildAuth(stub);
        var http = NewRequest(sp, EditPath);

        var principal = (await http.AuthenticateAsync(AuthenticationSchemes.EditApi)).Principal!;
        http.User = principal;
        _out.WriteLine("scope claims: " + string.Join(" | ", principal.FindAll("scope").Select(c => c.Value)));

        // The library splits "a b" into one claim per scope, so RequireClaim sees each scope separately
        principal.FindAll("scope").Select(c => c.Value).Should().BeEquivalentTo(Orafin, Info);

        // Step 1: the built-in policy filter (RequireClaim)
        var policy = await sp.GetRequiredService<IAuthorizationService>()
            .AuthorizeAsync(principal, null, PolicyNames.Keys);
        policy.Succeeded.Should().BeTrue("orafinbeheerder is accepted by the KEYS policy");

        // Step 2: the controller gate
        (await RunKeysAttribute(http)).Should().BeNull("orafin may manage keys");

        // Step 3: the command handler's KeyPolicy per key type
        var user = await sp.GetRequiredService<ISecurityService>().GetRequiredUser(principal);
        KeyPolicyAllows(user, OrafinKeyType).Should().BeTrue();
        KeyPolicyAllows(user, OtherKeyType).Should().BeFalse();
    }

    [Fact]
    public async Task Controller_gate_forbids_with_the_EditApi_scheme_when_CanManageKeys_is_missing()
    {
        // An identity without CanManageKeys must be forbidden with the EditApi scheme (not an empty scheme list).
        var security = new Mock<ISecurityService>();
        security.Setup(s => s.GetRequiredUser(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(WellknownUsers.Orafin(PermissionSet.Of(Permission.CanReadOrafin)));
        var http = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection().AddSingleton(security.Object).BuildServiceProvider(),
        };

        (await RunKeysAttribute(http)).Should().BeOfType<ForbidResult>()
            .Which.AuthenticationSchemes.Should().Equal(AuthenticationSchemes.EditApi);
    }

    [Fact]
    public async Task Testclient_token_may_manage_keys()
    {
        var stub = new AcmStub(editApiScope: Testclient);
        await using var sp = BuildAuth(stub);
        var http = NewRequest(sp, EditPath);
        http.User = (await http.AuthenticateAsync(AuthenticationSchemes.EditApi)).Principal!;

        (await RunKeysAttribute(http)).Should().BeNull();
    }

    // ---------------------------------------------------------------- 3. every token type

    // status:  401 token not accepted | 403-policy RequireClaim(scope) of KEYS fails | 403-gate CanManageKeys missing | 200 reaches the controller
    // orafin/other: what KeyPolicy (command handler) decides for the Orafin key type / any other key type; "-" when the request never gets there
    private static readonly Case[] Cases =
    [
        new("M2M  testclient",                          "t1",    AcmStub.Active(Testclient),                          "200", "allow", "allow"),
        new("M2M  orafinbeheerder",                     "t2",    AcmStub.Active(Orafin),                              "200", "allow", "deny"),
        new("M2M  orafinbeheerder + info (stg guess)",  "t3",    AcmStub.Active(Orafin + " " + Info),                 "200", "allow", "deny"),
        new("M2M  info",                                "t4",    AcmStub.Active(Info),                                "403-policy", "-", "-"),
        new("M2M  cjmbeheerder only (retired)",         "t5",    AcmStub.Active(RetiredCjm),                          "403-policy", "-", "-"),
        new("M2M  orafinbeheerder + cjmbeheerder",      "t6",    AcmStub.Active(Orafin + " " + RetiredCjm),           "200", "allow", "deny"),
        new("M2M  testclient + cjmbeheerder",           "t7",    AcmStub.Active(Testclient + " " + RetiredCjm),       "200", "allow", "allow"),
        new("M2M  testclient + orafin + info",          "t8",    AcmStub.Active(Testclient + " " + Orafin + " " + Info), "200", "allow", "allow"),
        new("M2M  all four scopes",                     "t9",    AcmStub.Active(Orafin + " " + RetiredCjm + " " + Info + " " + Testclient), "200", "allow", "allow"),
        new("M2M  unrelated scopes (openid profile)",   "t10",   AcmStub.Active("openid profile"),                    "403-policy", "-", "-"),
        new("M2M  active token without any scope",      "t11",   AcmStub.Active(null),                                "403-policy", "-", "-"),
        new("ACM user token (vo_id, names, no scope)",  "t12",   AcmStub.Active("vo", ",\"vo_id\":\"a5720746\",\"given_name\":\"Jane\",\"family_name\":\"Doe\""), "403-policy", "-", "-"),
        new("Expired / inactive opaque token",          "t13",   null,                                                "401", "-", "-"),
        new("Legacy own JWT (3 segments)",              "a.b.c", null,                                                "401", "-", "-"),
    ];

    [Fact]
    public async Task Introspection_is_cached_across_requests()
    {
        var stub = new AcmStub(editApiScope: Orafin);
        await using var sp = BuildAuth(stub);
        for (var i = 0; i < 3; i++)
        {
            using var scope = sp.CreateScope();
            await NewRequest(scope.ServiceProvider, EditPath).AuthenticateAsync(AuthenticationSchemes.EditApi);
        }
        stub.CallsByClient.Should().Equal(AcmStub.EditApiClient);
    }

    [Fact]
    public async Task Legacy_jwt_is_never_sent_to_acm()
    {
        var stub = new AcmStub(editApiScope: Orafin);
        await using var sp = BuildAuth(stub);
        var http = NewRequest(sp, "/v1/organisations/x", "a.b.c");
        await http.AuthenticateAsync(AuthenticationSchemes.TokenExchange);
        await http.AuthenticateAsync(AuthenticationSchemes.EditApi);
        stub.CallsByClient.Should().BeEmpty();
    }

    [Fact]
    public async Task A_token_cached_by_EditApi_is_not_accepted_by_TokenExchange()
    {
        var stub = new AcmStub(editApiScope: Orafin);   // active for the EditApi client only
        await using var sp = BuildAuth(stub);
        (await NewRequest(sp, EditPath).AuthenticateAsync(AuthenticationSchemes.EditApi)).Succeeded.Should().BeTrue();
        using var scope = sp.CreateScope();
        (await NewRequest(scope.ServiceProvider, EditPath).AuthenticateAsync(AuthenticationSchemes.TokenExchange))
            .Succeeded.Should().BeFalse();
        stub.CallsByClient.Should().Equal(AcmStub.EditApiClient, AcmStub.TokenExchangeClient);
    }

    [Fact]
    public async Task Token_type_matrix()
    {
        var rows = new List<string>
        {
            $"{"token type",-42} {"auth",-5} {"KEYS",-5} {"gate",-10} {"=> status",-11} {"orafin key",-11} {"other key",-10} expected",
            new string('-', 130),
        };

        using var scope = new AssertionScope();
        foreach (var c in Cases)
        {
            var json = c.EditApiJson is null
                ? new Dictionary<string, string>()
                : new Dictionary<string, string> { [c.Token] = c.EditApiJson };

            await using var sp = BuildAuth(new AcmStub(json));
            var http = NewRequest(sp, EditPath, c.Token);

            var auth = await http.AuthenticateAsync(AuthenticationSchemes.EditApi);
            string keys = "-", gate = "-", orafinKey = "-", otherKey = "-", status;

            if (!auth.Succeeded)
            {
                status = "401";
            }
            else
            {
                http.User = auth.Principal!;
                var keysPolicy = (await sp.GetRequiredService<IAuthorizationService>()
                    .AuthorizeAsync(auth.Principal!, null, PolicyNames.Keys)).Succeeded;
                keys = keysPolicy ? "pass" : "FAIL";

                if (!keysPolicy)
                {
                    status = "403-policy";   // in the real pipeline the policy filter short-circuits here
                }
                else
                {
                    var gateAllows = (await RunKeysAttribute(http)) is null;
                    gate = gateAllows ? "allow" : "forbid";
                    status = gateAllows ? "200" : "403-gate";

                    if (gateAllows)
                    {
                        var user = await sp.GetRequiredService<ISecurityService>().GetRequiredUser(auth.Principal!);
                        orafinKey = KeyPolicyAllows(user, OrafinKeyType) ? "allow" : "deny";
                        otherKey = KeyPolicyAllows(user, OtherKeyType) ? "allow" : "deny";
                    }
                }
            }

            var ok = status == c.Expected && orafinKey == c.ExpectedOrafinKey && otherKey == c.ExpectedOtherKey;
            rows.Add($"{c.Description,-42} {(auth.Succeeded ? "ok" : "FAIL"),-5} {keys,-5} {gate,-10} {status,-11} {orafinKey,-11} {otherKey,-10} {c.Expected}/{c.ExpectedOrafinKey}/{c.ExpectedOtherKey}"
                     + (ok ? "" : "   <-- MISMATCH"));

            status.Should().Be(c.Expected, $"'{c.Description}' status");
            orafinKey.Should().Be(c.ExpectedOrafinKey, $"'{c.Description}' Orafin key type");
            otherKey.Should().Be(c.ExpectedOtherKey, $"'{c.Description}' other key type");
        }

        rows.ForEach(_out.WriteLine);
    }

    private sealed record Case(
        string Description, string Token, string? EditApiJson,
        string Expected, string ExpectedOrafinKey, string ExpectedOtherKey);

    // ---------------------------------------------------------------- helpers

    private static bool KeyPolicyAllows(IUser user, Guid keyType)
        => new KeyPolicy(isUnderVlimpersManagement: false, keyType).Check(user).IsSuccessful;

    private static async Task<IActionResult?> RunKeysAttribute(HttpContext http)
    {
        // Same settings as OrganisationKeyController.cs:27-30
        var attribute = new OrganisationRegistryAuthorizeAttribute
        {
            RequiredPermissions = [Permission.CanManageKeys],
            AuthenticationSchemes = AuthenticationSchemes.EditApi,
            Policy = PolicyNames.Keys,
        };
        var context = new AuthorizationFilterContext(
            new ActionContext(http, new RouteData(), new ActionDescriptor()),
            new List<IFilterMetadata>());

        await attribute.OnAuthorizationAsync(context);
        return context.Result;
    }

    private static DefaultHttpContext NewRequest(IServiceProvider sp, string path, string token = AcmStub.DefaultToken)
    {
        var http = new DefaultHttpContext { RequestServices = sp };
        http.Request.Path = path;
        http.Request.Headers.Authorization = "Bearer " + token;
        return http;
    }

    private static ServiceProvider BuildAuth(AcmStub stub)
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services
            .AddAuthentication(o =>
            {
                o.DefaultScheme = BearerOrIntrospection;
                o.DefaultAuthenticateScheme = BearerOrIntrospection;
            })
            .AddPolicyScheme(BearerOrIntrospection, "JWT or opaque token", o => o.ForwardDefaultSelector = ctx => TokenExchangeSchemeSelector.SelectDefaultScheme(ctx, tokenExchangeEnabled: true))
            .AddOAuth2Introspection(AuthenticationSchemes.EditApi, o =>
            {
                o.ClientId = AcmStub.EditApiClient;
                o.ClientSecret = "x";
                o.Authority = "https://acm.test";
                o.IntrospectionEndpoint = "https://acm.test/op/v1/introspect";
                IntrospectionCaching.Apply(o, "or:editapi:", TimeSpan.FromMinutes(2));
            })
            .AddOAuth2Introspection(AuthenticationSchemes.TokenExchange, o =>
            {
                o.ClientId = AcmStub.TokenExchangeClient;
                o.ClientSecret = "x";
                o.Authority = "https://acm.test";
                o.IntrospectionEndpoint = "https://acm.test/op/v1/introspect";
                IntrospectionCaching.Apply(o, "or:tokenexchange:", TimeSpan.FromMinutes(2));
            });
        services.AddDistributedMemoryCache();
        // The library builds its HttpClient via IHttpClientFactory under this name
        // (PostConfigureOAuth2IntrospectionOptions.cs), so the stub is plugged in here.
        services
            .AddHttpClient(OAuth2IntrospectionDefaults.BackChannelHttpClientName)
            .ConfigurePrimaryHttpMessageHandler(() => stub);

        // Copy of Startup.cs:369-400 (the policies relevant for the Edit API)
        services.AddAuthorization(o =>
        {
            o.AddPolicy(PolicyNames.Organisations, b => b.RequireClaim(AcmIdmConstants.Claims.Scope, Testclient));
            o.AddPolicy(PolicyNames.Keys, b => b.RequireClaim(AcmIdmConstants.Claims.Scope, Orafin, Testclient));
        });

        // GetRequiredUser returns before touching the DB for testclient/orafin scopes; it reads KeyIdsAllowedOnlyForOrafin.
        var configuration = new Mock<IOrganisationRegistryConfiguration>();
        configuration.SetupGet(c => c.Authorization.KeyIdsAllowedOnlyForOrafin).Returns([OrafinKeyType]);

        services.AddSingleton<ISecurityService>(new SecurityService(
            Mock.Of<IContextFactory>(),
            configuration.Object,
            Mock.Of<ICache<OrganisationSecurityInformation>>()));

        return services.BuildServiceProvider();
    }

    /// <summary>Stands in for authenticatie-ti.vlaanderen.be/op/v1/introspect and records who called.</summary>
    private sealed class AcmStub : HttpMessageHandler
    {
        public const string TokenExchangeClient = "token-exchange-client";
        public const string EditApiClient = "edit-api-client";
        public const string DefaultToken = "opaque-m2m-token";

        // token -> introspection JSON as answered to the EditApi client. Unknown tokens are inactive.
        private readonly Dictionary<string, string> _editApiJsonByToken;

        public List<string> CallsByClient { get; } = new();

        public AcmStub(string editApiScope)
            : this(new Dictionary<string, string> { [DefaultToken] = Active(editApiScope) })
        {
        }

        public AcmStub(Dictionary<string, string> editApiJsonByToken) => _editApiJsonByToken = editApiJsonByToken;

        public static string Active(string? scope, string extraJsonFields = "")
            => "{\"active\":true,\"client_id\":\"m2m-client\",\"sub\":\"m2m-client\""
               + ",\"exp\":" + DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()
               + (scope is null ? "" : ",\"scope\":\"" + scope + "\"")
               + extraJsonFields + "}";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var form = await request.Content!.ReadAsStringAsync(ct);
            var clientId = FormValue(form, "client_id") ?? throw new InvalidOperationException("client_id missing");
            var token = FormValue(form, "token") ?? string.Empty;
            CallsByClient.Add(clientId);

            // From the log: the M2M token is "not active" for the TokenExchange client,
            // but active for the EditApi client it was issued for.
            var json = clientId == EditApiClient && _editApiJsonByToken.TryGetValue(token, out var known)
                ? known
                : "{\"active\":false}";

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
            };
        }

        // The library's default ClientCredentialStyle is PostBody: client_id and token are form fields.
        private static string? FormValue(string formBody, string key)
            => formBody.Split('&')
                .Select(pair => pair.Split('=', 2))
                .Where(pair => pair[0] == key)
                .Select(pair => Uri.UnescapeDataString(pair[1]))
                .FirstOrDefault();
    }
}
