# Feature Specification: Internal/Screen API OpenAPI Documentation

**Feature Branch**: `001-frontend-api-docs`  
**Created**: 2026-03-17  
**Status**: Draft  
**Input**: User description: "write docs for the 'front-end' api's"

## Clarifications

### Session 2026-03-17

- Q: Feature scope conflict — Out-of-scope excluded Swagger tooling, but requirement is Swagger docs. How to resolve? → A: Scope shifts to creating/maintaining OpenAPI specs for internal APIs. External public API docs already exist; this feature documents the internal/screen APIs used by Angular SPA.
- Q: Which OpenAPI version for internal APIs? → A: Match the version already used for existing public API docs (consistency).
- Q: Which internal/screen APIs to document? → A: All internal/screen APIs used by Angular SPA.
- Q: How to generate/maintain OpenAPI spec? → A: Auto-generate from C# code annotations (Swashbuckle), matching the approach used for existing public API docs.
- Q: Where to host Swagger UI? → A: Keep current approach (match how public API docs are currently hosted).

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Angular SPA Developer Discovers Internal API Endpoints (Priority: P1)

As an Angular SPA front-end developer, I need to find and understand what internal/screen API endpoints are available, what parameters they accept, and what responses they return—so that I can integrate with the Organisation Registry backend APIs without guessing or trial-and-error.

**Why this priority**: The internal APIs are used almost exclusively by the Angular SPA. Without clear interactive documentation, developers waste time debugging or reading backend code to understand endpoints.

**Independent Test**: Can be fully tested by providing a front-end developer with only the Swagger UI and asking them to: (1) identify all available internal API endpoints, (2) understand what each endpoint does, (3) see example request/response payloads, (4) test an endpoint interactively. If they can answer these without backend code inspection, the documentation is sufficient.

**Acceptance Scenarios**:

1. **Given** a front-end developer accessing the Swagger UI, **When** they browse internal APIs, **Then** they can identify all available endpoints and their purposes
2. **Given** an endpoint in the Swagger UI, **When** they expand it, **Then** they see required and optional parameters with types and descriptions
3. **Given** the Swagger UI endpoint documentation, **When** they click "Try it out", **Then** they can test the endpoint with example data and see the response

---

### User Story 2 - API Consumer Authenticates and Authorizes Requests (Priority: P1)

As a front-end developer, I need clear documentation on how to authenticate my API requests and what authorization rules apply—so that I can correctly construct requests that will be accepted by the API.

**Why this priority**: Authentication/authorization is a common failure point. Without clear docs, developers spend time debugging "401 Unauthorized" errors instead of building features. This directly impacts time-to-productivity.

**Independent Test**: Can be fully tested by having a developer read only the documentation (not the code) and then: (1) successfully construct an authenticated request, (2) identify which endpoints require which permissions, (3) explain what happens when they lack required permissions.

**Acceptance Scenarios**:

1. **Given** authentication requirements, **When** a developer reads the docs, **Then** they understand the authentication method (API keys, OAuth2, JWT, session cookies, etc.)
2. **Given** the authentication mechanism, **When** they follow the documented steps, **Then** they can successfully include credentials in a request
3. **Given** an endpoint that requires special roles, **When** they read the docs, **Then** they understand which roles are required and how to verify their role

---

### User Story 3 - API Consumer Handles Errors and Edge Cases (Priority: P1)

As a front-end developer, I need to understand what error responses are possible, what error codes mean, and how to handle them gracefully—so that my application can recover from failures instead of crashing or showing confusing messages.

**Why this priority**: Error handling is critical for production applications. Undocumented error cases lead to poor user experience and debugging nightmares. This is essential for reliability.

**Independent Test**: Can be fully tested by having a developer read the error documentation and then: (1) list all possible error codes, (2) explain what each error code means, (3) describe the recommended recovery action for each, (4) identify which errors are user-recoverable vs. fatal.

**Acceptance Scenarios**:

1. **Given** an API endpoint, **When** a developer reads the docs, **Then** they see documented error codes and their meanings
2. **Given** a specific error code (e.g., 404, 409, 422), **When** they look it up in the docs, **Then** they understand why it occurred and what to do about it
3. **Given** rate limiting or quota errors, **When** they read the docs, **Then** they understand retry strategies and backoff policies

---

### User Story 4 - API Consumer Integrates Search and Filtering Capabilities (Priority: P2)

As a front-end developer building a search interface, I need documentation on how to construct complex queries, filter results, sort results, and paginate through large result sets—so that I can provide users with powerful search and navigation capabilities.

**Why this priority**: Search is essential for usability of the registry, but the query syntax and filtering options are complex. Documentation prevents developers from re-implementing or guessing at correct query construction.

**Independent Test**: Can be fully tested by having a developer: (1) construct a multi-filter search query from documentation alone, (2) explain pagination parameters and behavior, (3) identify available sort fields and sort orders, (4) understand search result ranking or relevance behavior if applicable.

**Acceptance Scenarios**:

1. **Given** a search API endpoint, **When** they read the docs, **Then** they understand the supported query syntax and filter operators
2. **Given** the need to search by multiple criteria, **When** they consult the docs, **Then** they can construct a compound query
3. **Given** the need to paginate results, **When** they read the docs, **Then** they understand limit/offset or cursor-based pagination

---

### User Story 5 - API Consumer Understands Data Models and Relationships (Priority: P2)

As a front-end developer, I need clear documentation of the data entities returned by the API, their attributes, and how they relate to each other—so that I can properly parse responses and display information correctly.

**Why this priority**: Understanding data structure is important for correct frontend implementation, but is often inferred from code inspection rather than documented. This is P2 because some developers can infer structure from examples, but clear documentation accelerates understanding.

**Independent Test**: Can be fully tested by having a developer: (1) identify all fields in a response object, (2) explain the type and format of each field, (3) understand relationships between entities (e.g., Organisatie → Organen), (4) identify which fields are always present vs. conditional.

**Acceptance Scenarios**:

1. **Given** a response payload, **When** they read the entity documentation, **Then** they understand all fields in the response
2. **Given** a nested object, **When** they read the docs, **Then** they understand its purpose and how it relates to the parent
3. **Given** optional or conditional fields, **When** they read the docs, **Then** they understand when those fields are present

---

### User Story 6 - API Consumer Monitors and Debugs API Usage (Priority: P3)

As a front-end developer, I need documentation on request/response headers, rate limits, quotas, and how to inspect API calls for debugging—so that I can monitor my application's API usage and troubleshoot integration issues.

**Why this priority**: Debugging and monitoring are important for production support, but not critical for initial integration. This is P3 because developers can often infer this information from browser DevTools or API response headers.

**Independent Test**: Can be fully tested by having a developer: (1) identify rate limit headers and understand their meaning, (2) construct a request with appropriate headers, (3) explain how to interpret API response headers for debugging.

**Acceptance Scenarios**:

1. **Given** an API response, **When** they read the docs, **Then** they understand the meaning of common response headers
2. **Given** rate limiting in effect, **When** they read the docs, **Then** they understand retry-after and rate-limit-reset headers
3. **Given** a debugging scenario, **When** they consult the docs, **Then** they know what information to capture for support requests

---

### Edge Cases

- What happens when the API documentation is outdated compared to the actual API behavior? (Versioning strategy needed)
- How should breaking API changes be communicated and documented?
- How should deprecated endpoints be documented and retired?
- What happens when a developer is not authenticated or lacks permissions for a particular endpoint?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: OpenAPI spec MUST be auto-generated from C# Swashbuckle annotations in `src/OrganisationRegistry.Api/Backoffice` controllers
- **FR-002**: OpenAPI spec version MUST match the version used for existing public API documentation (for consistency)
- **FR-003**: ALL internal/screen API endpoints in the Backoffice controllers MUST be included in the generated spec
- **FR-004**: Each endpoint MUST include: HTTP method, URL path, description of purpose, required/optional parameters with types
- **FR-005**: Each endpoint MUST include example request payloads (for POST/PUT/PATCH) with realistic example values
- **FR-006**: Each endpoint MUST include example response payloads showing success responses (200, 201) and relevant error responses
- **FR-007**: Each endpoint MUST document all possible HTTP status codes it can return (200, 201, 400, 401, 403, 404, 422, 500, etc.)
- **FR-008**: Error responses MUST include error code/type and explanation of the error condition
- **FR-009**: Authentication requirements MUST be documented for each endpoint (OAuth2, JWT, session cookie, or API key)
- **FR-010**: Authorization requirements MUST be documented (required roles, permissions, or scopes) for each endpoint
- **FR-011**: Response data models MUST document all properties with type, format, and whether field is required/optional
- **FR-012**: Related entities (Organisatie, OVO-nummer, Orgaan, contacts, delegations) MUST be documented with relationships
- **FR-013**: Query parameters for search/filter operations MUST document supported operators (equals, contains, range, date formats, etc.)
- **FR-014**: Pagination parameters MUST be documented if applicable (limit, offset, sort order, page size limits)
- **FR-015**: Rate limits and quota headers MUST be documented if applicable
- **FR-016**: HTTP headers (Content-Type, Accept, Authorization, custom headers) MUST be documented
- **FR-017**: Swagger UI MUST allow interactive "Try It Out" testing of all endpoints (with proper authentication context)
- **FR-018**: OpenAPI spec MUST be regenerated automatically as part of build process to ensure accuracy
- **FR-019**: Dutch domain terminology (Organisatie, OVO-nummer, KBO, Orgaan, etc.) MUST be used consistently in spec descriptions

### Key Entities

- **Organisatie (Organisation)**: Central entity representing a Flemish public organisation, identified by OVO-nummer. Attributes include name, status, address, contact details, and relationships to external systems (KBO).
- **OVO-nummer**: Unique identifier for organisations, format: OVO + 6 digits (e.g., OVO000123). Required for all organisation queries and used as primary key in URLs.
- **Orgaan (Body)**: Governance structure within an organisation, representing decision-making bodies or roles. Relates to Organisatie with many-to-one relationship.
- **KBO**: External federal business registry from which legal entities are synchronized. Documentation must explain which organisation data comes from KBO and synchronization frequency.
- **Authentication Credentials**: API keys, OAuth2 tokens, or session identifiers used to authenticate requests.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: All internal/screen API endpoints in Backoffice controllers are reflected in generated OpenAPI spec (100% coverage)
- **SC-002**: OpenAPI spec is regenerated automatically on each build; no manual maintenance required
- **SC-003**: Swagger UI displays all endpoints with interactive "Try It Out" functionality; developers can test endpoints without custom tools
- **SC-004**: A new front-end developer can understand an internal API endpoint by reading the Swagger UI; can test it interactively without asking backend team
- **SC-005**: All response data models are fully documented with field types and descriptions; no undocumented or unexplained fields in examples
- **SC-006**: Error responses are documented with HTTP status codes and error code explanations; developers can understand failure scenarios
- **SC-007**: Authentication/authorization requirements are clear for each endpoint; developers know which credentials/roles are needed
- **SC-008**: OpenAPI spec version and generation timestamp are visible (for debugging/auditing)

## Assumptions

- OpenAPI documentation is already generated for the public APIs; this feature extends that for internal/screen APIs
- Swashbuckle/Swagger is already configured in the build pipeline for public APIs; same tooling will be used for internal APIs
- C# attributes/annotations already in use in the Backoffice controllers can be enhanced with Swagger metadata
- Internal APIs use the same authentication mechanisms as public APIs (OAuth2, JWT, etc.)
- Rate limits and quotas exist for internal APIs and should be documented
- The Angular SPA runs in the same environment as the Backoffice API and can access Swagger UI via the same hostname/port
- Swagger UI hosting approach will match current public API documentation (no changes to infrastructure needed)

## Out of Scope

- Modifying the internal API endpoints themselves; this feature is documentation only
- Creating or changing API behaviors; documenting existing behavior
- Updating existing public API documentation (already complete)
- Creating client libraries or SDKs for specific languages
- Building custom interactive API testing tools beyond what Swagger UI provides
- Performance optimization or API caching strategies
- Documenting the external/public API endpoints (KBO sync, MAGDA integration, Wegwijs); only internal/screen APIs
