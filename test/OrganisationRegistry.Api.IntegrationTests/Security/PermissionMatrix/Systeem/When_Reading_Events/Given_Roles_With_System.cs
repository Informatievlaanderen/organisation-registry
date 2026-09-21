namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Systeem.When_Reading_Events;

using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

/// <summary>
/// Enkel AlgemeenBeheerder mag het Systeem-scherm "Events" lezen (<c>Permission.System</c>).
/// Dit recht is bewust niet fijnmazig per scherm (zie ui-permission-matrix.md).
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

        var response = await ApiFixture.Get(client, "/v1/events");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
