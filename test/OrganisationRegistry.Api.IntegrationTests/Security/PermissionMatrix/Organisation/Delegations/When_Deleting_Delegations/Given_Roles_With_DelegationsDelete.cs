namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Delegations.When_Deleting_Delegations;

using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

/// <summary>
/// Enkel AlgemeenBeheerder mag een delegatietoewijzing verwijderen (<c>Permission.DelegationsDelete</c>).
/// Met een niet-bestaand delegationId komt de aanvraag voorbij de permissiecheck en resulteert in
/// 404 (NotFound), niet 403 (Forbidden) — dat bewijst dat het recht wél is toegekend.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_DelegationsDelete
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_DelegationsDelete(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Algemeenbeheerder)]
    public async Task Then_Passes_The_Permission_Check(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var delegationId = Guid.NewGuid();
        var response = await ApiFixture.Delete(
            client,
            $"/v1/manage/delegations/{delegationId}/assignments/{Guid.NewGuid()}/{Guid.NewGuid()}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
