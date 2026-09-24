namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Children.When_Adding_Parent;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Parent;
using OrganisationRegistry.Api.Backoffice.Vlimpers;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;

/// <summary>
/// Matrixrij <b>Onderliggende organisaties (kinderen)</b> voor de
/// <see cref="Role.VlimpersBeheerder" />. Een vlimpersbeheerder bezit
/// <see cref="Permission.CanManageParent" /> enkel als restricted grant: hij mag
/// de ouder/kind-structuur uitsluitend beheren voor organisaties die onder
/// Vlimpersbeheer vallen. Alle andere combinaties leveren 403 op.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Vlimpersbeheerder
{
    private readonly ApiFixture _apiFixture;

    public Given_Vlimpersbeheerder(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    /// <summary>
    /// Positief: een vlimpersbeheerder mag een bovenliggende organisatie toevoegen
    /// aan een organisatie onder Vlimpersbeheer. Ouder en kind moeten beide onder
    /// Vlimpersbeheer vallen (domeinregel).
    /// </summary>
    [Fact]
    public async Task For_Vlimpersbeheerder_WithVlimpersManagedOrganisation_Then_Returns_Created()
    {
        var childOrganisationId = await CreateVlimpersManagedOrganisation();
        var parentOrganisationId = await CreateVlimpersManagedOrganisation();

        var client = await _apiFixture.CreateBackofficeUserClientFor(ApiFixture.Backoffice.Vlimpersbeheerder);

        var response = await AddParent(client, childOrganisationId, parentOrganisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    /// <summary>
    /// Negatief: een vlimpersbeheerder mag de ouder/kind-structuur niet beheren voor
    /// een organisatie die niet onder Vlimpersbeheer valt.
    /// </summary>
    [Fact]
    public async Task For_Vlimpersbeheerder_WhenOrganisationNotUnderVlimpersManagement_Then_Returns_Forbidden()
    {
        var childOrganisationId = await CreateOrganisation();
        var parentOrganisationId = await CreateOrganisation();

        var client = await _apiFixture.CreateBackofficeUserClientFor(ApiFixture.Backoffice.Vlimpersbeheerder);

        var response = await AddParent(client, childOrganisationId, parentOrganisationId);

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

    private async Task<HttpResponseMessage> AddParent(
        HttpClient client,
        Guid childOrganisationId,
        Guid parentOrganisationId)
        => await ApiFixture.Post(
            client,
            $"/v1/organisations/{childOrganisationId}/parents",
            new AddOrganisationParentRequest
            {
                OrganisationOrganisationParentId = _apiFixture.Fixture.Create<Guid>(),
                ParentOrganisationId = parentOrganisationId,
                ValidFrom = null,
                ValidTo = null,
            });
}
