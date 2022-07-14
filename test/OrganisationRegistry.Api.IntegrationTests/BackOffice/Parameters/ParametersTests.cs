namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Parameters;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class ParametersTests
{
    private readonly ApiFixture _apiFixture;
    private readonly Fixture _fixture;

    public ParametersTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
        _fixture = new Fixture();
    }

    [Theory]
    [InlineData("/v1/keytypes", true)]
    [InlineData("/v1/labeltypes", false)]
    [InlineData("/v1/bodyclassificationtypes", false)]
    [InlineData("/v1/capacities", true)]
    [InlineData("/v1/contacttypes", false)]
    [InlineData("/v1/formalframeworkcategories", false)]
    [InlineData("/v1/functiontypes", false)]
    [InlineData("/v1/locationtypes", false)]
    [InlineData("/v1/mandateroletypes", false)]
    [InlineData("/v1/purposes", false)]
    [InlineData("/v1/regulationthemes", false)]
    public async Task ParameterTest(string baseRoute, bool supportsRemoval)
    {
        var (id, createResponseDictionary) = await CreateAndVerify(baseRoute);

        var updateResponseDictionary = await UpdateAndVerify(baseRoute, id);

        if (!supportsRemoval)
            return;

        createResponseDictionary["isRemoved"].Should().Be(false);
        updateResponseDictionary["isRemoved"].Should().Be(false);

        var removeResponseDictionary = await RemoveAndVerify(baseRoute, id);
        removeResponseDictionary["isRemoved"].Should().Be(true);
    }

    private async Task<(Guid, Dictionary<string, object>)> CreateAndVerify(string baseRoute)
    {
        var id = _fixture.Create<Guid>();

        // create
        var name = _fixture.Create<string>();
        var createResponse = await Create(_apiFixture.HttpClient, baseRoute, id, name);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // get
        var getResponse = await ApiFixture.Get(_apiFixture.HttpClient, createResponse.Headers.Location!.ToString());
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        return (id, await ValidateResponse(getResponse, id, name));
    }

    private async Task<Dictionary<string, object>> UpdateAndVerify(string baseRoute, Guid id)
    {
        var updatedName = _fixture.Create<string>();
        // update
        var updateResponse = await Update(_apiFixture.HttpClient, baseRoute, id, updatedName);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // get
        var getResponse = await ApiFixture.Get(_apiFixture.HttpClient, $"{baseRoute}/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        return await ValidateResponse(getResponse, id, updatedName);
    }

    private async Task<Dictionary<string, object>> RemoveAndVerify(string baseRoute, Guid id)
    {
        // removekey
        var deleteResponse = await ApiFixture.Delete(_apiFixture.HttpClient, $"{baseRoute}/{id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // getkey
        var getResponse = await ApiFixture.Get(_apiFixture.HttpClient, $"{baseRoute}/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        return await ApiFixture.Deserialize(getResponse);
    }

    private static async Task<Dictionary<string, object>> ValidateResponse(HttpResponseMessage message, Guid id, string name)
    {
        var deserializedResponse = await ApiFixture.Deserialize(message);
        deserializedResponse.Should().NotBeNull();
        deserializedResponse["id"].Should().Be(id.ToString());
        deserializedResponse["name"].Should().Be(name);
        return deserializedResponse;
    }

    private static async Task<HttpResponseMessage> Create(HttpClient client, string baseRoute, Guid id, string name)
        => await ApiFixture.Post(
            client,
            $"{baseRoute}",
            new
            {
                Id = id,
                Name = name,
            });

    private static async Task<HttpResponseMessage> Update(HttpClient client, string baseRoute, Guid id, string name)
        => await ApiFixture.Put(
            client,
            $"{baseRoute}/{id}",
            new
            {
                Name = name,
            });
}
