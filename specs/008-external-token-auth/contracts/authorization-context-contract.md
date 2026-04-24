# Authorization Context Contract

**Version**: 1.0  
**Purpose**: Unified authorization context interface for all authentication methods in Organisation Registry

## AuthorizationContext Interface

### Core Properties
```csharp
public interface IAuthorizationContext
{
    ClaimsPrincipal User { get; }
    AuthenticationMethod AuthenticationMethod { get; }
    Role[] Roles { get; }
    string[] Organizations { get; }
    Permission[] Permissions { get; }
    bool IsAuthenticated { get; }
    string? VoId { get; }
}
```

### Computed Authorization Methods
```csharp
public interface IAuthorizationContext
{
    bool IsUserAuthentication { get; }      // VoId != null
    bool IsM2MAuthentication { get; }       // VoId == null
    bool CanAccessOrganization(string ovoNumber);
    bool HasRole(Role role);
    bool HasPermission(Permission permission);
    bool HasAnyRole(params Role[] roles);
    bool CanPerformAction(string action, string? organizationId = null);
}
```

## Authentication Method Enumeration
```csharp
public enum AuthenticationMethod
{
    JwtBearer,      // Self-minted JWT tokens with vo_id claim
    TokenExchange,  // OAuth2 introspection for external user tokens 
    EditApi         // OAuth2 introspection for M2M client tokens
}
```

## Authorization Equivalence Contract

### Same VoId = Same Permissions
```csharp
// REQUIREMENT: These contexts must have identical authorization behavior
var jwtContext = CreateJwtBearerContext(voId: "VO12345", roles: ["AlgemeenBeheerder"]);
var tokenExchangeContext = CreateTokenExchangeContext(voId: "VO12345", roles: ["AlgemeenBeheerder"]);

Assert.Equal(jwtContext.Permissions, tokenExchangeContext.Permissions);
Assert.Equal(jwtContext.Organizations, tokenExchangeContext.Organizations);
Assert.Equal(jwtContext.CanAccessOrganization("OVO002949"), tokenExchangeContext.CanAccessOrganization("OVO002949"));
```

### M2M Context Preservation
```csharp
// REQUIREMENT: EditApi context behavior must not change
var editApiContext = CreateEditApiContext(clientId: "orafin", scopes: ["read", "write"]);

// Existing M2M authorization logic must remain unchanged
Assert.True(editApiContext.IsM2MAuthentication);
Assert.Null(editApiContext.VoId);
Assert.Contains(Permission.EditOrganisations, editApiContext.Permissions);
```

## Claims Mapping Contract

### JWT Bearer Claims → Authorization Context
```csharp
public class JwtBearerClaimsMapping 
{
    public AuthorizationContext Map(ClaimsPrincipal principal)
    {
        return new AuthorizationContext
        {
            User = principal,
            AuthenticationMethod = AuthenticationMethod.JwtBearer,
            VoId = principal.FindFirst("vo_id")?.Value,
            Roles = ParseRoles(principal.FindAll("role")),
            Organizations = ParseOrganizations(principal.FindAll("organisation")),
            IsAuthenticated = principal.Identity?.IsAuthenticated ?? false
        };
    }
}
```

### Token Exchange Claims → Authorization Context  
```csharp
public class TokenExchangeClaimsMapping
{
    public AuthorizationContext Map(IntrospectionResponse introspection)
    {
        return new AuthorizationContext
        {
            User = CreateClaimsPrincipal(introspection),
            AuthenticationMethod = AuthenticationMethod.TokenExchange,
            VoId = introspection.VoId, // Required for user tokens
            Roles = MapWegwijsRoles(introspection.AdditionalClaims["iv_wegwijs_rol_3D"]),
            Organizations = introspection.AdditionalClaims.GetValueOrDefault("organizations", []),
            IsAuthenticated = introspection.Active
        };
    }
}
```

### EditApi Claims → Authorization Context
```csharp
public class EditApiClaimsMapping
{
    public AuthorizationContext Map(IntrospectionResponse introspection)
    {
        return new AuthorizationContext
        {
            User = CreateClaimsPrincipal(introspection),
            AuthenticationMethod = AuthenticationMethod.EditApi,
            VoId = null, // M2M clients have no VoId
            Roles = MapClientRoles(introspection.ClientId),
            Organizations = [], // M2M clients have no org restrictions
            IsAuthenticated = introspection.Active
        };
    }
}
```

## Role Mapping Contract

### Wegwijs Role Format → Organization Registry Roles
```csharp
public static readonly Dictionary<string, Role> WegwijsRoleMapping = new()
{
    ["algemeenbeheerder"] = Role.AlgemeenBeheerder,
    ["orafininvoerder"] = Role.OrafinInvoerder,
    ["cjminvoerder"] = Role.CjmInvoerder,
    ["ontwikkelaar"] = Role.Ontwikkelaar,
    ["organisatiebeheerder"] = Role.OrganisatieBeheerder
    // Additional mappings as needed
};

public Role[] MapWegwijsRoles(string iv_wegwijs_rol_3D)
{
    // Parse: "WegwijsBeheerder-{role}:OVO{number}"
    var match = Regex.Match(iv_wegwijs_rol_3D, @"WegwijsBeheerder-(?<role>\w+):OVO(?<org>\d+)");
    if (!match.Success) return [];
    
    var roleKey = match.Groups["role"].Value;
    return WegwijsRoleMapping.TryGetValue(roleKey, out var role) ? [role] : [];
}
```

## Permission Derivation Contract

### Role → Permission Mapping
```csharp
public Permission[] DerivePermissions(Role[] roles, AuthenticationMethod method)
{
    var permissions = new List<Permission>();
    
    foreach (var role in roles)
    {
        permissions.AddRange(role switch
        {
            Role.AlgemeenBeheerder => [
                Permission.ViewOrganisations,
                Permission.EditOrganisations,
                Permission.ManageUsers,
                Permission.ViewReports
            ],
            Role.OrganisatieBeheerder => [
                Permission.ViewOrganisations,
                Permission.EditOwnOrganisation
            ],
            Role.OrafinInvoerder => [
                Permission.ViewOrganisations,
                Permission.ImportOrafinData
            ],
            _ => []
        });
    }
    
    // Method-specific permission adjustments
    if (method == AuthenticationMethod.EditApi)
    {
        // M2M clients get full API access regardless of roles
        permissions.AddRange([Permission.EditOrganisations, Permission.ViewOrganisations]);
    }
    
    return permissions.Distinct().ToArray();
}
```

## Organization Access Contract

### Organization-Scoped Authorization
```csharp
public bool CanAccessOrganization(string ovoNumber)
{
    // M2M clients can access all organizations
    if (IsM2MAuthentication) return true;
    
    // AlgemeenBeheerder can access all organizations
    if (HasRole(Role.AlgemeenBeheerder)) return true;
    
    // Other roles are restricted to their authorized organizations
    return Organizations.Contains(ovoNumber);
}
```

## Security Policy Integration

### SecurityService Interface
```csharp
public interface ISecurityService  
{
    Task<SecurityInformation> GetSecurityInformationAsync(ClaimsPrincipal principal);
    IAuthorizationContext CreateAuthorizationContext(ClaimsPrincipal principal);
    Task<bool> IsAuthorizedAsync(IAuthorizationContext context, string action, string? resource = null);
}
```

### Policy Evaluation Contract
```csharp
public class SecurityPolicy
{
    public AuthorizationResult Evaluate(IAuthorizationContext context, string action, string? resource)
    {
        // Standard policy evaluation regardless of authentication method
        if (!context.IsAuthenticated) 
            return AuthorizationResult.Failed("Authentication required");
            
        if (context.IsM2MAuthentication)
            return EvaluateM2MPolicy(context, action, resource);
            
        return EvaluateUserPolicy(context, action, resource);
    }
}
```

## Backward Compatibility Contract

### Existing API Behavior Preservation
```csharp
[Test]
public void SecurityService_WithTokenExchange_PreservesExistingBehavior()
{
    // GIVEN: Existing JWT Bearer token with specific claims
    var jwtPrincipal = CreateJwtBearerPrincipal("VO12345", ["AlgemeenBeheerder"]);
    var jwtSecurity = securityService.GetSecurityInformation(jwtPrincipal);
    
    // AND: TokenExchange token with equivalent claims
    var tokenExchangePrincipal = CreateTokenExchangePrincipal("VO12345", ["AlgemeenBeheerder"]);
    var tokenExchangeSecurity = securityService.GetSecurityInformation(tokenExchangePrincipal);
    
    // THEN: Security information must be identical
    Assert.Equal(jwtSecurity.User, tokenExchangeSecurity.User);
    Assert.Equal(jwtSecurity.Roles, tokenExchangeSecurity.Roles);
    Assert.Equal(jwtSecurity.Organizations, tokenExchangeSecurity.Organizations);
}
```

## Test Verification Requirements

### Authorization Context Factory Tests
```csharp
[Theory]
[InlineData(AuthenticationMethod.JwtBearer, "VO12345", true)]
[InlineData(AuthenticationMethod.TokenExchange, "VO12345", true)]
[InlineData(AuthenticationMethod.EditApi, null, true)]
public void CreateAuthorizationContext_WithValidPrincipal_ReturnsCorrectContext(
    AuthenticationMethod method, string? expectedVoId, bool expectedAuthenticated)
{
    // Test context creation for all authentication methods
}
```

### Role Mapping Tests
```csharp
[Theory]
[InlineData("WegwijsBeheerder-algemeenbeheerder:OVO002949", Role.AlgemeenBeheerder)]
[InlineData("WegwijsBeheerder-organisatiebeheerder:OVO002949", Role.OrganisatieBeheerder)]
public void MapWegwijsRoles_WithValidFormat_ReturnsCorrectRole(string roleClaimValue, Role expectedRole)
{
    // Test iv_wegwijs_rol_3D claim parsing
}
```

### Permission Derivation Tests
```csharp
[Fact]
public void DerivePermissions_WithAlgemeenBeheerder_IncludesAllPermissions()
{
    var permissions = DerivePermissions([Role.AlgemeenBeheerder], AuthenticationMethod.TokenExchange);
    
    Assert.Contains(Permission.ViewOrganisations, permissions);
    Assert.Contains(Permission.EditOrganisations, permissions);
    Assert.Contains(Permission.ManageUsers, permissions);
}
```