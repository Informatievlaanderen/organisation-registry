namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Parameters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class FunctionTypeListFilterTests
{
    private readonly ApiFixture _apiFixture;

    public FunctionTypeListFilterTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task Given_A_Removed_FunctionType_List_Returns_It_Only_With_ShowAll()
    {
        var name = $"FT-REMOVED-{Guid.NewGuid():N}";
        var functionTypeId = Guid.NewGuid();

        using var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var createResponse = await ApiFixture.Post(
            client,
            "/v1/functiontypes",
            new { id = functionTypeId, name });
        await ApiFixture.VerifyStatusCode(createResponse, HttpStatusCode.Created);

        var deleteResponse = await ApiFixture.Delete(client, $"/v1/functiontypes/{functionTypeId}");
        await ApiFixture.VerifyStatusCode(deleteResponse, HttpStatusCode.NoContent);

        await Task.Delay(TimeSpan.FromSeconds(1));

        using var defaultListResponse = await ApiFixture.Get(client, "/v1/functiontypes");
        defaultListResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var defaultList = await ApiFixture.DeserializeAsList(defaultListResponse);
        var defaultIds = defaultList.Select(ParseId).ToList();
        defaultIds.Should().NotContain(functionTypeId);

        using var request = new HttpRequestMessage(HttpMethod.Get, "/v1/functiontypes");
        request.Headers.Add("x-filtering", JsonConvert.SerializeObject(new { showAll = true }));
        using var showAllListResponse = await client.SendAsync(request);
        showAllListResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var showAllList = await ApiFixture.DeserializeAsList(showAllListResponse);

        var removedItem = showAllList.SingleOrDefault(item => ParseId(item) == functionTypeId);
        removedItem.Should().NotBeNull();
        removedItem.Should().ContainKey("isRemoved");
        removedItem!["isRemoved"].Should().Be(true);
    }

    private static Guid ParseId(Dictionary<string, object> item)
    {
        if (!item.TryGetValue("id", out var rawValue) || rawValue is null)
            return Guid.Empty;

        _ = Guid.TryParse(rawValue.ToString(), out var id);
        return id;
    }
}
