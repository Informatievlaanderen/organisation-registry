namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Body;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using Backoffice.Body.FormalFramework;
using FluentAssertions;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class BodyFormalFrameworkTests
{
    private readonly ApiFixture _apiFixture;

    public BodyFormalFrameworkTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task TestBodyFormalFramework()
    {
        var bodyId = _apiFixture.Fixture.Create<Guid>();
        var route = $"/v1/bodies/{bodyId}/formalframeworks";
        await _apiFixture.Create.Body(bodyId, _apiFixture.Fixture.Create<string>());

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
    }

    private async Task<Guid> CreateAndVerify(string baseRoute)
    {
        var today = _apiFixture.Fixture.Create<DateTime>();
        var formalFrameworkCategoryId = await _apiFixture.Create.FormalFrameworkCategory();
        var formalFrameworkId = await _apiFixture.Create.FormalFramework(formalFrameworkCategoryId);

        var id = _apiFixture.Fixture.Create<Guid>();
        var body = new AddBodyFormalFrameworkRequest
        {
            BodyFormalFrameworkId = id,
            FormalFrameworkId = formalFrameworkId,
            ValidFrom = today.AddDays(-10),
            ValidTo = today.AddDays(10),
        };

        await _apiFixture.CreateAndVerify(baseRoute, body, VerifyResult);

        return id;
    }

    private async Task UpdateAndVerify(string baseRoute, Guid id)
    {
        var today = _apiFixture.Fixture.Create<DateTime>();
        var formalFrameworkCategoryId = await _apiFixture.Create.FormalFrameworkCategory();
        var formalFrameworkId = await _apiFixture.Create.FormalFramework(formalFrameworkCategoryId);

        var body = new UpdateBodyFormalFrameworkRequest
        {
            BodyFormalFrameworkId = id,
            FormalFrameworkId = formalFrameworkId,
            ValidFrom = today.AddDays(-10),
            ValidTo = today.AddDays(10),
        };

        await _apiFixture.UpdateAndVerify(baseRoute, id, body, VerifyResult);
    }

    private static void VerifyResult(IReadOnlyDictionary<string, object> responseBody, dynamic body)
    {
        responseBody["formalFrameworkId"].Should().Be(body.FormalFrameworkId.ToString());
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
