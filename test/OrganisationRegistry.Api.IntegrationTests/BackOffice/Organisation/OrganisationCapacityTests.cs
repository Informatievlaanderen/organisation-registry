namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Organisation;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using Backoffice.Organisation.Capacity;
using FluentAssertions;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class OrganisationCapacityTests
{
    private readonly ApiFixture _apiFixture;

    public OrganisationCapacityTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task TestOrganisationCapacities()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        var route = $"/v1/organisations/{organisationId}/capacities";
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        var getResponse = await ApiFixture.Get(_apiFixture.HttpClient, $"{route}/{_apiFixture.Fixture.Create<Guid>()}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        // CREATE
        await _apiFixture.CreateWithInvalidDataAndVerifyBadRequest(route);

        var id = await CreateAndVerify(route);

        // UPDATE
        await UpdateAndVerify(route, id);

        await _apiFixture.UpdateWithInvalidDataAndVerifyBadRequest(route, id);

        // LIST
        await GetListAndVerify(route);

        //REMOVE
        await _apiFixture.RemoveAndVerify(route, id);
    }

    private async Task<Guid> CreateAndVerify(string baseRoute)
    {
        var personId = await _apiFixture.Create.Person();
        var functionId = await _apiFixture.Create.Function();
        var capacityId = await _apiFixture.Create.Capacity();
        var today = _apiFixture.Fixture.Create<DateTime>();

        var id = _apiFixture.Fixture.Create<Guid>();
        var body = new AddOrganisationCapacityRequest
        {
            OrganisationCapacityId = id,
            FunctionId = functionId,
            CapacityId = capacityId,
            PersonId = personId,
            ValidFrom = today.AddDays(-10),
            ValidTo = today.AddDays(10),
            Contacts = new Dictionary<Guid, string>(),
            LocationId = null,
        };

        await _apiFixture.CreateAndVerify(baseRoute, body, VerifyResult);

        return id;
    }

    private async Task UpdateAndVerify(string baseRoute, Guid id)
    {
        var personId = await _apiFixture.Create.Person();
        var functionId = await _apiFixture.Create.Function();
        var capacityId = await _apiFixture.Create.Capacity();
        var today = _apiFixture.Fixture.Create<DateTime>();

        var body = new UpdateOrganisationCapacityRequest
        {
            OrganisationCapacityId = id,
            FunctionId = functionId,
            CapacityId = capacityId,
            PersonId = personId,
            ValidFrom = today.AddDays(-10),
            ValidTo = today.AddDays(10),
            Contacts = new Dictionary<Guid, string>(),
            LocationId = null,
        };

        await _apiFixture.UpdateAndVerify(baseRoute, id, body, VerifyResult);
    }

    private static void VerifyResult(IReadOnlyDictionary<string, object> responseBody, dynamic body)
    {
        responseBody["functionId"].Should().Be(body.FunctionId?.ToString());
        responseBody["personId"].Should().Be(body.PersonId?.ToString());
        responseBody["capacityId"].Should().Be(body.CapacityId.ToString());
        responseBody["validFrom"].Should().Be(body.ValidFrom?.Date);
        responseBody["validTo"].Should().Be(body.ValidTo?.Date);
    }

    private async Task GetListAndVerify(string route)
    {
        await CreateAndVerify(route);
        await CreateAndVerify(route);

        await _apiFixture.GetListAndVerify(route);
    }
}
