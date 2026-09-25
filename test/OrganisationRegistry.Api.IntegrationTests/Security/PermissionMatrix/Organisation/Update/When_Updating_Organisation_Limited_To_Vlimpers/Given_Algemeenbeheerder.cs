namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Update.When_Updating_Organisation_Limited_To_Vlimpers;

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
/// Matrixrij <b>Organisatie</b> — het aanpassen van de vier velden die
/// voorbehouden zijn aan Vlimpers (formele naam, formele korte naam, lidwoord,
/// operationele geldigheid — <c>PUT /v1/organisations/{id}/limitedtovlimpers</c>)
/// wordt gedreven door <see cref="Permission.CanManageOrganisationInfoLimitedToVlimpers" />
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
