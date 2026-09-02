namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Contacts.When_Updating_Contacts;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Contact;
using Xunit;

/// <summary>
/// Matrixrij <b>Sleutels</b> — UI-recht <c>canManageKeys</c> op de organisatieresponse.
/// Eén klasse per rij uit de rechtenmatrix (scherm/functionaliteit).
/// </summary>
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
        var entityId =  _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

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

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
