namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Imports.When_Creating_Imports;

using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using OrganisationRegistry.Tests.Shared;
using Xunit;

/// <summary>
/// Enkel AlgemeenBeheerder en VlimpersBeheerder mogen imports aanmaken
/// (<c>Permission.CanImport</c>). Er wordt bewust een ongeldig CSV-bestand gebruikt: dit
/// bewijst dat de rol de authorisatie-gate passeert (het antwoord is 400 wegens een
/// content-validatiefout, niet 403 wegens een ontbrekend recht) zonder een import-record
/// achter te laten dat andere (gedeelde) tests zou kunnen beïnvloeden.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_CanImport
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_CanImport(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Algemeenbeheerder, "organisation-creations")]
    [InlineData(ApiFixture.Backoffice.Algemeenbeheerder, "organisation-terminations")]
    [InlineData(ApiFixture.Backoffice.Vlimpersbeheerder, "organisation-creations")]
    [InlineData(ApiFixture.Backoffice.Vlimpersbeheerder, "organisation-terminations")]
    public async Task Then_Returns_BadRequest_For_The_Invalid_File_Not_Forbidden(string role, string endpoint)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var importFileStream = GetType().Assembly.GetResource(
            "OrganisationRegistry.Api.IntegrationTests.BulkImport.Invalid_TestImportFile.csv");

        using var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
        content.Add(new StreamContent(importFileStream), "bulkimportfile", "upload.csv");

        using var response = await client.PostAsync($"/v1/imports/{endpoint}", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
