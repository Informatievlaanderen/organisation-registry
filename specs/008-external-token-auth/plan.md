# Implementation Plan: External Token Authentication

**Branch**: `008-external-token-auth` | **Date**: April 24, 2026 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/008-external-token-auth/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Implement external token authentication using OAuth 2.0 token introspection for third-party token exchange, providing equivalent access rights to existing JWT Bearer authentication while preserving current M2M introspection functionality. This includes creating a new TokenExchange authentication scheme with content-based authorization and configurable introspection caching.

## Technical Context

**Language/Version**: C# / .NET 6.0.201  
**Primary Dependencies**: ASP.NET Core, Entity Framework Core, IdentityModel, FluentValidation, AutoFixture, Serilog  
**Storage**: SQL Server (event store + read models), ElasticSearch (search projections)  
**Testing**: xUnit, AggregateSource.Testing framework, OrganisationRegistry.Tests.Shared harness  
**Target Platform**: Linux server containers, ASP.NET Core web service  
**Project Type**: Web service with RESTful API, event sourcing + CQRS architecture  
**Performance Goals**: 99.9% authentication success rate, <2s introspection response time  
**Constraints**: Content-based authorization (not scheme-based), preserve M2M functionality  
**Scale/Scope**: Government-scale authentication system, existing admin API endpoints

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### ✅ I. Event Sourcing is Immutable Law
**Status**: Compliant - This feature only adds authentication middleware and authorization logic, does not modify event sourcing patterns.

### ✅ II. Commands and Events Drive All State Changes  
**Status**: Compliant - No new domain commands/events required for authentication middleware. Existing security events remain unchanged.

### ✅ III. CQRS: Commands and Queries are Architecturally Separate
**Status**: Compliant - Authentication is cross-cutting concern that doesn't violate CQRS separation. Both command and query paths will use new auth scheme.

### ✅ IV. Respect Aggregate Boundaries
**Status**: Compliant - Authentication operates at middleware layer, above aggregates. No aggregate internal state exposure.

### ⚠️ V. Testing is Not Optional—It is a Guardrail
**Status**: Test-First Approach Required - User specifically requested "plan to first implement tests". Must write failing tests before implementation.

**Test-First Strategy:**
- Unit tests for new authentication scheme configuration 
- Integration tests for token introspection workflows
- Authorization policy tests for content-based decisions
- Cache behavior tests for introspection results
- Edge case tests for provider failures and token revocation

### Architecture Compliance
- ✅ Domain Language: Using established terms (no new domain concepts)
- ✅ External Integration: Following existing OAuth2 introspection pattern (EditApi)
- ✅ Module Boundaries: Changes contained within Infrastructure and API modules

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

## Project Structure

### Documentation (this feature)

```text
specs/008-external-token-auth/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
src/OrganisationRegistry.Api/
├── Infrastructure/
│   ├── Security/
│   │   ├── AuthenticationSchemes.cs           # Add TokenExchange scheme
│   │   ├── TokenExchangeConfigurationSection.cs  # New: Provider configuration
│   │   └── TokenExchangeIntrospectionHandler.cs   # New: Introspection logic
│   ├── Middleware/
│   │   └── ConfigureClaimsPrincipalSelectorMiddleware.cs  # Update: Add TokenExchange scheme
│   └── Startup.cs                             # Update: Configure new auth scheme
├── Security/
│   ├── SecurityService.cs                     # Update: Content-based authorization
│   └── TokenExchangeAuthorizationHandler.cs   # New: Authorization logic
└── Controllers/                               # Update: Authorization attributes

src/OrganisationRegistry.Infrastructure/
├── Configuration/
│   └── TokenExchangeConfigurationSection.cs   # Configuration model
└── Authorization/
    └── ContentBasedAuthorizationExtensions.cs # New: Content-based auth helpers

test/
├── unit/
│   ├── Authentication/
│   │   ├── TokenExchangeSchemeTests.cs        # New: Auth scheme tests
│   │   ├── IntrospectionCacheTests.cs        # New: Cache behavior tests
│   │   └── ContentBasedAuthorizationTests.cs # New: Authorization logic tests
├── integration/
│   ├── Authentication/
│   │   ├── TokenExchangeIntegrationTests.cs   # New: End-to-end auth tests
│   │   ├── AuthorizationEquivalenceTests.cs  # New: JWT Bearer vs TokenExchange
│   │   └── M2MCompatibilityTests.cs          # New: Ensure M2M still works
└── contract/
    ├── TokenIntrospectionContractTests.cs     # New: OAuth2 introspection contract
    └── AuthorizationPolicyContractTests.cs    # New: Policy behavior contracts
```

**Structure Decision**: The feature follows Organisation Registry's established module boundaries:
- **Infrastructure**: Authentication schemes, middleware, configuration
- **Security**: Authorization logic and content-based decision making  
- **Tests**: Comprehensive unit/integration/contract test coverage with test-first approach

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |
