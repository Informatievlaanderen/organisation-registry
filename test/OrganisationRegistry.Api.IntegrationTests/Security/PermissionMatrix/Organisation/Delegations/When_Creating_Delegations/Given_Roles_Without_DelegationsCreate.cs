namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Delegations.When_Creating_Delegations;

using System;
using System.Net;
using System.Threading.Tasks;
using Backoffice.Management.DelegationAssignments.Requests;
using FluentAssertions;
using Xunit;

/// <summary>
/// Er bestaat geen "Create" op delegaties zelf: <c>Permission.DelegationsCreate</c> geldt enkel
/// voor het aanmaken van een delegatietoewijzing, en wordt bewust NIET toegekend aan
/// AlgemeenBeheerder (noch aan enige andere backoffice-rol) — enkel <see cref="Role.Developer"/>
/// krijgt dit (interne, niet via <c>/v1/me</c> blootgestelde) recht. Elke backoffice-rol,
/// inclusief AlgemeenBeheerder, krijgt dus 403 op het aanmaken van een toewijzing.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_DelegationsCreate
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_DelegationsCreate(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Algemeenbeheerder)]
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

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
