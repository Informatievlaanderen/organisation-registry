# Research Phase: External Token Authentication

**Date**: April 24, 2026  
**Feature**: External Token Authentication  
**Purpose**: Resolve technical unknowns and establish implementation approach

## Authentication Scheme Architecture

### Decision: Dedicated TokenExchange Authentication Scheme
**Rationale**: Create separate authentication scheme to maintain clean separation from existing JWT Bearer and M2M EditApi schemes while enabling distinct configuration and middleware handling.

**Alternatives Considered**: 
- Extending JWT Bearer scheme to handle introspection tokens
- Reusing EditApi scheme with content-based differentiation
- Hybrid approach with shared introspection infrastructure

**Why Chosen**: Dedicated scheme provides clearest separation of concerns, independent configuration, and maintains backward compatibility with existing M2M workflows.

## Content-Based Authorization Strategy

### Decision: Claims-Based Authorization with vo_id Differentiation
**Rationale**: Move from scheme-based to content-based authorization using token claims, with `vo_id` presence distinguishing user tokens from M2M tokens.

**Implementation Approach**:
- Token exchange users: Contains `vo_id` + equivalent claims to JWT Bearer (`iv_wegwijs_rol_3D`, roles, organizations)
- M2M clients: No `vo_id`, scope-based authorization with wellknown users
- Authorization middleware examines token content, not authentication scheme

**Alternatives Considered**:
- Maintaining scheme-based authorization with complex routing
- User type claims (`human` vs `machine`)
- Client ID-based differentiation

**Why Chosen**: Leverages existing claim structure, enables JWT Bearer authorization logic reuse, and provides clear differentiation based on existing patterns.

## Introspection Caching Strategy

### Decision: Configurable Short-TTL Caching (1-5 minutes default)
**Rationale**: Balance between security (quick revocation detection) and performance (reduced provider load).

**Cache Implementation**:
- Per-provider configurable TTL in `TokenExchangeConfigurationSection`
- In-memory cache with sliding expiration
- Cache key: token hash + provider identifier
- Cache invalidation: TTL expiry + manual cache clear API

**Security Considerations**:
- Short TTL limits exposure window for revoked tokens
- Cache miss on revoked tokens ensures eventual consistency
- Configurable TTL allows security-performance trade-off per provider

**Performance Benefits**:
- 80-95% reduction in introspection calls
- Improved response times for repeated requests
- Reduced load on third-party introspection endpoints

## OAuth2 Introspection Integration

### Decision: RFC 7662 Token Introspection with ASP.NET Core Integration
**Rationale**: Leverage existing `IdentityModel.AspNetCore.OAuth2Introspection` library used for EditApi M2M authentication.

**Integration Pattern**:
- Separate `IConfigureOptions<OAuth2IntrospectionOptions>` for TokenExchange scheme
- Custom introspection handler for vo_id detection and claims mapping
- Reuse existing secure HTTP client infrastructure

**Configuration Structure**:
```csharp
public class TokenExchangeConfigurationSection
{
    public string Authority { get; set; }
    public string IntrospectionEndpoint { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public TimeSpan CacheTtl { get; set; } = TimeSpan.FromMinutes(2);
}
```

## Testing Strategy (Test-First Approach)

### Decision: Comprehensive Test Coverage with Test-First Development
**Rationale**: User explicitly requested test-first implementation. Follow TDD principles with red-green-refactor cycles.

**Test Pyramid Structure**:

**Unit Tests** (Fast, Isolated):
- Authentication scheme configuration and options
- Content-based authorization logic
- Introspection response parsing and claims mapping
- Cache behavior and TTL enforcement
- Error handling and edge cases

**Integration Tests** (Realistic, End-to-End):
- Token introspection workflows with mock providers
- Authorization equivalence between JWT Bearer and TokenExchange
- M2M compatibility verification (existing EditApi unchanged)
- Configuration changes and hot-reload scenarios

**Contract Tests** (External Interface Validation):
- OAuth2 introspection endpoint contracts
- Token format validation and claims structure
- Provider response format compatibility

**Test Execution Order**:
1. Write failing unit tests for core authentication logic
2. Write failing integration tests for end-to-end scenarios  
3. Implement minimum code to make tests pass
4. Refactor and expand test coverage
5. Add contract tests for external dependencies

## Performance and Scalability Considerations

### Decision: Async-First with Circuit Breaker Pattern
**Rationale**: Introspection calls are I/O bound and can fail; implement resilience patterns.

**Resilience Strategy**:
- HttpClient with timeout configuration (2s default)
- Circuit breaker for introspection endpoint failures
- Fallback to cache-only mode during provider outages
- Retry policy with exponential backoff

**Monitoring and Observability**:
- Structured logging for authentication events
- Metrics for introspection performance and cache hit rates
- Health checks for introspection endpoint availability
- Correlation IDs for request tracing

## Security Implementation

### Decision: Defense-in-Depth Security Model
**Rationale**: Authentication changes require careful security validation to prevent privilege escalation.

**Security Measures**:
- TLS-only introspection communication
- Token hash-based cache keys (no token storage)
- Rate limiting on introspection endpoints
- Input validation on all token claims
- Audit logging for authorization decisions

**Security Testing**:
- Penetration testing scenarios
- Token injection attacks
- Cache poisoning attempts
- Privilege escalation tests
- Timing attack mitigation verification

## Migration and Deployment Strategy

### Decision: Gradual Rollout with Feature Flags
**Rationale**: Minimize risk when introducing new authentication path to production system.

**Deployment Phases**:
1. Deploy with TokenExchange scheme disabled by default
2. Enable for specific test users/organizations
3. Monitor performance and security metrics
4. Gradually expand to broader user base
5. eventual deprecation of self-minted tokens (future phase)

**Rollback Plan**:
- Configuration-based scheme enable/disable
- Independent deployment of authentication components
- Preserve existing JWT Bearer as primary fallback

## Technology Integration Points

### ASP.NET Core Authentication Pipeline Integration
- Custom `IAuthenticationSchemeProvider` registration
- `IClaimsTransformation` for vo_id-based claim mapping  
- `IAuthorizationPolicyProvider` for content-based policies
- Middleware registration order: Authentication → Claims Transformation → Authorization

### Entity Framework Integration
- No direct EF changes required (authentication is stateless)
- Leverage existing `SecurityService` for organization-based authorization
- Audit tables for authentication events (existing infrastructure)

### Configuration System Integration
- `IOptions<TokenExchangeConfigurationSection>` with validation
- Configuration binding with development/staging/production overrides
- Sensitive configuration (client secrets) via Azure Key Vault/environment variables