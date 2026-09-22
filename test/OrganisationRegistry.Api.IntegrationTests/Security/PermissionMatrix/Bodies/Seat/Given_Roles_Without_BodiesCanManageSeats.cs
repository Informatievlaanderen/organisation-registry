namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Bodies.Seat;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Body.Seat;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_BodiesCanManageSeats
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_BodiesCanManageSeats(ApiFixture apiFixture)
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
            $"/v1/bodies/{bodyId}/seats",
            new AddBodySeatRequest
            {
                BodySeatId = _apiFixture.Fixture.Create<Guid>(),
                Name = _apiFixture.Fixture.Create<string>(),
                PaidSeat = _apiFixture.Fixture.Create<bool>(),
                EntitledToVote = _apiFixture.Fixture.Create<bool>(),
                SeatTypeId = _apiFixture.Fixture.Create<Guid>(),
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
