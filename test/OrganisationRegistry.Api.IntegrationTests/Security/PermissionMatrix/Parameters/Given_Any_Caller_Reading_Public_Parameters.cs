namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Parameters;

using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

/// <summary>
/// Reading a parameter master-data list carries no restriction for the
/// public: most list/collection screens (see
/// <see cref="ParameterEndpoints.PubliclyReadableListRoutes"/>) are decorated
/// with <c>[AllowAnonymous]</c> on top of the default
/// <c>[OrganisationRegistryAuthorize]</c>, so even a caller with no bearer
/// token at all (not just an unprivileged backoffice role) must get 200.
///
/// Only <c>organisationrelationtypes</c> is excluded from this set and
/// remains restricted to authenticated backoffice users (see
/// <see cref="Given_Any_Role_Reading_Parameters"/>).
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Any_Caller_Reading_Public_Parameters
{
    private readonly ApiFixture _apiFixture;

    public Given_Any_Caller_Reading_Public_Parameters(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [MemberData(nameof(ParameterEndpoints.PubliclyReadableListRouteData), MemberType = typeof(ParameterEndpoints))]
    public async Task Then_Reading_Returns_Ok_For_Anonymous_Caller(string route)
    {
        using var client = _apiFixture.CreateAnonymousClient();

        using var response = await ApiFixture.Get(client, $"/v1/{route}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Then_Reading_OrganisationRelationTypes_Returns_Unauthorized_For_Anonymous_Caller()
    {
        using var client = _apiFixture.CreateAnonymousClient();

        using var response = await ApiFixture.Get(client, "/v1/organisationrelationtypes");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
