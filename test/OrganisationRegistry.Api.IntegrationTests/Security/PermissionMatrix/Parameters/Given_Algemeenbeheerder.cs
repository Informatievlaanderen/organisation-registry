namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Parameters;

using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

/// <summary>
/// Verifies that <see cref="OrganisationRegistry.Infrastructure.Authorization.Role.AlgemeenBeheerder" />
/// retains full access to the parameter master-data screens after the migration
/// to fine-grained <c>Parameters&lt;Thing&gt;Read/Write/Delete</c> permissions.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Algemeenbeheerder
{
    private readonly ApiFixture _apiFixture;

    public Given_Algemeenbeheerder(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [MemberData(nameof(ParameterEndpoints.ListRouteData), MemberType = typeof(ParameterEndpoints))]
    public async Task Then_Reading_Returns_Ok(string route)
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var response = await ApiFixture.Get(client, $"/v1/{route}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Then_Deleting_A_KeyType_Is_Authorized()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();
        var keyTypeId = await _apiFixture.Create.KeyType(Guid.NewGuid());

        var response = await ApiFixture.Delete(client, $"/v1/keytypes/{keyTypeId}");

        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Then_Deleting_A_Capacity_Is_Authorized()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();
        var capacityId = await _apiFixture.Create.Capacity();

        var response = await ApiFixture.Delete(client, $"/v1/capacities/{capacityId}");

        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }
}
