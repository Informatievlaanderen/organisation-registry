namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Locations.When_Deleting_Locations;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Location;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_CanManageLocations
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_CanManageLocations(ApiFixture apiFixture)
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
        var locationId = await _apiFixture.Create.Location();

        var response = await ApiFixture.Delete(
            client,
            $"/v1/organisations/{organisationId}/locations/{entityId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
