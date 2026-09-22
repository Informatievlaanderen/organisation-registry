namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Systeem.When_Reading_TerminatedInKbo;

using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

/// <summary>
/// Enkel AlgemeenBeheerder mag het Systeem-scherm "Stopgezet in KBO" lezen
/// (<c>Permission.System</c>). Voorheen had CjmBeheerder hier ook (rol-gebaseerde)
/// toegang toe — dat is met de overstap naar het ongesplitste System-recht komen te
/// vervallen.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_System
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_System(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Algemeenbeheerder)]
    public async Task Then_Returns_Ok(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var response = await ApiFixture.Get(client, "/v1/organisations/kbo/terminated");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
