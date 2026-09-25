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
/// Matrixrij <b>Organisatie</b> — het aanpassen van de organisatievelden die
/// <em>niet</em> voorbehouden zijn aan Vlimpers (omschrijving, doelstellingen,
/// tonen op Vlaamse overheid sites — <c>PUT /v1/organisations/{id}/notlimitedtovlimpers</c>)
/// wordt gedreven door <see cref="Permission.CanManageOrganisationInfoNotLimitedToVlimpers" />
/// (<c>OrganisationPolicy</c>). AlgemeenBeheerder bezit deze permissie
/// ongerestricteerd en mag ze op eender welke organisatie aanpassen,
/// ongeacht Vlimpersbeheer.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Algemeenbeheerder
{
    private readonly ApiFixture _apiFixture;

    public Given_Algemeenbeheerder(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Organisation_Not_Under_Vlimpers_Management_Then_Returns_OK()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var organisationId = await CreateOrganisation();

        var response = await UpdateOrganisation(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
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
