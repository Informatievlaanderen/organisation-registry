namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Delegations.When_Deleting_Delegations;

using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

/// <summary>
/// Rollen zonder <c>Permission.DelegationsDelete</c> mogen delegatietoewijzingen niet
/// verwijderen en krijgen 403 terug.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_DelegationsDelete
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_DelegationsDelete(ApiFixture apiFixture)
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
        var response = await ApiFixture.Delete(
            client,
            $"/v1/manage/delegations/{delegationId}/assignments/{Guid.NewGuid()}/{Guid.NewGuid()}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
