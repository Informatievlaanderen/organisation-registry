namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Delegations.When_Reading_Delegations;

using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

/// <summary>
/// Publiek (niet-ingelogd) heeft geen toegang tot delegaties en krijgt 401 terug
/// (geen geldig token).
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Publiek_Without_DelegationsRead
{
    private readonly ApiFixture _apiFixture;

    public Given_Publiek_Without_DelegationsRead(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task Then_Returns_Unauthorized()
    {
        var client = _apiFixture.CreateAnonymousClient();

        var response = await ApiFixture.Get(client, "/v1/manage/delegations");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
