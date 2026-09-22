namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Person.When_Updating_Person;

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Person.Detail;
using OrganisationRegistry.Person;
using Xunit;

/// <summary>
/// Alleen AlgemeenBeheerder mag een persoon aanpassen (<c>PeopleWrite</c>).
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_PeopleWrite
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_PeopleWrite(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_Ok()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var personId = await _apiFixture.Create.Person();

        var response = await UpdatePerson(client, personId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<HttpResponseMessage> UpdatePerson(HttpClient client, System.Guid personId)
        => await ApiFixture.Put(
            client,
            $"/v1/people/{personId}",
            new UpdatePersonRequest
            {
                FirstName = _apiFixture.Fixture.Create<string>(),
                Name = _apiFixture.Fixture.Create<string>(),
                Sex = Sex.Female,
                DateOfBirth = _apiFixture.Fixture.Create<System.DateTime>(),
            });
}
