namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Delegations.When_Updating_Delegations;

using System;
using System.Net;
using System.Threading.Tasks;
using Backoffice.Management.DelegationAssignments.Requests;
using FluentAssertions;
using Xunit;

/// <summary>
/// Rollen zonder <c>Permission.DelegationsWrite</c> mogen delegatietoewijzingen niet aanpassen
/// en krijgen 403 terug.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_DelegationsWrite
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_DelegationsWrite(ApiFixture apiFixture)
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

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
