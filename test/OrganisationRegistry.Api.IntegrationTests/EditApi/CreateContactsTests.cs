namespace OrganisationRegistry.Api.IntegrationTests.EditApi;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using BackOffice.Organisation;
using FluentAssertions;
using Tests.Shared;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class CreateContactsTests
{
    private const string TestOrganisationForCreateContacts = "test organisation for createContacts";
    private const string ContactTypeName = "contactTypeName";
    private readonly ApiFixture _apiFixture;
    private readonly Guid _organisationId;
    private readonly OrganisationHelpers _helpers;

    public CreateContactsTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
        _helpers = new OrganisationHelpers(_apiFixture);
        _organisationId = _apiFixture.Fixture.Create<Guid>();
    }

    [EnvVarIgnoreFact]
    public async Task WithoutBearer_ReturnsUnauthorized()
    {
        var response = await CreateContacts(
            _apiFixture.HttpClient,
            _apiFixture.Fixture.Create<Guid>(),
            _apiFixture.Fixture.Create<Guid>(),
            _apiFixture.Fixture.Create<string>());

        await ApiFixture.VerifyStatusCode(response, HttpStatusCode.Unauthorized);
    }

    [EnvVarIgnoreFact]
    public async Task AsOrafinBeheerder_ReturnsForbidden()
    {
        var response = await CreateContacts(
            await _apiFixture.CreateMachine2MachineClientFor(ApiFixture.Orafin.Client, ApiFixture.Orafin.Scope),
            _apiFixture.Fixture.Create<Guid>(),
            _apiFixture.Fixture.Create<Guid>(),
            _apiFixture.Fixture.Create<string>());
        await ApiFixture.VerifyStatusCode(response, HttpStatusCode.Forbidden);
    }

    [EnvVarIgnoreFact]
    public async Task AsCJM_CanAddAndUpdate()
    {
        await _helpers.CreateOrganisation(_organisationId, TestOrganisationForCreateContacts);
        var contactTypeId = await _helpers.CreateContactType(ContactTypeName);

        var httpClient = await _apiFixture.CreateMachine2MachineClientFor(ApiFixture.CJM.Client, ApiFixture.CJM.Scope);
        var organisationContactId = await CreatAndVerify(httpClient, contactTypeId);

        await UpdateAndVerify(httpClient, contactTypeId, organisationContactId);
    }

    private async Task UpdateAndVerify(HttpClient httpClient, Guid contactTypeId, Guid organisationContactId)
    {
        var value = _apiFixture.Fixture.Create<string>();

        var response = await UpdateContacts(
            httpClient,
            _organisationId,
            organisationContactId,
            contactTypeId,
            value);
        await ApiFixture.VerifyStatusCode(response, HttpStatusCode.OK);

        var getResponse = await ApiFixture.Get(httpClient, $"/v1/organisations/{_organisationId}/contacts/{organisationContactId}");
        await ApiFixture.VerifyStatusCode(getResponse, HttpStatusCode.OK);
        VerifyResponse(await ApiFixture.Deserialize(getResponse), contactTypeId, value);
    }

    private async Task<Guid> CreatAndVerify(HttpClient httpClient, Guid contactTypeId)
    {
        var value = _apiFixture.Fixture.Create<string>();

        var response = await CreateContacts(
            httpClient,
            _organisationId,
            contactTypeId,
            value);
        await ApiFixture.VerifyStatusCode(response, HttpStatusCode.Created);
        var organisationContactId = ApiFixture.GetIdFrom(response.Headers);

        var getResponse = await ApiFixture.Get(httpClient, $"/v1/organisations/{_organisationId}/contacts/{organisationContactId}");
        await ApiFixture.VerifyStatusCode(getResponse, HttpStatusCode.OK);
        VerifyResponse(await ApiFixture.Deserialize(getResponse), contactTypeId, value);

        return organisationContactId;
    }

    private static void VerifyResponse(IReadOnlyDictionary<string, object> deserializedResponse, Guid contactTypeId, string value)
    {
        deserializedResponse["contactTypeId"].Should().Be(contactTypeId.ToString());
        deserializedResponse["contactTypeName"].Should().Be(ContactTypeName);
        deserializedResponse["contactValue"].Should().Be(value);
        deserializedResponse["validFrom"].Should().BeNull();
        deserializedResponse["validTo"].Should().BeNull();
    }

    private static async Task<HttpResponseMessage> CreateContacts(HttpClient httpClient, Guid organisationId, Guid contactType, string value)
        => await ApiFixture.Post(
            httpClient,
            $"/v1/edit/organisations/{organisationId.ToString()}/contacts",
            new
            {
                ContactTypeId = contactType,
                ContactValue = value,
            });

    private static async Task<HttpResponseMessage> UpdateContacts(HttpClient httpClient, Guid organisationId, Guid organisationContactId, Guid contactType, string value)
        => await ApiFixture.Put(
            httpClient,
            $"/v1/edit/organisations/{organisationId.ToString()}/contacts/{organisationContactId.ToString()}",
            new
            {
                ContactTypeId = contactType,
                ContactValue = value,
            });
}
