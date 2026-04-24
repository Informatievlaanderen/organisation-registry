# Research: Authentication Rework — Multi-Method Support

**Branch**: `002-auth-rework` | **Date**: 2026-03-23

---

## 1. Composite / Policy Scheme for Multi-Scheme Default Authentication

**Decision**: Use `AddPolicyScheme` to register a composite forwarding scheme (`"CompositeBearer"`) as `DefaultAuthenticateScheme`. The `ForwardDefaultSelector` inspects the request and forwards to `Bearer` or `EditApi`; if it cannot determine the type in advance, it tries `Bearer` first and falls back to `EditApi` in `ConfigureClaimsPrincipalSelectorMiddleware`.

**Rationale**:  
ASP.NET Core's `[Authorize]` without an explicit `AuthenticationSchemes` only calls `DefaultAuthenticateScheme`. If `DefaultAuthenticateScheme` is `Bearer`, an ACM/IDM token will never reach the `EditApi` introspection handler. A `PolicyScheme` (added via `services.AddAuthentication(...).AddPolicyScheme(...)`) is the idiomatic .NET 6 mechanism to express "try scheme A, fall back to scheme B" without writing a custom `IAuthenticationHandler`.

The forwarding logic:
- Inspect the `Authorization: Bearer <token>` header value (not the token content — avoid JWT parsing overhead at the middleware level).
- All three token types arrive as `Authorization: Bearer <value>`. Token type cannot be determined by the header format alone.
- Instead, the `ForwardDefaultSelector` always forwards to `Bearer` first. JWT validation is a fast local HMAC check; if it fails, the `PolicyScheme` does NOT automatically retry — `ForwardDefaultSelector` must return a single target per request.
- Therefore: the `ForwardDefaultSelector` should forward to `Bearer` unconditionally. `ConfigureClaimsPrincipalSelectorMiddleware` (which already tries `Bearer` then `EditApi`) remains the mechanism that populates `ClaimsPrincipal` for requests authenticated via `EditApi`.
- For `[Authorize]` without `AuthenticationSchemes`, ASP.NET Core calls `DefaultAuthenticateScheme` (the `PolicyScheme`). The `PolicyScheme` forwards to `Bearer`. If `Bearer` returns a non-null, authenticated principal, authorization proceeds. If `Bearer` returns unauthenticated, `[Authorize]` issues a 401 challenge. **But we need EditApi tokens to also pass.**
- Correct approach: the `PolicyScheme`'s `ForwardDefaultSelector` inspects the `Authorization` header. If the JWT can be decoded (not verified) and looks like a self-minted JWT (check the `iss` claim or `alg: HS256`), forward to `Bearer`; otherwise forward to `EditApi`. This pre-classification avoids double network calls.
  - Simpler heuristic: `Base64Url.Decode` the JWT header; if `alg` is `HS256`, forward to `Bearer`; otherwise forward to `EditApi`. This is a decode-only operation (no signature check), which is fast and allocation-cheap.
  - If the header cannot be decoded (malformed token), default to `EditApi` (introspection will return `active: false` → 401).

**Alternatives considered**:  
- *Custom `IAuthenticationHandler` that tries both*: More code, harder to test, reimplements what `PolicyScheme` already does. Rejected.  
- *Keep `DefaultAuthenticateScheme = Bearer` and add `AuthenticationSchemes = "Bearer,EditApi"` to every controller*: Would require touching 135 controllers and perpetuates scheme-coupling. Rejected.  
- *Dedicated scheme for token exchange*: Spec explicitly states no new scheme registrations (NFR-003). Rejected.

---

## 2. Removing `AuthenticationSchemes` from `[OrganisationRegistryAuthorize]`

**Decision**: Delete the `AuthenticationSchemes = string.Join(", ", JwtBearerDefaults.AuthenticationScheme)` assignment from `OrganisationRegistryAuthorizeAttribute`. Do not set `AuthenticationSchemes` at all — let `[Authorize]` use `DefaultAuthenticateScheme` (the composite `PolicyScheme`).

**Rationale**: `[Authorize]` with `AuthenticationSchemes` set overrides the `DefaultAuthenticateScheme` and makes it impossible for any other scheme to satisfy the attribute. Removing the assignment means the composite `PolicyScheme` is used, which correctly forwards to whichever scheme matches the token.

**Alternatives considered**:  
- *Set `AuthenticationSchemes` to a comma-joined list of all schemes*: Supported by ASP.NET Core, but requires updating the attribute whenever a new scheme is added (NFR-004 violation). Rejected.

---

## 3. Removing `AuthenticationSchemes = AuthenticationSchemes.EditApi` from Edit API Controllers

**Decision**: Remove the `AuthenticationSchemes = AuthenticationSchemes.EditApi` from the five Edit API controller `[Authorize]` attributes. Leave `Policy = PolicyNames.X` intact. The scope policies (`BANKACCOUNTS`, `ORGANISATIONS`, etc.) already gate access on claim values — no scheme restriction is needed.

**Rationale**: The scope-checking policies use `RequireClaim(AcmIdmConstants.Claims.Scope, ...)`. This is purely a claims check. Any scheme that populates the `ClaimsPrincipal` with the correct scope claim satisfies the policy. Removing the scheme restriction allows the composite scheme to authenticate the request and then let the policy decide.

**Risk**: After removing `AuthenticationSchemes`, the composite `PolicyScheme` is used. For a client credentials token (ACM/IDM token), the `ForwardDefaultSelector` will forward to `EditApi` (because the JWT header `alg ≠ HS256`). Introspection runs, principal is populated with scope claims, policy passes. Behaviour is identical to before for M2M clients.

---

## 4. `GetAuthenticateInfoAsync` / `GetAuthenticateInfo` — Multi-Scheme Support

**Current behaviour**: `GetAuthenticateInfoAsync` only tries `Bearer`. Returns `null` if `Bearer` fails. Used in:
- `HttpContextAccessorExtensions.UserIsDecentraalBeheerder` — returns `true/false` based on principal.
- `ElasticSearchFacade.FilterOrganisationFields` — checks whether the request is authenticated at all.
- `OrganisationDetailCommandController`, `BodyDetailCommandController`, `PersonDetailController` — obtain principal for `GetRequiredUser`.

**Decision**: Change `GetAuthenticateInfoAsync` to try `Bearer` first (fast, local), then `EditApi` if `Bearer` did not succeed. Return the first successful result.

**Implementation**:
```csharp
public static async Task<AuthenticateResult?> GetAuthenticateInfoAsync(this HttpContext source)
{
    var bearerInfo = await source.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
    if (bearerInfo.Succeeded) return bearerInfo;

    var editApiInfo = await source.AuthenticateAsync(AuthenticationSchemes.EditApi);
    return editApiInfo.Succeeded ? editApiInfo : null;
}
```

The synchronous `GetAuthenticateInfo` variant in `ConfigureClaimsPrincipalSelectorMiddleware` already implements this pattern. The async version must be updated to match.

**Alternatives considered**:  
- *Only use the composite `PolicyScheme`*: `AuthenticateAsync(compositeScheme)` forwards to one of the two handlers and returns one result — already the right approach for `[Authorize]`. However, `GetAuthenticateInfoAsync` needs to return the actual identity principal, not just a success/failure flag, and the composite scheme's result already contains the principal from the forwarded handler. Could work, but is less explicit. Keeping try-Bearer-then-EditApi is clearer for the transition state.

---

## 5. `SecurityService.GetRequiredUser()` — Token Exchange Claims

**Current behaviour**: `GetRequiredUser` first checks `scope` claim:
- `dv_organisatieregister_cjmbeheerder` → returns `WellknownUsers.Cjm`
- `dv_organisatieregister_orafinbeheerder` → returns `WellknownUsers.Orafin`
- `dv_organisatieregister_testclient` → returns `WellknownUsers.TestClient`
- Otherwise → looks for `GivenName`, `Surname`, `vo_id` claims (human user path)

**Token exchange token claims profile**: A token exchange token from ACM/IDM is an ACM/IDM-issued token validated via introspection. It carries the original user's identity claims (`given_name`, `family_name`, `vo_id`) AND may carry scopes. Its claims profile is like a human user token but issued by ACM/IDM (not self-minted).

**Decision**: The existing `GetRequiredUser` logic already handles this correctly as long as the token exchange token does NOT carry one of the three M2M scope values. If it does carry user identity claims (`GivenName`, `Surname`, `vo_id`), it falls through the switch to the human-user path and returns a `User` object with the correct claims.

**Edge case — token carries both scope and user identity claims**: The switch on `scope` is checked first. If a token exchange token carries `dv_organisatieregister_cjmbeheerder` scope AND user identity claims, it will be treated as `WellknownUsers.Cjm`. This is the correct behaviour — scope wins over identity for M2M-scoped tokens regardless of token type.

**Required change**: No change to the logic. However, the `GetSecurityInformation` path (called from the human-user branch) calls `GetRequiredClaim(ClaimTypes.GivenName)` etc. ACM/IDM introspection returns claims using standard OIDC claim names (`given_name`, `family_name`), not `ClaimTypes.GivenName` (`http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname`). The `Bearer` (self-minted JWT) path maps these correctly via `OrganisationRegistryTokenBuilder.ParseRoles`. The introspection path does NOT do this mapping — it returns raw ACM/IDM claim names.

**Research finding**: Inspect `OrganisationRegistryTokenBuilder` to confirm claim mapping:
- `ClaimTypes.GivenName` = standard .NET claim URI  
- ACM/IDM returns `given_name` (OIDC standard) or `urn:be:vlaanderen:acm:voornaam` (custom)

If the `EditApi` path (introspection) returns `given_name` but the code expects `ClaimTypes.GivenName`, `GetRequiredUser` will throw for token exchange tokens. This needs verification and potentially a claim mapping step in `GetRequiredUser` for the EditApi path.

**Recommendation**: Add a fallback in `GetRequiredUser` so it tries `ClaimTypes.GivenName` first (for self-minted JWTs), then `AcmIdmConstants.Claims.Firstname` (the ACM/IDM custom claim). This makes `GetRequiredUser` token-type-agnostic without changing the flow for existing paths.

---

## 6. `ConfigureClaimsPrincipalSelectorMiddleware` — Current Behaviour Is Correct

**Current behaviour**: Already tries `Bearer` then `EditApi`. Returns the first successful principal and adds the IP claim. No changes needed here beyond confirming `GetAuthenticateInfo` (synchronous) matches the async version after the async fix.

---

## 7. Integration Test Strategy for Token Exchange

**Context**: The test harness uses a local Keycloak/WireMock identity server (see `ApiFixture`, `WiremockSetup`). Token exchange (RFC 8693) requires a dedicated grant type (`urn:ietf:params:oauth:grant-type:token-exchange`). The local Keycloak (`appsettings.json` points to a Keycloak instance) may or may not support RFC 8693 token exchange depending on the realm configuration.

**Decision**: For integration tests, simulate a token exchange token by obtaining a client credentials token from the local Keycloak using a dedicated test client (`tokenExchangeClient`) that has user identity claims populated in its introspection response. This avoids needing full RFC 8693 support in the test environment while still exercising the full `EditApi` introspection path with user identity claims.

Alternatively, extend `ApiFixture` with a `CreateTokenExchangeClient()` method that obtains a token via a grant type that returns user identity claims (e.g., a `password` grant or a pre-configured Keycloak test user). The token arrives as `Authorization: Bearer <acm-idm-token>` and is introspected via `EditApi` — the same path as a real token exchange token.

**What the test must cover (SC-006)**:
1. Token exchange token with claims satisfying a backoffice endpoint policy → 200.
2. Token exchange token with insufficient claims for a restricted endpoint → 403.
3. Invalid/revoked token exchange token → 401.

---

## Summary of NEEDS CLARIFICATION Items — All Resolved

| Item | Resolution |
|------|-----------|
| Composite scheme mechanism | `AddPolicyScheme` with `ForwardDefaultSelector` based on JWT `alg` header inspection |
| Can `EditApi` tokens reach backoffice endpoints? | Yes, once composite scheme is default and `OrganisationRegistryAuthorize` removes scheme hardcoding |
| `GetRequiredUser` for token exchange tokens | Correct path already exists; add claim name fallback for OIDC `given_name` vs `ClaimTypes.GivenName` |
| Integration test for token exchange | Use a Keycloak test client configured to return user identity claims in introspection response |
| `GetAuthenticateInfoAsync` — only tries Bearer | Must be extended to try Bearer then EditApi |
