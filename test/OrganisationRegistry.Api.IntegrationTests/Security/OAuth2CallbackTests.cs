namespace OrganisationRegistry.Api.IntegrationTests.Security;

using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Tests.Shared;
using Xunit;

/// <summary>
/// Integration tests for OAuth2 callback URL handling and nginx proxy behavior.
/// Tests Track B: nginx OAuth callback code extraction for hash-based routing.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class OAuth2CallbackTests : IDisposable
{
    private readonly ApiFixture _apiFixture;
    private readonly HttpClient _httpClient;

    public OAuth2CallbackTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
        _httpClient = new HttpClient();
    }

    // ========================================================================
    // Track B: nginx OAuth Callback URL Rewriting Tests
    // ========================================================================

    [EnvVarIgnoreFact]
    public async Task NginxOAuthCallback_WithQueryParameters_RedirectsCorrectly()
    {
        // Test nginx rewrite rule that transforms OAuth callback URLs
        // /?code=ABC123&state=XYZ → /#/oic?code=ABC123&state=XYZ

        var callbackUrl = "http://ui.localhost:9080/oic?code=test_auth_code_123&state=test_state_456";

        var response = await _httpClient.GetAsync(callbackUrl);

        response.StatusCode.Should().Be(HttpStatusCode.Redirect,
            "nginx should redirect /oic callback to hash-based route");

        var locationHeader = response.Headers.Location?.ToString();
        locationHeader.Should().NotBeNullOrEmpty("Redirect should have Location header");
        
        locationHeader.Should().Contain("#/oic",
            "nginx should rewrite to hash-based routing format");
        
        locationHeader.Should().Contain("code=test_auth_code_123",
            "OAuth authorization code should be preserved in redirect");
        
        locationHeader.Should().Contain("state=test_state_456",
            "OAuth state parameter should be preserved in redirect");
    }

    [EnvVarIgnoreFact]
    public async Task NginxOAuthCallback_WithSpecialCharacters_HandlesUrlEncoding()
    {
        // Test nginx handling of URL-encoded OAuth parameters
        
        var encodedCode = Uri.EscapeDataString("code+with/special=characters");
        var encodedState = Uri.EscapeDataString("state&with?special#chars");
        var callbackUrl = $"http://ui.localhost:9080/oic?code={encodedCode}&state={encodedState}";

        var response = await _httpClient.GetAsync(callbackUrl);

        response.StatusCode.Should().Be(HttpStatusCode.Redirect,
            "nginx should handle URL-encoded OAuth parameters");

        var locationHeader = response.Headers.Location?.ToString();
        locationHeader.Should().NotBeNullOrEmpty();
        
        locationHeader.Should().Contain("#/oic",
            "nginx should rewrite to hash-based routing");
        
        locationHeader.Should().Contain($"code={encodedCode}",
            "URL-encoded authorization code should be preserved");
        
        locationHeader.Should().Contain($"state={encodedState}",
            "URL-encoded state parameter should be preserved");
    }

    [EnvVarIgnoreFact]
    public async Task NginxOAuthCallback_WithMinimalParameters_HandlesCodeOnly()
    {
        // Test nginx handling when only authorization code is present
        
        var callbackUrl = "http://ui.localhost:9080/oic?code=minimal_test_code";

        var response = await _httpClient.GetAsync(callbackUrl);

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        var locationHeader = response.Headers.Location?.ToString();
        locationHeader.Should().Contain("#/oic");
        locationHeader.Should().Contain("code=minimal_test_code");
    }

    [EnvVarIgnoreFact]
    public async Task NginxOAuthCallback_WithoutParameters_StillRedirects()
    {
        // Test nginx behavior when OAuth callback has no query parameters
        
        var callbackUrl = "http://ui.localhost:9080/oic";

        var response = await _httpClient.GetAsync(callbackUrl);

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        var locationHeader = response.Headers.Location?.ToString();
        locationHeader.Should().Contain("#/oic",
            "nginx should redirect to hash route even without parameters");
    }

    [EnvVarIgnoreFact]
    public async Task NginxOAuthCallback_WithErrorParameters_PreservesErrorInfo()
    {
        // Test nginx handling of OAuth error responses
        
        var callbackUrl = "http://ui.localhost:9080/oic?error=access_denied&error_description=User+denied+access&state=test_state";

        var response = await _httpClient.GetAsync(callbackUrl);

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        var locationHeader = response.Headers.Location?.ToString();
        locationHeader.Should().Contain("#/oic");
        locationHeader.Should().Contain("error=access_denied");
        locationHeader.Should().Contain("error_description=User+denied+access");
        locationHeader.Should().Contain("state=test_state");
    }

    [EnvVarIgnoreFact]
    public async Task AngularHashRouting_CanAccessOAuthParameters_AfterNginxRewrite()
    {
        // Test the complete OAuth callback flow:
        // 1. nginx rewrites /?code=X to /#/oic?code=X
        // 2. Angular hash router can access parameters
        
        var callbackUrl = "http://ui.localhost:9080/oic?code=hash_route_test&state=angular_test";

        var response = await _httpClient.GetAsync(callbackUrl);
        
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        
        var redirectLocation = response.Headers.Location?.ToString();
        redirectLocation.Should().NotBeNullOrEmpty();
        
        var hashIndex = redirectLocation!.IndexOf('#');
        hashIndex.Should().BeGreaterThan(-1, "Redirected URL should contain hash for Angular routing");
        
        var hashPart = redirectLocation.Substring(hashIndex + 1);
        hashPart.Should().StartWith("/oic", "Hash part should start with /oic route");
        
        var queryIndex = hashPart.IndexOf('?');
        if (queryIndex > -1)
        {
            var queryPart = hashPart.Substring(queryIndex + 1);
            queryPart.Should().Contain("code=hash_route_test");
            queryPart.Should().Contain("state=angular_test");
        }
    }

    [EnvVarIgnoreFact]
    public async Task UIEndpoint_IsAccessible_ForOAuthCallbacks()
    {
        // Verify the UI endpoint is reachable for OAuth callback testing
        
        var response = await _httpClient.GetAsync("http://ui.localhost:9080/");

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK, 
            HttpStatusCode.Redirect);
        
        response.StatusCode.Should().NotBe(HttpStatusCode.ServiceUnavailable,
            "UI endpoint should be accessible for OAuth callbacks");
    }

    [EnvVarIgnoreFact]
    public async Task NginxConfiguration_HandlesOicPathVariations()
    {
        // Test nginx rule handles /oic variations and subpaths
        
        var testPaths = new[]
        {
            "/oic",
            "/oic/",
            "/oic/callback",
            "/oic/complete"
        };

        foreach (var path in testPaths)
        {
            var fullUrl = $"http://ui.localhost:9080{path}?code=test&state=test";
            var response = await _httpClient.GetAsync(fullUrl);
            
            response.StatusCode.Should().Be(HttpStatusCode.Redirect,
                $"nginx should handle OAuth callback path: {path}");
                
            var location = response.Headers.Location?.ToString();
            location.Should().Contain("#", $"Path {path} should redirect to hash routing");
        }
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