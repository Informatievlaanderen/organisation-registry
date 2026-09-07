namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Bodies.Mandate;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Body.Mandate;
using OrganisationRegistry.Body;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_BodiesCanManageMandates
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_BodiesCanManageMandates(ApiFixture apiFixture)
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
            $"/v1/bodies/{bodyId}/mandates",
            new AddBodyMandateRequest
            {
                BodyMandateId = _apiFixture.Fixture.Create<Guid>(),
                BodySeatId = _apiFixture.Fixture.Create<Guid>(),
                BodyMandateType = BodyMandateType.Organisation,
                DelegatorId = _apiFixture.Fixture.Create<Guid>(),
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
