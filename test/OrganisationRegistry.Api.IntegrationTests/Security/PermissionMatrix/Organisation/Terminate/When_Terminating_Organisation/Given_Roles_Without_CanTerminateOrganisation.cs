namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Terminate.When_Terminating_Organisation;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Detail;
using Xunit;

/// <summary>
/// Enkel AlgemeenBeheerder en Developer bezitten <c>CanTerminateOrganisation</c>.
/// Alle andere rollen — inclusief CjmBeheerder, VlimpersBeheerder en
/// DecentraalBeheerder, die elders wel organisatiegebonden rechten hebben —
/// krijgen 403 terug bij het beëindigen van een organisatie.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_CanTerminateOrganisation
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_CanTerminateOrganisation(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Cjmbeheerder)]
    [InlineData(ApiFixture.Backoffice.Vlimpersbeheerder)]
    [InlineData(ApiFixture.Backoffice.Decentraalbeheerder)]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orgaanbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orafinbeheerder)]
    public async Task Then_Returns_Forbidden(string role)
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        var client = await _apiFixture.CreateDynamicClient(role);

        var response = await ApiFixture.Put(
            client,
            $"/v1/organisations/{organisationId}/terminate",
            new OrganisationTerminationRequest
            {
                DateOfTermination = DateTime.Today,
                ForceKboTermination = false,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
