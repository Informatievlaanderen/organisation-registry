namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Systeem.When_Reading_TerminatedInKbo;

using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

/// <summary>
/// Rollen zonder <c>Permission.System</c> mogen het Systeem-scherm "Stopgezet in KBO"
/// niet lezen en krijgen 403 terug, inclusief CjmBeheerder (had voorheen rol-gebaseerd
/// wel toegang) en Orafinbeheerder.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_System
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_System(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Decentraalbeheerder)]
    [InlineData(ApiFixture.Backoffice.Vlimpersbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orgaanbeheerder)]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder)]
    [InlineData(ApiFixture.Backoffice.Cjmbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orafinbeheerder)]
    public async Task Then_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var response = await ApiFixture.Get(client, "/v1/organisations/kbo/terminated");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
