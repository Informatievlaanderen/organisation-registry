# Testing Contract

**Version**: 1.0  
**Purpose**: Testing interfaces, patterns, and requirements for external token authentication implementation

## Test-First Development Contract

### TDD Red-Green-Refactor Cycle
```csharp
// RED: Write failing test first
[Fact]
public async Task TokenIntrospectionService_WithValidToken_ReturnsActiveResponse()
{
    // This test MUST fail initially - no implementation exists yet
    var service = new TokenIntrospectionService(mockHttpClient, mockConfig);
    var result = await service.IntrospectTokenAsync("valid_token");
    
    Assert.True(result.Active);
    Assert.Equal("VO12345", result.VoId);
}

// GREEN: Implement minimal code to make test pass
public async Task<IntrospectionResponse> IntrospectTokenAsync(string token)
{
    // Minimal implementation that makes the test pass
    if (token == "valid_token")
        return new IntrospectionResponse { Active = true, VoId = "VO12345" };
    throw new NotImplementedException();
}

// REFACTOR: Improve implementation while keeping tests green
```

### Test Pyramid Structure
```
          /\
         /  \
        /    \    Contract Tests (5-10 tests)
       /      \   - OAuth2 introspection endpoint contracts
      /        \  - Authorization equivalence contracts  
     /          \
    /____________\ Integration Tests (20-30 tests)
   /              \ - Authentication scheme end-to-end
  /                \ - SecurityService with real introspection
 /                  \ - Authorization middleware integration
/____________________\
     Unit Tests (100+ tests)
     - Token parsing, claims mapping
     - Cache behavior, configuration validation
     - Error handling, security policies
```

## Unit Testing Contract

### Token Introspection Service Tests
```csharp
public class TokenIntrospectionServiceTests
{
    private readonly Mock<HttpClient> _mockHttpClient;
    private readonly Mock<IOptions<TokenExchangeConfiguration>> _mockConfig;
    private readonly TokenIntrospectionService _service;
    
    [Theory]
    [InlineData("valid_token", true, "VO12345")]
    [InlineData("expired_token", false, null)]
    [InlineData("invalid_format", false, null)]
    public async Task IntrospectTokenAsync_WithVariousTokens_ReturnsExpectedResults(
        string token, bool expectedActive, string? expectedVoId)
    {
        // Arrange: Mock HTTP response based on token
        SetupMockHttpResponse(token, expectedActive, expectedVoId);
        
        // Act: Call introspection service
        var result = await _service.IntrospectTokenAsync(token);
        
        // Assert: Verify introspection response
        Assert.Equal(expectedActive, result.Active);
        Assert.Equal(expectedVoId, result.VoId);
    }
    
    [Fact]
    public async Task IntrospectTokenAsync_WithHttpTimeout_ThrowsTimeoutException()
    {
        // Test error handling and resilience
    }
    
    [Fact]
    public async Task IntrospectTokenAsync_WithInvalidCredentials_ThrowsAuthenticationException()
    {
        // Test authentication error handling
    }
}
```

### Claims Transformation Tests
```csharp
public class TokenExchangeClaimsTransformerTests
{
    [Theory]
    [InlineData("WegwijsBeheerder-algemeenbeheerder:OVO002949", Role.AlgemeenBeheerder, "OVO002949")]
    [InlineData("WegwijsBeheerder-organisatiebeheerder:OVO001234", Role.OrganisatieBeheerder, "OVO001234")]
    [InlineData("WegwijsBeheerder-orafininvoerder:OVO005678", Role.OrafinInvoerder, "OVO005678")]
    public void TransformClaims_WithValidRoleFormat_MapsCorrectly(
        string roleClaimValue, Role expectedRole, string expectedOrganization)
    {
        // Test iv_wegwijs_rol_3D claim parsing to Organization Registry roles
        var introspectionResponse = CreateIntrospectionResponse(roleClaimValue);
        var principal = transformer.TransformClaims(introspectionResponse);
        
        Assert.Contains(expectedRole.ToString(), principal.FindAll("role").Select(c => c.Value));
        Assert.Contains(expectedOrganization, principal.FindAll("organisation").Select(c => c.Value));
    }
    
    [Theory]
    [InlineData("InvalidFormat")]
    [InlineData("WegwijsBeheerder-unknownrole:OVO123456")]
    [InlineData("")]
    public void TransformClaims_WithInvalidRoleFormat_HandlesGracefully(string invalidRoleClaimValue)
    {
        // Test error handling for malformed role claims
        var introspectionResponse = CreateIntrospectionResponse(invalidRoleClaimValue);
        var principal = transformer.TransformClaims(introspectionResponse);
        
        Assert.Empty(principal.FindAll("role"));
        // Should not throw exceptions for invalid input
    }
}
```

### Cache Behavior Tests
```csharp
public class IntrospectionCacheTests
{
    [Fact]
    public async Task GetCachedResponseAsync_WithValidCacheEntry_ReturnsFromCache()
    {
        // Arrange: Store valid cache entry
        var tokenHash = "hash123";
        var cachedResponse = new IntrospectionResponse { Active = true, VoId = "VO12345" };
        await cache.SetAsync(tokenHash, cachedResponse, TimeSpan.FromMinutes(2));
        
        // Act: Retrieve from cache
        var result = await cache.GetCachedResponseAsync(tokenHash);
        
        // Assert: Returns cached response
        Assert.Equal(cachedResponse.VoId, result?.VoId);
    }
    
    [Fact]
    public async Task GetCachedResponseAsync_WithExpiredEntry_ReturnsNull()
    {
        // Test cache expiry behavior
    }
    
    [Fact]
    public async Task InvalidateAsync_WithProviderId_ClearsProviderCache()
    {
        // Test cache invalidation
    }
}
```

## Integration Testing Contract

### Authentication Middleware Integration Tests
```csharp
[Collection("ApiIntegrationTests")]
public class TokenExchangeAuthenticationIntegrationTests : IClassFixture<ApiFixture>
{
    [Fact]
    public async Task Request_WithValidTokenExchangeToken_AuthenticatesSuccessfully()
    {
        // Arrange: Mock introspection endpoint to return valid response
        var mockIntrospectionResponse = new IntrospectionResponse
        {
            Active = true,
            VoId = "VO12345",
            AdditionalClaims = new Dictionary<string, object>
            {
                ["iv_wegwijs_rol_3D"] = "WegwijsBeheerder-algemeenbeheerder:OVO002949"
            }
        };
        
        MockIntrospectionEndpoint("valid_token", mockIntrospectionResponse);
        
        // Act: Make authenticated request
        var client = await fixture.CreateTokenExchangeClientAsync("valid_token");
        var response = await client.GetAsync("/api/organisations");
        
        // Assert: Request succeeds with proper authentication
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        // Verify authentication context was created correctly
        var authContext = GetAuthenticationContextFromRequest();
        Assert.Equal(AuthenticationMethod.TokenExchange, authContext.AuthenticationMethod);
        Assert.Equal("VO12345", authContext.VoId);
        Assert.Contains(Role.AlgemeenBeheerder, authContext.Roles);
    }
    
    [Fact]
    public async Task Request_WithInactiveToken_Returns401Unauthorized()
    {
        // Test authentication failure scenarios
    }
    
    [Fact]
    public async Task Request_WithMalformedToken_Returns401Unauthorized()
    {
        // Test malformed token handling
    }
}
```

### SecurityService Integration Tests
```csharp
public class SecurityServiceTokenExchangeIntegrationTests
{
    [Fact]
    public async Task GetSecurityInformation_WithTokenExchangePrincipal_EquivalentToJwtBearer()
    {
        // CRITICAL TEST: Verify authorization equivalence
        
        // Arrange: Create equivalent JWT Bearer and TokenExchange principals
        var jwtPrincipal = CreateJwtBearerPrincipal("VO12345", ["AlgemeenBeheerder"]);
        var tokenExchangePrincipal = CreateTokenExchangePrincipal("VO12345", ["AlgemeenBeheerder"]);
        
        // Act: Get security information for both
        var jwtSecurity = await securityService.GetSecurityInformationAsync(jwtPrincipal);
        var tokenExchangeSecurity = await securityService.GetSecurityInformationAsync(tokenExchangePrincipal);
        
        // Assert: Security information must be identical
        Assert.Equal(jwtSecurity.User, tokenExchangeSecurity.User);
        Assert.Equal(jwtSecurity.Roles, tokenExchangeSecurity.Roles);
        Assert.Equal(jwtSecurity.Organizations, tokenExchangeSecurity.Organizations);
        Assert.Equal(jwtSecurity.Permissions, tokenExchangeSecurity.Permissions);
    }
}
```

### Authorization Policy Integration Tests
```csharp
public class AuthorizationPolicyTokenExchangeIntegrationTests
{
    [Theory]
    [InlineData(AuthenticationMethod.JwtBearer, "VO12345")]
    [InlineData(AuthenticationMethod.TokenExchange, "VO12345")]
    public async Task AuthorizeAsync_WithSameVoId_ReturnsIdenticalResults(
        AuthenticationMethod authMethod, string voId)
    {
        // Test authorization equivalence across authentication methods
        var context = CreateAuthorizationContext(authMethod, voId, [Role.AlgemeenBeheerder]);
        var result = await authorizationService.AuthorizeAsync(context, "CanEditOrganisations");
        
        Assert.True(result.Succeeded);
    }
}
```

## Contract Testing

### OAuth2 Introspection Endpoint Contract Tests
```csharp
public class OAuth2IntrospectionContractTests
{
    [Fact]
    public async Task IntrospectionEndpoint_WithValidToken_ReturnsRFC7662CompliantResponse()
    {
        // Contract test verifying RFC 7662 compliance
        var response = await introspectionClient.IntrospectAsync("test_token");
        
        // Verify required RFC 7662 fields
        Assert.NotNull(response.Active);
        
        // Verify Organization Registry specific fields
        if (response.Active.Value)
        {
            Assert.NotNull(response.VoId); // Must have vo_id for user tokens
            Assert.NotNull(response.AdditionalClaims["iv_wegwijs_rol_3D"]);
        }
    }
    
    [Fact]
    public async Task IntrospectionEndpoint_WithInvalidCredentials_Returns401()
    {
        // Contract test for authentication failures
    }
    
    [Fact] 
    public async Task IntrospectionEndpoint_ResponseTime_UnderTwoSeconds()
    {
        // Contract test for performance requirements
        var stopwatch = Stopwatch.StartNew();
        await introspectionClient.IntrospectAsync("test_token");
        stopwatch.Stop();
        
        Assert.True(stopwatch.ElapsedMilliseconds < 2000);
    }
}
```

### Authorization Equivalence Contract Tests
```csharp
public class AuthorizationEquivalenceContractTests
{
    [Theory]
    [InlineData("VO12345", new[] { Role.AlgemeenBeheerder })]
    [InlineData("VO98765", new[] { Role.OrganisatieBeheerder })]
    [InlineData("VO55555", new[] { Role.OrafinInvoerder, Role.CjmInvoerder })]
    public async Task Authorization_JwtVsTokenExchange_ProducesIdenticalResults(
        string voId, Role[] roles)
    {
        // CONTRACT: Same VoId + roles = identical permissions regardless of auth method
        
        var jwtContext = CreateJwtBearerContext(voId, roles);
        var tokenExchangeContext = CreateTokenExchangeContext(voId, roles);
        
        // Test multiple authorization scenarios
        var testCases = new[]
        {
            ("CanViewOrganisations", null),
            ("CanEditOrganisations", null), 
            ("CanAccessOrganisation", "OVO002949"),
            ("CanManageUsers", null)
        };
        
        foreach (var (permission, resource) in testCases)
        {
            var jwtResult = await authorizationService.AuthorizeAsync(jwtContext, permission, resource);
            var tokenExchangeResult = await authorizationService.AuthorizeAsync(tokenExchangeContext, permission, resource);
            
            Assert.Equal(jwtResult.Succeeded, tokenExchangeResult.Succeeded,
                $"Authorization mismatch for {permission} with resource {resource}");
        }
    }
}
```

## Test Infrastructure Contract

### Test Fixture Interface
```csharp
public interface ITokenExchangeTestFixture
{
    Task<HttpClient> CreateTokenExchangeClientAsync(string token);
    Task<HttpClient> CreateJwtBearerClientAsync(string voId, Role[] roles);
    Task<HttpClient> CreateEditApiClientAsync(string clientId);
    void MockIntrospectionEndpoint(string token, IntrospectionResponse response);
    void MockIntrospectionError(string token, HttpStatusCode statusCode, string error);
    AuthorizationContext CreateTokenExchangeContext(string voId, Role[] roles);
    AuthorizationContext CreateJwtBearerContext(string voId, Role[] roles);
}
```

### Test Data Factory
```csharp
public static class TestDataFactory
{
    public static IntrospectionResponse CreateValidUserTokenResponse(
        string voId = "VO12345",
        string role = "algemeenbeheerder",
        string organization = "OVO002949")
    {
        return new IntrospectionResponse
        {
            Active = true,
            VoId = voId,
            Subject = $"user_{voId}",
            Scope = "openid profile organisation_admin",
            AdditionalClaims = new Dictionary<string, object>
            {
                ["iv_wegwijs_rol_3D"] = $"WegwijsBeheerder-{role}:{organization}"
            }
        };
    }
    
    public static IntrospectionResponse CreateInactiveTokenResponse()
    {
        return new IntrospectionResponse { Active = false };
    }
    
    public static ClaimsPrincipal CreateTokenExchangePrincipal(string voId, Role[] roles)
    {
        var identity = new ClaimsIdentity("TokenExchange");
        identity.AddClaim(new Claim("vo_id", voId));
        
        foreach (var role in roles)
        {
            identity.AddClaim(new Claim("role", role.ToString()));
        }
        
        return new ClaimsPrincipal(identity);
    }
}
```

## Test Execution Contract

### Continuous Integration Requirements
```yaml
# .github/workflows/test.yml
- name: Run Unit Tests
  run: dotnet test --filter "Category=Unit" --logger trx --results-directory TestResults/

- name: Run Integration Tests  
  run: dotnet test --filter "Category=Integration" --logger trx --results-directory TestResults/
  env:
    INTROSPECTION_ENDPOINT: https://test-auth.example.com/introspect
    INTROSPECTION_CLIENT_SECRET: ${{ secrets.TEST_INTROSPECTION_SECRET }}

- name: Run Contract Tests
  run: dotnet test --filter "Category=Contract" --logger trx --results-directory TestResults/
```

### Test Coverage Requirements
- **Unit Tests**: Minimum 95% code coverage for authentication components
- **Integration Tests**: Cover all authentication schemes and authorization paths
- **Contract Tests**: Verify RFC 7662 compliance and authorization equivalence
- **Performance Tests**: Response time under 2 seconds for introspection calls

### Test Categorization
```csharp
[TestMethod]
[TestCategory("Unit")]
[TestCategory("TokenIntrospection")]
public void TokenIntrospectionService_ParsesClaimsCorrectly() { }

[TestMethod]
[TestCategory("Integration")]
[TestCategory("Authentication")]
public async Task AuthenticationMiddleware_WithTokenExchange_WorksEndToEnd() { }

[TestMethod]
[TestCategory("Contract")]
[TestCategory("Authorization")]
public async Task AuthorizationEquivalence_JwtVsTokenExchange_IdenticalResults() { }
```

## Mock and Stub Contract

### HTTP Mock Configuration
```csharp
public static class IntrospectionMockSetup
{
    public static void ConfigureMockIntrospection(this Mock<HttpMessageHandler> mockHandler)
    {
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.RequestUri.ToString().Contains("/introspect")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken ct) =>
            {
                var token = ExtractTokenFromRequest(req);
                return CreateMockIntrospectionResponse(token);
            });
    }
}
```

### Test Database Setup
```csharp
public class TestDatabaseFixture : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        // Set up test database with minimal test data
        await SeedTestOrganisations();
        await SeedTestUsers();
    }
    
    public async Task DisposeAsync()
    {
        // Clean up test database
        await CleanupTestData();
    }
}
```

This testing contract ensures comprehensive coverage of external token authentication with a test-first approach that validates both functional requirements and authorization equivalence.