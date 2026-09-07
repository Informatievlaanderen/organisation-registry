namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Bodies.Classification;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Body.BodyClassification;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_BodiesCanManageClassifications
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_BodiesCanManageClassifications(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder)]
    public async Task Then_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);
        var bodyId = _apiFixture.Fixture.Create<Guid>();

        var response = await ApiFixture.Post(
            client,
            $"/v1/bodies/{bodyId}/classifications",
            new AddBodyBodyClassificationRequest
            {
                BodyBodyClassificationId = _apiFixture.Fixture.Create<Guid>(),
                BodyClassificationTypeId = _apiFixture.Fixture.Create<Guid>(),
                BodyClassificationId = _apiFixture.Fixture.Create<Guid>(),
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
