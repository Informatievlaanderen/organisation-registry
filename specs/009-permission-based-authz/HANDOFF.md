# Feature #009 — Permission-Based Authz — Handoff

**Branch:** `009-permission-based-authz`
**Status:** Phase 3 (US1) code complete; Phase 4 (US2) partially done through T026a. Unit suite green, 3 integration failures pending investigation.

## TL;DR State

- **Unit tests:** `test/OrganisationRegistry.UnitTests` → **820 passed / 3 skipped / 0 failed** (net10.0, ~2s).
- **Full solution `dotnet test`:** 3 red integration tests, all on the **CJM Client Credentials path** with error `"Geen machtiging op organisatie"`:
  - `EditApi.CreateBankAccountNumberTests.CanCreateAndUpdateAs(cjmClient, dv_organisatieregister_cjmbeheerder)` — 400 BadRequest
  - `EditApi.CreateBankAccountNumberTests.AsCjmBeheerder_CannotUpdateWithInvalidFrom_InvalidTo` — cascading NRE at `ApiFixture.cs:617`
  - `EditApi.CreateContactsTests.AsCJM_CanAddAndUpdate` — 400 BadRequest
- **Working tree:** dirty with all #009 work uncommitted. Nothing pushed. 24+ stashes exist locally.
- **Last fix applied but not committed:** `BeheerderForOrganisationRegardlessOfVlimpersPolicy.cs:17` — `CanAddContacts` → `CanEditAll`. This unblocked ~46 red unit tests.

## Architecture Decisions (locked)

1. **Single translation layer:** roles + scopes → `PermissionSet` in `ClaimsExtension.ToPermissionSet` (Api/Security). Applied at 3 entry points: EditApi role claims, TokenExchange role claims, bearer token scope claims (CC).
2. **`RolePermissionMap`** = single dictionary source-of-truth. **`ScopePermissionMap`** mirrors it for CC scopes.
3. **Fail-closed on unknown role/scope**, warning throttled once per role/scope per process.
4. **Union semantics** across multi-role and multi-scope.
5. **Controllers do general permission checks only**, via extended `[OrganisationRegistryAuthorize(RequiredPermissions = …)]` (OR-semantics, `IAsyncAuthorizationFilter`). **Policies do scope/restriction checks only**.
6. **`CanEditAll` short-circuits** the attribute (AlgemeenBeheerder always passes).
7. **Automated processes** enter via CC scopes, never a role. `AutomatedTask` role kept as transitional bridge (T036 removes).
8. **Cutover migration** — no dual-run. Roles + scope-strings purged from internal security model post-edge-translation.
9. **Resource-level restrictions** fetched JIT via `IUserRestrictionsProvider` (request-scoped, memoised). Concrete impl deferred to US2/US3 (T032).
10. **AB vs Developer invariant intentionally broken:** Developer is strict superset of AB by `CanRunScheduledJobs`.
11. **Resource-level `resource:action` permissions, `/v1/me` endpoint, canManage*/canEdit/canSelect per-org, new roles** → deferred to feature **#010**.

## The 20 Permissions (`Permission.cs`)

`CanEditAll, CanEditChildren, CanEditVlimpers, CanEditDelegations, CanAddLocations, CanAddContacts, CanAddBodies, CanEditBodies, CanRegisterBodies, CanManageKeys, CanManageLabels, CanManageCapacities, CanManageFormalFrameworks, CanManageOrganisationClassifications, CanManageRegulations, CanImport, CanRunScheduledJobs, CanReadOrafin, CanReadInfoEndpoints, CanReadConfiguration, CanEditOrganisationLabels`

Note: 20 listed, 21 in string above — verify count against `Permission.cs`.

## Tasks Progress (`tasks.md`)

- **Phase 1 Setup** T001–T003: ✅ done
- **Phase 2 Foundational** T004–T012: ✅ done
- **Phase 3 US1** T013–T022: ✅ done
- **T023–T025** (attribute extension): ✅ done
- **T026a** (11 controller conversions, backoffice folder): ✅ done — see modified files in `git status`
- **T026b** (Search controllers): ⏳ pending
- **T026c** (Integration/other controllers): ⏳ pending
- **T027, T028**: ⏳ pending
- **Phase 6 (US3)**, **T032**, **T034**, **T035**, **T036**: ⏳ pending

## The 3 Red Integration Tests — Investigation Target

All fail on **CJM Client Credentials scope path** with `"Geen machtiging op organisatie"`. Hypothesis:

`src/OrganisationRegistry.Infrastructure/Authorization/ScopePermissionMap.cs` maps scope `dv_organisatieregister_cjmbeheerder` to **fewer permissions** than `RolePermissionMap.For(Role.CjmBeheerder)`. The policies (`BeheerderForOrganisationButNotUnderVlimpersManagementPolicy` and siblings) likely check for permissions granted to CJM users via role but missing from the CC scope translation.

**Next dev action:**
1. Read `ScopePermissionMap.cs` — enumerate CJM scope permissions.
2. Read `RolePermissionMap.cs` L16-97 — enumerate `CjmBeheerder` role permissions.
3. Diff them. Anything the role has but the scope doesn't = suspect.
4. Read the failing endpoints' policies (`BankAccounts`, `OrganisationContacts` in `Startup.cs` L321-356) and the `ISecurityPolicy.Check(IUser)` impls they invoke.
5. Bring CJM scope mapping in line with CJM role mapping — likely add `CanAddContacts` (was the recent bug) and BankAccount-related perms.

Then audit **all 17 `ISecurityPolicy` impls** in `src/OrganisationRegistry/Handling/Authorization/` for wrong `HasPermission(Permission.Can*)` calls introduced during T026a-style conversions:

```bash
grep -rn "HasPermission(Permission\." src/OrganisationRegistry/Handling/Authorization/
```

## Suggested Commit Cadence (next session)

Before touching anything else, commit the current green state in small focused chunks:

1. `fix: OR-XXXX correct permission check in BeheerderForOrganisationRegardlessOfVlimpersPolicy`
2. `feat: OR-XXXX add Permission enum, RolePermissionMap, ScopePermissionMap, PermissionSet`
3. `feat: OR-XXXX translate roles + scopes to permissions at token consumption`
4. `feat: OR-XXXX extend OrganisationRegistryAuthorize with RequiredPermissions`
5. `refactor: OR-XXXX convert 11 backoffice controllers to permission-based authz (T026a)`
6. `test: OR-XXXX add authorization unit + integration test suites`
7. `docs: OR-XXXX add feature #009 spec + security architecture`

(Replace `OR-XXXX` with the actual Jira ticket; check `git log --oneline` for the parent story ticket.)

## Key File Paths (absolute)

**Core impl:**
- `/code/aiv/organisation-registry/src/OrganisationRegistry.Infrastructure/Authorization/Permission.cs`
- `/code/aiv/organisation-registry/src/OrganisationRegistry.Infrastructure/Authorization/RolePermissionMap.cs`
- `/code/aiv/organisation-registry/src/OrganisationRegistry.Infrastructure/Authorization/ScopePermissionMap.cs` ← **investigate for CJM failures**
- `/code/aiv/organisation-registry/src/OrganisationRegistry.Infrastructure/Authorization/PermissionSet.cs`
- `/code/aiv/organisation-registry/src/OrganisationRegistry.Infrastructure/Authorization/IUserRestrictionsProvider.cs`
- `/code/aiv/organisation-registry/src/OrganisationRegistry.Infrastructure/Authorization/User.cs`
- `/code/aiv/organisation-registry/src/OrganisationRegistry.Api/Security/ClaimsExtension.cs`
- `/code/aiv/organisation-registry/src/OrganisationRegistry.Api/Infrastructure/Security/OrganisationRegistryAuthorizeAttribute.cs`
- `/code/aiv/organisation-registry/src/OrganisationRegistry.Api/Infrastructure/Startup.cs` (L321-356 CC policies)

**Recently fixed:**
- `/code/aiv/organisation-registry/src/OrganisationRegistry/Handling/Authorization/BeheerderForOrganisationRegardlessOfVlimpersPolicy.cs:17`

**Failing tests:**
- `/code/aiv/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/EditApi/CreateBankAccountNumberTests.cs:114`
- `/code/aiv/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/EditApi/CreateContactsTests.cs`

**Docs:**
- `/code/aiv/organisation-registry/specs/009-permission-based-authz/spec.md`
- `/code/aiv/organisation-registry/specs/009-permission-based-authz/plan.md`
- `/code/aiv/organisation-registry/specs/009-permission-based-authz/tasks.md`
- `/code/aiv/organisation-registry/specs/009-permission-based-authz/security-architecture.md`

## Gotchas

- **JSON:** API uses Newtonsoft with `StringEnumConverter { CamelCaseNamingStrategy }`. Test-side use `JObject.Parse` + `payload["roles"].Values<string>()`.
- **FluentAssertions on `PermissionSet`:** implements `IEnumerable<Permission>`, so `.Should().Be(other)` picks collection assertion. Cast: `((object)set).Should().Be(other)`.
- **`dotnet build`:** pass **one** csproj at a time. Use `--no-incremental` when hunting CS0618 obsolete warnings.
- **Integration tests** require Keycloak at `http://keycloak.localhost:9080/realms/wegwijs`.
- **`SecurityInformation`:** 2 public ctors, get-only props, no `[JsonConstructor]` — deserialization fragile.
- **CC test seeding:** `ApiFixture` nested `Orafin`/`CJM`/`Test`; secrets `{cjm,orafin,test}-client-secret-2024`.
- **Latent bug (pre-existing):** `Permission.CanAddContacts` is unmapped in **every** role. `OrganisationContactCommandController.cs:31` uses it. Address in T026b or T026c.
