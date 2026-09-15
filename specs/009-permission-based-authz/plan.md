# Implementation Plan: Permission-Based Authorization

**Branch**: `009-permission-based-authz` | **Date**: 2026-08-27 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/009-permission-based-authz/spec.md`

## Summary

Refactor authorization from role-based checks scattered across controllers and policies to a permission-based model. Claims from ACM/IDM are translated to a `PermissionSet` at three well-defined entry points: (1) edit-api endpoint role claims, (2) token-exchange role claims, and (3) bearer token / Client Credentials **scope** claims. Roles and scopes translate via two static maps (`RolePermissionMap`, `ScopePermissionMap`); the deprecated `AutomatedTask` role is removed — automated processes enter exclusively via CC scopes. Downstream code (controllers, `ISecurityPolicy` implementations) checks permissions or resource-level scope restrictions, never roles or scope strings. Controllers use the declarative `[OrganisationRegistryAuthorize(RequiredPermissions = …)]` attribute for general permission gating; policies handle resource-level scope checks via an `IUserRestrictionsProvider` that fetches restrictions just-in-time from SQL Server projections, memoised for the request lifetime. Cutover migration: no dual-check period; roles and scope-strings fully purged from internal security model post-translation. Unknown/unmapped roles or scopes fail closed (empty permission set + error log). Multiple roles/scopes resolve via union (widest scope wins).

## Technical Context

**Language/Version**: C# / .NET 8, nullable reference types enabled
**Primary Dependencies**: ASP.NET Core, Be.Vlaanderen.Basisregisters.AggregateSource, FluentValidation, Serilog, AutoFixture
**Storage**: SQL Server (event store + read models — JIT restrictions read from projections); ElasticSearch (search projections) — not touched by this feature
**Testing**: xUnit + FluentAssertions + AutoFixture; AggregateSource.Testing for domain tests
**Target Platform**: Linux (Docker), ASP.NET Core web service
**Project Type**: Single web-service (existing multi-project .NET solution)
**Performance Goals**: Permission checks O(1) hash-set lookup; JIT restrictions fetched at most once per (user, resource-type) per request
**Constraints**: Cutover migration — no dual-check window; fail-closed on unknown roles/scopes; Dutch domain naming preserved for role/scope/domain concepts; permission ids PascalCase English
**Scale/Scope**: ~9 roles (→ 8 after `AutomatedTask` removal), 4 CC scopes, ~18 permissions, ~30-50 controllers, 17 `ISecurityPolicy` implementations, 3 entry points; users may hold restrictions for up to ~100 organisations (JIT rationale)

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

Evaluated against `.specify/memory/constitution.md` v0.1.0:

- **I. Event Sourcing Integrity**: PASS — no events created, modified, or deleted. Authorization is infrastructure; JIT restrictions read existing projections.
- **II. CQRS Separation**: PASS — no command/query path changes; authorization sits above both. `IUserRestrictionsProvider` consumes existing read-models, does not introduce new ones.
- **III. Domain Language (Dutch)**: PASS — role and scope names (`AlgemeenBeheerder`, `DecentraalBeheerder`, `dv_organisatieregister_orafinbeheerder`, …) preserved; permission ids are technical/English PascalCase (`CanEditChildren`), consistent with existing infra naming.
- **IV. Aggregate Boundaries**: PASS — no aggregate touched; `ISecurityPolicy` scope logic reshaped, not relocated across aggregates.
- **V. Testing Discipline**: APPLIES — unit tests for role/scope mapping, restrictions provider, scope-only policies, integration tests per entry point (incl. CC scope path). Enforced in tasks phase.

**Post-Phase-1 gate**: PASS. No violations to record in Complexity Tracking.

## Project Structure

### Documentation (this feature)

```
specs/009-permission-based-authz/
├── plan.md              # This file
├── spec.md              # Feature specification (clarified — 5 Q&A recorded)
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output
│   └── permission-check-api.md
└── checklists/
    └── requirements.md  # Quality checklist
```

### Source Code (repository root)

```
src/
├── OrganisationRegistry.Infrastructure/
│   └── Authorization/
│       ├── Role.cs                          # existing — kept for edge translation only
│       ├── User.cs                          # MODIFY — Permissions + HasPermission
│       ├── AcmIdmConstants.cs               # existing — scope catalog reference
│       ├── Permission.cs                    # NEW — enum
│       ├── PermissionSet.cs                 # NEW — immutable set with union
│       ├── RolePermissionMap.cs             # NEW — role → PermissionSet
│       ├── ScopePermissionMap.cs            # NEW — scope-string → PermissionSet
│       └── IUserRestrictionsProvider.cs     # NEW — JIT restrictions contract
├── OrganisationRegistry.Api/
│   ├── Security/
│   │   ├── RoleMapping.cs                              # MODIFY — output PermissionSet
│   │   ├── SecurityService.cs                          # MODIFY — replace scope→WellknownUser dispatch with ScopePermissionMap
│   │   ├── ClaimsExtension.cs                          # MODIFY — extract permissions
│   │   ├── TokenExchangeClaimsTransformation.cs        # MODIFY — entry point #2
│   │   ├── OrganisationRegistryTokenBuilder.cs         # MODIFY — entry point #1
│   │   └── UserRestrictionsProvider.cs                 # NEW — SQL Server projection reads, request-scoped
│   └── Infrastructure/
│       ├── Security/
│       │   └── OrganisationRegistryAuthorizeAttribute.cs  # MODIFY — RequiredPermissions parameter
│       └── PolicyNames.cs                              # MODIFY — align with permission ids
└── OrganisationRegistry/
    └── Handling/Authorization/
        └── *SecurityPolicy.cs               # MODIFY — permission gate + JIT restrictions

test/
├── OrganisationRegistry.UnitTests/
│   └── Authorization/                       # NEW — mapping + restrictions provider + policy scope tests
└── OrganisationRegistry.Api.IntegrationTests/
    └── Security/                            # NEW — per-entry-point e2e (incl. CC scope path)
```

**Structure Decision**: Single web-service layout — no new projects. All changes localised to existing `OrganisationRegistry.Infrastructure` (new authorization types), `OrganisationRegistry.Api` (entry-point translation, attribute, restrictions provider implementation), and `OrganisationRegistry` (policy refactor). Module boundaries respected: infra defines contracts (`Permission`, maps, `IUserRestrictionsProvider`), api implements the provider and consumes the maps at claim-translation sites, domain policies use only the `IUser` abstraction + injected `IUserRestrictionsProvider`.

## Complexity Tracking

*No constitutional violations. Section intentionally empty.*
