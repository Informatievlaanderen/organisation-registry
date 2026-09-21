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
/// Rollen zonder <c>Permission.CanImport</c> mogen geen imports aanmaken en krijgen 403
/// terug — fail-closed, ongeacht de inhoud van het geüploade bestand (de authorisatie-gate
/// wordt vóór modelbinding/validatie geëvalueerd).
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_CanImport
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_CanImport(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Decentraalbeheerder, "organisation-creations")]
    [InlineData(ApiFixture.Backoffice.Decentraalbeheerder, "organisation-terminations")]
    [InlineData(ApiFixture.Backoffice.Orgaanbeheerder, "organisation-creations")]
    [InlineData(ApiFixture.Backoffice.Orgaanbeheerder, "organisation-terminations")]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder, "organisation-creations")]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder, "organisation-terminations")]
    [InlineData(ApiFixture.Backoffice.Cjmbeheerder, "organisation-creations")]
    [InlineData(ApiFixture.Backoffice.Cjmbeheerder, "organisation-terminations")]
    [InlineData(ApiFixture.Backoffice.Orafinbeheerder, "organisation-creations")]
    [InlineData(ApiFixture.Backoffice.Orafinbeheerder, "organisation-terminations")]
    public async Task Then_Returns_Forbidden(string role, string endpoint)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var importFileStream = GetType().Assembly.GetResource(
            "OrganisationRegistry.Api.IntegrationTests.BulkImport.Invalid_TestImportFile.csv");

        using var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
        content.Add(new StreamContent(importFileStream), "bulkimportfile", "upload.csv");

        using var response = await client.PostAsync($"/v1/imports/{endpoint}", content);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
