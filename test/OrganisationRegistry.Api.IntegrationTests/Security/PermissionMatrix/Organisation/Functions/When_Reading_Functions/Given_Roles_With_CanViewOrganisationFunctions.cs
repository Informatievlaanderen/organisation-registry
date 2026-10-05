namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Functions.When_Reading_Functions;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Function;
using Xunit;

/// <summary>
/// Elke backoffice-rol mag de functies van een organisatie lezen (<c>CanViewOrganisationFunctions</c>).
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_CanViewOrganisationFunctions
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_CanViewOrganisationFunctions(ApiFixture apiFixture)
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
        var response = await ApiFixture.Get(client, $"/v1/organisations/{organisationId}/functions");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
