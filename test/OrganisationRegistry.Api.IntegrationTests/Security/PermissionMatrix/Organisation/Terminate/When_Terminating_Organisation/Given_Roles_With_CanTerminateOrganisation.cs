namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Terminate.When_Terminating_Organisation;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Detail;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;

/// <summary>
/// Matrixrij <b>Organisatie</b> — het beëindigen van een organisatie
/// (<c>PUT /v1/organisations/{id}/terminate</c>) wordt gedreven door
/// <see cref="Permission.CanTerminateOrganisation" />.
///
/// Deze permissie is uitsluitend ongerestricteerd toegekend, en enkel aan
/// AlgemeenBeheerder (en Developer): VlimpersBeheerder/DecentraalBeheerder
/// — die hun <see cref="Permission.CanManageOrganisation" /> enkel als
/// restricted grant bezitten — komen hier nooit voor in aanmerking, zelfs
/// niet voor een organisatie die ze wel mogen bewerken.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_CanTerminateOrganisation
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_CanTerminateOrganisation(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_OK()
    {
        var organisationId = await CreateOrganisation();

        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var response = await TerminateOrganisation(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Developer_Then_Returns_OK()
    {
        var organisationId = await CreateOrganisation();

        var client = _apiFixture.DeveloperHttpClient;

        var response = await TerminateOrganisation(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<Guid> CreateOrganisation()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        return organisationId;
    }

    private async Task<HttpResponseMessage> TerminateOrganisation(HttpClient client, Guid organisationId)
        => await ApiFixture.Put(
            client,
            $"/v1/organisations/{organisationId}/terminate",
            new OrganisationTerminationRequest
            {
                DateOfTermination = DateTime.Today,
                ForceKboTermination = false,
            });
}
