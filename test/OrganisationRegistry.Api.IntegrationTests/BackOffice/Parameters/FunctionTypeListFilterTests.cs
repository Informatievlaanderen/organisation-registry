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

        // The FunctionTypeRemoved projection is asynchronous: poll until it has
        // processed the event instead of relying on a fixed delay.
        var removedItem = await WaitForRemovedItem(client, functionTypeId, TimeSpan.FromSeconds(30));
        removedItem.Should().NotBeNull("the removed function type should appear in the list when showAll is true");
        removedItem.Should().ContainKey("isRemoved");
        removedItem!["isRemoved"].Should().Be(true);
        using var defaultListResponse = await ApiFixture.Get(client, "/v1/functiontypes");
        defaultListResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var defaultList = await ApiFixture.DeserializeAsList(defaultListResponse);
        var defaultIds = defaultList.Select(ParseId).ToList();
        defaultIds.Should().NotContain(functionTypeId);
    }

    private static async Task<Dictionary<string, object>?> WaitForRemovedItem(
        HttpClient client,
        Guid functionTypeId,
        TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow.Add(timeout);
        while (DateTime.UtcNow < deadline)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "/v1/functiontypes");
            request.Headers.Add("x-filtering", JsonConvert.SerializeObject(new { showAll = true }));
            request.Headers.Add("x-pagination", "none");
            using var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var list = await ApiFixture.DeserializeAsList(response);
                var item = list.SingleOrDefault(entry => ParseId(entry) == functionTypeId);
                if (item is not null && item.TryGetValue("isRemoved", out var isRemoved) && isRemoved?.ToString() == "True")
                    return item;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(500));
        }

        return null;
    }

    private static Guid ParseId(Dictionary<string, object> item)
    {
        if (!item.TryGetValue("id", out var rawValue) || rawValue is null)
            return Guid.Empty;

        _ = Guid.TryParse(rawValue.ToString(), out var id);
        return id;
    }
}
