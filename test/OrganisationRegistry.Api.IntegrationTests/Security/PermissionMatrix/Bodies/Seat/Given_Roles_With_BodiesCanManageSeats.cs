namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Bodies.Seat;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Body.Seat;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_BodiesCanManageSeats
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_BodiesCanManageSeats(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();
        var bodyId = await BodyPermissionMatrix.CreateBareBody(_apiFixture);

        var response = await AddBodySeat(client, bodyId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Orgaanbeheerder_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Orgaanbeheerder);
        var bodyId = await BodyPermissionMatrix.CreateBareBody(_apiFixture);

        var response = await AddBodySeat(client, bodyId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOwnOrganisation_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);
        var bodyId = await BodyPermissionMatrix.CreateBodyForOrganisation(_apiFixture, _apiFixture.DecentraalbeheerderOrganisationId);

        var response = await BodyPermissionMatrix.WaitUntilAllowed(bodyId, () => AddBodySeat(client, bodyId));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithChildOrganisationInScope_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);
        var bodyId = await BodyPermissionMatrix.CreateBodyForOrganisation(_apiFixture, _apiFixture.DecentraalbeheerderChildOrganisationId);

        var response = await BodyPermissionMatrix.WaitUntilAllowed(bodyId, () => AddBodySeat(client, bodyId));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithBodyOutsideScope_Then_Returns_Forbidden()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);
        var bodyId = await BodyPermissionMatrix.CreateBareBody(_apiFixture);

        var response = await AddBodySeat(client, bodyId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<HttpResponseMessage> AddBodySeat(HttpClient client, Guid bodyId)
    {
        var seatTypeId = await _apiFixture.Create.SeatType();

        return await ApiFixture.Post(
            client,
            $"/v1/bodies/{bodyId}/seats",
            new AddBodySeatRequest
            {
                BodySeatId = _apiFixture.Fixture.Create<Guid>(),
                Name = _apiFixture.Fixture.Create<string>(),
                PaidSeat = _apiFixture.Fixture.Create<bool>(),
                EntitledToVote = _apiFixture.Fixture.Create<bool>(),
                SeatTypeId = seatTypeId,
                ValidFrom = null,
                ValidTo = null,
            });
    }
}
