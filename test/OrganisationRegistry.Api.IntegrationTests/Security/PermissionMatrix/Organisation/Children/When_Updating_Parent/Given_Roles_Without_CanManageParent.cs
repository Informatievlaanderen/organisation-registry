namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Children.When_Updating_Parent;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Parent;
using Xunit;

/// <summary>
/// Rollen zonder <c>CanManageParent</c> mogen de ouder/kind-structuur van een
/// organisatie niet aanpassen en krijgen 403 terug.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_CanManageParent
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_CanManageParent(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Cjmbeheerder)]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orgaanbeheerder)]
    public async Task Then_Returns_Forbidden(string role)
    {
        var childOrganisationId = await CreateOrganisation();
        var parentOrganisationId = await CreateOrganisation();
        var organisationParentId = await AddParentAsAlgemeenbeheerder(childOrganisationId, parentOrganisationId);

        var client = await _apiFixture.CreateDynamicClient(role);

        var response = await ApiFixture.Put(
            client,
            $"/v1/organisations/{childOrganisationId}/parents/{organisationParentId}",
            new UpdateOrganisationParentRequest
            {
                OrganisationOrganisationParentId = organisationParentId,
                ParentOrganisationId = parentOrganisationId,
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<Guid> CreateOrganisation()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        return organisationId;
    }

    private async Task<Guid> AddParentAsAlgemeenbeheerder(Guid childOrganisationId, Guid parentOrganisationId)
    {
        var organisationParentId = _apiFixture.Fixture.Create<Guid>();
        var algemeenbeheerderClient = await _apiFixture.CreateAlgemeenbeheerderClient();

        var response = await ApiFixture.Post(
            algemeenbeheerderClient,
            $"/v1/organisations/{childOrganisationId}/parents",
            new AddOrganisationParentRequest
            {
                OrganisationOrganisationParentId = organisationParentId,
                ParentOrganisationId = parentOrganisationId,
                ValidFrom = null,
                ValidTo = null,
            });
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        return organisationParentId;
    }
}
