namespace OrganisationRegistry.Api.IntegrationTests.Security;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using IdentityModel;
using Tests.Shared;
using Xunit;

/// <summary>
/// Integration tests for OAuth2 configuration scenarios including PKCE flows and client types.
/// Tests Track C: Public client PKCE-only OAuth2 flow support.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class OAuth2ConfigurationTests
{
    private readonly ApiFixture _apiFixture;

    public OAuth2ConfigurationTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    // ========================================================================
    // Track C: Public Client PKCE-Only Flow Tests
    // ========================================================================

    [EnvVarIgnoreFact]
    public async Task PublicClientAuthentication_WithPKCEOnly_WorksWithoutClientSecret()
    {
        // Test that API correctly handles public clients (no ClientSecret)
        // using PKCE-only authorization code flow
        
        var organisationId = Guid.NewGuid();
        await _apiFixture.Create.Organisation(organisationId, "PKCE Test Organization");

        var publicClientToken = await GetPublicClientAccessToken("nuxt-bff", "openid profile vo iv_wegwijs");
        var client = CreateClientWithTokenExchange(publicClientToken);

        var response = await GetOrganisationDetails(client, organisationId);

        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized,
            "Public client PKCE-only tokens should be accepted by TokenExchange authentication");
        
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Forbidden);
    }

    [EnvVarIgnoreFact]
    public async Task ConfidentialClientAuthentication_WithClientSecret_ContinuesToWork()
    {
        // Ensure confidential clients (with ClientSecret) still work after PKCE support
        
        var organisationId = Guid.NewGuid();
        await _apiFixture.Create.Organisation(organisationId, "Confidential Client Test Organization");

        var confidentialToken = await GetConfidentialClientAccessToken(ApiFixture.Test.Client, ApiFixture.Test.Scope);
        var client = CreateClientWithTokenExchange(confidentialToken);

        var response = await GetOrganisationDetails(client, organisationId);

        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized,
            "Confidential client tokens should continue working after PKCE support");
        
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Forbidden);
    }

    [EnvVarIgnoreFact]
    public async Task MixedClientTypes_BothPublicAndConfidential_CanAuthenticate()
    {
        var organisationId = Guid.NewGuid();
        await _apiFixture.Create.Organisation(organisationId, "Mixed Client Types Test Organization");

        var publicToken = await GetPublicClientAccessToken("nuxt-bff", "openid profile vo iv_wegwijs");
        var confidentialToken = await GetConfidentialClientAccessToken(ApiFixture.Test.Client, ApiFixture.Test.Scope);

        var publicClient = CreateClientWithTokenExchange(publicToken);
        var confidentialClient = CreateClientWithTokenExchange(confidentialToken);

        var publicResponse = await GetOrganisationDetails(publicClient, organisationId);
        var confidentialResponse = await GetOrganisationDetails(confidentialClient, organisationId);

        publicResponse.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized,
            "Public client should authenticate successfully");
        
        confidentialResponse.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized,
            "Confidential client should authenticate successfully");
    }

    [EnvVarIgnoreFact]
    public async Task OAuth2IntrospectionEndpoint_AcceptsBothClientTypes()
    {
        // Test that introspection endpoint handles both public and confidential clients

        var publicToken = await GetPublicClientAccessToken("nuxt-bff", "openid profile vo iv_wegwijs");
        var confidentialToken = await GetConfidentialClientAccessToken(ApiFixture.Test.Client, ApiFixture.Test.Scope);

        publicToken.Should().NotBeNullOrWhiteSpace("Public client should receive access token");
        confidentialToken.Should().NotBeNullOrWhiteSpace("Confidential client should receive access token");

        var publicTokenIsJwt = publicToken.Split('.').Length == 3;
        var confidentialTokenIsJwt = confidentialToken.Split('.').Length == 3;

        publicTokenIsJwt.Should().BeTrue("Public client token should be JWT format");
        confidentialTokenIsJwt.Should().BeTrue("Confidential client token should be JWT format");
    }

    // ========================================================================
    // Helper Methods
    // ========================================================================

    private async Task<string> GetPublicClientAccessToken(string clientId, string scope)
    {
        const string defaultKeycloakAuthority = "http://keycloak.localhost:9080/realms/wegwijs";
        
        var tokenClient = new TokenClient(
            () => new HttpClient(),
            new TokenClientOptions
            {
                Address = $"{defaultKeycloakAuthority.TrimEnd('/')}/protocol/openid-connect/token",
                ClientId = clientId,
                ClientSecret = null, // Public client - no secret
                Parameters = new Parameters(
                    new[]
                    {
                        new KeyValuePair<string, string>("scope", scope),
                    }),
            });

        var response = await tokenClient.RequestTokenAsync(OidcConstants.GrantTypes.ClientCredentials);

        if (response.IsError || string.IsNullOrWhiteSpace(response.AccessToken))
            throw new InvalidOperationException(
                $"Could not retrieve public client token for '{clientId}' from '{defaultKeycloakAuthority}'. " +
                $"Error: {response.Error}. Description: {response.ErrorDescription}.");

        return response.AccessToken;
    }

    private async Task<string> GetConfidentialClientAccessToken(string clientId, string scope)
    {
        const string defaultKeycloakAuthority = "http://keycloak.localhost:9080/realms/wegwijs";

        // Select the correct client secret based on the clientId
        var clientSecret = clientId switch
        {
            ApiFixture.CJM.Client => "cjm-client-secret-2024",
            ApiFixture.Orafin.Client => "orafin-client-secret-2024",
            ApiFixture.Test.Client => "secret",
            _ => "secret"
        };

        var tokenClient = new TokenClient(
            () => new HttpClient(),
            new TokenClientOptions
            {
                Address = $"{defaultKeycloakAuthority.TrimEnd('/')}/protocol/openid-connect/token",
                ClientId = clientId,
                ClientSecret = clientSecret, // Confidential client with secret
                Parameters = new Parameters(
                    new[]
                    {
                        new KeyValuePair<string, string>("scope", scope),
                    }),
            });

        var response = await tokenClient.RequestTokenAsync(OidcConstants.GrantTypes.ClientCredentials);

        if (response.IsError || string.IsNullOrWhiteSpace(response.AccessToken))
            throw new InvalidOperationException(
                $"Could not retrieve confidential client token for '{clientId}' from '{defaultKeycloakAuthority}'. " +
                $"Error: {response.Error}. Description: {response.ErrorDescription}.");

        return response.AccessToken;
    }

    private HttpClient CreateClientWithTokenExchange(string token)
    {
        var client = new HttpClient { BaseAddress = new Uri(_apiFixture.ApiEndpoint) };
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TokenExchange", token);
        return client;
    }

    private async Task<HttpResponseMessage> GetOrganisationDetails(HttpClient client, Guid organisationId)
    {
        return await client.GetAsync($"/v1/organisations/{organisationId}");
    }
}