namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Parameters;

using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

/// <summary>
/// Verifies that interactive roles without the fine-grained parameter
/// permissions are still denied (403) on writing/deleting parameter
/// master-data, even though reading those same lists is now open to any
/// authenticated backoffice user (see <see cref="Given_Any_Role_Reading_Parameters"/>).
///
/// The assertions are factorised: one theory proves every route is gated
/// (single representative role across all routes), a second proves every
/// non-privileged role is denied (single representative route across all
/// roles). Together they cover the "every route × every role → 403" matrix
/// without paying for the full cross product of Keycloak logins.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_Parameter_Permissions
{
    // Regelgevingbeheerder is a scoped role that legitimately manages some
    // organisation-scoped data, which makes it the strongest witness that
    // master-data parameter screens remain out of reach for non-AB roles.
    private const string RepresentativeRole = ApiFixture.Backoffice.Regelgevingbeheerder;

    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_Parameter_Permissions(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [MemberData(nameof(ParameterEndpoints.ListRouteData), MemberType = typeof(ParameterEndpoints))]
    public async Task Then_Writing_Returns_Forbidden(string route)
    {
        var client = await _apiFixture.CreateDynamicClient(RepresentativeRole);

        // Authorization filters run before model binding, so the (empty) body
        // is irrelevant: the request must be rejected with 403, never 400.
        var response = await ApiFixture.Post(client, $"/v1/{route}", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [MemberData(nameof(ParameterEndpoints.NonPrivilegedRoleData), MemberType = typeof(ParameterEndpoints))]
    public async Task Then_Writing_Locations_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var response = await ApiFixture.Post(client, "/v1/locations", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [MemberData(nameof(ParameterEndpoints.NonPrivilegedRoleData), MemberType = typeof(ParameterEndpoints))]
    public async Task Then_Deleting_KeyType_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var response = await ApiFixture.Delete(client, $"/v1/keytypes/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [MemberData(nameof(ParameterEndpoints.NonPrivilegedRoleData), MemberType = typeof(ParameterEndpoints))]
    public async Task Then_Deleting_Capacity_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var response = await ApiFixture.Delete(client, $"/v1/capacities/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
