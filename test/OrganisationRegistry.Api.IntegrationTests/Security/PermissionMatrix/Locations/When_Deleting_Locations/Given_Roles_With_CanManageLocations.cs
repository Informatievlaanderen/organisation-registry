namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Locations.When_Deleting_Locations;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Location;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_CanManageLocations
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_CanManageLocations(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_NoContent()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = await AddLocation(client, organisationId);

        var response = await DeleteLocation(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOwnOrganisation_Then_Returns_NoContent()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.DecentraalbeheerderOrganisationId;
        var entityId = await AddLocation(client, organisationId);

        var response = await DeleteLocation(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithChildOrganisationInScope_Then_Returns_NoContent()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.DecentraalbeheerderChildOrganisationId;
        var entityId = await AddLocation(client, organisationId);

        var response = await DeleteLocation(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOrganisationOutsideScope_Then_Returns_Forbidden()
    {
        var privilegedClient = await _apiFixture.CreateAlgemeenbeheerderClient();
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = await AddLocation(privilegedClient, organisationId);

        var response = await DeleteLocation(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<Guid> AddLocation(HttpClient client, Guid organisationId)
    {
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var locationId = await _apiFixture.Create.Location();

        await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/locations",
            new AddOrganisationLocationRequest()
            {
                OrganisationLocationId = entityId,
                LocationId = locationId,
                IsMainLocation = false,
                LocationTypeId = null,
                ValidFrom = null,
                ValidTo = null,
            });

        return entityId;
    }


    private async Task<HttpResponseMessage> DeleteLocation(HttpClient client, Guid organisationId, Guid entityId)
    {
        return await ApiFixture.Delete(
            client,
            $"/v1/organisations/{organisationId}/locations/{entityId}");
    }
}
