namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Delegations.When_Reading_Delegations;

using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

/// <summary>
/// Rollen zonder <c>Permission.DelegationsRead</c> mogen delegaties niet lezen en
/// krijgen 403 terug, ongeacht andere rechten die ze eventueel wel bezitten.
/// Delegaties zijn fail-closed: ook lezen is niet toegestaan.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_DelegationsRead
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_DelegationsRead(ApiFixture apiFixture)
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

        var response = await ApiFixture.Get(client, "/v1/manage/delegations");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
