namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Buildings.When_Updating_Buildings;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Building;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_CanManageBuildings
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_CanManageBuildings(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_OK()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        // Algemeenbeheerder heeft een ongescopete CanManageBuildings-toekenning: elke organisatie is toegestaan.
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = await AddBuilding(client, organisationId);

        var response = await UpdateBuilding(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOwnOrganisation_Then_Returns_OK()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        // OVO000002 is de eigen organisatie van de decentraalbeheerder-persona en valt dus binnen de scope.
        var organisationId = _apiFixture.DecentraalbeheerderOrganisationId;
        var entityId = await AddBuilding(client, organisationId);

        var response = await UpdateBuilding(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithChildOrganisationInScope_Then_Returns_OK()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        // De scope van een decentraalbeheerder omvat de volledige boom (OrganisationTree), dus ook dochterorganisaties.
        var organisationId = _apiFixture.DecentraalbeheerderChildOrganisationId;
        var entityId = await AddBuilding(client, organisationId);

        var response = await UpdateBuilding(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOrganisationOutsideScope_Then_Returns_Forbidden()
    {
        var privilegedClient = await _apiFixture.CreateAlgemeenbeheerderClient();
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        // De organisatie valt buiten de scope van de decentraalbeheerder. Het bestaande gebouw wordt met een
        // bevoorrechte client aangemaakt, waarna de update door de decentraalbeheerder geweigerd moet worden.
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = await AddBuilding(privilegedClient, organisationId);

        var response = await UpdateBuilding(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<Guid> AddBuilding(HttpClient client, Guid organisationId)
    {
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var buildingId = await _apiFixture.Create.Building();

        await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/buildings",
            new AddOrganisationBuildingRequest
            {
                OrganisationBuildingId = entityId,
                BuildingId = buildingId,
                IsMainBuilding = false,
                ValidFrom = null,
                ValidTo = null,
            });

        return entityId;
    }

    private async Task<HttpResponseMessage> UpdateBuilding(HttpClient client, Guid organisationId, Guid entityId)
    {
        var buildingId = await _apiFixture.Create.Building();

        return await ApiFixture.Put(
            client,
            $"/v1/organisations/{organisationId}/buildings/{entityId}",
            new UpdateOrganisationBuildingRequest
            {
                OrganisationBuildingId = entityId,
                BuildingId = buildingId,
                IsMainBuilding = false,
                ValidFrom = null,
                ValidTo = null,
            });
    }
}
