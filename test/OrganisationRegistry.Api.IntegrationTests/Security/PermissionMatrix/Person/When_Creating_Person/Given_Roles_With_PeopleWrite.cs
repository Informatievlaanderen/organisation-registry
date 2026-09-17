namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Person.When_Creating_Person;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Person.Detail;
using OrganisationRegistry.Person;
using Xunit;

/// <summary>
/// Alleen AlgemeenBeheerder mag een persoon aanmaken (<c>PeopleWrite</c>).
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
    public async Task For_Algemeenbeheerder_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var response = await CreatePerson(client);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    private async Task<HttpResponseMessage> CreatePerson(HttpClient client)
        => await ApiFixture.Post(
            client,
            "/v1/people",
            new CreatePersonRequest
            {
                Id = _apiFixture.Fixture.Create<Guid>(),
                FirstName = _apiFixture.Fixture.Create<string>(),
                Name = _apiFixture.Fixture.Create<string>(),
                Sex = Sex.Male,
                DateOfBirth = _apiFixture.Fixture.Create<DateTime>(),
            });
}
