namespace OrganisationRegistry.Api.IntegrationTests.Security;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Tests.Shared;
using Xunit;

/// <summary>
/// Integration tests for the Nuxt BFF authorization-code + PKCE entry point.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class OAuth2ConfigurationTests : IDisposable
{
    private const string NuxtBffBaseUrl = "http://app.localhost:9080";
    private const string KeycloakAuthorizationPath = "/realms/wegwijs/protocol/openid-connect/auth";

    private readonly HttpClient _httpClient;

    public OAuth2ConfigurationTests(ApiFixture apiFixture)
    {
        _ = apiFixture;
        _httpClient = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false });
    }

    [EnvVarIgnoreFact]
    public async Task NuxtBffLogin_RedirectsToKeycloak_WithAuthorizationCodePkceParameters()
    {
        var response = await _httpClient.GetAsync($"{NuxtBffBaseUrl}/api/login");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        var location = response.Headers.Location;
        location.Should().NotBeNull();
        location!.Host.Should().Be("keycloak.localhost");
        location.Port.Should().Be(9080);
        location.AbsolutePath.Should().Be(KeycloakAuthorizationPath);

        var query = ParseQuery(location);
        query.Should().ContainKey("response_type").WhoseValue.Should().Be("code");
        query.Should().ContainKey("client_id").WhoseValue.Should().Be("nuxt-bff");
        query.Should().ContainKey("redirect_uri").WhoseValue.Should().Be($"{NuxtBffBaseUrl}/callback");
        query.Should().ContainKey("scope").WhoseValue.Should().Be("openid profile vo iv_wegwijs");
        query.Should().ContainKey("code_challenge_method").WhoseValue.Should().Be("S256");
        query.Should().ContainKey("prompt").WhoseValue.Should().Be("select_account");
        query.Should().ContainKey("state").WhoseValue.Should().NotBeNullOrWhiteSpace();
        query.Should().ContainKey("code_challenge").WhoseValue.Should().NotBeNullOrWhiteSpace();
    }

    [EnvVarIgnoreFact]
    public async Task NuxtBffAuthorizationRequest_IsAcceptedByKeycloak()
    {
        var loginResponse = await _httpClient.GetAsync($"{NuxtBffBaseUrl}/api/login");
        var authorizationUrl = loginResponse.Headers.Location;

        authorizationUrl.Should().NotBeNull();

        var response = await _httpClient.GetAsync(authorizationUrl);

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.Found,
            HttpStatusCode.SeeOther);

        response.StatusCode.Should().NotBe(HttpStatusCode.BadRequest,
            "the Nuxt BFF redirect URI, client id and PKCE parameters must match the Keycloak realm configuration");
    }

    [EnvVarIgnoreFact]
    public async Task NuxtBffEndpoint_IsReachableThroughTiltIngress()
    {
        var response = await _httpClient.GetAsync(NuxtBffBaseUrl);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Redirect);
        response.StatusCode.Should().NotBe(HttpStatusCode.ServiceUnavailable);
        response.StatusCode.Should().NotBe(HttpStatusCode.BadGateway);
    }

    private static IDictionary<string, string> ParseQuery(Uri uri)
    {
        var query = uri.Query.TrimStart('?');
        var result = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var part in query.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var separatorIndex = part.IndexOf('=');
            var key = separatorIndex >= 0 ? part[..separatorIndex] : part;
            var value = separatorIndex >= 0 ? part[(separatorIndex + 1)..] : string.Empty;

            result[Uri.UnescapeDataString(key.Replace("+", " "))] =
                Uri.UnescapeDataString(value.Replace("+", " "));
        }

        return result;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            _httpClient.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
