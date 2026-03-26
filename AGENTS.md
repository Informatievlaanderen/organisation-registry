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

## Voortgang

- `701a604` feat: subdomain-based routing for k3d demo environment — configureerde *.localhost:9080 subdomains voor alle services (keycloak, api, ui, app), Angular UI deployment, Tiltfile output, session cookie splitting
- `38ece45` chore: add keycloak folder to solution file — keycloak map toegevoegd aan solution
- `d02822c` chore: update configuration files for keycloak demo environment — configuratie aanpassingen voor keycloak demo
- `60314050` chore: update keycloak environment variables in docker-compose — environment variabelen bijgewerkt
- `c2c99d7` fix: seed configuration and keycloak realm for token exchange demo — seed en realm configuratie gefixed
- `ca0a452` feat: nuxt bff demo with rfc 8693 token exchange — Nuxt BFF demo met token exchange functionaliteit
- `f0bc98f` chore: add kubernetes manifests and tiltfile for k3d development environment — k8s manifests en Tiltfile toegevoegd
- `c11ae37` fix: keycloak cookies not being set on http — KC_COOKIE_SECURE uitgeschakeld voor development
- `a190825` refactor: move api/ and .env.example out of demo/ to repo root — api/ en .env.example verplaatst naar repo root
- `6ba1d92` feat: implement RFC 8693 token exchange in nuxt BFF via Keycloak 26 — RFC 8693 token exchange geïmplementeerd
- `9503c58` feat: us3 nuxt bff demo, authorization code pkce flow — Nuxt BFF met PKCE flow en allowed/forbidden API calls
- `31d6488` fix: us2 m2m demo, switch to orafinClient — M2M demo gefixed met orafinClient en keycloak urls
- `e4e9332` feat: implement US2 M2M demo — .NET 6 console met orafinClient client_credentials flow
- `79a646f` feat: migrate seed to .NET 6 console app — seed gemigreerd naar demos/seed/
- `f5bd597` feat: seed — restore pymssql DB config, add Vlimpers test org, add M2M demo project skeleton
- `82874e7` fix: seed script — Accept JSON header, juiste claims in JWT, idempotente GET-check
- `f70ffca` feat: add local dev seed container for parameter types — seed container voor parameter types
- `478df1d` fix: configure API and UI for Keycloak local dev — API en UI geconfigureerd voor lokale Keycloak
- `08836974` feat: add Keycloak demo infrastructure and realm config — Keycloak demo infrastructuur toegevoegd
- `8a19059` docs: add speckit spec for 003-keycloak-demo — speckit spec en keycloak service in docker-compose