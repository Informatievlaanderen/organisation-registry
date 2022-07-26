namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Organisation;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using Backoffice.Organisation.Function;
using FluentAssertions;
using OrganisationRegistry.Organisation;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class OrganisationFunctionTests
{
    private readonly ApiFixture _apiFixture;

    public OrganisationFunctionTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task TestOrganisationFunctions()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        var route = $"/v1/organisations/{organisationId}/functions";
        await _apiFixture.CreateOrganisation(organisationId, _apiFixture.Fixture.Create<string>());

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
        var personId = await _apiFixture.CreatePerson();
        var functionId = await _apiFixture.CreateFunction();
        var today = _apiFixture.Fixture.Create<DateTime>();

        var creationId = _apiFixture.Fixture.Create<Guid>();
        var body = new AddOrganisationFunctionRequest
        {
            OrganisationFunctionId = creationId,
            FunctionId = functionId,
            PersonId = personId,
            ValidFrom = today.AddDays(-10),
            ValidTo = today.AddDays(10),
        };

        await _apiFixture.CreateAndVerify(baseRoute, body, VerifyResult);

        return creationId;
    }

    private async Task UpdateAndVerify(string baseRoute, Guid id)
    {
        var personId = await _apiFixture.CreatePerson();
        var functionId = await _apiFixture.CreateFunction();
        var today = _apiFixture.Fixture.Create<DateTime>();

        var body = new UpdateOrganisationFunctionRequest
        {
            OrganisationFunctionId = id,
            FunctionId = functionId,
            PersonId = personId,
            ValidFrom = today.AddDays(-10),
            ValidTo = today.AddDays(10),
        };

        await _apiFixture.UpdateAndVerify(baseRoute, id, body, VerifyResult);
    }

    private static void VerifyResult(IReadOnlyDictionary<string, object> responseBody, dynamic body)
    {
        responseBody["functionId"].Should().Be(body.FunctionId.ToString());
        responseBody["personId"].Should().Be(body.PersonId.ToString());
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
