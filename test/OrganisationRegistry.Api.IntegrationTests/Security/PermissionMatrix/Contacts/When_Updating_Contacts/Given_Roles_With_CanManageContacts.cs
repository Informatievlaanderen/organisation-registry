namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Contacts.When_Updating_Contacts;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Contact;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_CanManageContacts
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_CanManageContacts(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        var entityId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        await ExecuteCreateRequest(client, organisationId, entityId);

        var response = await ExecuteUpdateRequest(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task ExecuteCreateRequest(HttpClient client, Guid organisationId, Guid entityId)
    {
        await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/contacts",
            new AddOrganisationContactRequest
            {
                ContactValue = _apiFixture.Fixture.Create<string>(),
                ContactTypeId = await _apiFixture.Create.ContactType(),
                OrganisationContactId = entityId,
                ValidFrom = null,
                ValidTo = null,
            });
    }

    private async Task<HttpResponseMessage> ExecuteUpdateRequest(HttpClient client, Guid organisationId, Guid entityId)
    {
        var response = await ApiFixture.Put(
            client,
            $"/v1/organisations/{organisationId}/contacts/{entityId}",
            new UpdateOrganisationContactRequest()
            {
                OrganisationContactId = entityId,
                ContactValue = _apiFixture.Fixture.Create<string>(),
                ContactTypeId = await _apiFixture.Create.ContactType(),
                ValidFrom = null,
                ValidTo = null,
            });
        return response;
    }
}
