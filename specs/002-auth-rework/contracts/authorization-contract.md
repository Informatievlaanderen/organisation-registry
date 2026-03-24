# Contracts: Authentication Rework

**Branch**: `002-auth-rework` | **Date**: 2026-03-23

---

## Summary

This feature introduces **no new API endpoints** and **no changes to existing request/response schemas**. The external contract (HTTP API surface) is unchanged.

The only observable external change is:

**Before**: Requests authenticated with an ACM/IDM token (client credentials or token exchange) directed at backoffice endpoints (`/v1/backoffice/**`) return 401 because `[OrganisationRegistryAuthorize]` only accepts the `Bearer` (self-minted JWT) scheme.

**After**: Requests authenticated with any valid token (self-minted JWT, client credentials, token exchange) directed at any endpoint return 2xx/4xx based solely on the policy claims of the token — not on which scheme validated it.

---

## Authentication Contract

### Accepted token formats

All three are sent as `Authorization: Bearer <value>`:

| Token type | Validated by | Required claims for backoffice access |
|---|---|---|
| Self-minted HMAC-SHA256 JWT | `AddJwtBearer` (local) | `ClaimTypes.Role` with valid role value |
| ACM/IDM client credentials | `AddOAuth2Introspection` (network) | `scope` with valid scope value |
| ACM/IDM token exchange | `AddOAuth2Introspection` (network) | `ClaimTypes.Role` or `scope` depending on endpoint |

### Response codes (unchanged semantics)

| Condition | Response |
|---|---|
| No `Authorization` header | 401 |
| Invalid or expired token | 401 |
| Valid token, insufficient claims for endpoint | 403 |
| Valid token, claims satisfy policy | 200/201/204 |

---

## No Breaking Changes

- `GET /v1/security/exchange` — unchanged.
- All existing M2M endpoints and scopes — unchanged.
- All existing SPA-facing endpoints — unchanged.
