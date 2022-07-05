namespace OrganisationRegistry.Api.IntegrationTests.EditApi;

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Edit.Organisation.Key;
using FluentAssertions;
using Tests.Shared;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class CreateOrUpdateOrganisationKeyTests
{
    private const string TestOrganisationName = "test for keys";
    private readonly ApiFixture _fixture;
    private readonly Guid _orafinKeyType;

    public CreateOrUpdateOrganisationKeyTests(ApiFixture fixture)
    {
        _fixture = fixture;
        _orafinKeyType = _fixture.Configuration.Authorization.KeyIdsAllowedOnlyForOrafin.First();
    }

    [EnvVarIgnoreFact]
    public async Task WithoutBearer_ReturnsUnauthorized()
    {
        var organisationId = Guid.NewGuid();
        await _fixture.CreateOrganisation(organisationId, TestOrganisationName);

        var response = await CreateKey(
            organisationId,
            Guid.NewGuid(),
            _fixture.HttpClient,
            _orafinKeyType);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [EnvVarIgnoreFact]
    public async Task AsOrafinBeheerder_CanCreateAndUpdate()
    {
        var organisationId = Guid.NewGuid();
        await _fixture.CreateOrganisation(organisationId, TestOrganisationName);

        var httpClient = await _fixture.CreateOrafinClient();

        var organisationKeyId = Guid.NewGuid();
        var response = await CreateKey(organisationId, organisationKeyId, httpClient, _orafinKeyType);

        await ApiFixture.VerifyStatusCode(response, HttpStatusCode.Created);

        var updateResponse = await UpdateKey(organisationId, organisationKeyId, httpClient, _orafinKeyType);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [EnvVarIgnoreFact]
    public async Task AsCjmBeheerder_ReturnsForbidden()
    {
        var organisationId = Guid.NewGuid();
        await _fixture.CreateOrganisation(organisationId, TestOrganisationName);

        var httpClient = await _fixture.CreateCjmClient();

        var organisationKeyId = Guid.NewGuid();
        var response = await CreateKey(organisationId, organisationKeyId, httpClient, _orafinKeyType);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private static async Task<HttpResponseMessage> UpdateKey(Guid organisationId, Guid organisationKeyId, HttpClient httpClient, Guid orafinKeyType)
        => await ApiFixture.Put(
            httpClient,
            $"edit/organisations/{organisationId}/keys/{organisationKeyId}",
            new AddOrganisationKeyRequest
            {
                KeyTypeId = orafinKeyType,
                KeyValue = "updates value",
                OrganisationKeyId = organisationKeyId,
            });

    private static async Task<HttpResponseMessage> CreateKey(Guid organisationId, Guid organisationKeyId, HttpClient fixtureHttpClient, Guid orafinKeyType)
        => await ApiFixture.Post(
            fixtureHttpClient,
            $"edit/organisations/{organisationId}/keys",
            new AddOrganisationKeyRequest
            {
                KeyTypeId = orafinKeyType,
                KeyValue = "test keys",
                OrganisationKeyId = organisationKeyId,
            });
}
