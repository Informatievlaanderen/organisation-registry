namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Delegations.When_Updating_Delegations;

using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Parameters.DelegationAssignments.Requests;
using Xunit;

/// <summary>
/// Publiek (niet-ingelogd) heeft geen toegang tot het aanpassen van delegatietoewijzingen
/// en krijgt 401 terug.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Publiek_Without_DelegationsWrite
{
    private readonly ApiFixture _apiFixture;

    public Given_Publiek_Without_DelegationsWrite(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task Then_Returns_Unauthorized()
    {
        var client = _apiFixture.CreateAnonymousClient();

        var delegationId = Guid.NewGuid();
        var response = await ApiFixture.Put(
            client,
            $"/v1/manage/delegations/{delegationId}/assignments/{Guid.NewGuid()}",
            new UpdateDelegationAssignmentRequest
            {
                DelegationAssignmentId = Guid.NewGuid(),
                BodyId = Guid.NewGuid(),
                BodySeatId = Guid.NewGuid(),
                PersonId = Guid.NewGuid(),
            });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
