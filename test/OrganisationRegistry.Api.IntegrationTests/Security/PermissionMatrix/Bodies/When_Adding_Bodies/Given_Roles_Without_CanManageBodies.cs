namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Bodies.When_Adding_Bodies;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Body.Detail;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_CanManageBodies
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_CanManageBodies(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orgaanbeheerder)]
    public async Task Then_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var response = await ApiFixture.Post(
            client,
            "/v1/bodies",
            new RegisterBodyRequest
            {
                Id = _apiFixture.Fixture.Create<Guid>(),
                Name = _apiFixture.Fixture.Create<string>(),
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
