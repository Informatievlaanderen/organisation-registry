# OAuth2 Token Introspection Contract

**Version**: 1.0  
**Standard**: RFC 7662 - OAuth 2.0 Token Introspection  
**Purpose**: Contract for third-party token validation via OAuth2 introspection endpoint

## Introspection Request Contract

### HTTP Request Format
```http
POST /oauth2/introspect HTTP/1.1
Host: provider.example.com
Authorization: Basic {base64(client_id:client_secret)}
Content-Type: application/x-www-form-urlencoded

token={access_token}&token_type_hint=access_token
```

### Request Parameters
| Parameter | Required | Type | Description |
|-----------|----------|------|-------------|
| `token` | ✅ Yes | string | The token to introspect |
| `token_type_hint` | ❌ No | string | Hint about token type ("access_token", "refresh_token") |

### Authentication
- **Method**: HTTP Basic Authentication
- **Credentials**: OAuth2 client credentials (client_id:client_secret)
- **Format**: `Authorization: Basic {base64encoded(client_id:client_secret)}`

## Introspection Response Contract

### Success Response (200 OK)
```json
{
  "active": true,
  "scope": "openid profile organisation_admin",
  "client_id": "organisation_registry_client",
  "username": "john.doe@vlaanderen.be", 
  "token_type": "Bearer",
  "exp": 1640995200,
  "iat": 1640908800,
  "sub": "user123",
  "aud": ["organisation_registry"],
  "iss": "https://auth.vlaanderen.be",
  "jti": "token_unique_id",
  "vo_id": "VO12345",
  "iv_wegwijs_rol_3D": "WegwijsBeheerder-algemeenbeheerder:OVO002949",
  "roles": ["AlgemeenBeheerder"],
  "organizations": ["OVO002949", "OVO001234"]
}
```

### Required Claims for User Tokens (TokenExchange)
| Claim | Type | Required | Description |
|-------|------|----------|-------------|
| `active` | boolean | ✅ Yes | Token validity status |
| `vo_id` | string | ✅ Yes | Flemish government user ID (distinguishes from M2M) |
| `iv_wegwijs_rol_3D` | string | ✅ Yes | Role claim in ACM format |
| `scope` | string | ✅ Yes | Space-delimited scope values |
| `sub` | string | ✅ Yes | Subject identifier |
| `exp` | number | ❌ No | Expiration time (Unix timestamp) |
| `iat` | number | ❌ No | Issued at time (Unix timestamp) |
| `client_id` | string | ❌ No | Client identifier |
| `username` | string | ❌ No | Human-readable user identifier |
| `roles` | string[] | ❌ No | Simplified role names |
| `organizations` | string[] | ❌ No | Authorized organization OVO numbers |

### Inactive Token Response (200 OK)
```json
{
  "active": false
}
```

### Error Responses

#### Invalid Client (401 Unauthorized)
```json
{
  "error": "invalid_client",
  "error_description": "Client authentication failed"
}
```

#### Invalid Token Format (400 Bad Request)  
```json
{
  "error": "invalid_request",
  "error_description": "Malformed token parameter"
}
```

#### Server Error (500 Internal Server Error)
```json
{
  "error": "server_error", 
  "error_description": "The authorization server encountered an unexpected condition"
}
```

## Content-Type Requirements
- **Request**: `application/x-www-form-urlencoded`
- **Response**: `application/json`

## Security Requirements

### Transport Security
- **TLS**: HTTPS required (TLS 1.2+)
- **Certificate Validation**: Server certificate must be valid
- **HSTS**: HTTP Strict Transport Security recommended

### Authentication Security  
- **Client Credentials**: Securely stored and transmitted
- **Basic Auth**: Base64 encoding over HTTPS only
- **Token Confidentiality**: Introspection tokens never logged or cached

### Rate Limiting
- **Introspection Endpoint**: Max 1000 requests/minute per client
- **Error Responses**: Standard rate limiting applies
- **Circuit Breaker**: Client implements circuit breaker pattern

## Performance Contract

### Response Time Requirements
- **Target**: < 200ms for cached responses
- **Acceptable**: < 2000ms for live introspection
- **Timeout**: Client timeout at 5000ms

### Availability Requirements
- **SLA**: 99.9% uptime for introspection endpoint
- **Circuit Breaker**: Client failover after 5 consecutive failures
- **Cache Strategy**: Short-term caching (1-5 minutes) acceptable

## Claim Format Specifications

### vo_id Format
- **Pattern**: `^[A-Z]{2}[0-9]{5}$`
- **Example**: `"VO12345"`
- **Purpose**: Distinguishes user tokens from M2M tokens

### iv_wegwijs_rol_3D Format
- **Pattern**: `WegwijsBeheerder-{role}:OVO{number}`  
- **Example**: `"WegwijsBeheerder-algemeenbeheerder:OVO002949"`
- **Mapping**: Used to derive Organization Registry roles

### Organization Format
- **Pattern**: `^OVO[0-9]{6}$`
- **Example**: `"OVO002949"`
- **Purpose**: Organization-scoped authorization

## Test Data Contracts

### Valid Test Token Response
```json
{
  "active": true,
  "scope": "openid profile test_scope",
  "client_id": "test_client",
  "username": "test.user@vlaanderen.be",
  "token_type": "Bearer", 
  "exp": 9999999999,
  "iat": 1640908800,
  "sub": "test_user_123",
  "aud": ["organisation_registry"],
  "iss": "https://test-auth.vlaanderen.be",
  "jti": "test_token_id",
  "vo_id": "VO99999",
  "iv_wegwijs_rol_3D": "WegwijsBeheerder-testbeheerder:OVO999999",
  "roles": ["TestBeheerder"],
  "organizations": ["OVO999999"]
}
```

### Expired Test Token Response
```json
{
  "active": false,
  "exp": 1234567890
}
```

## Compliance and Validation

### RFC 7662 Compliance
- ✅ Standard introspection endpoint format
- ✅ Required `active` boolean claim
- ✅ Optional metadata claims as specified
- ✅ Standard error response format
- ✅ HTTPS transport requirement

### Organization Registry Extensions
- ✅ vo_id claim for user identification
- ✅ iv_wegwijs_rol_3D claim for role mapping
- ✅ Custom roles and organizations arrays
- ✅ Backward compatibility with existing M2M format

### Breaking Changes Policy
- **Major Version**: Changes that affect required claims or response format
- **Minor Version**: Addition of new optional claims
- **Patch Version**: Documentation clarifications only
- **Deprecation**: 6-month notice for any claim removal