namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Contacts.When_Adding_Contacts;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using Backoffice.Organisation.Contact;
using FluentAssertions;
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
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        var response = await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/contacts",
            new AddOrganisationContactRequest
            {
                ContactValue = _apiFixture.Fixture.Create<string>(),
                ContactTypeId = await _apiFixture.Create.ContactType(),
                OrganisationContactId = _apiFixture.Fixture.Create<Guid>(),
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
