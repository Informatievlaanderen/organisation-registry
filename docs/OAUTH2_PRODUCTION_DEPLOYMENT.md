# OAuth2 Production Deployment Guide
## Organisation Registry

**Document Versie**: 1.0  
**Datum**: 24 april 2026  
**Status**: Production Ready

---

## 📋 Deployment Checklist

### Pre-Deployment Verification
- [ ] OAuth2 implementation gecommit en getest (commits b12594bfa, dc29c0cc4, c723e3d0a)
- [ ] Unit tests passing (12/12 OAuth2 introspection tests)
- [ ] Security tests passing (105/105 tests)
- [ ] Build verification succesvol
- [ ] Integration test framework ready

### Keycloak Configuration
- [ ] Production realm geconfigureerd
- [ ] Client configurations bijgewerkt
- [ ] Client secrets gegenereerd (uniek per client)
- [ ] Redirect URIs geconfigureerd voor productie domains
- [ ] HTTPS endpoints geconfigureerd

### API Configuration
- [ ] `appsettings.production.json` bijgewerkt
- [ ] Environment variables ingesteld
- [ ] Database connection strings bijgewerkt
- [ ] OAuth2 endpoints geconfigureerd

### Security Verification
- [ ] Client secrets uniek en sterk
- [ ] HTTPS verplicht voor alle endpoints
- [ ] CORS configuratie restrictief (geen wildcards)
- [ ] Certificate validatie actief

---

## 🔧 Keycloak Configuration

### 1. Realm Configuration

Ensure the production Keycloak realm contains the following configuration:

```json
{
  "realm": "wegwijs",
  "enabled": true,
  "sslRequired": "external",
  "tokenLifespan": 3600,
  "accessTokenLifespan": 3600,
  "accessTokenLifespanForImplicitFlow": 900,
  "ssoSessionIdleTimeout": 1800,
  "ssoSessionMaxLifespan": 36000,
  "offlineSessionIdleTimeout": 2592000
}
```

### 2. Client: organisation-registry-ui (SPA)

```json
{
  "clientId": "organisation-registry-ui",
  "name": "Organisation Registry - Angular UI",
  "description": "Angular UI for Organisation Registry backoffice",
  "enabled": true,
  "publicClient": true,
  "clientSecret": null,
  "standardFlowEnabled": true,
  "implicitFlowEnabled": false,
  "directAccessGrantsEnabled": false,
  "serviceAccountsEnabled": false,
  "authorizationServicesEnabled": false,
  "redirectUris": [
    "https://organisatie.vlaanderen.be/#/oic",
    "https://organisatie.vlaanderen.be/"
  ],
  "webOrigins": [
    "https://organisatie.vlaanderen.be"
  ],
  "defaultScopes": [
    "openid",
    "profile",
    "vo",
    "iv_wegwijs"
  ],
  "consentRequired": false,
  "fullScopeAllowed": true
}
```

### 3. Client: organisation-registry-api (M2M)

```json
{
  "clientId": "organisation-registry-api",
  "name": "Organisation Registry - API",
  "description": "API introspection client for Organisation Registry",
  "enabled": true,
  "publicClient": false,
  "clientSecret": "GENERATED-STRONG-SECRET-HERE",
  "standardFlowEnabled": false,
  "implicitFlowEnabled": false,
  "directAccessGrantsEnabled": true,
  "serviceAccountsEnabled": true,
  "authorizationServicesEnabled": false,
  "serviceAccountClientId": "organisation-registry-api",
  "defaultScopes": [
    "dv_organisatieregister_introspection"
  ]
}
```

### 4. Client: organisation-registry-cjm (M2M)

```json
{
  "clientId": "organisation-registry-cjm",
  "name": "Organisation Registry - CJM Client",
  "description": "CJM client for Organisation Registry API access",
  "enabled": true,
  "publicClient": false,
  "clientSecret": "GENERATED-CJM-SECRET-HERE",
  "standardFlowEnabled": false,
  "directAccessGrantsEnabled": true,
  "serviceAccountsEnabled": true,
  "defaultScopes": [
    "dv_organisatieregister_cjmbeheerder"
  ]
}
```

---

## ⚙️ API Configuration

### 1. Production appsettings.json

```json
{
  "EditApi": {
    "ClientId": "organisation-registry-api",
    "ClientSecret": "PRODUCTION-API-INTROSPECTION-SECRET",
    "Authority": "https://authenticatie.vlaanderen.be/realms/wegwijs",
    "IntrospectionEndpoint": "https://authenticatie.vlaanderen.be/realms/wegwijs/protocol/openid-connect/token/introspect"
  },
  "OIDCAuth": {
    "Authority": "https://authenticatie.vlaanderen.be/realms/wegwijs",
    "TokenEndPoint": "/v1/token",
    "AuthorizationEndpoint": "https://authenticatie.vlaanderen.be/realms/wegwijs/protocol/openid-connect/auth",
    "UserInfoEndPoint": "https://authenticatie.vlaanderen.be/realms/wegwijs/protocol/openid-connect/userinfo",
    "EndSessionEndPoint": "https://authenticatie.vlaanderen.be/realms/wegwijs/protocol/openid-connect/logout",
    "AuthorizationIssuer": "https://authenticatie.vlaanderen.be/realms/wegwijs",
    "JwksUri": "https://authenticatie.vlaanderen.be/realms/wegwijs/protocol/openid-connect/certs",
    "AuthorizationRedirectUri": "https://organisatie.vlaanderen.be/#/oic",
    "PostLogoutRedirectUri": "https://organisatie.vlaanderen.be",
    "ClientId": "organisation-registry-ui",
    "ClientSecret": null,
    "JwtSharedSigningKey": "PRODUCTION-JWT-SIGNING-KEY-32-BYTES-MINIMUM",
    "JwtIssuer": "https://api.organisation-registry.vlaanderen.be",
    "JwtAudience": "organisation-registry-api",
    "JwtExpiresInMinutes": 120,
    "Developers": "admin@vlaanderen.be"
  }
}
```

### 2. Environment Variables

```bash
# Required environment variables for production
EDITAPI__CLIENTSECRET=PRODUCTION-API-INTROSPECTION-SECRET
OIDCAUTH__CLIENTSECRET=  # Empty for public clients
OIDCAUTH__JWTSHAREDSIGNINGKEY=PRODUCTION-JWT-SIGNING-KEY-32-BYTES-MINIMUM

# Database connection
INFRASTRUCTURE__EVENTSTORECONNECTIONSTRING="Server=prod-sql;Database=OrganisationRegistry;Integrated Security=true;TrustServerCertificate=false;"
```

---

## 🔐 Security Requirements

### 1. Client Secrets

**Generation Requirements**:
- Minimum 32 characters
- Cryptographically random
- Unique per client
- Stored securely (Azure Key Vault, Kubernetes Secrets, etc.)

**Example Generation**:
```bash
# Generate strong client secret
openssl rand -base64 48
```

### 2. JWT Signing Keys

**Requirements**:
- Minimum 256 bits (32 bytes)
- Cryptographically secure random
- Rotated regularly (e.g., quarterly)

### 3. HTTPS Configuration

**Required for Production**:
- All Keycloak endpoints must use HTTPS
- Valid CA-signed certificates
- TLS 1.2 minimum
- HSTS headers enabled

### 4. CORS Configuration

**Restrictive CORS Policy**:
```json
{
  "Cors": [
    "https://organisatie.vlaanderen.be",
    "https://api.organisation-registry.vlaanderen.be"
  ]
}
```

**NO wildcards in production** (`*` not allowed)

---

## 🚀 Deployment Steps

### 1. Pre-Deployment Preparation

1. **Update Keycloak Configuration**:
   ```bash
   # Export current realm
   kcadm export --realm wegwijs --file current-realm.json
   
   # Update realm with new client configurations
   kcadm import --realm wegwijs --file updated-realm-export.json
   ```

2. **Generate Production Secrets**:
   ```bash
   # API client secret
   API_SECRET=$(openssl rand -base64 48)
   
   # JWT signing key
   JWT_KEY=$(openssl rand -base64 32)
   ```

3. **Update Configuration**:
   - Deploy new `appsettings.production.json`
   - Set environment variables with secrets
   - Update connection strings

### 2. Deployment Process

1. **Deploy API Changes**:
   ```bash
   # Deploy new API version with OAuth2 support
   kubectl apply -f api-deployment.yaml
   ```

2. **Update Database Migrations**:
   ```bash
   # Apply any new migrations
   dotnet ef database update --context OrganisationRegistryContext
   ```

3. **Deploy Frontend Changes**:
   ```bash
   # Deploy Angular UI with OAuth2 fixes
   kubectl apply -f ui-deployment.yaml
   ```

### 3. Post-Deployment Verification

1. **Health Check**:
   ```bash
   curl https://api.organisation-registry.vlaanderen.be/health
   ```

2. **OAuth2 Endpoint Check**:
   ```bash
   # Should return 405 (Method Not Allowed) for GET
   curl https://authenticatie.vlaanderen.be/realms/wegwijs/protocol/openid-connect/token/introspect
   ```

3. **Authentication Flow Test**:
   - Login via UI
   - Verify token exchange works
   - Test API access with OAuth2 tokens

---

## 📊 Monitoring & Troubleshooting

### 1. Key Metrics

Monitor these metrics in production:

- **OAuth2 Introspection Response Time**: Target < 500ms
- **Token Cache Hit Rate**: Target > 80%
- **Authentication Success Rate**: Target > 99%
- **Failed Login Attempts**: Alert on spikes

### 2. Common Issues

**Issue**: "Invalid format of the code" errors
- **Cause**: OAuth2 authorization codes not URL encoded
- **Solution**: Verify `encodeURIComponent()` in Angular UI

**Issue**: Unauthorized responses for valid tokens  
- **Cause**: Client secret mismatch or missing scopes
- **Solution**: Verify client configuration and token scopes

**Issue**: Public client authentication failures
- **Cause**: `clientSecret` provided for public client
- **Solution**: Ensure `"clientSecret": null` for SPA clients

### 3. Log Analysis

**Important Log Entries to Monitor**:
```
# Successful OAuth2 introspection
"OAuth2 token introspection successful for client {clientId}"

# Failed introspection
"OAuth2 token introspection failed: {error}"

# Cache performance
"OAuth2 introspection cache hit for token {tokenHash}"
```

### 4. Performance Optimization

**Caching Strategy**:
- Token introspection responses cached for 300 seconds (default)
- Cache key based on token hash
- Distributed cache for multi-instance deployments

**Recommended Cache Configuration**:
```json
{
  "Authentication": {
    "Schemes": {
      "EditApi": {
        "CacheOptions": {
          "AbsoluteExpirationRelativeToNow": "00:05:00",
          "SlidingExpiration": "00:02:00"
        }
      }
    }
  }
}
```

---

## 🔄 Rollback Plan

### Emergency Rollback

If OAuth2 implementation causes issues:

1. **Immediate**: Revert to previous API version
   ```bash
   kubectl rollout undo deployment/api
   ```

2. **Configuration**: Restore previous appsettings.json
   ```bash
   kubectl create configmap api-config --from-file=appsettings.production.json.backup
   ```

3. **Database**: No rollback needed (backward compatible)

4. **Keycloak**: Revert to backup realm configuration
   ```bash
   kcadm import --realm wegwijs --file realm-backup.json
   ```

### Verification After Rollback

- [ ] API health check passes
- [ ] Authentication flows work
- [ ] No authentication errors in logs
- [ ] Frontend access restored

---

## 📞 Support & Documentation

### Technical Documentation

- **Security Overview**: `/docs/SECURITY_OVERVIEW.md`
- **API Documentation**: Swagger UI at `/swagger`
- **Code Examples**: `/test/OrganisationRegistry.Api.IntegrationTests/Security/`

### Support Contacts

- **Technical Implementation**: Development Team
- **Keycloak Configuration**: Infrastructure Team  
- **Production Issues**: Operations Team

### Additional Resources

- **OAuth2 RFC 7662**: https://tools.ietf.org/html/rfc7662
- **ASP.NET Core OAuth2**: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/oauth
- **Keycloak Documentation**: https://www.keycloak.org/documentation

---

**Document END**