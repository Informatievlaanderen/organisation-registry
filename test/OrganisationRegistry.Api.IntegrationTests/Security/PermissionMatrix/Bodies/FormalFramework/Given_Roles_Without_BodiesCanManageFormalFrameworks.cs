namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Bodies.FormalFramework;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Body.FormalFramework;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_BodiesCanManageFormalFrameworks
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_BodiesCanManageFormalFrameworks(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder)]
    public async Task Adding_Then_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);
        var bodyId = _apiFixture.Fixture.Create<Guid>();

        var response = await ApiFixture.Post(
            client,
            $"/v1/bodies/{bodyId}/formalframeworks",
            new AddBodyFormalFrameworkRequest
            {
                BodyFormalFrameworkId = _apiFixture.Fixture.Create<Guid>(),
                FormalFrameworkId = _apiFixture.Fixture.Create<Guid>(),
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder)]
    public async Task Updating_Then_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);
        var bodyId = _apiFixture.Fixture.Create<Guid>();
        var bodyFormalFrameworkId = _apiFixture.Fixture.Create<Guid>();

        var response = await ApiFixture.Put(
            client,
            $"/v1/bodies/{bodyId}/formalframeworks/{bodyFormalFrameworkId}",
            new UpdateBodyFormalFrameworkRequest
            {
                BodyFormalFrameworkId = bodyFormalFrameworkId,
                FormalFrameworkId = _apiFixture.Fixture.Create<Guid>(),
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
