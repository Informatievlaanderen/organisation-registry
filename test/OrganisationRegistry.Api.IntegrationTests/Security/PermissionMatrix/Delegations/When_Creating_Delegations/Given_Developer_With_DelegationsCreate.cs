namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Delegations.When_Creating_Delegations;

using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Parameters.DelegationAssignments.Requests;
using Xunit;

/// <summary>
/// <c>Permission.DelegationsCreate</c> is een intern, niet via <c>/v1/me</c> blootgesteld recht dat
/// enkel aan <see cref="Role.Developer"/> is toegekend (test-/tooling-doeleinden). Met een
/// niet-bestaand delegationId komt de aanvraag voorbij de permissiecheck en resulteert in
/// 404 (NotFound), niet 403 (Forbidden) — dat bewijst dat het recht wél is toegekend.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Developer_With_DelegationsCreate
{
    private readonly ApiFixture _apiFixture;

    public Given_Developer_With_DelegationsCreate(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task Then_Passes_The_Permission_Check()
    {
        var client = _apiFixture.DeveloperHttpClient;

        var delegationId = Guid.NewGuid();
        var response = await ApiFixture.Post(
            client,
            $"/v1/manage/delegations/{delegationId}/assignments",
            new AddDelegationAssignmentRequest
            {
                DelegationAssignmentId = Guid.NewGuid(),
                BodyId = Guid.NewGuid(),
                BodySeatId = Guid.NewGuid(),
                PersonId = Guid.NewGuid(),
            });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
