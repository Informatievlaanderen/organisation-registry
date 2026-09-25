namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Update.When_Updating_Organisation;

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Detail;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;
using AutoFixture;

/// <summary>
/// Matrixrij <b>Organisatie</b> — organisatiegegevens aanpassen — voor
/// <see cref="Role.DecentraalBeheerder" />. DecentraalBeheerder houdt
/// <see cref="Permission.CanManageOrganisation" /> niet (meer): het algemene
/// <c>PUT /v1/organisations/{id}</c> endpoint is uitsluitend voor
/// AlgemeenBeheerder/Developer. Dit blijft zo, zelfs voor de eigen
/// organisatie — hij moet het gesplitste <c>notlimitedtovlimpers</c>
/// endpoint gebruiken.
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
    public async Task For_Decentraalbeheerder_WithOwnOrganisation_Then_Returns_Forbidden()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        // OVO000003 is de eigen organisatie van de decentraalbeheerder-persona.
        var organisationId = _apiFixture.DecentraalbeheerderOrganisationId;

        var response = await UpdateOrganisation(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<HttpResponseMessage> UpdateOrganisation(HttpClient client, System.Guid organisationId)
        => await ApiFixture.Put(
            client,
            $"/v1/organisations/{organisationId}",
            new UpdateOrganisationInfoRequest
            {
                Name = _apiFixture.Fixture.Create<string>(),
                ShortName = _apiFixture.Fixture.Create<string>(),
                ShowOnVlaamseOverheidSites = false,
                ValidFrom = null,
                ValidTo = null,
            });
}
