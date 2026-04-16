# Feature Specification: Move Development Dependencies from Docker Compose to Tilt

**Feature Branch**: `004-deps-docker-to-tilt`  
**Created**: 2026-04-16  
**Status**: Draft  
**Input**: User description: "move all development dependencies from docker compose to tilt"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Start Full Dev Environment with One Command (Priority: P1)

A developer clones the repository and wants to spin up the full local development environment — including all infrastructure dependencies (SQL Server, Elasticsearch, Keycloak, WireMock, etc.) — using a single tool: `tilt up`. They no longer need to remember which services to start via `docker compose` separately.

**Why this priority**: This is the core goal of the feature. Without it, developers must juggle two tools and manually coordinate startup order.

**Independent Test**: A developer can run `tilt up` from the repo root and have all services reachable and healthy — without running any `docker compose` command at all.

**Acceptance Scenarios**:

1. **Given** a fresh clone with only a k3d cluster running, **When** the developer runs `tilt up`, **Then** all infrastructure services (database, search engine, identity provider, mock server, telemetry) start and become healthy within the Tilt UI.
2. **Given** Tilt is running, **When** a developer opens the Tilt UI, **Then** all previously docker-compose-managed services appear as named resources with their status visible.
3. **Given** Tilt is running, **When** a developer stops Tilt, **Then** all services are torn down cleanly without leftover containers or volumes requiring manual cleanup.

---

### User Story 2 - No Dependency on Docker Compose for Local Dev (Priority: P2)

After this change, developers no longer need to run `docker compose up` as a prerequisite or companion step. The `docker-compose.yml` either no longer exists or is no longer required for the standard local development workflow.

**Why this priority**: Eliminates workflow confusion and reduces the number of tools a developer must learn and maintain. A single entrypoint (Tilt) simplifies onboarding.

**Independent Test**: Remove or rename `docker-compose.yml` and verify `tilt up` still produces a fully functional environment.

**Acceptance Scenarios**:

1. **Given** docker compose is not running, **When** a developer runs `tilt up`, **Then** all services start correctly through Tilt alone.
2. **Given** the updated setup, **When** a new developer follows the README, **Then** they are not instructed to run any `docker compose` command for the standard dev workflow.

---

### User Story 3 - Hot Reload and Dependency Awareness Preserved (Priority: P3)

Services managed by Tilt respond to file changes the same way they did before — configuration changes (e.g., realm export, seed data) trigger the appropriate service restart or reconfiguration automatically.

**Why this priority**: Developer experience quality. Without this, migrating to Tilt would be a regression in workflow speed.

**Independent Test**: Modify `keycloak/realm-export.json` and verify Keycloak reconfigures without a manual restart command.

**Acceptance Scenarios**:

1. **Given** Tilt is running, **When** a configuration file watched by a service changes, **Then** Tilt detects the change and triggers the appropriate reload without manual intervention.
2. **Given** a service has declared dependencies in Tilt, **When** a dependency is not yet healthy, **Then** the dependent service waits before starting.

---

### Edge Cases

- What happens when a port required by a migrated service is already in use on the host?
- How does Tilt handle services that previously relied on docker compose networking (service-name DNS resolution)?
- What happens if the k3d cluster is not running when `tilt up` is executed — does it fail fast with a clear message?
- Services that previously used docker compose volumes: are persistent data volumes preserved across `tilt down` / `tilt up` cycles?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: All services currently defined in `docker-compose.yml` for local development MUST be available as Tilt-managed resources.
- **FR-002**: The Tilt setup MUST preserve startup order and health-check dependencies between services (e.g., API waits for database to be healthy).
- **FR-003**: Developers MUST be able to reach all services at the same host/port addresses as before (no change to local URLs or port numbers).
- **FR-004**: The `docker-compose.yml` MUST no longer be required to run the local development environment.
- **FR-005**: File-watch triggers currently handled via docker compose (e.g., config reloads) MUST be replicated in Tilt using `deps` or equivalent mechanisms.
- **FR-006**: The developer setup documentation (README or `dev-setup.sh`) MUST be updated to reflect the Tilt-only workflow.
- **FR-007**: Services MUST remain accessible via their existing subdomain routes (e.g., `keycloak.localhost:9080`, `ui.localhost:9080`) after migration.
- **FR-008**: Services currently missing from the Tilt/k8s setup (Elasticsearch, WireMock/ACM) MUST have the necessary deployment configuration added so they run under Tilt.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A developer can go from a fresh clone to a fully running local environment using only `tilt up` — with zero `docker compose` commands required.
- **SC-002**: All services that were previously managed by docker compose are visible and healthy in the Tilt UI within the same startup time as the previous docker compose workflow (no regression > 20%).
- **SC-003**: No existing local development URL or port changes — all service endpoints remain identical to before the migration.
- **SC-004**: The `docker-compose.yml` file can be removed or archived without breaking the standard development workflow.
- **SC-005**: A developer unfamiliar with the previous setup can follow the updated documentation and reach a working environment without consulting additional resources.

## Assumptions

- The target runtime for all migrated services is the existing k3d (local Kubernetes) cluster — not Docker Compose networking.
- The `docker-compose.yml` may be retained as a reference or for CI purposes, but must not be required for local development.
