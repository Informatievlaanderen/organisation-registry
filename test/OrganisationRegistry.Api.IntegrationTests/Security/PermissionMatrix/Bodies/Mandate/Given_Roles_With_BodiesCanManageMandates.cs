namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Bodies.Mandate;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Body.Mandate;
using OrganisationRegistry.Body;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_BodiesCanManageMandates
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_BodiesCanManageMandates(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();
        var bodyId = await BodyPermissionMatrix.CreateBareBody(_apiFixture);

        var response = await AddBodyMandate(client, bodyId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Orgaanbeheerder_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Orgaanbeheerder);
        var bodyId = await BodyPermissionMatrix.CreateBareBody(_apiFixture);

        var response = await AddBodyMandate(client, bodyId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOwnOrganisation_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);
        var bodyId = await BodyPermissionMatrix.CreateBodyForOrganisation(_apiFixture, _apiFixture.DecentraalbeheerderOrganisationId);

        var response = await BodyPermissionMatrix.WaitUntilAllowed(bodyId, () => AddBodyMandate(client, bodyId));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithChildOrganisationInScope_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);
        var bodyId = await BodyPermissionMatrix.CreateBodyForOrganisation(_apiFixture, _apiFixture.DecentraalbeheerderChildOrganisationId);

        var response = await BodyPermissionMatrix.WaitUntilAllowed(bodyId, () => AddBodyMandate(client, bodyId));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithBodyOutsideScope_Then_Returns_Forbidden()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);
        var bodyId = await BodyPermissionMatrix.CreateBareBody(_apiFixture);

        var response = await AddBodyMandate(client, bodyId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<HttpResponseMessage> AddBodyMandate(HttpClient client, Guid bodyId)
    {
        var seatTypeId = await _apiFixture.Create.SeatType();
        var bodySeatId = await _apiFixture.Create.BodySeat(bodyId, seatTypeId);

        var delegatorOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(delegatorOrganisationId, _apiFixture.Fixture.Create<string>());

        return await ApiFixture.Post(
            client,
            $"/v1/bodies/{bodyId}/mandates",
            new AddBodyMandateRequest
            {
                BodyMandateId = _apiFixture.Fixture.Create<Guid>(),
                BodySeatId = bodySeatId,
                BodyMandateType = BodyMandateType.Organisation,
                DelegatorId = delegatorOrganisationId,
                ValidFrom = null,
                ValidTo = null,
            });
    }
}
