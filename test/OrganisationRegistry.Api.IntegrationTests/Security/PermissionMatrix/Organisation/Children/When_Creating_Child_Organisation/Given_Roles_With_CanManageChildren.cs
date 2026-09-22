namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Children.When_Creating_Child_Organisation;

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
/// Matrixrij <b>Organisatie</b> — het registreren van een <em>dochter</em>organisatie
/// (<c>POST /v1/organisations</c> met <c>parentOrganisationId</c>) wordt gedreven
/// door <see cref="Permission.CanManageChildren" /> op de opgegeven
/// ouderorganisatie (<c>ChildPolicy</c>), net zoals het koppelen van een
/// bestaande ouder (<c>POST /v1/organisations/{id}/parents</c>).
///
/// AlgemeenBeheerder bezit deze permissie ongerestricteerd. Een
/// DecentraalBeheerder bezit ze enkel als restricted grant: uitsluitend voor de
/// eigen organisatie (of een organisatie in scope) én zolang die niet onder
/// Vlimpersbeheer valt.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_CanManageChildren
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_CanManageChildren(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var parentOrganisationId = await CreateOrganisation();

        var response = await CreateDaughterOrganisation(client, parentOrganisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOwnOrganisationAsParent_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        // OVO000003 is de eigen organisatie van de decentraalbeheerder-persona en valt dus binnen de scope.
        var response = await CreateDaughterOrganisation(client, _apiFixture.DecentraalbeheerderOrganisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOrganisationOutsideScopeAsParent_Then_Returns_Forbidden()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var parentOrganisationId = await CreateOrganisation();

        var response = await CreateDaughterOrganisation(client, parentOrganisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<Guid> CreateOrganisation()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        return organisationId;
    }

    private async Task<HttpResponseMessage> CreateDaughterOrganisation(HttpClient client, Guid parentOrganisationId)
        => await ApiFixture.Post(
            client,
            "/v1/organisations",
            new CreateOrganisationRequest
            {
                Id = _apiFixture.Fixture.Create<Guid>(),
                Name = _apiFixture.Fixture.Create<string>(),
                ParentOrganisationId = parentOrganisationId,
            });
}
