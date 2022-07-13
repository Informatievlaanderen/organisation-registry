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

        using var message = await client.PostAsync("/v1/imports/organisation-creations", content);

        message.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        using var result = await client.GetAsync("/v1/imports");

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var statusResultContent = await result.Content.ReadAsStringAsync();

        statusResultContent.Should().BeEquivalentTo("{\"imports\":[]}");
    }

    [Fact(Skip = "Unstable because of dependency on other tests (not) running. Needs to be redone to skip ovo number check, or put in separate context")]
    public async Task GivenAValidFile_ThenItAppearsInTheStatusList_AndAfterAWhileTheFileIsProcessedSuccessfully()
    {
        var client = _fixture.HttpClient;

        var importFileStream = GetType().Assembly.GetResource("OrganisationRegistry.Api.IntegrationTests.BulkImport.Valid_TestImportFile.csv");

        // Act: Send the CSV to the API
        using var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
        content.Add(new StreamContent(importFileStream), "bulkimportfile", "upload.csv");

        using var message = await client.PostAsync("/v1/imports/organisation-creations", content);

        // Assert that the CSV is accepted by the API
        message.StatusCode.Should().Be(HttpStatusCode.Accepted);

        // Act: retrieve the list of imports
        var importResult = await message.Content.ReadAsStringAsync();
        var importResultData = JToken.Parse(importResult);

        using var result = await client.GetAsync("/v1/imports");

        // Assert that the list of imports consists of the CSV that we just uploaded
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedStatusResult = GetType().Assembly.GetResourceString("OrganisationRegistry.Api.IntegrationTests.BulkImport.ExpectedStatusResult.json");
        var statusResultContent = await result.Content.ReadAsStringAsync();

        expectedStatusResult = expectedStatusResult.Replace("{id}", importResultData["task"]!["id"]!.Value<string>());

        var expected = JToken.Parse(expectedStatusResult);
        var actual = JToken.Parse(statusResultContent);

        // ignore uploadedAt property
        actual["imports"]![0]!["uploadedAt"] = "";

        actual.Should().BeEquivalentTo(expected);

        // Poll for the result of the imported CSV
        var pollResult = await PollImportStatus(client);
        while (!pollResult.completed)
        {
            await Task.Delay(100);
            pollResult = await PollImportStatus(client);
        }

        // Assert that the import succeeded
        pollResult.success.Should().BeTrue();

        await using var resultFileStream = await client.GetStreamAsync($"/v1/imports/{pollResult.id}/content");

        // Assert that result equals expected result
        using var streamReader = new StreamReader(resultFileStream);

        var resultFileContent = (await streamReader.ReadToEndAsync()).Replace(" ", "");
        var expectedResultFileContent = GetType().Assembly.GetResourceString("OrganisationRegistry.Api.IntegrationTests.BulkImport.Valid_TestImportFileOutput.csv");

        resultFileContent.Should().Be(expectedResultFileContent);
    }

    private static async Task<(bool completed, bool? success, string? id, string? filename)> PollImportStatus(HttpClient client)
    {
        using var pollResult = await client.GetAsync("/v1/imports");

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
