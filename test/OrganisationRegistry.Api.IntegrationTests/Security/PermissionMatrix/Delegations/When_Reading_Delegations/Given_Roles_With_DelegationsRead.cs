namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Delegations.When_Reading_Delegations;

using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

/// <summary>
/// Enkel AlgemeenBeheerder mag delegaties lezen (<c>Permission.DelegationsRead</c>).
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_DelegationsRead
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_DelegationsRead(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Algemeenbeheerder)]
    public async Task Then_Returns_Ok(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var response = await ApiFixture.Get(client, "/v1/manage/delegations");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
