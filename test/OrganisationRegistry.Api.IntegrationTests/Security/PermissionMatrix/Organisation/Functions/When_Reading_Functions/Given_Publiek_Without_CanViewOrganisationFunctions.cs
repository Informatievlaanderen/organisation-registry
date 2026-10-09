namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Functions.When_Reading_Functions;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Xunit;

/// <summary>
/// Publiek (niet-ingelogd) heeft geen toegang tot de functies van een organisatie
/// (<c>CanViewOrganisationFunctions</c>) en krijgt 401 terug (geen geldig token).
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Publiek_Without_CanViewOrganisationFunctions
{
    private readonly ApiFixture _apiFixture;

    public Given_Publiek_Without_CanViewOrganisationFunctions(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task Then_Returns_Unauthorized()
    {
        using var _client = await _apiFixture.CreateAlgemeenbeheerderClient();
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(
            organisationId,
            _apiFixture.Fixture.Create<string>(),
            _apiFixture.Fixture.Create<string>(),
            _client);

        using var client = _apiFixture.CreateAnonymousClient();
        using var response = await ApiFixture.Get(client, $"/v1/organisations/{organisationId}/functions");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

}
