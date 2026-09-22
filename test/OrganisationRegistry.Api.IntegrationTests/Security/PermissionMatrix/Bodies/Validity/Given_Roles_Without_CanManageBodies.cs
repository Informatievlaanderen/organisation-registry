namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Bodies.Validity;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Body.Validity;
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
    public async Task Then_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);
        var bodyId = _apiFixture.Fixture.Create<Guid>();

        var response = await ApiFixture.Put(
            client,
            $"/v1/bodies/{bodyId}/validity",
            new UpdateBodyValidityRequest
            {
                FormalValidFrom = null,
                FormalValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
