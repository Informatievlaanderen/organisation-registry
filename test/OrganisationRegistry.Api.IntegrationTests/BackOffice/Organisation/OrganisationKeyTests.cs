namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Organisation;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using Backoffice.Organisation.Key;
using FluentAssertions;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class OrganisationKeyTests
{
    private readonly ApiFixture _apiFixture;

    public OrganisationKeyTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task TestOrganisationKeys()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        var route = $"/v1/organisations/{organisationId}/keys";
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
    }

    private async Task<Guid> CreateAndVerify(string baseRoute)
    {
        var keyTypeId = await _apiFixture.CreateKeyType();
        var today = _apiFixture.Fixture.Create<DateTime>();

        var id = _apiFixture.Fixture.Create<Guid>();
        var body = new AddOrganisationKeyRequest
        {
            OrganisationKeyId = id,
            KeyValue = _apiFixture.Fixture.Create<string>(),
            KeyTypeId = keyTypeId,
            ValidFrom = today.AddDays(-10),
            ValidTo = today.AddDays(10),
        };

        await _apiFixture.CreateAndVerify(baseRoute, body, VerifyResult);

        return id;
    }

    private async Task UpdateAndVerify(string baseRoute, Guid id)
    {
        var keyTypeId = await _apiFixture.CreateKeyType();
        var today = _apiFixture.Fixture.Create<DateTime>();

        var body = new UpdateOrganisationKeyRequest
        {
            OrganisationKeyId = id,
            KeyValue = _apiFixture.Fixture.Create<string>(),
            KeyTypeId = keyTypeId,
            ValidFrom = today.AddDays(-10),
            ValidTo = today.AddDays(10),
        };

        await _apiFixture.UpdateAndVerify(baseRoute, id, body, VerifyResult);
    }

    private static void VerifyResult(IReadOnlyDictionary<string, object> responseBody, dynamic body)
    {
        responseBody["keyTypeId"].Should().Be(body.KeyTypeId.ToString());
        responseBody["keyValue"].Should().Be(body.KeyValue.ToString());
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
