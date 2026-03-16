# Organisation Registry (Organisatieregister)

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

## Architecture

### Event Sourcing (this is the core pattern)

All state changes are stored as immutable events in SQL Server.
Events are NEVER modified or deleted. They are the source of truth.

- Events are named in Dutch, past tense: `OrganisationCreated`, `OrganisationBuildingAdded`
- Aggregates are reconstituted from their event stream
- State is derived, never stored directly
- Events inherit from `BaseEvent<T>`

### CQRS

Commands (writes) and queries (reads) follow completely separate paths:

- **Command side**: HTTP Request → Controller → ICommandSender → Command Handler → Aggregate → Events stored in SQL Server
- **Query side**: Events → Projections → Read models (ElasticSearch for search, SQL Server for detail views)

### API Structure

The API provides:
- **Backoffice**: Full management interface for administrators
- **Search**: Public and authenticated search functionality
- **Integration endpoints**: For external systems (MAGDA, Wegwijs, etc.)

All commands go through dedicated CommandControllers that use `ICommandSender`.

### Synchronized data from external sources (KBO Sync)

Organisations with legal personality are synchronized from KBO.
This happens through dedicated KBO mutation handlers and scheduled jobs.

### Projections

The system uses projection handlers to build read models from events:
- **ElasticSearch projections**: For search functionality (`OrganisationRegistry.ElasticSearch.Projections`)
- **SQL Server projections**: For detail views and reporting
- **Delegations projections**: Special authorization-related projections

Projections are updated asynchronously as events are processed.

## Tech Stack

- .NET 6, C#, nullable reference types enabled
- SQL Server (event store + read models)
- Entity Framework Core with migrations
- ElasticSearch (search projections)
- Be.Vlaanderen.Basisregisters.AggregateSource (aggregate base classes)
- FluentValidation, Serilog, AutoFixture
- FAKE build system, Paket package manager
- xUnit for tests

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

## Command and Event Patterns

### Commands

Commands represent intent to change state. They:
- Are named in imperative form: `AddOrganisationBuilding`, `UpdateOrganisationContact`
- Live in `src/OrganisationRegistry/<Aggregate>/Commands/`
- Are sent via `ICommandSender` from controllers
- Are processed by command handlers in the domain

Example flow:
```
Controller receives AddOrganisationBuildingRequest
  → Maps to AddOrganisationBuilding command
  → Sends via CommandSender.Send()
  → Handler processes and calls aggregate method
  → Aggregate applies events
```

### Events

Events represent facts that happened. They:
- Are named in past tense: `OrganisationBuildingAdded`, `ContactUpdated`
- Live in `src/OrganisationRegistry/<Aggregate>/Events/`
- Inherit from `BaseEvent<T>`
- Are applied to update aggregate state
- Are persisted to the event store
- Trigger projection updates

### Request/Response Pattern

Controllers use a three-part request pattern:
1. `AddOrganisationBuildingRequest` - External API model
2. `AddOrganisationBuildingInternalRequest` - Internal model with route params
3. `AddOrganisationBuilding` - Domain command

Validation happens at the InternalRequest level using FluentValidation.

## Code Conventions

- Private fields: `_camelCase`
- Partial classes for aggregates to organize related functionality
- git commits follow conventional commits: `fix: OR-1234 allow X in Y`
- Value objects for domain concepts (wrapped primitives)
- Central package versions in `paket.dependencies`
- AI assistants must NOT add attribution or Co-Authored-By tags to commit messages

## Guardrails

- NEVER modify or delete existing events — event sourcing means append-only
- NEVER skip migrations that have run in production
- Do not introduce new NuGet packages without discussion
- Always validate commands before sending to domain
- Respect aggregate boundaries — don't expose internal state

## Testing Patterns

### Unit Tests

Domain unit tests should:
- Use the `AggregateSource.Testing` framework for event-based assertions
- Test command handling in isolation
- Verify events are raised with correct data
- Test business rule violations

### Integration Tests

API integration tests should:
- Use the test harness from `OrganisationRegistry.Tests.Shared`
- Test full request/response cycles
- Verify database state after commands
- Test authorization rules

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

## Security & Authorization

The system integrates with ACM/IDM for authentication.
Use `[OrganisationRegistryAuthorize]` attribute on controllers.
Security policies are defined in `OrganisationRegistry.Api/Security/`.

## External Integrations

- **KBO**: Synchronization of legal entities
- **MAGDA**: Government data exchange platform
- **Wegwijs**: Directory services
- **VlaanderenBe**: Public website notifications