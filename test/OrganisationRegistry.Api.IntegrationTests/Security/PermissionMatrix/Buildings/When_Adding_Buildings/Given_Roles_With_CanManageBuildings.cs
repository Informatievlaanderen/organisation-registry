namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Buildings.When_Adding_Buildings;

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
    public async Task For_Algemeenbeheerder_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        var response = await AddBuilding(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOwnOrganisation_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var response = await AddBuilding(client, _apiFixture.DecentraalbeheerderOrganisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithChildOrganisationInScope_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var response = await AddBuilding(client, _apiFixture.DecentraalbeheerderChildOrganisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOrganisationOutsideScope_Then_Returns_Forbidden()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        var response = await AddBuilding(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<HttpResponseMessage> AddBuilding(HttpClient client, Guid organisationId)
    {
        var buildingId = await _apiFixture.Create.Building();

        return await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/buildings",
            new AddOrganisationBuildingRequest
            {
                OrganisationBuildingId = _apiFixture.Fixture.Create<Guid>(),
                BuildingId = buildingId,
                IsMainBuilding = false,
                ValidFrom = null,
                ValidTo = null,
            });
    }
}
