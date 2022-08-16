namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Body;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using Backoffice.Body.LifecyclePhase;
using FluentAssertions;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class BodyLifecyclePhaseTests
{
    private readonly ApiFixture _apiFixture;

    public BodyLifecyclePhaseTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task TestBodyLifecyclePhases()
    {
        var bodyId = _apiFixture.Fixture.Create<Guid>();
        var route = $"/v1/bodies/{bodyId}/lifecyclePhases";
        await _apiFixture.Create.Body(bodyId, _apiFixture.Fixture.Create<string>());

        await VerifyNotFound(route);

        var getResponse = await ApiFixture.Get(_apiFixture.HttpClient, route);
        var responseBody = await ApiFixture.DeserializeAsList(getResponse);

        var bodyLifecyclePhaseId = GetBodyLifecyclePhaseId(responseBody);

        var today = _apiFixture.Fixture.Create<DateTime>();

        // UPDATE existing to end on a date
        await UpdateAndVerify(route, bodyLifecyclePhaseId, today.AddDays(-20), today.AddDays(10));

        // CREATE
        await _apiFixture.CreateWithInvalidDataAndVerifyBadRequest(route);

        var id = await CreateAndVerify(route, today.AddDays(11), today.AddDays(12));

        // UPDATE invalid
        await _apiFixture.UpdateWithInvalidDataAndVerifyBadRequest(route, id);
    }

    private static Guid GetBodyLifecyclePhaseId(Dictionary<string, object>[] responseBody)
        => new(responseBody[0]["bodyLifecyclePhaseId"].ToString()!);

    private async Task VerifyNotFound(string route)
    {
        var getResponse = await ApiFixture.Get(_apiFixture.HttpClient, $"{route}/{_apiFixture.Fixture.Create<Guid>()}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<Guid> CreateAndVerify(string baseRoute, DateTime validFrom, DateTime validTo)
    {
        var lifecyclePhaseTypeId = await _apiFixture.Create.LifecyclePhaseType();

        var id = _apiFixture.Fixture.Create<Guid>();
        var body = new AddBodyLifecyclePhaseRequest
        {
            BodyLifecyclePhaseId = id,
            LifecyclePhaseTypeId = lifecyclePhaseTypeId,
            ValidFrom = validFrom,
            ValidTo = validTo,
        };

        await _apiFixture.CreateAndVerify(baseRoute, body, VerifyResult);

        return id;
    }

    private async Task UpdateAndVerify(string baseRoute, Guid id, DateTime validFrom, DateTime validTo)
    {
        var lifecyclePhaseTypeId = await _apiFixture.Create.LifecyclePhaseType();

        var body = new UpdateBodyLifecyclePhaseRequest
        {
            BodyLifecyclePhaseId = id,
            LifecyclePhaseTypeId = lifecyclePhaseTypeId,
            ValidFrom = validFrom,
            ValidTo = validTo,
        };

        await _apiFixture.UpdateAndVerify(baseRoute, id, body, VerifyResult);
    }

    private static void VerifyResult(IReadOnlyDictionary<string, object> responseBody, dynamic body)
    {
        responseBody["lifecyclePhaseTypeId"].Should().Be(body.LifecyclePhaseTypeId.ToString());
        responseBody["validFrom"].Should().Be(body.ValidFrom?.Date);
        responseBody["validTo"].Should().Be(body.ValidTo?.Date);
    }
}
