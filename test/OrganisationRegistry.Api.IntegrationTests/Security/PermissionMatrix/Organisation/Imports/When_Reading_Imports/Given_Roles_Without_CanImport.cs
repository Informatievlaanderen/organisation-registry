namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Imports.When_Reading_Imports;

using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

/// <summary>
/// Rollen zonder <c>Permission.CanImport</c> mogen het Importeren-scherm niet lezen en
/// krijgen 403 terug — fail-closed, ook voor de leesroute.
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
    [InlineData(ApiFixture.Backoffice.Decentraalbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orgaanbeheerder)]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder)]
    [InlineData(ApiFixture.Backoffice.Cjmbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orafinbeheerder)]
    public async Task Then_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var response = await ApiFixture.Get(client, "/v1/imports");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
