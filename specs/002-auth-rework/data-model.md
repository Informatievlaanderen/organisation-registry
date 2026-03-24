# Data Model: Authentication Rework — Multi-Method Support

**Branch**: `002-auth-rework` | **Date**: 2026-03-23

---

## Overview

This feature is a pure infrastructure change. No domain events, aggregates, or read models are introduced or modified. The "data model" for this feature describes the authentication and authorization constructs and how they map to claims principals.

---

## Authentication Scheme Registry (as-is → transition state)

| Scheme Name | Handler | Token Type | Path | Change |
|---|---|---|---|---|
| `Bearer` (`JwtBearerDefaults.AuthenticationScheme`) | `AddJwtBearer` | Self-minted HMAC-SHA256 JWT | SPA | **Unchanged** — deprecated but retained |
| `EditApi` (`AuthenticationSchemes.EditApi`) | `AddOAuth2Introspection` | ACM/IDM tokens (client credentials + token exchange) | M2M + BFF | **Unchanged** |
| `Composite` (new) | `AddPolicyScheme` | Forwarding scheme — not a real handler | All | **NEW** — becomes `DefaultAuthenticateScheme` |

---

## Token Claims Profiles

### Self-minted JWT (Bearer path)

Built by `OrganisationRegistryTokenBuilder`. Claims set after `ParseRoles()`:

| Claim | Value example | Used by |
|---|---|---|
| `ClaimTypes.GivenName` | `"Test"` | `GetRequiredUser`, `GetSecurityInformation` |
| `ClaimTypes.Surname` | `"Api"` | `GetRequiredUser`, `GetSecurityInformation` |
| `AcmIdmConstants.Claims.AcmId` (`vo_id`) | `"1239879897-123123"` | `GetRequiredUser`, cache key |
| `ClaimTypes.Role` | `"algemeenBeheerder"` | Role-based authorization |
| `urn:be:vlaanderen:wegwijs:organisation` | `"OVO000123"` | Organisation access scoping |
| `AcmIdmConstants.Claims.Ip` | `"127.0.0.1"` | Added by middleware |

### ACM/IDM Client Credentials Token (EditApi introspection path)

Returned by ACM/IDM introspection endpoint for M2M tokens:

| Claim | Value example | Used by |
|---|---|---|
| `AcmIdmConstants.Claims.Scope` | `"dv_organisatieregister_cjmbeheerder"` | Scope policies (`PolicyNames.*`), `GetRequiredUser` scope switch |
| `sub` | `"cjmClient"` | (not used directly) |

`GetRequiredUser` switches on `scope` → returns `WellknownUsers.Cjm` / `WellknownUsers.Orafin` / `WellknownUsers.TestClient`.

### ACM/IDM Token Exchange Token (EditApi introspection path)

Same introspection path as client credentials. Claims depend on ACM/IDM realm configuration. For a token representing a human user via token exchange:

| Claim | Value example | Used by |
|---|---|---|
| `AcmIdmConstants.Claims.Scope` | `"dv_organisatieregister_info"` (or absent) | Scope policy if applicable |
| `given_name` OR `urn:be:vlaanderen:acm:voornaam` | `"Jan"` | `GetRequiredUser` human-user path |
| `family_name` OR `urn:be:vlaanderen:acm:familienaam` | `"Janssen"` | `GetRequiredUser` human-user path |
| `vo_id` | `"user-acm-id"` | `GetRequiredUser`, cache key |
| `iv_wegwijs_rol_3D` | `"algemeenBeheerder"` | Role-based authorization |
| `urn:be:vlaanderen:wegwijs:organisation` | `"OVO000123"` | Organisation access scoping |

**Key finding from research**: The `Bearer` self-minted JWT maps ACM/IDM claims to .NET `ClaimTypes.*` (e.g., `urn:be:vlaanderen:acm:voornaam` → `ClaimTypes.GivenName`) in `OrganisationRegistryTokenBuilder.ParseRoles()`. The `EditApi` introspection path does NOT perform this mapping — claims arrive as raw ACM/IDM claim names. `GetRequiredUser` and `GetSecurityInformation` call `GetRequiredClaim(ClaimTypes.GivenName)` which will fail for introspection-path tokens that carry user identity.

**Required claim name resolution in `GetRequiredUser`**:

```
GivenName:  ClaimTypes.GivenName ?? AcmIdmConstants.Claims.Firstname
Surname:    ClaimTypes.Surname  ?? AcmIdmConstants.Claims.FamilyName
AcmId:      AcmIdmConstants.Claims.AcmId (vo_id — same in both paths)
```

---

## Authorization Policies (unchanged claim values)

| Policy Name | Claim | Allowed Values | Defined in |
|---|---|---|---|
| `ORGANISATIONS` | `scope` | `dv_organisatieregister_cjmbeheerder`, `dv_organisatieregister_testclient` | `Startup.cs` |
| `BANKACCOUNTS` | `scope` | `dv_organisatieregister_cjmbeheerder`, `dv_organisatieregister_testclient` | `Startup.cs` |
| `ORGANISATIONCLASSIFICATIONS` | `scope` | `dv_organisatieregister_cjmbeheerder`, `dv_organisatieregister_testclient` | `Startup.cs` |
| `ORGANISATIONCONTACTS` | `scope` | `dv_organisatieregister_cjmbeheerder`, `dv_organisatieregister_testclient` | `Startup.cs` |
| `KEYS` | `scope` | `dv_organisatieregister_cjmbeheerder`, `dv_organisatieregister_orafinbeheerder`, `dv_organisatieregister_testclient` | `Startup.cs` |

All policy claim values are **unchanged** by this feature (FR-008).

---

## Affected Files Summary

| File | Change Type | What Changes |
|---|---|---|
| `Infrastructure/Startup.cs` | Modify | Add `AddPolicyScheme("Composite", ...)` with `ForwardDefaultSelector`; set `DefaultAuthenticateScheme = "Composite"` |
| `Infrastructure/Security/AuthenticationSchemes.cs` | Modify | Add `public const string Composite = "Composite"` |
| `Infrastructure/Security/OrganisationRegistryAuthorizeAttribute.cs` | Modify | Remove `AuthenticationSchemes = ...` assignment |
| `Infrastructure/Security/GetAuthenticateInfoExtensions.cs` | Modify | `GetAuthenticateInfoAsync` tries Bearer then EditApi |
| `Edit/*/[Controller].cs` (5 files) | Modify | Remove `AuthenticationSchemes = AuthenticationSchemes.EditApi` from `[Authorize]` |
| `Security/SecurityService.cs` | Modify | `GetRequiredUser` — fallback claim name resolution |
| `test/.../ApiFixture.cs` | Modify | Add `CreateTokenExchangeClient()` helper |
| `test/.../EditApi/TokenExchangeAuthorizationTests.cs` | Add | Integration tests 200/401/403 for token exchange path |

---

## State Transitions

No aggregate state transitions. This feature does not introduce domain events.

---

## Validation Rules

No FluentValidation changes. No new request types.
