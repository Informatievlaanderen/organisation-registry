namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Parameters;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using Backoffice.Parameters.ContactType.Requests;
using FluentAssertions;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class ContactTypeTests
{
    private readonly ApiFixture _apiFixture;

    public ContactTypeTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task TestContactTypes()
    {
        var route = $"/v1/contacttypes";

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
        var creationId = _apiFixture.Fixture.Create<Guid>();
        var body = new CreateContactTypeRequest
        {
            Id = creationId,
            Name = _apiFixture.Fixture.Create<string>(),
            Regex = ".*",
            Example = _apiFixture.Fixture.Create<string>(),
        };

        await _apiFixture.CreateAndVerify(baseRoute, body, VerifyResult);

        return creationId;
    }

    private async Task UpdateAndVerify(string baseRoute, Guid id)
    {
        var body = new UpdateContactTypeRequest()
        {
            Name = _apiFixture.Fixture.Create<string>(),
            Regex = ".+",
            Example = $"X{_apiFixture.Fixture.Create<string>()}",
        };

        await _apiFixture.UpdateAndVerify(baseRoute, id, body, VerifyResult);
    }

    private static void VerifyResult(IReadOnlyDictionary<string, object> responseBody, dynamic body)
    {
        responseBody["name"].Should().Be(body.Name);
        responseBody["regex"].Should().Be(body.Regex);
        responseBody["example"].Should().Be(body.Example);
    }

    private async Task GetListAndVerify(string route)
    {
        await CreateAndVerify(route);
        await CreateAndVerify(route);

        await _apiFixture.GetListAndVerify(route);
    }
}
