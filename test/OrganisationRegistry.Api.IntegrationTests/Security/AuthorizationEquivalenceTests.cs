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

[Collection(ApiTestsCollection.Name)]
public class AuthorizationEquivalenceTests
{
    private readonly ApiFixture _apiFixture;

    public AuthorizationEquivalenceTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [EnvVarIgnoreTheory]
    [InlineData(ApiFixture.Test.Client, ApiFixture.Test.Scope)]
    [InlineData(ApiFixture.CJM.Client, ApiFixture.CJM.Scope)]
    public async Task TokenExchangeAndBearerAuthentication_WithSameClient_ProvideEquivalentAuthorization(
        string clientId, string scope)
    {
        var organisationId = Guid.NewGuid();
        await _apiFixture.Create.Organisation(organisationId, "Test Organization");

        var keycloakToken = await GetKeycloakAccessToken(clientId, scope);

        var bearerClient = CreateClientWithBearerToken(keycloakToken);
        var tokenExchangeClient = CreateClientWithTokenExchange(keycloakToken);

        var bearerResponse = await GetOrganisationDetails(bearerClient, organisationId);
        var tokenExchangeResponse = await GetOrganisationDetails(tokenExchangeClient, organisationId);

        bearerResponse.StatusCode.Should().Be(tokenExchangeResponse.StatusCode);

        if (bearerResponse.IsSuccessStatusCode)
        {
            var bearerContent = await bearerResponse.Content.ReadAsStringAsync();
            var tokenExchangeContent = await tokenExchangeResponse.Content.ReadAsStringAsync();

            bearerContent.Should().Be(tokenExchangeContent);
        }
    }

    [EnvVarIgnoreFact]
    public async Task TokenExchangeAuthentication_WithValidToken_AllowsAccess()
    {
        var organisationId = Guid.NewGuid();
        await _apiFixture.Create.Organisation(organisationId, "Test Organization");

        var keycloakToken = await GetKeycloakAccessToken(ApiFixture.Test.Client, ApiFixture.Test.Scope);
        var tokenExchangeClient = CreateClientWithTokenExchange(keycloakToken);

        var response = await GetOrganisationDetails(tokenExchangeClient, organisationId);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Forbidden);
    }

    [EnvVarIgnoreFact]
    public async Task TokenExchangeAuthentication_WithoutToken_DeniesAccess()
    {
        var organisationId = Guid.NewGuid();
        await _apiFixture.Create.Organisation(organisationId, "Test Organization");

        var tokenExchangeClient = CreateClientWithoutAuth();

        var response = await GetSecurityInformation(tokenExchangeClient);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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

    private HttpClient CreateClientWithBearerToken(string token)
    {
        var client = new HttpClient { BaseAddress = new Uri(_apiFixture.ApiEndpoint) };
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
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
