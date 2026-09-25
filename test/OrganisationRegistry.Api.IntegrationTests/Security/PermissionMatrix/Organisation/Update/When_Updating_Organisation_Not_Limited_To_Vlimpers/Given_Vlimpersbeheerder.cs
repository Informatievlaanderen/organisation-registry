namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Update.When_Updating_Organisation_Not_Limited_To_Vlimpers;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Detail;
using OrganisationRegistry.Api.Backoffice.Vlimpers;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;

/// <summary>
/// Matrixrij <b>Organisatie</b> — de niet-Vlimpers-voorbehouden velden
/// aanpassen (<c>PUT /v1/organisations/{id}/notlimitedtovlimpers</c>) — voor
/// <see cref="Role.VlimpersBeheerder" />. VlimpersBeheerder houdt
/// <see cref="Permission.CanManageOrganisationInfoNotLimitedToVlimpers" /> niet
/// (dat zijn net de velden die <em>niet</em> aan Vlimpers voorbehouden zijn) —
/// dit blijft zo, ongeacht of de organisatie onder Vlimpersbeheer valt.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Vlimpersbeheerder
{
    private readonly ApiFixture _apiFixture;

    public Given_Vlimpersbeheerder(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Vlimpersbeheerder_WithVlimpersManagedOrganisation_Then_Returns_Forbidden()
    {
        var organisationId = await CreateVlimpersManagedOrganisation();

        var client = await _apiFixture.CreateBackofficeUserClientFor(ApiFixture.Backoffice.Vlimpersbeheerder);

        var response = await UpdateOrganisation(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task For_Vlimpersbeheerder_WhenOrganisationNotUnderVlimpersManagement_Then_Returns_Forbidden()
    {
        var organisationId = await CreateOrganisation();

        var client = await _apiFixture.CreateBackofficeUserClientFor(ApiFixture.Backoffice.Vlimpersbeheerder);

        var response = await UpdateOrganisation(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<Guid> CreateOrganisation()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        return organisationId;
    }

    private async Task<Guid> CreateVlimpersManagedOrganisation()
    {
        var organisationId = await CreateOrganisation();

        var algemeenbeheerderClient = await _apiFixture.CreateAlgemeenbeheerderClient();
        var response = await ApiFixture.Patch(
            algemeenbeheerderClient,
            $"/v1/organisations/{organisationId}/vlimpers",
            new VlimpersRequest { VlimpersManagement = true });
        response.StatusCode.Should().Be(HttpStatusCode.OK);

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
