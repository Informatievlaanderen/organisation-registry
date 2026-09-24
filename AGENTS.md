# Organisation Registry (Organisatieregister)

# Part 1 — Domain

## What This Is

The Organisation Registry for Digitaal Vlaanderen (Flemish government). It manages
organisations (organisaties) in Flanders — primarily public organisations, integrated
with external systems like KBO (federal business registry).

The system is the authoritative source (basisregister) for organisation data in Flanders.

## Domain Language

This project uses Dutch domain terms. Do not translate them to English.

- **Organisatie**: Organisation — the central aggregate
- **OVO-nummer**: Unique identifier, format `OVO` + 6 digits (e.g., OVO000123)
- **KBO**: Kruispuntbank van Ondernemingen — federal business registry, source for legal entities
- **Wegwijs**: Navigation/directory functionality
- **Beheer**: Administration/management
- **Orgaan**: Body — governance structure within organisations

## Domain Model

- Aggregate `Organisatie` in `src/OrganisationRegistry/`
- Aggregates are partial classes, split by related functionality
- Value objects for domain concepts (wrapped primitives)
- TODO: other aggregates (e.g. Orgaan) and their location

## Business Rules

- Events are the source of truth and are append-only
- Organisations with legal personality are sourced from KBO
- Every organisation has an OVO-nummer in the format `OVO` + 6 digits
- Respect aggregate boundaries — don't expose internal state
- TODO: invariants per aggregate

## External Sources & Integrations

- **KBO**: Synchronization of legal entities, through dedicated KBO mutation handlers and scheduled jobs (`OrganisationRegistry.KboMutations`)
- **MAGDA**: Government data exchange platform
- **Wegwijs**: Directory services
- **VlaanderenBe**: Public website notifications
- **ACM/IDM**: Authentication

# Part 2 — Technical

## Architecture

### Event Sourcing (this is the core pattern)

All state changes are stored as immutable events in SQL Server.
Events are NEVER modified or deleted. They are the source of truth.

<!-- CHECK: "named in Dutch" contradicts the English examples. Confirm which is correct. -->
- Events are named in Dutch, past tense: `OrganisationCreated`, `OrganisationBuildingAdded`
- Events inherit from `BaseEvent<T>`
- Aggregates are reconstituted from their event stream
- State is derived, never stored directly

### CQRS

- **Command side**: HTTP Request → Controller → ICommandSender → Command Handler → Aggregate → Events stored in SQL Server
- **Query side**: Events → Projections → Read models (ElasticSearch for search, SQL Server for detail views)

### Hosts

- **Api – Backoffice**: Full management interface for administrators
- **Api – Search**: Public and authenticated search functionality
- **Api – Integration endpoints**: For external systems (MAGDA, Wegwijs, etc.)
- **Projection hosts**: ElasticSearch projections (search), SQL Server projections (detail views and reporting), Delegations projections (authorization-related)
- **UI / Vue**: Admin UI and frontend

Projections are updated asynchronously as events are processed.

## Tech Stack

- .NET 8, C#, nullable reference types enabled
- SQL Server (event store + read models)
- Entity Framework Core with migrations
- ElasticSearch (search projections)
- Be.Vlaanderen.Basisregisters.AggregateSource (aggregate base classes)
- FluentValidation, Serilog, AutoFixture
- NuGet PackageReference package management (versions live in project `PackageReference` entries)
- xUnit

## Project Structure

```
src/
  OrganisationRegistry/                  # Core domain (aggregates, events, value objects)
  OrganisationRegistry.Api/              # Main API (Backoffice, Search, etc.)
  OrganisationRegistry.SqlServer/        # EF Core context, migrations, SQL projections
  OrganisationRegistry.ElasticSearch/    # ElasticSearch client abstractions
  OrganisationRegistry.ElasticSearch.Projections/  # Search projection handlers
  OrganisationRegistry.Projections.*/    # Various projection hosts
  OrganisationRegistry.Infrastructure/   # Shared infrastructure
  OrganisationRegistry.KboMutations/     # KBO synchronization logic
  OrganisationRegistry.UI/               # Admin UI
  OrganisationRegistry.Vue/              # Vue.js frontend
test/
  OrganisationRegistry.UnitTests/        # Core domain unit tests
  OrganisationRegistry.Api.IntegrationTests/  # API integration tests
  OrganisationRegistry.SqlServer.IntegrationTests/  # Database integration tests
  OrganisationRegistry.Tests.Shared/     # Shared test utilities
```

## Build & Test

```bash
# To build
dotnet build

# Run all tests
dotnet test

# Run specific test class
dotnet test test/OrganisationRegistry.UnitTests/OrganisationRegistry.UnitTests.csproj \
  --filter "FullyQualifiedName~Namespace.ClassName"

# Add a migration
cd src/OrganisationRegistry.SqlServer/
dotnet ef migrations add <Name> --context OrganisationRegistryContext --startup-project ../OrganisationRegistry.Api/

# Apply migrations
dotnet ef database update --context OrganisationRegistryContext

# Run UI in development
nvm use
npm install
npm run start:hmr
```

## Patterns

### Commands

- Named in imperative form: `AddOrganisationBuilding`, `UpdateOrganisationContact`
- Live in `src/OrganisationRegistry/<Aggregate>/Commands/`
- Sent via `ICommandSender` from dedicated CommandControllers
- Processed by command handlers in the domain

Example flow:
```
Controller receives AddOrganisationBuildingRequest
  → Maps to AddOrganisationBuilding command
  → Sends via CommandSender.Send()
  → Handler processes and calls aggregate method
  → Aggregate applies events
```

### Events

- Named in past tense: `OrganisationBuildingAdded`, `ContactUpdated`
- Live in `src/OrganisationRegistry/<Aggregate>/Events/`
- Inherit from `BaseEvent<T>`
- Applied to update aggregate state, persisted to the event store, trigger projection updates

### Request/Response

Controllers use a three-part request pattern:
1. `AddOrganisationBuildingRequest` - External API model
2. `AddOrganisationBuildingInternalRequest` - Internal model with route params
3. `AddOrganisationBuilding` - Domain command

Validation happens at the InternalRequest level using FluentValidation.

## Persistence Rules

### Event store

- Events in SQL Server are append-only; never modify or delete them

### Migrations

- EF Core migrations live in `src/OrganisationRegistry.SqlServer/`
- NEVER skip migrations that have run in production

## Code Conventions

- Private fields: `_camelCase`
- Partial classes for aggregates to organize related functionality
- Value objects for domain concepts (wrapped primitives)
- Git commits follow conventional commits: `fix: OR-1234 allow X in Y`
- AI assistants must NOT add attribution or Co-Authored-By tags to commit messages

## Testing

### Unit Tests

- Use the `AggregateSource.Testing` framework for event-based assertions
- Test command handling in isolation
- Verify events are raised with correct data
- Test business rule violations

### Integration Tests

- Use the test harness from `OrganisationRegistry.Tests.Shared`
- Test full request/response cycles
- Verify database state after commands
- Test authorization rules

## Security & Authorization

- Authentication via ACM/IDM
- Use `[OrganisationRegistryAuthorize]` attribute on controllers
- Security policies are defined in `OrganisationRegistry.Api/Security/`

# Part 3 — Working Agreements

## Workflow

Make frequent and small commits following conventional commits format.

When adding new domain functionality:
1. Define the command in `Commands/`
2. Define the event in `Events/`
3. Add the aggregate method
4. Add command handler
5. Add controller endpoint
6. Add projections if needed
7. Add tests

## Guardrails

Shared baseline (identical in OR and VR):
- NEVER modify or delete existing events — event sourcing means append-only
- Do not introduce new packages without discussion
- Commits follow conventional commits; make small, frequent commits
- AI assistants must NOT add attribution or Co-Authored-By tags to commit messages

Repository-specific:
- NEVER skip migrations that have run in production
- Always validate commands before sending to domain
- Respect aggregate boundaries — don't expose internal state

<!-- Tool-managed sections (speckit) below: do not edit by hand -->

## Active Technologies
- C# / .NET 8, nullable reference types enabled + ASP.NET Core, Be.Vlaanderen.Basisregisters.AggregateSource, FluentValidation, Serilog, AutoFixture (009-permission-based-authz)
- SQL Server (event store + read models); ElasticSearch (search projections) — not touched by this feature (009-permission-based-authz)

## Recent Changes
- 009-permission-based-authz: Added C# / .NET 8, nullable reference types enabled + ASP.NET Core, Be.Vlaanderen.Basisregisters.AggregateSource, FluentValidation, Serilog, AutoFixture

<!-- SPECKIT START -->
For additional context about technologies to be used, project structure,
shell commands, and other important information, read the current plan
<!-- SPECKIT END -->
