namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Organisation;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using Backoffice.Organisation.Location;
using FluentAssertions;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class OrganisationLocationTests
{
    private readonly ApiFixture _apiFixture;

    public OrganisationLocationTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task TestOrganisationFunctions()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        var route = $"/v1/organisations/{organisationId}/locations";
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
        var locationId = await _apiFixture.Create.Location();
        var today = _apiFixture.Fixture.Create<DateTime>();

        var creationId = _apiFixture.Fixture.Create<Guid>();
        var body = new AddOrganisationLocationRequest
        {
            OrganisationLocationId = creationId,
            LocationId = locationId,
            LocationTypeId = null,
            IsMainLocation = _apiFixture.Fixture.Create<bool>(),
            ValidFrom = today.AddDays(-10),
            ValidTo = today.AddDays(10),
        };

        await _apiFixture.CreateAndVerify(baseRoute, body, VerifyResult);

        return creationId;
    }

    private async Task UpdateAndVerify(string baseRoute, Guid id)
    {
        var locationId = await _apiFixture.Create.Location();
        var today = _apiFixture.Fixture.Create<DateTime>();

        var body = new UpdateOrganisationLocationRequest()
        {
            OrganisationLocationId = id,
            LocationId = locationId,
            LocationTypeId = null,
            IsMainLocation = _apiFixture.Fixture.Create<bool>(),
            ValidFrom = today.AddDays(-10),
            ValidTo = today.AddDays(10),
        };

        await _apiFixture.UpdateAndVerify(baseRoute, id, body, VerifyResult);
    }

    private static void VerifyResult(IReadOnlyDictionary<string, object> responseBody, dynamic body)
    {
        responseBody["locationId"].Should().Be(body.LocationId.ToString());
        responseBody["isMainLocation"].Should().Be(body.IsMainLocation);
        responseBody["locationTypeId"].Should().Be(body.LocationTypeId?.ToString());
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
