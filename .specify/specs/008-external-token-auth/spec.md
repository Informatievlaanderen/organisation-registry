# Feature Specification: External Token Authentication

**Feature Branch**: `008-external-token-auth`  
**Created**: April 24, 2026  
**Status**: Draft  
**Input**: User description: "we want a new authorization workflow where we allow a 3rd party to auth token exchange with us (they do token exchange, we introspect the token) on the admin api. The users authenticating through this scheme, should have the same access to resources as the ones going through the default authentication scheme (which uses a selfminted token)."

## Clarifications

### Session 2026-04-24

- Q: How should authorization decisions change from current scheme-based approach to support token exchange users (introspection → same rights as JWT Bearer) and M2M users (introspection → different ruleset)? → A: Move from scheme-based to token content-based authorization (claims, scopes, user type indicators)
- Q: How will token exchange users be identified differently from M2M clients in the introspected token content? → A: jwtbearer and token exchange will always have a vo_id
- Q: How should token exchange users with vo_id be mapped to equivalent JWT Bearer permissions? → A: Map vo_id to user claims (roles, organizations) equivalent to JWT Bearer token for same user - users will get iv_wegwijs_rol_3D and other claims, similar to the ones jwtbearer ones get when exchangecode is happening
- Q: How should token exchange introspection be configured relative to existing M2M introspection configuration? → A: Create separate configuration section for token exchange introspection (similar to EditApiConfigurationSection)
- Q: How should token exchange authentication scheme be configured relative to existing JWT Bearer and EditApi schemes? → A: Create new authentication scheme (e.g., "TokenExchange") with dedicated introspection configuration
- Q: What caching strategy should be used for token introspection to balance security and performance? → A: Short-term cache (1-5 minutes) with configurable TTL per provider

## User Scenarios & Testing

### User Story 1 - Third-Party Authentication Access (Priority: P1)

Administrators authenticated through external identity providers can access the admin API with the same permissions as those using the current self-minted token system. The external provider handles token exchange, while the Organisation Registry validates tokens through introspection.

**Why this priority**: This is the core functionality - enabling equivalent access through external authentication is the primary goal of this feature.

**Independent Test**: Can be fully tested by configuring a third-party provider, obtaining a token through their exchange, and verifying admin API access delivers the same resources and permissions as the current authentication method.

**Acceptance Scenarios**:

1. **Given** a user has valid credentials with an approved third-party provider, **When** they obtain a token through the provider's token exchange, **Then** they can access admin API endpoints with the same scope as self-minted token users
2. **Given** an external token is presented to the admin API, **When** the system performs token introspection, **Then** the user receives appropriate authorization based on their validated identity
3. **Given** a user with external authentication, **When** they attempt to access organization management functions, **Then** they have identical permissions to users authenticated with self-minted tokens

---

### User Story 2 - Token Introspection Validation (Priority: P2)

The system validates external tokens by performing introspection requests to the third-party provider, ensuring tokens are active, valid, and contain necessary user information for authorization decisions.

**Why this priority**: Token validation is essential for security but secondary to basic access functionality.

**Independent Test**: Can be tested by presenting various token states (valid, expired, revoked) and verifying appropriate access grants or denials.

**Acceptance Scenarios**:

1. **Given** a valid external token, **When** the system performs introspection, **Then** the user gains access with correct permissions
2. **Given** an expired or revoked token, **When** introspection is performed, **Then** access is denied with appropriate error messaging
3. **Given** token introspection fails due to provider unavailability, **When** a user attempts access, **Then** the system handles the failure gracefully with appropriate fallback behavior

---

### User Story 3 - Administrative Configuration (Priority: P3)

System administrators can configure third-party provider settings, introspection endpoints, and authentication policies for external token validation.

**Why this priority**: Configuration capabilities enable the feature but are not needed for basic operation with a single provider.

**Independent Test**: Can be tested by configuring provider settings and verifying correct token validation behavior.

**Acceptance Scenarios**:

1. **Given** administrator access to authentication settings, **When** they configure a third-party provider, **Then** the system correctly validates tokens from that provider
2. **Given** multiple provider configurations, **When** tokens from different providers are presented, **Then** each is validated against the appropriate configuration
3. **Given** configuration changes to provider settings, **When** new tokens are presented, **Then** validation uses the updated configuration

---

### Edge Cases

- What happens when token introspection endpoint becomes unavailable during authentication attempts?
- How does the system handle tokens with insufficient scope or missing required claims?
- What occurs when external provider token format changes or becomes incompatible?
- How are concurrent authentication attempts handled during provider configuration updates?
- How does the system handle token revocation scenarios when introspection results are cached?
- What happens when cached introspection results conflict with current token status due to revocation or expiration?

## Requirements

### Functional Requirements

- **FR-001**: System MUST accept authentication tokens from configured third-party providers on admin API endpoints
- **FR-002**: System MUST perform token introspection to validate external tokens before granting access
- **FR-003**: Users authenticated via external tokens MUST have identical resource access to those using self-minted tokens
- **FR-004**: System MUST maintain secure communication with third-party introspection endpoints
- **FR-005**: System MUST handle introspection failures gracefully without exposing sensitive information
- **FR-006**: System MUST support OAuth 2.0 Token Introspection (RFC 7662) protocol for external token validation
- **FR-007**: System MUST log authentication and authorization events for external token usage
- **FR-008**: System MUST validate token scope and claims against required admin API permissions
- **FR-009**: System MUST support simultaneous use of both external tokens and existing self-minted tokens without changing current behavior
- **FR-010**: System MUST cache token introspection results with configurable TTL (default 1-5 minutes) to balance security and performance requirements
- **FR-011**: System MUST determine authorization based on token content (claims, scopes, user type indicators) rather than authentication scheme
- **FR-012**: System MUST preserve existing M2M authorization rules while enabling token exchange users to have equivalent rights to JWT Bearer users
- **FR-013**: System MUST identify token exchange users by presence of vo_id claim, distinguishing them from M2M clients which lack this claim
- **FR-014**: System MUST provide token exchange users with equivalent claims to JWT Bearer tokens (iv_wegwijs_rol_3D, roles, organizations) to enable identical authorization logic
- **FR-015**: System MUST use separate configuration section for token exchange introspection settings, independent from existing M2M configuration
- **FR-016**: System MUST implement dedicated authentication scheme for token exchange with its own introspection configuration, separate from JWT Bearer and EditApi schemes
- **FR-017**: System MUST allow per-provider configuration of introspection cache TTL to accommodate different security requirements

### Key Entities

- **External Token**: Represents authentication token issued by third-party provider, containing user identity and scope information
- **Introspection Response**: Contains token validation results including active status, user identity, and authorized scopes
- **Provider Configuration**: Stores third-party provider settings including introspection endpoint, credentials, validation rules, and cache TTL configuration in dedicated configuration section separate from M2M settings
- **TokenExchange Authentication Scheme**: Dedicated authentication scheme for external token validation with separate introspection configuration from existing EditApi M2M scheme
- **Introspection Cache**: Temporary storage for introspection results with configurable TTL per provider to balance security and performance
- **Authentication Context**: Represents validated user session with unified permissions regardless of authentication method
- **Token Claims**: Content-based authorization data including user type indicators, roles, and permissions that determine access rights independent of authentication scheme, with vo_id presence indicating user tokens vs M2M clients, and equivalent claims structure (iv_wegwijs_rol_3D, roles, organizations) for token exchange and JWT Bearer users

## Success Criteria

### Measurable Outcomes

- **SC-001**: External token authentication provides identical admin API access scope as self-minted tokens for equivalent users
- **SC-002**: Token introspection cache reduces provider load by 80-95% while maintaining security through configurable short TTL (1-5 minutes default)
- **SC-003**: System maintains 99.9% authentication success rate for valid external tokens
- **SC-004**: Zero security vulnerabilities introduced through external token validation pathway
- **SC-005**: Authentication failure rates due to external provider issues remain below 1% during normal operation

## Assumptions

- Third-party providers support OAuth 2.0 Token Introspection (RFC 7662) standard
- External tokens contain sufficient identity and authorization information for admin API access decisions equivalent to self-minted tokens
- Network connectivity to third-party introspection endpoints is reliable
- Existing authorization logic can be modified to use token content rather than authentication scheme for authorization decisions
- Configurable introspection caching with short TTL provides acceptable balance between security (quick revocation detection) and performance (reduced provider load)
- Migration from self-minted to external tokens will occur in a future phase after successful implementation
- Current admin API functionality and access patterns remain unchanged during external token integration
- Current M2M token introspection lacks vo_id claim, while token exchange and JWT Bearer tokens contain vo_id for user identification
- Token exchange introspection responses will contain equivalent claims to JWT Bearer tokens (iv_wegwijs_rol_3D, roles, organizations) enabling reuse of existing authorization logic
- New TokenExchange authentication scheme can be implemented alongside existing JWT Bearer and EditApi schemes without conflicts
- Authorization middleware can be refactored from scheme-based to content-based without breaking existing API contracts