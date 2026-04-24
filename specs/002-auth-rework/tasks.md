---

description: "Task list for authentication rework — multi-method support with migration path"
---

# Tasks: Authentication Rework — Multi-Method Support with Migration Path

**Feature**: `002-auth-rework`
**Specification**: `specs/002-auth-rework/spec.md`
**Implementation Plan**: `specs/002-auth-rework/plan.md`

---

## Task Overview

**Total Tasks**: 22
**Critical constraint**: Zero regressions on SPA and M2M paths throughout all phases
**Key sequence rule**: Scheme restrictions MUST NOT be removed from attributes (Phase 3) until named policies exist for all role combinations (Phase 2). Phase 2 MUST NOT start until the composite scheme is in place (Phase 1).

### Phase Sequence

1. **Phase 0**: Investigation — resolve unknowns before writing any production code
2. **Phase 1**: Composite default scheme registration
3. **Phase 2**: Introduce named policies for all role combinations (replaces `Roles =` checks)
4. **Phase 3**: Remove `AuthenticationSchemes` from all attributes
5. **Phase 4**: SecurityService fix (if needed per Phase 0 findings)
6. **Phase 5**: Integration tests + final verification

---

## Phase 0: Investigation — Resolve Unknowns Before Coding

**Purpose**: Answer three unknowns that drive implementation decisions in later phases.

- [ ] T001 — **Token shape**: Using the local identity server at `http://127.0.0.1:5050`, obtain a client credentials token. Determine whether it is an opaque token or a JWT. Document the result — drives the forwarding heuristic in T005.

- [ ] T002 — **Token exchange claim shape**: Attempt an RFC 8693 token exchange request against `http://127.0.0.1:5050`. If successful, call the introspection endpoint and record all returned claims. Compare against `SecurityService.GetRequiredUser()` branching logic (scope switch, then GivenName/Surname/AcmId path) AND `GetSecurityInformation()` (`GetRequiredClaim(ClaimTypes.GivenName)` etc.). Document which code paths handle the claims correctly and which need modification.

- [ ] T003 — **Local identity server RFC 8693 support**: Determined as part of T002. If the local server supports token exchange, integration tests (T019) use the `[EnvVarIgnoreFact]` pattern with a real token. If not, use WireMock to stub the introspection endpoint.

**Checkpoint**: All three unknowns documented. Implementation phases may begin.

---

## Phase 1: Composite Default Scheme

**Purpose**: Make both `Bearer` and `EditApi` available as authentication paths for any `[Authorize]` attribute that does not specify a scheme. Prerequisite for all later phases.

**⚠️ CRITICAL**: Run `dotnet test` after each task. Zero regressions allowed.

- [ ] T004 [P] — Add `Composite` constant to `src/OrganisationRegistry.Api/Infrastructure/Security/AuthenticationSchemes.cs`

- [ ] T005 — In `src/OrganisationRegistry.Api/Infrastructure/Startup.cs`, add `AddPolicyScheme(AuthenticationSchemes.Composite, ...)` **before** the existing `AddJwtBearer` and `AddOAuth2Introspection` calls:
  - `ForwardDefaultSelector`: inspects the raw `Authorization` header — if the value after `"Bearer "` is a three-part `header.payload.signature` token → forward to `JwtBearerDefaults.AuthenticationScheme`; otherwise → forward to `AuthenticationSchemes.EditApi`
  - `ForwardChallenge = JwtBearerDefaults.AuthenticationScheme` (preserves current 401 challenge behavior)
  - `ForwardForbid = JwtBearerDefaults.AuthenticationScheme`
  - Update `options.DefaultScheme`, `options.DefaultAuthenticateScheme`, `options.DefaultChallengeScheme` from `JwtBearerDefaults.AuthenticationScheme` to `AuthenticationSchemes.Composite`
  - Add a code comment block before the `AddAuthentication` call explaining: (1) `Bearer` = self-minted HMAC-SHA256 JWT for legacy SPA — deprecated, (2) `EditApi` = ACM/IDM introspection for M2M and BFF token exchange — stays, (3) `Composite` = forwards to one of the above per request
  - **Note**: if T001 reveals that ACM/IDM tokens are also JWTs, update the heuristic to additionally check the JWT's `iss` claim to distinguish them from the self-minted JWT

- [ ] T006 — Verify `src/OrganisationRegistry.Api/Infrastructure/Security/ConfigureClaimsPrincipalSelectorMiddleware.cs`:
  - The middleware explicitly calls `GetAuthenticateInfo("Bearer")` then `GetAuthenticateInfo("EditApi")` — it does not use `DefaultAuthenticateScheme`, so the composite scheme change does not affect it
  - If this assumption is confirmed by reading the code, mark complete with note "no change required"
  - If the middleware does use the default scheme anywhere, update it here

**Checkpoint**: `dotnet test` — all tests green. No behavior change for SPA or M2M callers.

---

## Phase 2: Introduce Named Policies for Role-Based Authorization

**Purpose**: Replace `AuthorizeAttribute.Roles` checks (which depend on `RoleClaimType` — scheme-specific) with named policies using `RequireClaim` (scheme-agnostic). This is a prerequisite for Phase 3 — removing scheme restrictions is only safe once policies express authorization in terms of claims, not role type resolution.

**Why this is necessary**: `EditApi` introspection does not set `RoleClaimType = ClaimTypes.Role`. If the `Bearer` scheme restriction is removed while `Roles = "algemeenBeheerder"` is still set, all `EditApi`-authenticated tokens fail every backoffice role check with 403.

**⚠️ CRITICAL**: Run `dotnet test` after each task.

- [ ] T007 — Audit all distinct role combinations used across `[OrganisationRegistryAuthorize(Role.X, ...)]` call sites. Output: a complete list of unique `Role[]` combinations and the policy name to assign each. Example combinations found:
  - No roles (authenticated only) — no policy, just `IsAuthenticated`
  - `[Role.AlgemeenBeheerder]`
  - `[Role.AlgemeenBeheerder, Role.Developer]`
  - `[Role.AlgemeenBeheerder, Role.CjmBeheerder, Role.Developer, Role.VlimpersBeheerder]`
  - `[Role.AlgemeenBeheerder, Role.DecentraalBeheerder]`
  - `[Role.AlgemeenBeheerder, Role.OrgaanBeheerder, Role.CjmBeheerder]`
  - `[Role.AutomatedTask, Role.Developer]`
  - `[Role.AlgemeenBeheerder, Role.VlimpersBeheerder]`
  - `[Role.Developer]`
  - `[Role.AlgemeenBeheerder, Role.CjmBeheerder, Role.Developer]`
  - *(run `grep -rn "OrganisationRegistryAuthorize(Role" src/` to get the complete list)*
  - Document the complete list here before proceeding

- [ ] T008 — In `Startup.cs` (inside the `Authorization = options =>` block), register a named policy for each unique role combination from T007:
  - Policy name format: a deterministic, stable string (e.g. `"Role:AlgemeenBeheerder"`, `"Role:AlgemeenBeheerder+Developer"`)
  - Policy builder: `builder.RequireClaim(ClaimTypes.Role, roleValues...)` using the `RoleMapping.Map(role)` string values
  - These policies check `ClaimTypes.Role` claims directly — independent of `ClaimsIdentity.RoleClaimType`

- [ ] T009 — Modify `src/OrganisationRegistry.Api/Infrastructure/Security/OrganisationRegistryAuthorizeAttribute.cs`:
  - The role-parameterized constructor: instead of `Roles = string.Join(",", roles.Select(RoleMapping.Map))`, set `Policy = <deterministic policy name from T008>`
  - The no-arg constructor: remove `AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme` — rely on composite default scheme from Phase 1
  - Remove `using Microsoft.AspNetCore.Authentication.JwtBearer;` if no longer used after this change

**Checkpoint**: `dotnet test` — all tests green. Authorization behavior is identical to before (same roles accepted/rejected), but now implemented via named claim-based policies instead of `RoleClaimType`-dependent `Roles` property.

---

## Phase 3: Remove Scheme Restrictions from Attributes

**Purpose**: With scheme-agnostic policies in place (Phase 2) and a composite default scheme (Phase 1), scheme restrictions on attributes are now redundant and can be removed.

**⚠️ CRITICAL**: Run `dotnet test` after each task.

- [ ] T010 [P] — `src/OrganisationRegistry.Api/Edit/Organisation/Contacts/OrganisationContactsController.cs`: change `[Authorize(AuthenticationSchemes = AuthenticationSchemes.EditApi, Policy = PolicyNames.OrganisationContacts)]` → `[Authorize(Policy = PolicyNames.OrganisationContacts)]`

- [ ] T011 [P] — `src/OrganisationRegistry.Api/Edit/Organisation/BankAccount/OrganisationBankAccountController.cs`: remove `AuthenticationSchemes = AuthenticationSchemes.EditApi`; keep `Policy = PolicyNames.BankAccounts`

- [ ] T012 [P] — `src/OrganisationRegistry.Api/Edit/Organisation/Key/OrganisationKeyController.cs`: remove `AuthenticationSchemes = AuthenticationSchemes.EditApi`; keep `Policy = PolicyNames.Keys`

- [ ] T013 [P] — `src/OrganisationRegistry.Api/Edit/Organisation/Classification/OrganisationOrganisationClassificationController.cs`: remove `AuthenticationSchemes = AuthenticationSchemes.EditApi`; keep `Policy = PolicyNames.OrganisationClassifications`

- [ ] T014 [P] — `src/OrganisationRegistry.Api/Edit/Organisation/KboNumber/OrganisationsController.cs`: remove `AuthenticationSchemes = AuthenticationSchemes.EditApi`; keep `Policy = PolicyNames.Organisations`

**Note**: T010–T014 are fully parallel — each touches a different file.

**Checkpoint**: `dotnet test` — all tests green. Verify SC-004: `grep -rn "AuthenticationSchemes"` in controller and attribute files — zero matches.

---

## Phase 4: SecurityService Fix (if needed)

**Purpose**: Handle token exchange claim shape in `SecurityService` if Phase 0 T002 revealed gaps.

- [ ] T015 — Based on T002 findings, update `src/OrganisationRegistry.Api/Security/SecurityService.cs` as needed:
  - `GetRequiredUser()`: if token exchange tokens carry a scope value not yet in the `switch` block, add it; if they carry user identity claims without a recognized scope, confirm or fix the fallthrough path
  - `GetSecurityInformation()`: if token exchange tokens lack `ClaimTypes.GivenName` / `ClaimTypes.Surname` / `AcmId`, add a guard or short-circuit path that returns `SecurityInformation.None()` for M2M-like tokens
  - If no change is needed based on T002, mark complete with a comment confirming this
  - **Constraint**: no `IClaimsTransformation` — handle the claim shape directly here

**Checkpoint**: Unit test confirms `GetRequiredUser()` and `GetSecurityInformation()` return correct results for a token exchange introspection claim set (mocked `ClaimsPrincipal`).

---

## Phase 5: Integration Tests + Final Verification

- [ ] T016 [P] — Write integration test class `TokenExchangeAuthenticationTests` in `test/OrganisationRegistry.Api.IntegrationTests/Security/`:
  - **Test A**: valid token exchange token + policy satisfied → HTTP 200
  - **Test B**: valid token exchange token + policy NOT satisfied → HTTP 403
  - **Test C**: invalid/expired token exchange token → HTTP 401
  - Follow `[EnvVarIgnoreFact]` pattern if local identity server supports RFC 8693 (T003 finding); otherwise use WireMock to stub the introspection endpoint with a pre-crafted claims response for a synthetic token value

- [ ] T017 [P] — Verify SC-001 and SC-002: run `dotnet test` — all existing SPA-facing and M2M integration tests pass green

- [ ] T018 — Final verification:
  - Run `dotnet test` — full suite green (SC-001, SC-002, SC-006)
  - `grep -rn "AuthenticationSchemes" src/OrganisationRegistry.Api/ --include="*.cs"` — zero matches in `[Authorize]` or `[OrganisationRegistryAuthorize]` attribute usage sites (SC-004)
  - Confirm T016 tests cover 200/401/403 for token exchange path (SC-006)
  - Confirm `GetRequiredUser()` returns non-null `IUser` for token exchange path (SC-005)

**Checkpoint**: All success criteria SC-001 through SC-006 verified. Feature ready for review.

---

## Dependencies & Execution Order

```
T001+T002+T003 (parallel, Phase 0)
  → T004+T005 (parallel, Phase 1 infrastructure)
  → T006 (Phase 1 verification — read middleware)
  → T007 (Phase 2 audit — must complete before T008)
  → T008 → T009 (Phase 2 — sequential: register policies then update attribute)
  → T010+T011+T012+T013+T014 (parallel, Phase 3 — all after Phase 2 complete)
  → T015 (Phase 4 — after Phase 0 T002 + Phase 3 complete)
  → T016+T017 (parallel, Phase 5)
  → T018 (final — after T016+T017)
```

### Critical Path

**Phase 2 before Phase 3**: scheme restrictions cannot be removed until all role combinations have named policies.
**Phase 1 before Phase 2**: the composite default must be in place before any `AuthenticationSchemes` is removed.
**Phase 0 T002 before T015**: `GetRequiredUser()` fix depends on knowing the token exchange claim shape.

---

## Notes

- [P] = tasks in the same phase with no inter-dependency — can run in parallel
- **Do NOT modify** `OrganisationRegistryTokenBuilder`, `GET /v1/security/exchange`, or any existing ACM/IDM scope constants
- **Do NOT add** a new NuGet package, a new auth scheme registration, a new configuration section, or `IClaimsTransformation`
- The policy name format (T008) must be stable — it will be embedded in `OrganisationRegistryAuthorizeAttribute` and compared at runtime. Choose a simple deterministic format and document it.
- Commit after each task or logical group: `fix: OR-XXXX description` (conventional commits, no Co-Authored-By)
