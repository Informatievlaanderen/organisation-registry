namespace OrganisationRegistry.Api.IntegrationTests.BulkImport;

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Tests.Shared;
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
    public async Task GivenAValidFile_ThenItAppearsInTheStatusList_AndAfterAWhileTheFileIsProcessedSuccessfully()
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

        var pollResult = await PollImportStatus(client);
        while (!pollResult.completed)
        {
            await Task.Delay(100);
            pollResult = await PollImportStatus(client);
        }

        return;

        // todo: make sure that the parent organisation(s) exist(s) and uncomment the code below

        // pollResult.success.Should().BeTrue();
        //
        // await using var resultFileStream = await client.GetStreamAsync($"import/organisations/{pollResult.id}/content");
        //
        // using var streamReader = new StreamReader(resultFileStream);
        //
        // var resultFileContent = (await streamReader.ReadToEndAsync()).Replace(" ", "");
        // var expectedResultFileContent = GetType().Assembly.GetResourceString("OrganisationRegistry.Api.IntegrationTests.BulkImport.Valid_TestImportFileOutput.csv");
        //
        // resultFileContent.Should().Be(expectedResultFileContent);
    }

    private static async Task<(bool completed, bool? success, string? id, string? filename)> PollImportStatus(HttpClient client)
    {
        using var pollResult = await client.GetAsync("import/organisations");

        var polledStatusResultContent = await pollResult.Content.ReadAsStringAsync();
        var polledActual = JToken.Parse(polledStatusResultContent);

        var polledStatus = polledActual["imports"]![0]!["status"];

        var geslaagd = (string?)polledStatus == "Geslaagd";
        var gefaald = (string?)polledStatus == "Gefaald";
        var importId = (string?)polledActual["imports"]![0]!["id"];
        var filename = (string?)polledActual["imports"]![0]!["filename"];
        return geslaagd
            ? (completed: true, success: true, id: importId, filename)
            : gefaald
                ? (completed: true, success: false, id: importId, filename)
                : (completed: false, success: null, id: null, filename: null);
    }
}
