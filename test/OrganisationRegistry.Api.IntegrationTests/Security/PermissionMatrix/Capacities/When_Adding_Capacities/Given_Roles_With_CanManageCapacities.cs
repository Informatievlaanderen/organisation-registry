namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Capacities.When_Adding_Capacities;

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
    public async Task For_Algemeenbeheerder_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        var response = await AddCapacity(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOwnOrganisation_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var response = await AddCapacity(client, _apiFixture.DecentraalbeheerderOrganisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithChildOrganisationInScope_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var response = await AddCapacity(client, _apiFixture.DecentraalbeheerderChildOrganisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOrganisationOutsideScope_Then_Returns_Forbidden()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        var response = await AddCapacity(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task For_Regelgevingbeheerder_WithOwnedCapacity_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Regelgevingbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        // The Regelgevingbeheerder capacity grant is restricted to the configured
        // Regelgeving-owned capacities (and is not organisation-scoped), so a
        // capacity on that allow-list may be added to any organisation.
        var ownedCapacityId = await _apiFixture.Create.Capacity(
            _apiFixture.Configuration.Authorization.CapacityIdsOwnedByRegelgevingDbBeheerder.First());

        var response = await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/capacities",
            new AddOrganisationCapacityRequest()
            {
                OrganisationCapacityId = _apiFixture.Fixture.Create<Guid>(),
                CapacityId = ownedCapacityId,
                PersonId = null,
                FunctionId = null,
                LocationId = null,
                Contacts = null,
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Regelgevingbeheerder_WithNonOwnedCapacity_Then_Returns_Forbidden()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Regelgevingbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        // A freshly created capacity is never on the configured
        // CapacityIdsOwnedByRegelgevingDbBeheerder allow-list, so the handler's
        // CapacityPolicy must reject it: Regelgevingbeheerder may only manage the
        // Regelgeving-owned capacities.
        var response = await AddCapacity(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<HttpResponseMessage> AddCapacity(HttpClient client, Guid organisationId)
    {
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var capacityId = await _apiFixture.Create.Capacity();

        return await ApiFixture.Post(
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
    }
}