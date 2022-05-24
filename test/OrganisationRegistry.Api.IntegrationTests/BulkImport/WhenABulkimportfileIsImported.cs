namespace OrganisationRegistry.Api.IntegrationTests.BulkImport;

using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class WhenABulkimportfileIsImported
{
    private readonly ApiFixture _fixture;

    public WhenABulkimportfileIsImported(ApiFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GivenTheFileIsNotValid_ThenHttp400BadRequestIsReceivedAndItDoesNotAppearInTheList()
    {
        var client = _fixture.HttpClient;

        var importFileStream = GetType().Assembly.GetResource("OrganisationRegistry.Api.IntegrationTests.BulkImport.Invalid_TestImportFile.csv");

        using var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
        content.Add(new StreamContent(importFileStream), "bulkimportfile", "upload.csv");

        using var message = await client.PostAsync("import/organisations", content);

        message.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        using var result = await client.GetAsync("import/organisations");

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var statusResultContent = await result.Content.ReadAsStringAsync();

        statusResultContent.Should().BeEquivalentTo("{\"imports\":[]}");
    }

    [Fact]
    public async Task GivenAValidFile_ThenItAppearsInTheStatusList()
    {
        var client = _fixture.HttpClient;

        var importFileStream = GetType().Assembly.GetResource("OrganisationRegistry.Api.IntegrationTests.BulkImport.Valid_TestImportFile.csv");

        using var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
        content.Add(new StreamContent(importFileStream), "bulkimportfile", "upload.csv");

        using var message = await client.PostAsync("import/organisations", content);

        message.StatusCode.Should().Be(HttpStatusCode.Accepted);
        var importResult = await message.Content.ReadAsStringAsync();
        var importResultData = JToken.Parse(importResult);

        using var result = await client.GetAsync("import/organisations");

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedStatusResult = GetType().Assembly.GetResourceString("OrganisationRegistry.Api.IntegrationTests.BulkImport.ExpectedStatusResult.json");
        var statusResultContent = await result.Content.ReadAsStringAsync();

        expectedStatusResult = expectedStatusResult.Replace("{id}", importResultData["task"]!["id"]!.Value<string>());

        var expected = JToken.Parse(expectedStatusResult);
        var actual = JToken.Parse(statusResultContent);

        // ignore uploadedAt property
        actual["imports"]![0]!["uploadedAt"] = "";

        actual.Should().BeEquivalentTo(expected);

        // todo: poll status
    }
}
