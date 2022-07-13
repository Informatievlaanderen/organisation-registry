namespace OrganisationRegistry.Api.IntegrationTests.EditApi;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Edit.Organisation.Classification;
using FluentAssertions;
using Tests.Shared;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class CreateOrganisationOrganisationClassificationTests
{
    private readonly ApiFixture _apiFixture;
    private readonly Guid _organisationId;

    public CreateOrganisationOrganisationClassificationTests(ApiFixture apiApiFixture)
    {
        _apiFixture = apiApiFixture;
        _organisationId = _apiFixture.Fixture.Create<Guid>();
    }

    [EnvVarIgnoreFact]
    public async Task WithoutBearer_ReturnsUnauthorized()
    {
        await _apiFixture.CreateOrganisation(_organisationId, _apiFixture.Fixture.Create<string>());
        var organisationClassificationTypeId = await _apiFixture.CreateOrganisationClassificationType(true);
        var organisationClassificationId = await _apiFixture.CreateOrganisationClassification(organisationClassificationTypeId);

        var response = await AddOrganisationOrganisationClassification(
            _apiFixture.HttpClient,
            _organisationId,
            organisationClassificationTypeId,
            organisationClassificationId);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [EnvVarIgnoreTheory]
    [InlineData(ApiFixture.CJM.Client, ApiFixture.CJM.Scope)]
    [InlineData(ApiFixture.Test.Client, ApiFixture.Test.Scope)]
    public async Task CanCreateAndUpdateAs(string client, string scope)
    {
        await _apiFixture.CreateOrganisation(_organisationId, _apiFixture.Fixture.Create<string>());

        var organisationClassificationType1Id = _apiFixture.Configuration.Authorization.OrganisationClassificationTypeIdsOwnedByCjm.First();
        var organisationClassification1Id = await _apiFixture.CreateOrganisationClassification(organisationClassificationType1Id);
        var organisationClassificationType2Id = _apiFixture.Configuration.Authorization.OrganisationClassificationTypeIdsOwnedByCjm.First();
        var organisationClassification2Id = await _apiFixture.CreateOrganisationClassification(organisationClassificationType2Id);

        var httpClient = await _apiFixture.CreateMachine2MachineClientFor(client, scope);

        var organisationOrganisationClassificationId = await CreateAndVerify(httpClient, organisationClassificationType1Id, organisationClassification1Id);

        await UpdateAndVerify(httpClient, organisationOrganisationClassificationId, organisationClassificationType2Id, organisationClassification2Id);
    }

    [EnvVarIgnoreTheory]
    [InlineData(ApiFixture.Orafin.Client, ApiFixture.Orafin.Scope)]
    public async Task CannotCreateAs(string client, string scope)
    {
        await _apiFixture.CreateOrganisation(_organisationId, _apiFixture.Fixture.Create<string>());

        var organisationClassificationTypeId = await _apiFixture.CreateOrganisationClassificationType(false);
        var organisationClassificationId = await _apiFixture.CreateOrganisationClassification(organisationClassificationTypeId);

        var httpClient = await _apiFixture.CreateMachine2MachineClientFor(client, scope);

        var response = await Create(httpClient, organisationClassificationTypeId, organisationClassificationId);
        await ApiFixture.VerifyStatusCode(response, HttpStatusCode.Forbidden);
    }

    [EnvVarIgnoreFact]
    public async Task CannotCreateAsCjm()
    {
        await _apiFixture.CreateOrganisation(_organisationId, _apiFixture.Fixture.Create<string>());

        var organisationClassificationTypeId = await _apiFixture.CreateOrganisationClassificationType(false);
        var organisationClassificationId = await _apiFixture.CreateOrganisationClassification(organisationClassificationTypeId);

        var response = await Create(await _apiFixture.CreateCjmClient(), organisationClassificationTypeId, organisationClassificationId);
        await ApiFixture.VerifyStatusCode(response, HttpStatusCode.BadRequest);
    }

    [EnvVarIgnoreFact]
    public async Task CannotUpdateAsCjm()
    {
        await _apiFixture.CreateOrganisation(_organisationId, _apiFixture.Fixture.Create<string>());

        var organisationClassificationTypeId = await _apiFixture.CreateOrganisationClassificationType(false);
        var organisationClassificationId = await _apiFixture.CreateOrganisationClassification(organisationClassificationTypeId);
        var organisationOrganisationClassificationId = await _apiFixture.CreateOrganisationOrganisationClassification(_organisationId, organisationClassificationTypeId, organisationClassificationId);

        var response = await Update(await _apiFixture.CreateCjmClient(), organisationOrganisationClassificationId, organisationClassificationTypeId, organisationClassificationId);
        await ApiFixture.VerifyStatusCode(response, HttpStatusCode.BadRequest);
    }

    private async Task<Guid> CreateAndVerify(HttpClient httpClient, Guid organisationClassificationTypeId, Guid organisationClassificationId)
    {
        var response = await Create(httpClient, organisationClassificationTypeId, organisationClassificationId);
        await ApiFixture.VerifyStatusCode(response, HttpStatusCode.Created);
        var organisationOrganisationClassificationId = ApiFixture.GetIdFrom(response.Headers);

        var getResponse = await ApiFixture.Get(httpClient, $"/v1/organisations/{_organisationId}/classifications/{organisationOrganisationClassificationId}");
        await ApiFixture.VerifyStatusCode(getResponse, HttpStatusCode.OK);
        VerifyResponse(await ApiFixture.Deserialize(getResponse), organisationClassificationTypeId, organisationClassificationId);

        return organisationOrganisationClassificationId;
    }

    private async Task<HttpResponseMessage> Create(HttpClient httpClient, Guid organisationClassificationTypeId, Guid organisationClassificationId)
        => await AddOrganisationOrganisationClassification(
            httpClient,
            _organisationId,
            organisationClassificationTypeId,
            organisationClassificationId);

    private async Task UpdateAndVerify(HttpClient httpClient, Guid organisationOrganisationClassificationId, Guid organisationClassificationTypeId, Guid organisationClassificationId)
    {
        var validFrom = _apiFixture.Fixture.Create<DateTime>().Date;
        var validTo = validFrom.AddDays(1);
        var updateResponse = await Update(httpClient, organisationOrganisationClassificationId, organisationClassificationTypeId, organisationClassificationId, validFrom, validTo);
        await ApiFixture.VerifyStatusCode(updateResponse, HttpStatusCode.OK);

        var getResponse = await ApiFixture.Get(httpClient, $"/v1/organisations/{_organisationId}/classifications/{organisationOrganisationClassificationId}");
        await ApiFixture.VerifyStatusCode(getResponse, HttpStatusCode.OK);
        VerifyResponse(await ApiFixture.Deserialize(getResponse), organisationClassificationTypeId, organisationClassificationId, validFrom, validTo);
    }

    private async Task<HttpResponseMessage> Update(
        HttpClient httpClient,
        Guid organisationOrganisationClassificationId,
        Guid organisationClassificationTypeId,
        Guid organisationClassificationId,
        DateTime? validFrom = null,
        DateTime? validTo = null)
        => await UpdateOrganisationOrganisationClassification(
            httpClient,
            _organisationId,
            organisationOrganisationClassificationId,
            organisationClassificationTypeId,
            organisationClassificationId,
            validFrom,
            validTo);

    private static void VerifyResponse(IReadOnlyDictionary<string, object> deserializedResponse, Guid organisationClassificationTypeId, Guid organisationClassificationId, DateTime? validFrom = null, DateTime? validTo = null)
    {
        deserializedResponse["organisationClassificationTypeId"].Should().Be(organisationClassificationTypeId.ToString());
        deserializedResponse["organisationClassificationId"].Should().Be(organisationClassificationId.ToString());
        deserializedResponse["validFrom"].Should().Be(validFrom);
        deserializedResponse["validTo"].Should().Be(validTo);
    }

    private static async Task<HttpResponseMessage> AddOrganisationOrganisationClassification(
        HttpClient httpClient,
        Guid organisationId,
        Guid organisationClassificationTypeId,
        Guid organisationClassificationId)
        => await ApiFixture.Post(
            httpClient,
            $"/v1/edit/organisations/{organisationId}/classifications",
            new AddOrganisationOrganisationClassificationRequest
            {
                OrganisationClassificationTypeId = organisationClassificationTypeId,
                OrganisationClassificationId = organisationClassificationId,
                ValidFrom = null,
                ValidTo = null,
            });

    private static async Task<HttpResponseMessage> UpdateOrganisationOrganisationClassification(
        HttpClient httpClient,
        Guid organisationId,
        Guid organisationOrganisationClassificationId,
        Guid organisationClassificationTypeId,
        Guid organisationClassificationId,
        DateTime? validFrom = null,
        DateTime? validTo = null)
        => await ApiFixture.Put(
            httpClient,
            $"/v1/edit/organisations/{organisationId}/classifications/{organisationOrganisationClassificationId}",
            new UpdateOrganisationOrganisationClassificationRequest
            {
                OrganisationClassificationTypeId = organisationClassificationTypeId,
                OrganisationClassificationId = organisationClassificationId,
                ValidFrom = validFrom,
                ValidTo = validTo,
            });
}
