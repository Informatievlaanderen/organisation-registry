namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Capacities.When_Deleting_Capacities;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Capacity;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_CanManageCapacities
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_CanManageCapacities(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_NoContent()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var capacityId = await _apiFixture.Create.Capacity();

        await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/capacities",
            new AddOrganisationCapacityRequest()
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

        var response = await ApiFixture.Delete(
            client,
            $"/v1/organisations/{organisationId}/capacities/{entityId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact(Skip = "TODO: scoped role 'Decentraalbeheerder' is allowed by the permission matrix but the domain authorization policy requires the organisation (or entity) to be within the role's own scope (BeheerderForOrganisation / configured owned-ids). No fixture precedent exists for creating an organisation inside a scoped role's Keycloak OVO scope, so this positive cannot yet assert a 2xx. Enable once scoped-org test setup is available.")]
    public async Task For_Decentraalbeheerder_Then_Returns_NoContent()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var capacityId = await _apiFixture.Create.Capacity();

        await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/capacities",
            new AddOrganisationCapacityRequest()
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

        var response = await ApiFixture.Delete(
            client,
            $"/v1/organisations/{organisationId}/capacities/{entityId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact(Skip = "TODO: scoped role 'Regelgevingbeheerder' is allowed by the permission matrix but the domain authorization policy requires the organisation (or entity) to be within the role's own scope (BeheerderForOrganisation / configured owned-ids). No fixture precedent exists for creating an organisation inside a scoped role's Keycloak OVO scope, so this positive cannot yet assert a 2xx. Enable once scoped-org test setup is available.")]
    public async Task For_Regelgevingbeheerder_Then_Returns_NoContent()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Regelgevingbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var capacityId = await _apiFixture.Create.Capacity();

        await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/capacities",
            new AddOrganisationCapacityRequest()
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

        var response = await ApiFixture.Delete(
            client,
            $"/v1/organisations/{organisationId}/capacities/{entityId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
