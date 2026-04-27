namespace OrganisationRegistry.Api.IntegrationTests.Security;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Tests.Shared;
using Xunit;

/// <summary>
/// Diagnostic tests for OAuth2 "login does nothing" scenarios.
/// Tests Track D: Comprehensive OAuth2 integration testing with silent failure detection.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class OAuth2DiagnosticsTests : IDisposable
{
    private readonly ApiFixture _apiFixture;
    private readonly HttpClient _httpClient;

    public OAuth2DiagnosticsTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
        _httpClient = new HttpClient();
    }

    // ========================================================================
    // Track D: Silent OAuth2 Failure Diagnostic Tests
    // ========================================================================

    [EnvVarIgnoreFact]
    public async Task OAuth2AuthorizationEndpoint_IsAccessible_FromPublicNetwork()
    {
        // Test that the OAuth2 authorization endpoint is reachable
        // This is where "login does nothing" often fails silently

        var authorizationUrl = "http://keycloak.localhost:9080/realms/wegwijs/protocol/openid-connect/auth";

        var response = await _httpClient.GetAsync(authorizationUrl);

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.BadRequest,
            HttpStatusCode.Found
        );

        response.StatusCode.Should().NotBe(HttpStatusCode.ServiceUnavailable,
            "Keycloak authorization endpoint must be accessible for OAuth2 login to work");
        
        response.StatusCode.Should().NotBe(HttpStatusCode.NotFound,
            "Authorization endpoint must exist for OAuth2 flows to initiate");
    }

    [EnvVarIgnoreFact]
    public async Task OAuth2TokenEndpoint_IsAccessible_FromPublicNetwork()
    {
        // Test that the OAuth2 token endpoint is reachable
        // This is where silent authorization code exchange failures occur

        var tokenUrl = "http://keycloak.localhost:9080/realms/wegwijs/protocol/openid-connect/token";

        var response = await _httpClient.PostAsync(tokenUrl, new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        }));

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest,
            HttpStatusCode.Unauthorized,
            HttpStatusCode.OK
        );

        response.StatusCode.Should().NotBe(HttpStatusCode.ServiceUnavailable,
            "Keycloak token endpoint must be accessible for OAuth2 flows to complete");
        
        response.StatusCode.Should().NotBe(HttpStatusCode.NotFound,
            "Token endpoint must exist for authorization code exchange");
    }

    [EnvVarIgnoreFact]
    public async Task OAuth2WellKnownConfiguration_IsAccessible_AndValid()
    {
        // Test that OpenID Connect discovery document is accessible
        // Angular UI relies on this for OAuth2 configuration

        var discoveryUrl = "http://keycloak.localhost:9080/realms/wegwijs/.well-known/openid_configuration";

        var response = await _httpClient.GetAsync(discoveryUrl);

        response.StatusCode.Should().Be(HttpStatusCode.OK,
            "OpenID Connect discovery document must be accessible");

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrWhiteSpace();

        var discovery = JsonSerializer.Deserialize<JsonElement>(content);
        
        discovery.TryGetProperty("authorization_endpoint", out var authEndpoint).Should().BeTrue(
            "Discovery document must contain authorization_endpoint");
        
        discovery.TryGetProperty("token_endpoint", out var tokenEndpoint).Should().BeTrue(
            "Discovery document must contain token_endpoint");
        
        discovery.TryGetProperty("issuer", out var issuer).Should().BeTrue(
            "Discovery document must contain issuer");

        authEndpoint.GetString().Should().NotBeNullOrWhiteSpace();
        tokenEndpoint.GetString().Should().NotBeNullOrWhiteSpace();
        issuer.GetString().Should().NotBeNullOrWhiteSpace();
    }

    [EnvVarIgnoreFact]
    public async Task ApiSecurityConfiguration_ExposesCorrectOAuth2Settings()
    {
        // Test that API security configuration returns correct OAuth2 endpoints
        // UI retrieves these to configure OAuth2 client

        var securityConfigUrl = $"{_apiFixture.ApiEndpoint}/v1/security/configuration";

        var response = await _httpClient.GetAsync(securityConfigUrl);

        response.StatusCode.Should().Be(HttpStatusCode.OK,
            "Security configuration endpoint must be accessible");

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrWhiteSpace();

        var config = JsonSerializer.Deserialize<JsonElement>(content);
        
        config.TryGetProperty("authority", out var authority).Should().BeTrue(
            "Security config must contain authority");
        
        config.TryGetProperty("clientId", out var clientId).Should().BeTrue(
            "Security config must contain clientId");

        var authorityValue = authority.GetString();
        var clientIdValue = clientId.GetString();

        authorityValue.Should().NotBeNullOrWhiteSpace("Authority must be configured");
        clientIdValue.Should().NotBeNullOrWhiteSpace("ClientId must be configured");
        
        authorityValue.Should().Contain("keycloak", "Authority should point to Keycloak");
        clientIdValue.Should().Be("nuxt-bff", "ClientId should match UI client configuration");
    }

    [EnvVarIgnoreFact]
    public async Task OAuth2AuthorizationFlow_CanInitiate_WithCorrectParameters()
    {
        // Test that OAuth2 authorization flow can be initiated with proper parameters
        // This simulates what happens when user clicks "login" button

        var authorizationUrl = BuildAuthorizationUrl(
            authority: "http://keycloak.localhost:9080/realms/wegwijs",
            clientId: "nuxt-bff",
            redirectUri: "http://ui.localhost:9080/oic",
            state: "test_state_" + Guid.NewGuid().ToString("N")[..8],
            scopes: "openid profile vo iv_wegwijs"
        );

        var response = await _httpClient.GetAsync(authorizationUrl);

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.Found,
            HttpStatusCode.SeeOther
        );

        response.StatusCode.Should().NotBe(HttpStatusCode.BadRequest,
            "Authorization request parameters should be valid");
        
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized,
            "Public client should be able to initiate authorization flow");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            content.ToLowerInvariant().Should().Contain("login");
        }
    }

    [EnvVarIgnoreFact]
    public async Task UIProxyConfiguration_AllowsOAuth2Callbacks()
    {
        // Test that UI nginx proxy correctly handles OAuth2 callback URLs
        // This is where "login does nothing" often fails due to routing issues

        var callbackUrls = new[]
        {
            "http://ui.localhost:9080/oic?code=test_code&state=test_state",
            "http://ui.localhost:9080/oic/callback?code=test_code&state=test_state",
            "http://ui.localhost:9080/auth/callback?code=test_code&state=test_state"
        };

        foreach (var callbackUrl in callbackUrls)
        {
            var response = await _httpClient.GetAsync(callbackUrl);

            response.StatusCode.Should().NotBe(HttpStatusCode.NotFound,
                $"UI should handle OAuth2 callback URL: {callbackUrl}");
            
            response.StatusCode.Should().NotBe(HttpStatusCode.BadGateway,
                $"nginx proxy should be configured correctly for: {callbackUrl}");
            
            response.StatusCode.Should().BeOneOf(
                HttpStatusCode.OK,
                HttpStatusCode.Redirect,
                HttpStatusCode.Found
            );
        }
    }

    [EnvVarIgnoreFact]
    public async Task NetworkConnectivity_BetweenUIAndAPI_IsWorking()
    {
        // Test basic network connectivity from UI domain to API domain
        // Network issues can cause silent OAuth2 failures

        var apiHealthUrl = $"{_apiFixture.ApiEndpoint}/health";
        var uiDomainRequest = new HttpRequestMessage(HttpMethod.Get, apiHealthUrl);
        uiDomainRequest.Headers.Add("Host", "ui.localhost:9080");
        uiDomainRequest.Headers.Add("Origin", "http://ui.localhost:9080");

        var response = await _httpClient.SendAsync(uiDomainRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK,
            "API should be accessible from UI domain for OAuth2 flows");
    }

    [EnvVarIgnoreFact]
    public async Task CORSConfiguration_AllowsOAuth2Requests()
    {
        // Test CORS configuration for OAuth2-related requests
        // CORS failures can cause silent OAuth2 errors in browsers

        var securityConfigUrl = $"{_apiFixture.ApiEndpoint}/v1/security/configuration";
        var corsRequest = new HttpRequestMessage(HttpMethod.Options, securityConfigUrl);
        corsRequest.Headers.Add("Origin", "http://ui.localhost:9080");
        corsRequest.Headers.Add("Access-Control-Request-Method", "GET");
        corsRequest.Headers.Add("Access-Control-Request-Headers", "Authorization");

        var response = await _httpClient.SendAsync(corsRequest);

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.NoContent
        );

        response.Headers.Should().Contain(h => h.Key == "Access-Control-Allow-Origin",
            "CORS must allow requests from UI domain");
    }

    [EnvVarIgnoreFact]
    public async Task OAuth2Flow_EndToEnd_ConnectivityTest()
    {
        // Test complete OAuth2 flow connectivity without actual authentication
        // This identifies where the flow breaks in "login does nothing" scenarios

        // Step 1: Get security configuration from API
        var securityConfigResponse = await _httpClient.GetAsync(
            $"{_apiFixture.ApiEndpoint}/v1/security/configuration");
        securityConfigResponse.StatusCode.Should().Be(HttpStatusCode.OK, "Step 1: Security config failed");

        // Step 2: Access Keycloak discovery document
        var discoveryResponse = await _httpClient.GetAsync(
            "http://keycloak.localhost:9080/realms/wegwijs/.well-known/openid_configuration");
        discoveryResponse.StatusCode.Should().Be(HttpStatusCode.OK, "Step 2: OIDC discovery failed");

        // Step 3: Test authorization endpoint accessibility
        var authResponse = await _httpClient.GetAsync(
            "http://keycloak.localhost:9080/realms/wegwijs/protocol/openid-connect/auth");
        authResponse.StatusCode.Should().NotBe(HttpStatusCode.ServiceUnavailable, "Step 3: Authorization endpoint unreachable");

        // Step 4: Test UI callback endpoint accessibility
        var callbackResponse = await _httpClient.GetAsync("http://ui.localhost:9080/oic");
        callbackResponse.StatusCode.Should().NotBe(HttpStatusCode.ServiceUnavailable, "Step 4: UI callback endpoint unreachable");

        // Step 5: Test token endpoint accessibility
        var tokenResponse = await _httpClient.PostAsync(
            "http://keycloak.localhost:9080/realms/wegwijs/protocol/openid-connect/token",
            new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("test", "connectivity") }));
        tokenResponse.StatusCode.Should().NotBe(HttpStatusCode.ServiceUnavailable, "Step 5: Token endpoint unreachable");
    }

    // ========================================================================
    // Helper Methods
    // ========================================================================

    private string BuildAuthorizationUrl(string authority, string clientId, string redirectUri, 
        string state, string scopes)
    {
        var baseUrl = $"{authority.TrimEnd('/')}/protocol/openid-connect/auth";
        var parameters = new[]
        {
            $"client_id={Uri.EscapeDataString(clientId)}",
            $"redirect_uri={Uri.EscapeDataString(redirectUri)}",
            $"response_type=code",
            $"scope={Uri.EscapeDataString(scopes)}",
            $"state={Uri.EscapeDataString(state)}",
            $"code_challenge_method=S256",
            $"code_challenge={Uri.EscapeDataString(GenerateCodeChallenge())}"
        };

        return $"{baseUrl}?{string.Join("&", parameters)}";
    }

    private string GenerateCodeChallenge()
    {
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("test_code_challenge"))
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _httpClient?.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}