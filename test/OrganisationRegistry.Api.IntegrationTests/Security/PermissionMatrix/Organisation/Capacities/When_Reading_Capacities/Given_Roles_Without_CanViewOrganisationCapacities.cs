namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Capacities.When_Reading_Capacities;

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Capacity;
using OrganisationRegistry.Api.Backoffice.Organisation.Detail;
using Xunit;

/// <summary>
/// Publiek (niet-ingelogd) heeft geen toegang tot de hoedanigheden van een organisatie
/// (<c>CanViewOrganisationCapacities</c>) en krijgt 401 terug (geen geldig token).
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_CanViewOrganisationCapacities
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_CanViewOrganisationCapacities(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task Then_Returns_Unauthorized()
    {
        var _client = await _apiFixture.CreateDecentraalBeheerderClient();
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>(),_apiFixture.Fixture.Create<string>(), _client);

        var client = _apiFixture.CreateAnonymousClient();
        var response = await ApiFixture.Get(client, $"/v1/organisations/{organisationId}/capacities");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
