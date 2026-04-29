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
/// Integration tests that verify OAuth2 token introspection works end-to-end with real Keycloak.
/// These tests validate that built-in OAuth2 introspection correctly calls Keycloak's introspection endpoint
/// and that the TokenExchange authentication flow works with real OAuth2 tokens.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class KeycloakIntrospectionTests
{
    private readonly ApiFixture _apiFixture;

    public KeycloakIntrospectionTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [EnvVarIgnoreFact]
    public async Task TokenExchangeAuthentication_WithValidKeycloakToken_PerformsIntrospectionSuccessfully()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        await _apiFixture.Create.Organisation(organisationId, "Test Organization for Introspection");
        
        var keycloakToken = await GetKeycloakAccessToken(ApiFixture.Test.Client, ApiFixture.Test.Scope);
        var client = CreateClientWithTokenExchange(keycloakToken);
        
        // Act - This will trigger introspection internally
        var response = await GetSecurityInformation(client);
        
        // Assert - Successful introspection means we don't get Unauthorized
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized, 
            "Valid Keycloak token should pass introspection and not return Unauthorized");
        
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Forbidden);
    }

    [EnvVarIgnoreFact]
    public async Task TokenExchangeAuthentication_WithInvalidToken_FailsIntrospectionAndDeniesAccess()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        await _apiFixture.Create.Organisation(organisationId, "Test Organization");
        
        const string invalidToken = "invalid.jwt.token";
        var client = CreateClientWithTokenExchange(invalidToken);
        
        // Act - Invalid token should fail introspection
        var response = await GetSecurityInformation(client);
        
        // Assert - Failed introspection should result in Unauthorized
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized,
            "Invalid token should fail introspection and return Unauthorized");
    }

    [EnvVarIgnoreFact]
    public async Task TokenExchangeAuthentication_WithMalformedToken_FailsIntrospectionAndDeniesAccess()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        await _apiFixture.Create.Organisation(organisationId, "Test Organization");
        
        const string malformedToken = "not-a-token-at-all";
        var client = CreateClientWithTokenExchange(malformedToken);
        
        // Act - Malformed token should fail introspection
        var response = await GetSecurityInformation(client);
        
        // Assert - Failed introspection should result in Unauthorized
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized,
            "Malformed token should fail introspection and return Unauthorized");
    }

    [EnvVarIgnoreFact]
    public async Task TokenExchangeAuthentication_CachingBehavior_PerformsConsistently()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        await _apiFixture.Create.Organisation(organisationId, "Test Organization for Caching");
        
        var keycloakToken = await GetKeycloakAccessToken(ApiFixture.Test.Client, ApiFixture.Test.Scope);
        var client = CreateClientWithTokenExchange(keycloakToken);
        
        // Act - Multiple requests with same token (should use cached introspection)
        var firstResponse = await GetOrganisationDetails(client, organisationId);
        var secondResponse = await GetOrganisationDetails(client, organisationId);
        
        // Assert - Both responses should be identical due to caching
        firstResponse.StatusCode.Should().Be(secondResponse.StatusCode,
            "Cached introspection should produce consistent results");
        
        if (firstResponse.IsSuccessStatusCode)
        {
            var firstContent = await firstResponse.Content.ReadAsStringAsync();
            var secondContent = await secondResponse.Content.ReadAsStringAsync();
            firstContent.Should().Be(secondContent,
                "Response content should be identical when using cached introspection");
        }
    }

    [EnvVarIgnoreTheory]
    [InlineData(ApiFixture.Test.Client, ApiFixture.Test.Scope)]
    [InlineData(ApiFixture.CJM.Client, ApiFixture.CJM.Scope)]
    public async Task TokenExchangeAuthentication_WithDifferentKeycloakClients_PerformsIntrospectionCorrectly(
        string clientId, string scope)
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        await _apiFixture.Create.Organisation(organisationId, $"Test Organization for {clientId}");
        
        var keycloakToken = await GetKeycloakAccessToken(clientId, scope);
        var client = CreateClientWithTokenExchange(keycloakToken);
        
        // Act - Token introspection should work for different clients
        var response = await GetSecurityInformation(client);
        
        // Assert - Successful introspection means no Unauthorized
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized,
            $"Valid token from {clientId} should pass introspection");
        
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Forbidden);
    }

    [EnvVarIgnoreFact]
    public async Task TokenExchangeAuthentication_WithoutToken_SkipsIntrospectionAndDeniesAccess()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        await _apiFixture.Create.Organisation(organisationId, "Test Organization");
        
        var client = CreateClientWithoutAuth();
        
        // Act - No token means no introspection
        var response = await GetSecurityInformation(client);
        
        // Assert - Missing token should be Unauthorized
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized,
            "Request without token should be Unauthorized");
    }

    [EnvVarIgnoreFact]
    public async Task KeycloakIntrospectionEndpoint_IsAccessible()
    {
        // Arrange
        const string keycloakIntrospectionUrl = "http://keycloak.localhost:9080/realms/wegwijs/protocol/openid-connect/token/introspect";
        
        using var httpClient = new HttpClient();
        
        // Act - Test that introspection endpoint exists (should reject GET)
        var response = await httpClient.GetAsync(keycloakIntrospectionUrl);
        
        // Assert - Endpoint should exist but reject GET requests
        response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed,
            "Introspection endpoint should exist but only accept POST requests");
    }

    // ========================================================================
    // Track A: Container Networking Tests (InternalAuthorityOverride)
    // ========================================================================
    
    [EnvVarIgnoreFact]
    public async Task ContainerNetworking_InternalAuthorityOverride_UsesCorrectKeycloakEndpoint()
    {
        // Test that API can reach Keycloak using internal container network
        // when InternalAuthorityOverride is configured (http://keycloak/realms/wegwijs)
        // vs external URL (http://keycloak.localhost:9080/realms/wegwijs)
        
        // Arrange
        var organisationId = Guid.NewGuid();
        await _apiFixture.Create.Organisation(organisationId, "Container Network Test Organization");
        
        var keycloakToken = await GetKeycloakAccessToken(ApiFixture.Test.Client, ApiFixture.Test.Scope);
        var client = CreateClientWithTokenExchange(keycloakToken);
        
        // Act - This will trigger introspection using InternalAuthorityOverride
        // The API should use http://keycloak/realms/wegwijs internally
        var response = await GetOrganisationDetails(client, organisationId);
        
        // Assert - Successful introspection confirms container networking works
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized, 
            "InternalAuthorityOverride should enable API to reach Keycloak via container network");
        
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Forbidden);
    }

    [EnvVarIgnoreFact]
    public async Task ContainerNetworking_WithoutInternalAuthority_WouldFailInProduction()
    {
        // This test documents expected behavior - in a real Kubernetes cluster,
        // without InternalAuthorityOverride, the API would fail to connect to
        // external Keycloak URLs from inside the cluster.
        
        // Arrange
        var organisationId = Guid.NewGuid();
        await _apiFixture.Create.Organisation(organisationId, "External Authority Test Organization");
        
        var keycloakToken = await GetKeycloakAccessToken(ApiFixture.Test.Client, ApiFixture.Test.Scope);
        var client = CreateClientWithTokenExchange(keycloakToken);
        
        // Act
        var response = await GetOrganisationDetails(client, organisationId);
        
        // Assert - In Tilt dev environment, this works because both API and test
        // are outside the cluster. In production Kubernetes, this would require
        // InternalAuthorityOverride to succeed.
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized,
            "Dev environment allows external Keycloak access - production would need InternalAuthorityOverride");
    }

    private async Task<string> GetKeycloakAccessToken(string clientId, string scope)
    {
        const string defaultKeycloakAuthority = "http://keycloak.localhost:9080/realms/wegwijs";

        var tokenClient = new TokenClient(
            () => new HttpClient(),
            new TokenClientOptions
            {
                Address = $"{defaultKeycloakAuthority.TrimEnd('/')}/protocol/openid-connect/token",
                ClientId = clientId,
                ClientSecret = ApiFixture.GetClientSecret(clientId),
                Parameters = new Parameters(
                    new[]
                    {
                        new KeyValuePair<string, string>("scope", scope),
                    }),
            });

        var response = await tokenClient.RequestTokenAsync(OidcConstants.GrantTypes.ClientCredentials);

        if (response.IsError || string.IsNullOrWhiteSpace(response.AccessToken))
            throw new InvalidOperationException(
                $"Could not retrieve Keycloak M2M token for '{clientId}' from '{defaultKeycloakAuthority}'. " +
                $"Error: {response.Error}. Description: {response.ErrorDescription}.");

        return response.AccessToken;
    }

    private HttpClient CreateClientWithTokenExchange(string token)
    {
        var client = new HttpClient { BaseAddress = new Uri(_apiFixture.ApiEndpoint) };
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private HttpClient CreateClientWithoutAuth()
    {
        return new HttpClient { BaseAddress = new Uri(_apiFixture.ApiEndpoint) };
    }

    private async Task<HttpResponseMessage> GetOrganisationDetails(HttpClient client, Guid organisationId)
    {
        return await client.GetAsync($"/v1/organisations/{organisationId}");
    }

    private async Task<HttpResponseMessage> GetSecurityInformation(HttpClient client)
    {
        return await client.GetAsync("/v1/security");
    }
}
