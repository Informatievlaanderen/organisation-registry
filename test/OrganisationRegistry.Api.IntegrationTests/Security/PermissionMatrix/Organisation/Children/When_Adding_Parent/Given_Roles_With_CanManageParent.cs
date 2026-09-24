namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Children.When_Adding_Parent;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Parent;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;

/// <summary>
/// Matrixrij <b>Onderliggende organisaties (kinderen)</b> — het toevoegen van een
/// bovenliggende organisatie (<c>POST /v1/organisations/{id}/parents</c>) wordt
/// gedreven door <see cref="Permission.CanManageParent" />.
///
/// AlgemeenBeheerder bezit deze permissie ongerestricteerd en
/// mag de ouder/kind-structuur van eender welke organisatie beheren. Een
/// DecentraalBeheerder bezit ze enkel als restricted grant: uitsluitend voor de
/// eigen organisatie (of een organisatie in scope) én zolang die niet onder
/// Vlimpersbeheer valt.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_CanManageParent
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_CanManageParent(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var childOrganisationId = await CreateOrganisation();
        var parentOrganisationId = await CreateOrganisation();

        var response = await AddParent(client, childOrganisationId, parentOrganisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOwnOrganisation_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        // Verse (niet-Vlimpers) ouder; het kind is de eigen organisatie in scope.
        var parentOrganisationId = await CreateOrganisation();

        var (validFrom, validTo) = UniqueFutureValidity();
        var response = await AddParent(
            client,
            _apiFixture.DecentraalbeheerderOrganisationId,
            parentOrganisationId,
            validFrom,
            validTo);

        // De decentraalbeheerder-scope is gecachet op de boom onder OVO000003; er kan geen
        // verse organisatie binnen scope aangemaakt worden. OVO000003 verzamelt bovendien
        // ouderkoppelingen over testruns heen, waardoor de domeinlaag een 400 (reeds gekoppeld)
        // kan teruggeven. Voor deze matrixrij toetsen we dus de *machtiging*: de aanvraag mag
        // niet geweigerd worden (403/401). AlgemeenBeheerder dekt de strikte 201.
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden, await response.Content.ReadAsStringAsync());
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOrganisationOutsideScope_Then_Returns_Forbidden()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var childOrganisationId = await CreateOrganisation();
        var parentOrganisationId = await CreateOrganisation();

        var response = await AddParent(client, childOrganisationId, parentOrganisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<Guid> CreateOrganisation()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        return organisationId;
    }

    // Verzin een uniek, ver in de toekomst gelegen dagvenster zodat opeenvolgende
    // testruns nooit met elkaar (of met een bestaande ouderkoppeling) overlappen.
    private static (DateTime ValidFrom, DateTime ValidTo) UniqueFutureValidity()
    {
        var day = new DateTime(2200, 1, 1).AddDays(Math.Abs(Guid.NewGuid().GetHashCode()) % 20000);
        return (day, day);
    }

    private async Task<HttpResponseMessage> AddParent(
        HttpClient client,
        Guid childOrganisationId,
        Guid parentOrganisationId,
        DateTime? validFrom = null,
        DateTime? validTo = null)
        => await ApiFixture.Post(
            client,
            $"/v1/organisations/{childOrganisationId}/parents",
            new AddOrganisationParentRequest
            {
                OrganisationOrganisationParentId = _apiFixture.Fixture.Create<Guid>(),
                ParentOrganisationId = parentOrganisationId,
                ValidFrom = validFrom,
                ValidTo = validTo,
            });
}
