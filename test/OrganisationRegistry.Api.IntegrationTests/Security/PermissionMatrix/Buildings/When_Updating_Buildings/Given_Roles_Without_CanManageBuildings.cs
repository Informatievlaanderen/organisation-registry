namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Buildings.When_Updating_Buildings;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Building;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_CanManageBuildings
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_CanManageBuildings(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orgaanbeheerder)]
    public async Task Then_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var buildingId = await _apiFixture.Create.Building();

        var response = await ApiFixture.Put(
            client,
            $"/v1/organisations/{organisationId}/buildings/{entityId}",
            new UpdateOrganisationBuildingRequest()
            {
                OrganisationBuildingId = entityId,
                BuildingId = buildingId,
                IsMainBuilding = false,
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
