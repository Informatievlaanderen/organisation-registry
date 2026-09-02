namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Contacts.When_Adding_Contacts;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Contact;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_CanManageContacts
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_CanManageContacts(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Decentraalbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orgaanbeheerder)]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder)]
    public async Task Then_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

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

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
