namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Bodies.When_Adding_Bodies;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Body.Detail;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_CanManageBodies
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_CanManageBodies(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        await _apiFixture.Create.Body(_apiFixture.Fixture.Create<Guid>(), _apiFixture.Fixture.Create<string>());

        var response = await ApiFixture.Post(
            client,
            "/v1/bodies",
            new RegisterBodyRequest
            {
                Id = _apiFixture.Fixture.Create<Guid>(),
                Name = _apiFixture.Fixture.Create<string>(),
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact(Skip = "TODO: scoped role 'Decentraalbeheerder' is allowed by the permission matrix but RegisterBodyPolicy only grants a DecentraalBeheerder access when the body is registered for an organisation within the role's own scope (IsDecentraalBeheerderForOrganisation). No fixture precedent exists for creating an organisation inside a scoped role's Keycloak OVO scope, so this positive cannot yet assert a 2xx. Enable once scoped-org test setup is available.")]
    public async Task For_Decentraalbeheerder_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        await _apiFixture.Create.Body(_apiFixture.Fixture.Create<Guid>(), _apiFixture.Fixture.Create<string>());

        var response = await ApiFixture.Post(
            client,
            "/v1/bodies",
            new RegisterBodyRequest
            {
                Id = _apiFixture.Fixture.Create<Guid>(),
                Name = _apiFixture.Fixture.Create<string>(),
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
