# CRITICAL DEPLOYMENT MIGRATION REQUIRED

## OAuth Token Endpoint Configuration Fix

**BREAKING CHANGE**: The OAuth token endpoint configuration has been corrected to use the proper OpenID Connect standard endpoint format.

### Required Changes for ALL Environments

#### 1. Database Migration (CRITICAL!)
**MIGRATION**: `20260424160808_RemoveOIDCTokenEndPointFromDatabase`

```bash
# Apply the migration to production database
dotnet ef database update --context OrganisationRegistryContext
```

**What it does**: Removes the hardcoded `/v1/token` endpoint from database configuration, allowing environment variables to take precedence.

#### 2. Environment Variable Change  
**Variable**: `OIDCAuth__TokenEndPoint`

**BEFORE** (incorrect, hardcoded in database):
```
Database: /v1/token  ❌ WRONG ENDPOINT
```

**AFTER** (correct, relative path):
```
OIDCAuth__TokenEndPoint="/protocol/openid-connect/token"
```

**IMPORTANT**: Use relative path only! The system concatenates Authority + TokenEndPoint.

#### Example for Different Environments

**All Environments** (use relative path):
```bash
OIDCAuth__TokenEndPoint="/protocol/openid-connect/token"
```

**Authority URLs** (separate configuration):
- **Development/Staging**: `OIDCAuth__Authority="http://keycloak:80/realms/wegwijs"`
- **Production**: `OIDCAuth__Authority="https://auth.example.com/realms/wegwijs"`

### Configuration Priority Order
1. **Environment Variables** (highest priority after migration)
2. appsettings.json files
3. ~~Database Configuration~~ (removed by migration)

### Files That Need Updates

1. **RUN MIGRATION**: `20260424160808_RemoveOIDCTokenEndPointFromDatabase`
2. **Kubernetes/Docker environment variables**
3. **appsettings.production.json** (if TokenEndPoint is configured there)
4. **Any deployment scripts or Helm charts**
5. **Environment-specific configuration files**

### What This Fixes

- **UI Authentication Flow**: Fixes OAuth code exchange failures
- **Token Exchange**: Resolves "Not Found" errors on `/v1/security/exchange`
- **OpenID Connect Compliance**: Uses standard OAuth2 endpoint format

### Deployment Impact

- **REQUIRES**: Migration + Application restart after configuration change
- **BACKWARDS COMPATIBLE**: Migration includes rollback capability
- **BREAKING**: Old hardcoded `/v1/token` configuration will fail

### Testing Required After Deployment

1. UI login flow should complete successfully
2. OAuth code exchange should return valid tokens
3. No "Not Found" errors in logs for token endpoint calls

---

**Status**: Applied to development environment (migration + config) ✅  
**Next**: Apply migration + config to staging and production ❗  
