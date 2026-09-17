namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Person.When_Reading_Functions;

using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Xunit;

/// <summary>
/// Elke backoffice-rol mag de functies van een persoon lezen (<c>PeopleFunctionsRead</c>).
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_PeopleFunctionsRead
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_PeopleFunctionsRead(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Algemeenbeheerder)]
    [InlineData(ApiFixture.Backoffice.Decentraalbeheerder)]
    [InlineData(ApiFixture.Backoffice.Vlimpersbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orgaanbeheerder)]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder)]
    public async Task Then_Returns_Ok(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var personId = await _apiFixture.Create.Person();

        var response = await ApiFixture.Get(client, $"/v1/people/{personId}/functions");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
