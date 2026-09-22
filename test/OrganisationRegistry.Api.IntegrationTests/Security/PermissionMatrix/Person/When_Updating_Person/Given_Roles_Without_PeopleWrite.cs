namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Person.When_Updating_Person;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Person.Detail;
using OrganisationRegistry.Person;
using Xunit;

/// <summary>
/// Rollen zonder <c>PeopleWrite</c> mogen een persoon niet aanpassen en krijgen 403 terug.
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

        var personId = await _apiFixture.Create.Person();

        var response = await ApiFixture.Put(
            client,
            $"/v1/people/{personId}",
            new UpdatePersonRequest
            {
                FirstName = _apiFixture.Fixture.Create<string>(),
                Name = _apiFixture.Fixture.Create<string>(),
                Sex = Sex.Female,
                DateOfBirth = _apiFixture.Fixture.Create<DateTime>(),
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
