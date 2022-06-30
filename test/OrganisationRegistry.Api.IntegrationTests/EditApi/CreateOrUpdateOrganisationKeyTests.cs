namespace OrganisationRegistry.Api.IntegrationTests.EditApi;

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Edit.Organisation.Key;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class CreateOrUpdateOrganisationKeyTests
{
    private readonly ApiFixture _fixture;
    private readonly Guid _orafinKeyType;

    public CreateOrUpdateOrganisationKeyTests(ApiFixture fixture)
    {
        _fixture = fixture;
        _orafinKeyType = _fixture.Configuration.Authorization.KeyIdsAllowedOnlyForOrafin.First();

        CreateKeyType();
    }

    [Fact]
    public async Task WithoutBearer_ReturnsUnauthorized()
    {
        var organisationId = Guid.NewGuid();
        var createResponse = await CreateOrganisation(organisationId);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var response = await CreateKey(
            organisationId,
            Guid.NewGuid(),
            _fixture.HttpClient);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AsOrafin_CanCreateAndUpdate()
    {
        var organisationId = Guid.NewGuid();
        var createResponse = await CreateOrganisation(organisationId);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var httpClient = await _fixture.CreateOrafinClient();

        var organisationKeyId = Guid.NewGuid();
        var response = await CreateKey(organisationId, organisationKeyId, httpClient);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var updateResponse = await UpdateKey(organisationId, organisationKeyId, httpClient);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<HttpResponseMessage> UpdateKey(Guid organisationId, Guid organisationKeyId, HttpClient httpClient)
    {
        var updateResponse = await httpClient.PutAsync(
            $"edit/organisations/{organisationId}/keys/{organisationKeyId}",
            new StringContent(
                JsonConvert.SerializeObject(
                    new AddOrganisationKeyRequest
                    {
                        KeyTypeId = _orafinKeyType,
                        KeyValue = "updates value",
                        OrganisationKeyId = organisationKeyId
                    }),
                Encoding.UTF8,
                "application/json"));
        return updateResponse;
    }

    [Fact]
    public async Task AsCjmBeheerder_ReturnsForbidden()
    {
        var organisationId = Guid.NewGuid();
        var createResponse = await CreateOrganisation(organisationId);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var httpClient = await _fixture.CreateCjmClient();

        var organisationKeyId = Guid.NewGuid();
        var response = await CreateKey(organisationId, organisationKeyId, httpClient);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<HttpResponseMessage> CreateOrganisation(Guid organisationId)
    {
        var createResponse = await _fixture.HttpClient.PostAsync(
            "organisations",
            new StringContent(
                $"{{'id': '{organisationId}', 'name': 'test for keys'}}",
                Encoding.UTF8,
                "application/json"));
        return createResponse;
    }

    private void CreateKeyType()
    {
        _fixture.HttpClient.PostAsync(
            "keytypes",
            new StringContent(
                $"{{'id': '{_orafinKeyType}', 'name': 'orafin key type'}}",
                Encoding.UTF8,
                "application/json")).GetAwaiter().GetResult();
    }

    private async Task<HttpResponseMessage> CreateKey(Guid organisationId, Guid organisationKeyId, HttpClient fixtureHttpClient)
    {
        var response = await fixtureHttpClient.PostAsync(
            $"edit/organisations/{organisationId}/keys",
            new StringContent(
                JsonConvert.SerializeObject(
                    new AddOrganisationKeyRequest
                    {
                        KeyTypeId = _orafinKeyType,
                        KeyValue = "test keys",
                        OrganisationKeyId = organisationKeyId,
                    }),
                Encoding.UTF8,
                "application/json"));
        return response;
    }
}
