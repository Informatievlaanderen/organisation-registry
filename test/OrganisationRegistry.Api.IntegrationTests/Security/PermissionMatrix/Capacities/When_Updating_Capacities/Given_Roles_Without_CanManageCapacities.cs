namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Capacities.When_Updating_Capacities;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Capacity;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_CanManageCapacities
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_CanManageCapacities(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Orgaanbeheerder)]
    public async Task Then_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var capacityId = await _apiFixture.Create.Capacity();

        var response = await ApiFixture.Put(
            client,
            $"/v1/organisations/{organisationId}/capacities/{entityId}",
            new UpdateOrganisationCapacityRequest()
            {
                OrganisationCapacityId = entityId,
                CapacityId = capacityId,
                PersonId = null,
                FunctionId = null,
                LocationId = null,
                Contacts = null,
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
