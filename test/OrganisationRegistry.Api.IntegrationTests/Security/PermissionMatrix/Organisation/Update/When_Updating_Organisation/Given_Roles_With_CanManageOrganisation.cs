namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Update.When_Updating_Organisation;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Detail;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;

/// <summary>
/// Matrixrij <b>Organisatie</b> — het aanpassen van de organisatiegegevens
/// (<c>PUT /v1/organisations/{id}</c>) wordt gedreven door
/// <see cref="Permission.CanManageOrganisation" /> (<c>OrganisationPolicy</c>).
///
/// AlgemeenBeheerder bezit deze permissie ongerestricteerd en mag eender
/// welke organisatie aanpassen. CjmBeheerder heeft deze permissie niet
/// (meer). Een DecentraalBeheerder bezit
/// ze enkel als restricted grant: uitsluitend voor de eigen organisatie en
/// zolang die niet onder Vlimpersbeheer valt.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_CanManageOrganisation
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_CanManageOrganisation(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_OK()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var organisationId = await CreateOrganisation();

        var response = await UpdateOrganisation(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOwnOrganisation_Then_Returns_OK()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        // OVO000003 is de eigen organisatie van de decentraalbeheerder-persona en valt dus binnen de scope.
        var organisationId = _apiFixture.DecentraalbeheerderOrganisationId;

        var response = await UpdateOrganisation(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOrganisationOutsideScope_Then_Returns_Forbidden()
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
            $"/v1/organisations/{organisationId}",
            new UpdateOrganisationInfoRequest
            {
                Name = _apiFixture.Fixture.Create<string>(),
                ShortName = _apiFixture.Fixture.Create<string>(),
                PurposeIds = new List<Guid>(),
                ShowOnVlaamseOverheidSites = false,
                ValidFrom = null,
                ValidTo = null,
            });
}
