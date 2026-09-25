namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Update.When_Updating_Organisation_Limited_To_Vlimpers;

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Detail;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;

/// <summary>
/// Matrixrij <b>Organisatie</b> — de vier Vlimpers-voorbehouden velden
/// aanpassen (<c>PUT /v1/organisations/{id}/limitedtovlimpers</c>) — voor de
/// <see cref="Role.DecentraalBeheerder" />. In tegenstelling tot
/// <see cref="Permission.CanManageOrganisation" /> (waar DecentraalBeheerder
/// een restricted grant voor de eigen, niet-Vlimpersbeheerde organisatie
/// bezit) houdt DecentraalBeheerder <em>geen enkele</em> grant voor
/// <see cref="Permission.CanManageOrganisationInfoLimitedToVlimpers" />: deze
/// velden zijn uitsluitend voorbehouden aan Vlimpersbeheerder/AlgemeenBeheerder,
/// ongeacht of het de eigen organisatie betreft en ongeacht Vlimpersbeheer-status.
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
    public async Task For_OwnOrganisation_NotUnderVlimpersManagement_Then_Returns_Forbidden()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        // OVO000003 is de eigen organisatie van de decentraalbeheerder-persona;
        // ze valt binnen de scope van CanManageOrganisation, maar dat mag hier
        // geen effect hebben.
        var organisationId = _apiFixture.DecentraalbeheerderOrganisationId;

        var response = await UpdateOrganisation(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task For_OrganisationOutsideScope_Then_Returns_Forbidden()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = await CreateOrganisation();

        var response = await UpdateOrganisation(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<System.Guid> CreateOrganisation()
    {
        var organisationId = _apiFixture.Fixture.Create<System.Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        return organisationId;
    }

    private async Task<HttpResponseMessage> UpdateOrganisation(HttpClient client, System.Guid organisationId)
        => await ApiFixture.Put(
            client,
            $"/v1/organisations/{organisationId}/limitedtovlimpers",
            new UpdateOrganisationInfoLimitedToVlimpersRequest
            {
                Name = _apiFixture.Fixture.Create<string>(),
                ShortName = _apiFixture.Fixture.Create<string>(),
                Article = null,
                ValidFrom = null,
                ValidTo = null,
                OperationalValidFrom = null,
                OperationalValidTo = null,
            });
}
