namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Capacities.When_Updating_Capacities;

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    public async Task For_Algemeenbeheerder_Then_Returns_OK()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = await AddCapacity(client, organisationId);

        var response = await UpdateCapacity(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOwnOrganisation_Then_Returns_OK()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.DecentraalbeheerderOrganisationId;
        var entityId = await AddCapacity(client, organisationId);

        var response = await UpdateCapacity(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithChildOrganisationInScope_Then_Returns_OK()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.DecentraalbeheerderChildOrganisationId;
        var entityId = await AddCapacity(client, organisationId);

        var response = await UpdateCapacity(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOrganisationOutsideScope_Then_Returns_Forbidden()
    {
        var privilegedClient = await _apiFixture.CreateAlgemeenbeheerderClient();
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = await AddCapacity(privilegedClient, organisationId);

        var response = await UpdateCapacity(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task For_Regelgevingbeheerder_WithOwnedCapacity_Then_Returns_OK()
    {
        var privilegedClient = await _apiFixture.CreateAlgemeenbeheerderClient();
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Regelgevingbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = await AddCapacity(privilegedClient, organisationId);

        // Repointing the organisation capacity to a Regelgeving-owned capacity is
        // allowed: the update policy checks the (new) capacity id against the
        // configured allow-list, independent of organisation scope.
        var ownedCapacityId = await _apiFixture.Create.Capacity(
            _apiFixture.Configuration.Authorization.CapacityIdsOwnedByRegelgevingDbBeheerder.First());

        var response = await ApiFixture.Put(
            client,
            $"/v1/organisations/{organisationId}/capacities/{entityId}",
            new UpdateOrganisationCapacityRequest()
            {
                OrganisationCapacityId = entityId,
                CapacityId = ownedCapacityId,
                PersonId = null,
                FunctionId = null,
                LocationId = null,
                Contacts = null,
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Regelgevingbeheerder_WithNonOwnedCapacity_Then_Returns_Forbidden()
    {
        var privilegedClient = await _apiFixture.CreateAlgemeenbeheerderClient();
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Regelgevingbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = await AddCapacity(privilegedClient, organisationId);

        // Updating with a freshly created (non-owned) capacity must be rejected:
        // Regelgevingbeheerder may only manage the configured Regelgeving-owned
        // capacities.
        var response = await UpdateCapacity(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<Guid> AddCapacity(HttpClient client, Guid organisationId)
    {
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

        return entityId;
    }


    private async Task<HttpResponseMessage> UpdateCapacity(HttpClient client, Guid organisationId, Guid entityId)
    {
        var capacityId = await _apiFixture.Create.Capacity();

        return await ApiFixture.Put(
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
    }
}
