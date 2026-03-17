<!-- Sync Impact Report
Version: 0.1.0 (MINOR - Initial constitution for established project)
Rationale: First formal constitution for Organisation Registry. Five core principles derived from established event-sourcing + CQRS architecture, domain-driven design patterns, and proven guardrails.
Modified Principles: None (new document)
Added Sections: Core Principles, Domain Language, Event Sourcing Guarantees, Development Workflow, Governance
Removed Sections: None
Templates Requiring Updates: ✅ Aligned with established practices (no template changes needed)
Deferred: None
-->

# Organisation Registry Constitution

## Core Principles

### I. Event Sourcing is Immutable Law

**Every state change is stored as an immutable event in SQL Server. Events are NEVER modified or deleted. They are the source of truth.**

- Events are named in Dutch, past tense: `OrganisationCreated`, `OrganisationBuildingAdded`
- Aggregates are reconstituted from their event stream
- State is derived, never stored directly
- Events inherit from `BaseEvent<T>`

**Rationale**: Event sourcing is not optional—it is the system's foundational pattern. Immutability guarantees auditability, debuggability, and eventual consistency. Append-only semantics protect historical integrity.

### II. Commands and Events Drive All State Changes

**Commands represent intent; events represent facts. This separation is non-negotiable.**

- Commands are imperative: `AddOrganisationBuilding`, `UpdateOrganisationContact`
- Commands live in `src/OrganisationRegistry/<Aggregate>/Commands/`
- Commands are sent via `ICommandSender` from controllers
- Events are named in past tense and live in `src/OrganisationRegistry/<Aggregate>/Events/`
- Command handlers process intent and emit events; aggregates apply events to update state
- All validation occurs at the InternalRequest level using FluentValidation

**Rationale**: Separating command processing from event persistence ensures a clear audit trail and enables safe concurrent handling. This pattern enforces command idempotency and reliable event replay.

### III. CQRS: Commands and Queries are Architecturally Separate

**Write and read paths must never intersect. Commands mutate the event store; queries read from projections.**

- **Command side**: HTTP Request → Controller → ICommandSender → Command Handler → Aggregate → Event Store
- **Query side**: Events → Projections → Read models (ElasticSearch for search, SQL Server for detail views)
- Projections are updated asynchronously as events are processed
- ElasticSearch projections handle search functionality
- SQL Server projections handle detail views and reporting
- Delegations projections handle authorization-related data

**Rationale**: CQRS enables independent scaling of read and write paths, supports complex queries without burdening the command model, and decouples business logic from query optimization.

### IV. Respect Aggregate Boundaries

**Each aggregate is self-contained. Do not expose internal state; only expose behavior through commands.**

- Private fields use `_camelCase` convention
- Aggregates use partial classes to organize related functionality
- Value objects wrap primitives and encode domain concepts
- Commands operate at the aggregate level; business rules live in aggregates, not controllers
- Cross-aggregate queries happen only via projections, never by traversing aggregate references

**Rationale**: Clear boundaries prevent unintended side effects, enable parallel development, and make event replay and testing tractable. Enforcing privacy guards against technical debt.

### V. Testing is Not Optional—It is a Guardrail

**Unit tests validate domain logic; integration tests validate end-to-end flows. Both MUST be present for new functionality.**

- Unit tests use `AggregateSource.Testing` framework for event-based assertions
- Unit tests verify that commands emit the correct events and enforce business rules
- Integration tests use the test harness from `OrganisationRegistry.Tests.Shared`
- Integration tests validate request/response cycles, database state, and authorization
- Before implementing new domain functionality, write the failing test first

**Rationale**: Tests are guardrails against regression. Event-based tests are the only reliable way to validate aggregates. Integration tests catch deployment-time surprises.

## Domain Language

This project uses Dutch domain terms exclusively. Do not translate them to English. These are non-negotiable:

- **Organisatie**: Organisation — the central aggregate
- **OVO-nummer**: Unique identifier format `OVO` + 6 digits (e.g., OVO000123)
- **KBO**: Kruispuntbank van Ondernemingen — federal business registry, authoritative source for legal entities
- **Wegwijs**: Navigation/directory functionality
- **Beheer**: Administration/management
- **Orgaan**: Body — governance structure within organisations

## Event Sourcing Guarantees

- Never modify or delete events, even if a bug is discovered. Create a compensating event instead.
- Never skip migrations that have run in production. Create a new migration for fixes.
- Events are the single source of truth. The database schema (projections, read models) can be dropped and recreated from events.
- All projections and read models are disposable—they can be rebuilt from the event stream.

## Code Conventions

- Private fields: `_camelCase`
- Partial classes organize aggregate functionality by subdomain
- Commits follow conventional commits: `fix: OR-1234 allow X in Y` (no Co-Authored-By tags)
- Value objects encode domain concepts; primitives are wrapped
- Central package versions in `paket.dependencies`
- Classes use nullable reference types (enabled globally)

## Development Workflow

When adding new domain functionality, follow this sequence:

1. Define the command in `Commands/`
2. Define the event(s) in `Events/`
3. Add the aggregate method (or extend existing aggregate)
4. Add the command handler
5. Add the controller endpoint (CommandController with `[OrganisationRegistryAuthorize]` if restricted)
6. Add projections if a new read model is needed
7. Add unit tests (using `AggregateSource.Testing`)
8. Add integration tests
9. Make frequent, small commits

Make small, focused commits early and often. This enables efficient code review and safe rollback.

## External Integrations & Synchronization

- **KBO**: Organisations with legal personality are synchronized from KBO via dedicated mutation handlers and scheduled jobs
- **MAGDA**: Government data exchange platform integration points in API
- **Wegwijs**: Directory services use organisation data from projections
- **VlaanderenBe**: Public website notifications triggered by organisation events

All external integrations MUST be resilient to eventual consistency. Never assume projections are up-to-date.

## Architecture Boundaries

- **Core domain** (`OrganisationRegistry/`): Aggregates, events, value objects, command handlers
- **API** (`OrganisationRegistry.Api/`): Controllers, request/response mapping, authorization
- **Persistence** (`OrganisationRegistry.SqlServer/`): EF Core context, migrations, SQL projections
- **Search** (`OrganisationRegistry.ElasticSearch/` + `.Projections/`): ElasticSearch client, search projections
- **Projections** (`OrganisationRegistry.Projections.*/`): Independent projection hosts
- **Infrastructure** (`OrganisationRegistry.Infrastructure/`): Shared utilities, logging, configuration
- **KBO Sync** (`OrganisationRegistry.KboMutations/`): KBO mutation handling and synchronization

Each module has a single responsibility. Do not bypass these boundaries.

## Governance

### Principle Precedence

This constitution supersedes all other development practices and conventions. If a practice conflicts with a core principle, the constitution governs.

### Amendment Procedure

Amendments to this constitution require:
1. Explicit documentation of the principle being changed or added
2. Clear rationale explaining why the change improves the system
3. Identification of any dependent artifacts (templates, guides, code patterns) that must be updated
4. Version bump following semantic versioning rules:
   - **MAJOR**: Backward incompatible principle removals or redefinitions (rare)
   - **MINOR**: New principles or materially expanded guidance
   - **PATCH**: Clarifications, wording refinements, non-semantic improvements

### Compliance Review

- All pull requests must respect the core principles
- Code reviewers MUST flag violations of immutability, CQRS separation, or aggregate boundaries
- Complex architectural decisions must be justified against the constitution
- Quarterly reviews validate that the constitution remains aligned with system evolution

### Runtime Guidance

For day-to-day implementation questions, refer to `AGENTS.md` for architecture details and established patterns. When in doubt, ask: "Does this violate a core principle?"

---

**Version**: 0.1.0 | **Ratified**: 2026-03-17 | **Last Amended**: 2026-03-17
