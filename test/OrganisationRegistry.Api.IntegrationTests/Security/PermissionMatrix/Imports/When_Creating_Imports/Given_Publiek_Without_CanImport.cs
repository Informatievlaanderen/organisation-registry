namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Imports.When_Creating_Imports;

using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Tests.Shared;
using Xunit;

/// <summary>
/// Publiek (niet-ingelogd) mag geen imports aanmaken en krijgt 401 terug (geen geldig
/// token).
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Publiek_Without_CanImport
{
    private readonly ApiFixture _apiFixture;

    public Given_Publiek_Without_CanImport(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData("organisation-creations")]
    [InlineData("organisation-terminations")]
    public async Task Then_Returns_Unauthorized(string endpoint)
    {
        var client = _apiFixture.CreateAnonymousClient();

        var importFileStream = GetType().Assembly.GetResource(
            "OrganisationRegistry.Api.IntegrationTests.BulkImport.Invalid_TestImportFile.csv");

        using var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
        content.Add(new StreamContent(importFileStream), "bulkimportfile", "upload.csv");

        using var response = await client.PostAsync($"/v1/imports/{endpoint}", content);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
