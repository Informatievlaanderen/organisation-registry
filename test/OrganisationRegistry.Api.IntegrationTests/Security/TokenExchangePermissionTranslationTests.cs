namespace OrganisationRegistry.Api.IntegrationTests.Security;

using Xunit;

/// <summary>
/// T014 — Placeholder for token-exchange scheme regression coverage.
/// <para>
/// The <c>TokenExchange</c> authentication scheme is currently only wired into
/// the <c>BackofficeUser</c> policy, which additionally requires the ACM-IDM
/// <c>vo_id</c> claim (see <c>Startup.cs</c> BackofficeUser policy). Client
/// Credentials tokens never carry <c>vo_id</c>, so any endpoint gated by
/// <c>BackofficeUser</c> — including <c>/v1/security</c> — cannot be exercised
/// via the token-exchange scheme with an M2M client.
/// </para>
/// <para>
/// The Client Credentials entry point is instead exercised by
/// <see cref="ClientCredentialsScopePermissionTests"/> against the
/// <c>EditApi</c> scheme, which is the only CC-reachable authentication scheme
/// today.
/// </para>
/// <para>
/// This test is kept as an explicit skipped marker so that when the
/// permission-based cutover (US2/US3) changes the gating of <c>/v1/security</c>
/// — or the token-exchange scheme is used to protect additional endpoints —
/// there is a clear anchor to re-enable regression coverage for the
/// scope-to-permission translation performed via token exchange.
/// </para>
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class TokenExchangePermissionTranslationTests
{
    private readonly ApiFixture _apiFixture;

    public TokenExchangePermissionTranslationTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact(Skip =
        "T026: TokenExchange scheme currently guards only /v1/security, which requires the vo_id claim " +
        "that Client Credentials tokens never carry. Revisit once permission-based auth changes the " +
        "gating of /v1/security or the TokenExchange scheme is applied to additional endpoints.")]
    public void TokenExchange_ScopeToPermissionTranslation_Placeholder()
    {
        // Intentionally empty — see class summary for rationale.
    }
}
