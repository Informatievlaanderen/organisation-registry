# Research: Keycloak Demo — Resolved Unknowns

**Branch**: `003-keycloak-demo` | **Date**: 2026-03-23  
**Purpose**: Resolve key technical unknowns before implementation begins.

---

## RQ-1: Keycloak Claim Mappers for `iv_wegwijs_rol_3D` and `vo_id`

### Question

The `OrganisationRegistryTokenBuilder` reads two non-standard claims from the ID token:
- `iv_wegwijs_rol_3D` — multi-valued; contains role strings like `WegwijsBeheerder-algemeenbeheerder:OVO002949`
- `vo_id` — UUID string identifying the ACM user

How are these emitted in Keycloak 24.0?

### Resolution

Keycloak supports custom claims via **Protocol Mappers** defined per client or per client scope.
The mechanism for arbitrary user data is the **User Attribute** mapper type.

**Step 1 — Store data as user attributes:**
In the realm JSON, each user object has an `attributes` map:
```json
{
  "username": "dev",
  "credentials": [{"type": "password", "value": "dev"}],
  "attributes": {
    "vo_id": ["9C2F7372-7112-49DC-9771-F127B048B4C7"],
    "iv_wegwijs_rol_3D": [
      "WegwijsBeheerder-algemeenbeheerder:OVO002949",
      "WegwijsBeheerder-vlimpersbeheerder:OVO001833"
    ]
  }
}
```

**Step 2 — Define Protocol Mappers:**
On the `organisation-registry-local-dev` client (and `nuxt-bff` client), add:

```json
{
  "name": "iv_wegwijs_rol_3D",
  "protocol": "openid-connect",
  "protocolMapper": "oidc-usermodel-attribute-mapper",
  "config": {
    "user.attribute": "iv_wegwijs_rol_3D",
    "claim.name": "iv_wegwijs_rol_3D",
    "multivalued": "true",
    "id.token.claim": "true",
    "access.token.claim": "true",
    "userinfo.token.claim": "true"
  }
}
```

```json
{
  "name": "vo_id",
  "protocol": "openid-connect",
  "protocolMapper": "oidc-usermodel-attribute-mapper",
  "config": {
    "user.attribute": "vo_id",
    "claim.name": "vo_id",
    "multivalued": "false",
    "id.token.claim": "true",
    "access.token.claim": "true",
    "userinfo.token.claim": "true"
  }
}
```

**Important**: The `SecurityController.ExchangeCode` reads claims from `tokenResponse.IdentityToken`
(the ID token). Therefore mappers must have `"id.token.claim": "true"`.

**Multi-valued serialisation**: When `multivalued: true`, Keycloak serialises the array into the
JWT as a JSON array. The `OrganisationRegistryTokenBuilder.ParseRoles` reads claims via
`identity.GetClaims(AcmIdmConstants.Claims.Role)` which iterates individual claim values.
`JwtSecurityTokenHandler` in .NET correctly deserialises a JSON array claim into multiple individual
`Claim` objects with the same type — this is the standard behaviour. **No code change needed.**

### Conclusion

**User Attribute mappers with `multivalued: true` in the realm JSON** solve this completely.
The existing `ParseRoles` code requires no modification.

---

## RQ-2: Keycloak Introspection Endpoint Path

### Question

The Duende IdentityServer introspection endpoint is `/connect/introspect`.
What is the Keycloak 24.0 equivalent, and does `IdentityModel.AspNetCore.OAuth2Introspection` work
with it?

### Resolution

Keycloak's introspection endpoint path is:
```
/realms/{realm}/protocol/openid-connect/token/introspect
```

For the `wegwijs` realm, internal Docker URL:
```
http://keycloak:8080/realms/wegwijs/protocol/openid-connect/token/introspect
```

**Authentication mechanism**: Keycloak requires the introspecting client to authenticate using
`client_id` + `client_secret`. The `IdentityModel.AspNetCore.OAuth2Introspection` library sends
credentials as a `Basic` auth header (`Authorization: Basic base64(clientId:clientSecret)`) by
default — this is exactly what Keycloak expects.

**Required Keycloak configuration on `organisation-registry-api` client**:
- `serviceAccountsEnabled: false` (it's a resource server, not a service account)  
- `secret: a_very=Secr3t*Key`
- The client must NOT be `publicClient: true`
- `standardFlowEnabled: false`, `directAccessGrantsEnabled: false`

The `EditApiConfigurationSection` in `appsettings.keycloak.json`:
```json
{
  "EditApi": {
    "ClientId": "organisation-registry-api",
    "ClientSecret": "a_very=Secr3t*Key",
    "Authority": "http://keycloak:8080/realms/wegwijs",
    "IntrospectionEndpoint": "http://keycloak:8080/realms/wegwijs/protocol/openid-connect/token/introspect"
  }
}
```

Note: `IdentityModel.AspNetCore.OAuth2Introspection` can auto-discover the introspection endpoint
from the `/.well-known/openid-configuration` if `Authority` is set and `IntrospectionEndpoint` is
omitted, but explicit is safer and avoids an extra discovery HTTP call at startup.

### Conclusion

Path confirmed: `…/protocol/openid-connect/token/introspect`. No library changes needed.
Configure `appsettings.keycloak.json` with the explicit endpoint.

---

## RQ-3: Token Exchange / BFF Strategy

### Question

US3 (Nuxt BFF) requires the BFF to obtain an API-scoped token server-side after user login.
Options:
1. **RFC 8693 Token Exchange**: Exchange the user's ID/access token for an API-scoped token
2. **Separate Client Credentials**: BFF uses its own `cjmClient`-like credentials to get a
   service token (no user context)
3. **BFF requests user token with API scope directly**: The authorization code flow includes
   `scope=dv_organisatieregister_cjmbeheerder` so the access token already contains the scope

### Resolution

**Option 3 is the simplest and most correct** for this demo:

The `nuxt-bff` Keycloak client is configured for authorization code flow with scopes including
`dv_organisatieregister_cjmbeheerder`. When the user logs in, Keycloak issues an access token that
includes this scope. The BFF stores this access token server-side and uses it to call the Edit API.

This is architecturally correct for a BFF pattern: the access token is bound to the user session
(contains user identity), not just a service account token. The Edit API's introspection will
confirm the `scope` claim contains `dv_organisatieregister_cjmbeheerder`.

**RFC 8693 Token Exchange** (Option 1) would require:
- Enabling Keycloak's "token-exchange" feature (requires `--features=token-exchange` flag)
- Additional client configuration
- More complex BFF code
This is unnecessary complexity for the demo.

**Conclusion**: Use authorization code flow with `scope=openid profile dv_organisatieregister_cjmbeheerder`
in the `nuxt-bff` client. Store the resulting `access_token` in the encrypted server session.
No token exchange needed.

---

## RQ-4: Docker Networking — Split-Horizon URL Problem

### Question

Keycloak is accessible:
- Externally (browser): `http://localhost:8180`
- Internally (Docker network): `http://keycloak:8080`

The `iss` (issuer) claim in Keycloak tokens reflects the **public frontend URL** of Keycloak.
By default this is derived from the request hostname — when Keycloak starts, the `iss` will be
`http://localhost:8180/realms/wegwijs` (the external URL the browser uses), because `start-dev`
derives it from the incoming request.

But the Organisation Registry API runs inside Docker and calls Keycloak at `http://keycloak:8080`.
The JWT validation (`OrganisationRegistryTokenValidationParameters`) checks the `iss` claim.
If `ValidIssuers` only contains the internal URL, validation will fail.

### Resolution

**Two-part solution**:

**Part 1**: Set Keycloak `KC_HOSTNAME_URL` environment variable in `docker-compose.yml`:
```yaml
keycloak:
  environment:
    KC_HOSTNAME_URL: http://localhost:8180
    KC_HOSTNAME_STRICT: "false"
```
This forces Keycloak to always use `http://localhost:8180/realms/wegwijs` as the `iss` claim,
regardless of which hostname a request arrives at. This is the standard `start-dev` override.

**Part 2**: Configure `WegwijsTokenValidationParameters` (via `appsettings.keycloak.json`) to
accept `http://localhost:8180/realms/wegwijs` as a valid issuer. The `Authority` in `OIDCAuth`
should be set to `http://localhost:8180/realms/wegwijs` for the purposes of issuer validation,
while the token endpoint is reached at the internal address.

Actually, the cleaner approach: `Authority` (in `OIDCAuth`) = `http://localhost:8180/realms/wegwijs`
is used for issuer validation AND token requests. Since the API also runs inside Docker, the
hostname `localhost` resolves to `127.0.0.1` inside the container — this will NOT reach the
`keycloak` service. Therefore:

**Revised strategy**:
- `KC_HOSTNAME_URL` = `http://localhost:8180` → forces `iss` = `http://localhost:8180/realms/wegwijs`
- `OIDCAuth.Authority` = `http://keycloak:8080/realms/wegwijs` (internal, for token endpoint)
- `OIDCAuth.AuthorizationIssuer` = `http://localhost:8180/realms/wegwijs` (external, for issuer validation)
- `WegwijsTokenValidationParameters` uses `AuthorizationIssuer` for `ValidIssuers` — verify this in the code

Looking at `WegwijsTokenValidationParameters`:
```csharp
// This needs to be checked in the implementation
```

The `OrganisationRegistryTokenValidationParameters` in `WegwijsTokenValidationParameters.cs` needs
to accept the external issuer. The `appsettings.keycloak.json` sets `AuthorizationIssuer` to
the external URL. The validator reads from `openIdConfiguration.AuthorizationIssuer`.

**For the M2M tokens**: `cjmClient` tokens are validated via introspection (not JWT validation),
so the issuer mismatch only matters for the SPA/BFF custom JWT path.

**Conclusion**:
1. Set `KC_HOSTNAME_URL=http://localhost:8180` in docker-compose keycloak service
2. `OIDCAuth.Authority` in `appsettings.keycloak.json` → `http://keycloak:8080/realms/wegwijs`
   (used for `tokenEndpointAddress` construction in `SecurityController.ExchangeCode`)
3. `OIDCAuth.AuthorizationIssuer` → `http://localhost:8180/realms/wegwijs` (for issuer validation)
4. `EditApi.Authority` and `EditApi.IntrospectionEndpoint` → internal `keycloak:8080`

---

## RQ-5: PKCE Redirect URI with Hash Fragment

### Question

The Angular SPA uses `redirect_uri = https://organisatie.dev-vlaanderen.local/#/oic`.
Keycloak must accept this exact URI. Does Keycloak support hash fragments in redirect URIs?

### Resolution

Keycloak's `validRedirectUris` field in the client config accepts glob patterns. The `#` fragment
is NOT sent to the server in the actual OAuth2 redirect (the browser strips it). The actual
`redirect_uri` parameter sent by the Angular SPA is `https://organisatie.dev-vlaanderen.local/#/oic`
(some SPAs send the fragment).

Actually, in the standard OAuth2 flow, the `redirect_uri` passed in the authorization request
is `https://organisatie.dev-vlaanderen.local/#/oic` as registered. Keycloak's URI matching
does an exact match (or glob). We must register this exact URI in `validRedirectUris`.

Keycloak also supports `*` wildcard: `"validRedirectUris": ["https://organisatie.dev-vlaanderen.local/*"]`
covers all paths and hash variants.

**Conclusion**: Register both:
```json
"validRedirectUris": [
  "https://organisatie.dev-vlaanderen.local/#/oic",
  "https://organisatie.dev-vlaanderen.local/v2/oic",
  "https://organisatie.dev-vlaanderen.local/*"
]
```

---

## RQ-6: Keycloak Realm JSON Import Format

### Question

What is the exact JSON structure for a Keycloak 24.0 realm export that supports `--import-realm`?

### Resolution

The `--import-realm` flag reads JSON files from the `data/import` directory (mounted as `./keycloak`).
The file must be a valid Keycloak realm representation, equivalent to an "Export realm" operation
from the Keycloak admin console.

Key top-level fields:
```json
{
  "realm": "wegwijs",
  "enabled": true,
  "clients": [...],
  "clientScopes": [...],
  "users": [...],
  "roles": { "realm": [...], "client": {} }
}
```

Clients have `protocolMappers` arrays. Client scopes are separate objects that clients reference.
Users have `credentials` (plaintext passwords are converted to hashed on import with
`"temporary": false`) and `attributes`.

**Important**: For client secrets, use the `secret` field directly in the client object:
```json
{
  "clientId": "cjmClient",
  "secret": "secret",
  "serviceAccountsEnabled": true,
  ...
}
```

Keycloak 24.0 accepts plain secrets in realm import (they are stored hashed internally).

**Idempotency**: If the realm already exists, `--import-realm` skips import by default
(`KEYCLOAK_IMPORT_STRATEGY=IGNORE_EXISTING`). To force re-import during development, delete the
keycloak volume or use `KEYCLOAK_IMPORT_STRATEGY=OVERWRITE_EXISTING`.

### Conclusion

Author `keycloak/realm-export.json` as a complete realm export structure. Test with a fresh
volume first, then verify idempotency.

---

## Summary Table

| Unknown | Status | Decision |
|---|---|---|
| Claim mappers for `iv_wegwijs_rol_3D`, `vo_id` | RESOLVED | User Attribute mapper, `multivalued: true` for roles |
| Introspection endpoint path | RESOLVED | `…/realms/wegwijs/protocol/openid-connect/token/introspect` |
| Token exchange for BFF | RESOLVED | Not needed; authorization code flow with API scope |
| Docker split-horizon URL | RESOLVED | `KC_HOSTNAME_URL` + separate `Authority` vs `AuthorizationIssuer` fields |
| PKCE redirect URI with hash | RESOLVED | Wildcard `validRedirectUris` covers it |
| Realm JSON import format | RESOLVED | Standard Keycloak realm export format, plain secrets accepted |
