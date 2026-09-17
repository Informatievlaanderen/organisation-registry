namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Person.When_Creating_Person;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Person.Detail;
using OrganisationRegistry.Person;
using Xunit;

/// <summary>
/// Rollen zonder <c>PeopleWrite</c> mogen geen persoon aanmaken en krijgen 403 terug.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_PeopleWrite
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_PeopleWrite(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Decentraalbeheerder)]
    [InlineData(ApiFixture.Backoffice.Vlimpersbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orgaanbeheerder)]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder)]
    [InlineData(ApiFixture.Backoffice.Cjmbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orafinbeheerder)]
    public async Task Then_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var response = await ApiFixture.Post(
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

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
