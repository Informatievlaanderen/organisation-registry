# Token Exchange Configuration Contract

**Version**: 1.0  
**Purpose**: Configuration interface for OAuth2 token introspection providers in Organisation Registry

## Configuration Schema

### TokenExchangeConfiguration
```csharp
public class TokenExchangeConfiguration
{
    [Required]
    [Url(RequireHttps = true)]
    public string Authority { get; set; } = string.Empty;
    
    [Required]
    [Url(RequireHttps = true)]
    public string IntrospectionEndpoint { get; set; } = string.Empty;
    
    [Required]
    [MinLength(1)]
    public string ClientId { get; set; } = string.Empty;
    
    [Required]
    [MinLength(8)]
    public string ClientSecret { get; set; } = string.Empty;
    
    [Range(typeof(TimeSpan), "00:00:30", "00:30:00")]
    public TimeSpan CacheTtl { get; set; } = TimeSpan.FromMinutes(2);
    
    public bool Enabled { get; set; } = false;
    
    [Range(1000, 30000)]
    public int TimeoutMs { get; set; } = 5000;
    
    [Range(1, 10)]
    public int MaxRetries { get; set; } = 3;
    
    public string[]? RequiredScopes { get; set; }
    
    public string[]? AllowedIssuers { get; set; }
}
```

### appsettings.json Schema
```json
{
  "Authentication": {
    "TokenExchange": {
      "Authority": "https://auth.vlaanderen.be",
      "IntrospectionEndpoint": "https://auth.vlaanderen.be/oauth2/introspect", 
      "ClientId": "organisation_registry_introspection",
      "ClientSecret": "supersecret123",
      "CacheTtl": "00:02:00",
      "Enabled": false,
      "TimeoutMs": 5000,
      "MaxRetries": 3,
      "RequiredScopes": ["openid", "profile"],
      "AllowedIssuers": ["https://auth.vlaanderen.be"]
    }
  }
}
```

### Environment-Specific Configuration
```json
// appsettings.Development.json
{
  "Authentication": {
    "TokenExchange": {
      "Authority": "https://test-auth.vlaanderen.be",
      "IntrospectionEndpoint": "https://test-auth.vlaanderen.be/oauth2/introspect",
      "ClientId": "test_introspection_client", 
      "ClientSecret": "test_secret",
      "Enabled": true,
      "CacheTtl": "00:01:00"
    }
  }
}

// appsettings.Production.json  
{
  "Authentication": {
    "TokenExchange": {
      "Authority": "https://auth.vlaanderen.be",
      "IntrospectionEndpoint": "https://auth.vlaanderen.be/oauth2/introspect",
      "ClientId": "prod_introspection_client",
      "ClientSecret": "${INTROSPECTION_CLIENT_SECRET}", 
      "Enabled": true,
      "CacheTtl": "00:05:00",
      "RequiredScopes": ["openid", "profile", "organisation_admin"],
      "AllowedIssuers": ["https://auth.vlaanderen.be"]
    }
  }
}
```

## Configuration Validation Contract

### Startup Validation
```csharp
public class TokenExchangeConfigurationValidator : IValidateOptions<TokenExchangeConfiguration>
{
    public ValidateOptionsResult Validate(string name, TokenExchangeConfiguration config)
    {
        var errors = new List<string>();
        
        // Authority validation
        if (!Uri.TryCreate(config.Authority, UriKind.Absolute, out var authorityUri) || 
            authorityUri.Scheme != "https")
        {
            errors.Add("Authority must be a valid HTTPS URL");
        }
        
        // Introspection endpoint validation
        if (!Uri.TryCreate(config.IntrospectionEndpoint, UriKind.Absolute, out var introspectionUri) ||
            introspectionUri.Scheme != "https")
        {
            errors.Add("IntrospectionEndpoint must be a valid HTTPS URL");
        }
        
        // Client credentials validation
        if (string.IsNullOrWhiteSpace(config.ClientId))
        {
            errors.Add("ClientId is required");
        }
        
        if (string.IsNullOrWhiteSpace(config.ClientSecret) && config.Enabled)
        {
            errors.Add("ClientSecret is required when TokenExchange is enabled");
        }
        
        // Cache TTL validation
        if (config.CacheTtl < TimeSpan.FromSeconds(30) || config.CacheTtl > TimeSpan.FromMinutes(30))
        {
            errors.Add("CacheTtl must be between 30 seconds and 30 minutes");
        }
        
        // Timeout validation
        if (config.TimeoutMs < 1000 || config.TimeoutMs > 30000)
        {
            errors.Add("TimeoutMs must be between 1000 and 30000 milliseconds");
        }
        
        return errors.Any() 
            ? ValidateOptionsResult.Fail(errors)
            : ValidateOptionsResult.Success;
    }
}
```

### Runtime Configuration Checks
```csharp
public interface ITokenExchangeHealthCheck
{
    Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default);
}

public class TokenExchangeHealthCheck : ITokenExchangeHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Test introspection endpoint connectivity
            var testResult = await introspectionService.ValidateEndpointAsync();
            
            return testResult.IsSuccess 
                ? HealthCheckResult.Healthy("TokenExchange introspection endpoint is accessible")
                : HealthCheckResult.Degraded($"TokenExchange endpoint issue: {testResult.Error}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("TokenExchange introspection endpoint is unreachable", ex);
        }
    }
}
```

## Dependency Injection Configuration

### Service Registration
```csharp
public static class TokenExchangeServiceCollectionExtensions
{
    public static IServiceCollection AddTokenExchangeAuthentication(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Bind and validate configuration
        services.Configure<TokenExchangeConfiguration>(
            configuration.GetSection("Authentication:TokenExchange"));
        services.AddSingleton<IValidateOptions<TokenExchangeConfiguration>, 
            TokenExchangeConfigurationValidator>();
            
        // Register services
        services.AddScoped<ITokenIntrospectionService, TokenIntrospectionService>();
        services.AddScoped<IIntrospectionCache, IntrospectionCache>();
        services.AddScoped<ITokenExchangeClaimsTransformer, TokenExchangeClaimsTransformer>();
        
        // HTTP client configuration
        services.AddHttpClient<ITokenIntrospectionService>((provider, client) =>
        {
            var config = provider.GetRequiredService<IOptions<TokenExchangeConfiguration>>();
            client.BaseAddress = new Uri(config.Value.Authority);
            client.Timeout = TimeSpan.FromMilliseconds(config.Value.TimeoutMs);
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy());
        
        // Health checks
        services.AddHealthChecks()
            .AddCheck<TokenExchangeHealthCheck>("token_exchange");
            
        return services;
    }
    
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt => 
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
    
    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .CircuitBreakerAsync(3, TimeSpan.FromMinutes(1));
    }
}
```

### Authentication Scheme Configuration
```csharp
public static class AuthenticationBuilderExtensions
{
    public static AuthenticationBuilder AddTokenExchange(
        this AuthenticationBuilder builder, 
        string authenticationScheme = "TokenExchange")
    {
        return builder.AddScheme<TokenExchangeSchemeOptions, TokenExchangeAuthenticationHandler>(
            authenticationScheme, 
            configureOptions: null);
    }
}

// In Startup.cs or Program.cs
services.AddAuthentication()
    .AddJwtBearer("Bearer", options => { /* existing config */ })
    .AddTokenExchange("TokenExchange")
    .AddOAuth2Introspection("EditApi", options => { /* existing config */ });
```

## Security Configuration Contract

### Client Secret Management
```csharp
public interface IClientSecretProvider
{
    Task<string> GetClientSecretAsync(string clientId);
}

public class AzureKeyVaultSecretProvider : IClientSecretProvider
{
    public async Task<string> GetClientSecretAsync(string clientId)
    {
        var secretName = $"tokenexchange-{clientId}-secret";
        var secret = await keyVaultClient.GetSecretAsync(secretName);
        return secret.Value;
    }
}

// Configuration with secret provider
services.Configure<TokenExchangeConfiguration>(config =>
{
    // ClientSecret will be resolved at runtime from KeyVault
    config.ClientSecret = "${KEYVAULT_SECRET}";
});
```

### TLS Configuration
```csharp
services.AddHttpClient<ITokenIntrospectionService>()
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
        {
            // Custom certificate validation for introspection endpoint
            return CertificateValidator.IsValidIntrospectionCertificate(cert);
        }
    });
```

## Configuration Migration Contract

### Version 1.0 → 1.1 Migration
```csharp
public class TokenExchangeConfigurationMigrator
{
    public TokenExchangeConfiguration MigrateFrom1_0(LegacyTokenExchangeConfig legacy)
    {
        return new TokenExchangeConfiguration
        {
            Authority = legacy.AuthorityUrl,
            IntrospectionEndpoint = $"{legacy.AuthorityUrl}/oauth2/introspect",
            ClientId = legacy.IntrospectionClientId,
            ClientSecret = legacy.IntrospectionClientSecret,
            CacheTtl = legacy.CacheTimeoutMinutes > 0 
                ? TimeSpan.FromMinutes(legacy.CacheTimeoutMinutes)
                : TimeSpan.FromMinutes(2),
            Enabled = legacy.EnableTokenExchange,
            TimeoutMs = 5000,
            MaxRetries = 3
        };
    }
}
```

## Feature Flag Integration

### Configuration-Based Feature Toggle
```csharp
public class TokenExchangeFeatureFilter : IFeatureFilter
{
    private readonly IOptionsMonitor<TokenExchangeConfiguration> _options;
    
    public bool Evaluate(FeatureFilterEvaluationContext context)
    {
        var config = _options.CurrentValue;
        return config.Enabled && !string.IsNullOrEmpty(config.ClientSecret);
    }
}

// Usage in controllers
[FeatureGate("TokenExchange")]
public class TokenExchangeController : ControllerBase
{
    // Only accessible when TokenExchange is enabled
}
```

## Test Configuration Contract

### Test Configuration Factory
```csharp
public static class TestTokenExchangeConfiguration
{
    public static TokenExchangeConfiguration CreateValid()
    {
        return new TokenExchangeConfiguration
        {
            Authority = "https://test-auth.example.com",
            IntrospectionEndpoint = "https://test-auth.example.com/oauth2/introspect",
            ClientId = "test_client",
            ClientSecret = "test_secret_123",
            CacheTtl = TimeSpan.FromMinutes(1),
            Enabled = true,
            TimeoutMs = 2000,
            MaxRetries = 1
        };
    }
    
    public static TokenExchangeConfiguration CreateInvalid()
    {
        return new TokenExchangeConfiguration
        {
            Authority = "http://insecure-endpoint", // Invalid: not HTTPS
            ClientSecret = "123", // Invalid: too short
            CacheTtl = TimeSpan.FromSeconds(10) // Invalid: too short
        };
    }
}
```

### Integration Test Configuration Override
```csharp
[Collection("ApiIntegrationTests")]
public class TokenExchangeIntegrationTests
{
    [Fact]
    public async Task TokenExchange_WithValidConfiguration_AuthenticatesSuccessfully()
    {
        // Override configuration for testing
        var testConfig = TestTokenExchangeConfiguration.CreateValid();
        
        var services = new ServiceCollection();
        services.Configure<TokenExchangeConfiguration>(config =>
        {
            config.Authority = testConfig.Authority;
            config.IntrospectionEndpoint = testConfig.IntrospectionEndpoint;
            config.ClientId = testConfig.ClientId;
            config.ClientSecret = testConfig.ClientSecret;
            config.Enabled = true;
        });
        
        // Test with overridden configuration
    }
}
```