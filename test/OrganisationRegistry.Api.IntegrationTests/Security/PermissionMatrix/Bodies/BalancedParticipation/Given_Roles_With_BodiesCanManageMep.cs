namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Bodies.BalancedParticipation;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Body.BalancedParticipation;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_BodiesCanManageMep
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_BodiesCanManageMep(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_Ok()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();
        var bodyId = await BodyPermissionMatrix.CreateBareBody(_apiFixture);

        var response = await UpdateBalancedParticipation(client, bodyId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Orgaanbeheerder_Then_Returns_Ok()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Orgaanbeheerder);
        var bodyId = await BodyPermissionMatrix.CreateBareBody(_apiFixture);

        var response = await UpdateBalancedParticipation(client, bodyId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<HttpResponseMessage> UpdateBalancedParticipation(HttpClient client, Guid bodyId)
        => await ApiFixture.Put(
            client,
            $"/v1/bodies/{bodyId}/balancedparticipation",
            new UpdateBodyBalancedParticipationRequest
            {
                Obligatory = false,
                ExtraRemark = _apiFixture.Fixture.Create<string>(),
                ExceptionMeasure = _apiFixture.Fixture.Create<string>(),
            });
}
