namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Imports.When_Reading_Imports;

using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

/// <summary>
/// Enkel AlgemeenBeheerder en VlimpersBeheerder mogen het Importeren-scherm lezen
/// (<c>Permission.CanImport</c>, `/me`-string <c>imports</c>).
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
    [InlineData(ApiFixture.Backoffice.Algemeenbeheerder)]
    [InlineData(ApiFixture.Backoffice.Vlimpersbeheerder)]
    public async Task Then_Returns_Ok(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var response = await ApiFixture.Get(client, "/v1/imports");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
