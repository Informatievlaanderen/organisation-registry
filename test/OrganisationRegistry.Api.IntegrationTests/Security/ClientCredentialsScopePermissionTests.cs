namespace OrganisationRegistry.Api.IntegrationTests.Security;

using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Tests.Shared;
using Xunit;

/// <summary>
/// T015 — Baseline regression coverage for Client Credentials bearer tokens
/// hitting the <c>EditApi</c> authentication scheme.
/// <para>
/// Each M2M client (CJM, Orafin, TestClient) presents a Keycloak-issued token
/// directly to an <c>/edit/organisations/**</c> route. The <c>EditApi</c>
/// scheme (OAuth2 introspection) is the only authentication scheme reachable
/// by Client Credentials today; the token-exchange scheme is only used to
/// gate <c>BackofficeUser</c> which requires the <c>vo_id</c> claim.
/// </para>
/// <para>
/// The target route is <c>POST /edit/organisations/{id}/keys</c>: its
/// authorization policy (<c>PolicyNames.Keys</c>) accepts all three CC scopes
/// (<c>dv_organisatieregister_cjmbeheerder</c>,
/// <c>dv_organisatieregister_orafinbeheerder</c>,
/// <c>dv_organisatieregister_testclient</c>), which lets us assert the scope-
/// to-role translation with a single theory.
/// </para>
/// <para>
/// The assertions here freeze the current authorization outcome so US3's
/// unification of role and scope translation at
/// <see cref="OrganisationRegistry.Infrastructure.Authorization.ClaimsExtension.ToPermissionSet"/>
/// can be verified as non-regressing. We deliberately POST an empty body:
/// success is defined as authenticating and authorizing past the auth pipeline
/// (i.e. NOT 401/403). Model validation (400) or downstream errors are
/// acceptable — they prove auth was cleared.
/// </para>
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class ClientCredentialsScopePermissionTests
{
    private readonly ApiFixture _apiFixture;

    public ClientCredentialsScopePermissionTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [EnvVarIgnoreTheory]
    [InlineData(ApiFixture.CJM.Client, ApiFixture.CJM.Scope)]
    [InlineData(ApiFixture.Orafin.Client, ApiFixture.Orafin.Scope)]
    [InlineData(ApiFixture.Test.Client, ApiFixture.Test.Scope)]
    public async Task ClientCredentialsBearer_ScopeGrantsAccessToEditKeysEndpoint(
        string clientId,
        string scope)
    {
        var client = await _apiFixture.CreateMachine2MachineClientFor(clientId, scope);

        var route = $"/edit/organisations/{_apiFixture.ImportedParentOrganisationId}/keys";
        var response = await client.PostAsJsonAsync(route, new { });

        // The auth pipeline must NOT reject the token. Anything past 401/403 —
        // including 400 (validation), 201 (created), 404 (not found) or 500 —
        // proves the scope claim was accepted and translated into a set of
        // permissions that satisfy PolicyNames.Keys.
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }
}
