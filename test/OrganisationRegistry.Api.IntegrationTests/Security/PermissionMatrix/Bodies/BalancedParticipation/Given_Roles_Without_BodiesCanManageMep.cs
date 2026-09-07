namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Bodies.BalancedParticipation;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Body.BalancedParticipation;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_BodiesCanManageMep
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_BodiesCanManageMep(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    // DecentraalBeheerder is explicitly excluded from MEP-decreet (lacks BodiesCanManageMep).
    [Theory]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder)]
    [InlineData(ApiFixture.Backoffice.Decentraalbeheerder)]
    public async Task Then_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);
        var bodyId = _apiFixture.Fixture.Create<Guid>();

        var response = await ApiFixture.Put(
            client,
            $"/v1/bodies/{bodyId}/balancedparticipation",
            new UpdateBodyBalancedParticipationRequest
            {
                Obligatory = false,
                ExtraRemark = _apiFixture.Fixture.Create<string>(),
                ExceptionMeasure = _apiFixture.Fixture.Create<string>(),
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
