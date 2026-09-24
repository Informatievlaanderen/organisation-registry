namespace OrganisationRegistry.Api.IntegrationTests.EditApi;

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Edit.Organisation.Key;
using Tests.Shared;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class CreateOrUpdateOrganisationKeyTests
{
    private const string TestOrganisationName = "test for keys";
    private readonly ApiFixture _apiFixture;
    private readonly Guid _orafinKeyType;

    public CreateOrUpdateOrganisationKeyTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
        _orafinKeyType = _apiFixture.Configuration.Authorization.KeyIdsAllowedOnlyForOrafin.First();
    }

    [EnvVarIgnoreFact]
    public async Task WithoutBearer_ReturnsUnauthorized()
    {
        var organisationId = Guid.NewGuid();
        await _apiFixture.Create.Organisation(organisationId, TestOrganisationName);

        var response = await CreateKey(
            organisationId,
            _apiFixture.CreateAnonymousClient(),
            _orafinKeyType);

        await ApiFixture.VerifyStatusCode(response, HttpStatusCode.Unauthorized);
    }

    [EnvVarIgnoreTheory]
    [InlineData(ApiFixture.Orafin.Client, ApiFixture.Orafin.Scope)]
    [InlineData(ApiFixture.CJM.Client, ApiFixture.CJM.Scope)]
    public async Task CannotCreateAndUpdateAs(string client, string scope)
    {
        var organisationId = Guid.NewGuid();
        await _apiFixture.Create.Organisation(organisationId, TestOrganisationName);

        var httpClient = await _apiFixture.CreateMachine2MachineClientFor(client, scope);

        var response = await CreateKey(organisationId, httpClient, _orafinKeyType);

        await ApiFixture.VerifyStatusCode(response, HttpStatusCode.Forbidden);
    }

    [EnvVarIgnoreFact]
    public async Task AsTestClient_ReturnsCreatedAndCanUpdate()
    {
        var organisationId = Guid.NewGuid();
        await _apiFixture.Create.Organisation(organisationId, TestOrganisationName);

        var keyTypeId = await _apiFixture.Create.KeyType();
        var httpClient = await _apiFixture.CreateTestClient();

        var createResponse = await CreateKey(organisationId, httpClient, keyTypeId);
        await ApiFixture.VerifyStatusCode(createResponse, HttpStatusCode.Created);
        var organisationKeyId = ApiFixture.GetIdFrom(createResponse.Headers);

        var updateResponse = await UpdateKey(organisationId, organisationKeyId, httpClient, keyTypeId);
        await ApiFixture.VerifyStatusCode(updateResponse, HttpStatusCode.OK);
    }

    private static async Task<HttpResponseMessage> UpdateKey(Guid organisationId, Guid organisationKeyId, HttpClient httpClient, Guid orafinKeyType)
        => await ApiFixture.Put(
            httpClient,
            $"/v1/edit/organisations/{organisationId}/keys/{organisationKeyId}",
            new UpdateOrganisationKeyRequest
            {
                KeyTypeId = orafinKeyType,
                KeyValue = "updates value",
            });

    private static async Task<HttpResponseMessage> CreateKey(Guid organisationId, HttpClient fixtureHttpClient, Guid orafinKeyType)
        => await ApiFixture.Post(
            fixtureHttpClient,
            $"/v1/edit/organisations/{organisationId}/keys",
            new AddOrganisationKeyRequest
            {
                KeyTypeId = orafinKeyType,
                KeyValue = "test keys",
            });
}
