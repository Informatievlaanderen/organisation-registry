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
/// <see cref="Permission.CanManageOrganisation" /> (<c>OrganisationPolicy</c>),
/// die uitsluitend door AlgemeenBeheerder (en Developer) ongerestricteerd
/// gehouden wordt. VlimpersBeheerder en DecentraalBeheerder houden deze
/// permissie niet (meer) — zij gebruiken de gesplitste
/// <c>limitedtovlimpers</c>/<c>notlimitedtovlimpers</c> endpoints in plaats
/// daarvan (zie <c>When_Updating_Organisation_Limited_To_Vlimpers</c> en
/// <c>When_Updating_Organisation_Not_Limited_To_Vlimpers</c>).
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
        using var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var organisationId = await CreateOrganisation();

        using var response = await UpdateOrganisation(client, organisationId);

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
