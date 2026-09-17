namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Person.When_Reading_Functions;

using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Xunit;

/// <summary>
/// Publiek (niet-ingelogd) heeft geen toegang tot de functies van een persoon
/// (<c>PeopleFunctionsRead</c>) en krijgt 401 terug (geen geldig token).
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Publiek_Without_PeopleFunctionsRead
{
    private readonly ApiFixture _apiFixture;

    public Given_Publiek_Without_PeopleFunctionsRead(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task Then_Returns_Unauthorized()
    {
        var client = _apiFixture.CreateAnonymousClient();

        var personId = await _apiFixture.Create.Person();

        var response = await ApiFixture.Get(client, $"/v1/people/{personId}/functions");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
