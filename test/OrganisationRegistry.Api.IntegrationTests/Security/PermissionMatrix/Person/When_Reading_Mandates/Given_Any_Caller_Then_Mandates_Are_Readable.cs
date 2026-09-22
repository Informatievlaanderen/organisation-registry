namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Person.When_Reading_Mandates;

using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Xunit;

/// <summary>
/// Mandaten van een persoon vereisen geen enkel recht: elke rol, en zelfs Publiek
/// (niet-ingelogd), mag ze lezen.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Any_Caller_Then_Mandates_Are_Readable
{
    private readonly ApiFixture _apiFixture;

    public Given_Any_Caller_Then_Mandates_Are_Readable(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Publiek_Then_Returns_Ok()
    {
        var client = _apiFixture.CreateAnonymousClient();

        var personId = await _apiFixture.Create.Person();

        var response = await ApiFixture.Get(client, $"/v1/people/{personId}/mandates");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Algemeenbeheerder)]
    [InlineData(ApiFixture.Backoffice.Decentraalbeheerder)]
    [InlineData(ApiFixture.Backoffice.Vlimpersbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orgaanbeheerder)]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder)]
    public async Task For_Role_Then_Returns_Ok(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var personId = await _apiFixture.Create.Person();

        var response = await ApiFixture.Get(client, $"/v1/people/{personId}/mandates");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
