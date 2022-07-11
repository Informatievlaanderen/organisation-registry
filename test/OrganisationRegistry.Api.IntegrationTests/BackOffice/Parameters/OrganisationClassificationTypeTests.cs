namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Parameters;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using SqlServer.OrganisationClassificationType;
using Tests.Shared;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class OrganisationClassificationTypeTests
{
    private readonly ApiFixture _apiFixture;
    private readonly Fixture _fixture;

    public OrganisationClassificationTypeTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
        _fixture = new Fixture();
    }

    [EnvVarIgnoreFact]
    public async Task OrganisationClassificationTypeTest()
    {
        var client = _apiFixture.HttpClient;
        const string baseRoute = "organisationclassificationtypes";
        var id = _fixture.Create<Guid>();

        await CreateAndVerify(client, baseRoute, id, _fixture.Create<string>(), _fixture.Create<bool>());

        await UpdateAndVerify(client, baseRoute, id, _fixture.Create<string>(), _fixture.Create<bool>());
    }

    private static async Task UpdateAndVerify(HttpClient client, string baseRoute, Guid id, string name, bool allowDifferentClassificationsToOverlap)
    {
        var updateResponse = await Update(client, baseRoute, id, name, allowDifferentClassificationsToOverlap);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await ApiFixture.Get(client, $"{baseRoute}/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = JsonConvert.DeserializeObject<OrganisationClassificationTypeListItem>(await getResponse.Content.ReadAsStringAsync())!;
        responseContent.Id.Should().Be(id);
        responseContent.Name.Should().Be(name);
        responseContent.AllowDifferentClassificationsToOverlap.Should().Be(allowDifferentClassificationsToOverlap);
    }

    private static async Task CreateAndVerify(HttpClient client, string baseRoute, Guid id, string name, bool allowDifferentClassificationsToOverlap)
    {
        var createResponse = await Create(client, baseRoute, id, name, allowDifferentClassificationsToOverlap);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var getResponse = await ApiFixture.Get(client, $"{baseRoute}/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = JsonConvert.DeserializeObject<OrganisationClassificationTypeListItem>(await getResponse.Content.ReadAsStringAsync())!;
        responseContent.Id.Should().Be(id);
        responseContent.Name.Should().Be(name);
        responseContent.AllowDifferentClassificationsToOverlap.Should().Be(allowDifferentClassificationsToOverlap);
    }

    private static async Task<HttpResponseMessage> Create(HttpClient client, string baseRoute, Guid id, string name, bool allowDifferentClassificationsToOverlap)
        => await ApiFixture.Post(
            client,
            $"{baseRoute}",
            new
            {
                Id = id,
                Name = name,
                AllowDifferentClassificationsToOverlap = allowDifferentClassificationsToOverlap,
            });

    private static async Task<HttpResponseMessage> Update(HttpClient client, string baseRoute, Guid id, string name, bool allowDifferentClassificationsToOverlap)
        => await ApiFixture.Put(
            client,
            $"{baseRoute}/{id}",
            new
            {
                Name = name,
                AllowDifferentClassificationsToOverlap = allowDifferentClassificationsToOverlap,
            });
}
