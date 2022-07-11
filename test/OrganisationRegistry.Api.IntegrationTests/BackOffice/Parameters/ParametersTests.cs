namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Parameters;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Tests.Shared;
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

    [EnvVarIgnoreTheory]
    [InlineData("keytypes", true)]
    [InlineData("labeltypes", false)]
    public async Task KeyTest(string baseRoute, bool supportsRemoval)
    {
        var client = _apiFixture.HttpClient;

        var id = _fixture.Create<Guid>();
        var createdName = _fixture.Create<string>();

        // create
        var createResponse = await Create(client, baseRoute, id, createdName);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // get
        var getResponse1 = await ApiFixture.Get(client, $"{baseRoute}/{id}");
        getResponse1.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedName = _fixture.Create<string>();
        // update
        var updateResponse = await Update(client, baseRoute, id, updatedName);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // get
        var getResponse2 = await ApiFixture.Get(client, $"{baseRoute}/{id}");
        getResponse2.StatusCode.Should().Be(HttpStatusCode.OK);

        if (!supportsRemoval)
            return;

        // removekey
        var deleteResponse = await ApiFixture.Delete(client, $"{baseRoute}/{id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // getkey (-> not found)
        var getResponse3 = await ApiFixture.Get(client, $"{baseRoute}/{id}");
        getResponse3.StatusCode.Should().Be(HttpStatusCode.OK);
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
