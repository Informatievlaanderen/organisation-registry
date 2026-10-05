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

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_CanViewOrganisationCapacities
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_CanViewOrganisationCapacities(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Algemeenbeheerder)]
    [InlineData(ApiFixture.Backoffice.Decentraalbeheerder)]
    [InlineData(ApiFixture.Backoffice.Vlimpersbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orgaanbeheerder)]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder)]
    [InlineData(ApiFixture.Backoffice.VoMedewerker)]
    public async Task Then_Returns_Ok(string role)
    {
        var _client = await _apiFixture.CreateDecentraalBeheerderClient();
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>(),_apiFixture.Fixture.Create<string>(), _client);

        var client = await _apiFixture.CreateDynamicClient(role);
        var response = await ApiFixture.Get(client, $"/v1/organisations/{organisationId}/capacities");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
