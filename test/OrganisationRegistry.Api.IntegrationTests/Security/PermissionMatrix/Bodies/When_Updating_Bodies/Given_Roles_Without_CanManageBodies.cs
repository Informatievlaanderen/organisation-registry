namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Bodies.When_Updating_Bodies;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Body.Organisation;
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

        var bodyId = _apiFixture.Fixture.Create<Guid>();
        var entityId = _apiFixture.Fixture.Create<Guid>();

        var response = await ApiFixture.Put(
            client,
            $"/v1/bodies/{bodyId}/organisations/{entityId}",
            new UpdateBodyOrganisationRequest
            {
                BodyOrganisationId = entityId,
                OrganisationId = _apiFixture.Fixture.Create<Guid>(),
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
