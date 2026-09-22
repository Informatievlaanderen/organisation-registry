namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Delegations.When_Deleting_Delegations;

using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

/// <summary>
/// Publiek (niet-ingelogd) heeft geen toegang tot het verwijderen van delegatietoewijzingen
/// en krijgt 401 terug.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Publiek_Without_DelegationsDelete
{
    private readonly ApiFixture _apiFixture;

    public Given_Publiek_Without_DelegationsDelete(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task Then_Returns_Unauthorized()
    {
        var client = _apiFixture.CreateAnonymousClient();

        var delegationId = Guid.NewGuid();
        var response = await ApiFixture.Delete(
            client,
            $"/v1/manage/delegations/{delegationId}/assignments/{Guid.NewGuid()}/{Guid.NewGuid()}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
