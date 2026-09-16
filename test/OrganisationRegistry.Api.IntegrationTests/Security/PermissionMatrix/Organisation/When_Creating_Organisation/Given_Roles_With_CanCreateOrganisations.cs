namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.When_Creating_Organisation;

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
/// Matrixrij <b>Organisatie</b> — het registreren van een <em>top-level</em>
/// organisatie (<c>POST /v1/organisations</c> zonder <c>parentOrganisationId</c>)
/// wordt gedreven door <see cref="Permission.CanCreateOrganisations" />.
///
/// Deze permissie is uitsluitend ongerestricteerd toegekend, en enkel aan
/// AlgemeenBeheerder: er is geen bestaande (ouder)organisatie om een restrictie
/// tegen te evalueren, dus VlimpersBeheerder/DecentraalBeheerder — die hun
/// rechten enkel als restricted grant bezitten — komen hier nooit voor in
/// aanmerking, ongeacht Vlimpersbeheer of eigen organisatie.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_CanCreateOrganisations
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_CanCreateOrganisations(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var response = await CreateTopLevelOrganisation(client);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    private async Task<HttpResponseMessage> CreateTopLevelOrganisation(HttpClient client)
        => await ApiFixture.Post(
            client,
            "/v1/organisations",
            new CreateOrganisationRequest
            {
                Id = _apiFixture.Fixture.Create<Guid>(),
                Name = _apiFixture.Fixture.Create<string>(),
                ParentOrganisationId = null,
            });
}
