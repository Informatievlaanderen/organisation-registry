namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Update.When_Updating_Organisation_Not_Limited_To_Vlimpers;

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
/// Matrixrij <b>Organisatie</b> — de niet-Vlimpers-voorbehouden velden
/// aanpassen (<c>PUT /v1/organisations/{id}/notlimitedtovlimpers</c>) — voor
/// de <see cref="Role.DecentraalBeheerder" />. DecentraalBeheerder bezit
/// <see cref="Permission.CanManageOrganisationInfoNotLimitedToVlimpers" />
/// als restricted grant voor de eigen organisatie, ongeacht
/// Vlimpersbeheer-status van die organisatie (deze velden zijn nooit
/// Vlimpers-voorbehouden). Voor een organisatie buiten scope krijgt hij 403.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Decentraalbeheerder
{
    private readonly ApiFixture _apiFixture;

    public Given_Decentraalbeheerder(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_OwnOrganisation_Then_Returns_OK()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        // OVO000003 is de eigen organisatie van de decentraalbeheerder-persona.
        var organisationId = _apiFixture.DecentraalbeheerderOrganisationId;

        var response = await UpdateOrganisation(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_OrganisationOutsideScope_Then_Returns_Forbidden()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = await CreateOrganisation();

        var response = await UpdateOrganisation(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<Guid> CreateOrganisation()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        return organisationId;
    }

    private async Task<HttpResponseMessage> UpdateOrganisation(HttpClient client, Guid organisationId)
        => await ApiFixture.Put(
            client,
            $"/v1/organisations/{organisationId}/notlimitedtovlimpers",
            new UpdateOrganisationInfoNotLimitedToVlimpersRequest
            {
                Description = _apiFixture.Fixture.Create<string>(),
                ShowOnVlaamseOverheidSites = false,
            });
}
