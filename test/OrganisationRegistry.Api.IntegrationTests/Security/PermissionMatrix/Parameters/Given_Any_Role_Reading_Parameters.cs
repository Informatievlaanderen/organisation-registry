namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Parameters;

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

/// <summary>
/// Reading a parameter master-data list carries no dedicated
/// <c>Parameters*Read</c> permission (removed by design: business decided
/// any authenticated backoffice user must be able to read these lists, e.g.
/// to populate a dropdown while editing its own organisation). This is
/// enforced by the <c>[OrganisationRegistryAuthorize]</c> default
/// (<c>BackofficeUser</c> policy only, no permission check) on every list
/// controller, so every interactive role - privileged or not - gets 200.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Any_Role_Reading_Parameters
{
    private readonly ApiFixture _apiFixture;

    public Given_Any_Role_Reading_Parameters(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [MemberData(nameof(ParameterEndpoints.NonPrivilegedRoleData), MemberType = typeof(ParameterEndpoints))]
    public async Task Then_Reading_Locations_Returns_Ok(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var response = await ApiFixture.Get(client, "/v1/locations");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [MemberData(nameof(ParameterEndpoints.NonPrivilegedRoleData), MemberType = typeof(ParameterEndpoints))]
    public async Task Then_Reading_LocationTypes_Returns_Ok(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var response = await ApiFixture.Get(client, "/v1/locationtypes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [MemberData(nameof(ParameterEndpoints.ListRouteData), MemberType = typeof(ParameterEndpoints))]
    public async Task Then_Reading_Returns_Ok_For_Regelgevingbeheerder(string route)
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Regelgevingbeheerder);

        var response = await ApiFixture.Get(client, $"/v1/{route}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Then_Reading_FormalFrameworks_Returns_Ok_For_Authenticated_User()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Regelgevingbeheerder);

        var response = await ApiFixture.Get(client, "/v1/formalframeworks");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Then_Reading_FormalFrameworks_Returns_Ok_For_Anonymous_User()
    {
        var client = _apiFixture.CreateAnonymousClient();

        var response = await ApiFixture.Get(client, "/v1/formalframeworks");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Then_Reading_OrganisationClassificationTypes_ForOrganisation_Flags_Only_Types_Decentraalbeheerder_Can_Select()
    {
        var regelgevingDbClassificationTypeId = _apiFixture.Configuration.Authorization.OrganisationClassificationTypeIdsOwnedByRegelgevingDbBeheerder.First();
        await _apiFixture.Create.CreateOrganisationClassificationType(regelgevingDbClassificationTypeId);
        var decentraalClassificationTypeId = await _apiFixture.Create.CreateOrganisationClassificationType(false);
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/v1/organisationclassificationtypes?forOrganisationId={_apiFixture.DecentraalbeheerderOrganisationId}");
        request.Headers.Add("x-pagination", "none");
        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = JArray.Parse(await response.Content.ReadAsStringAsync());

        var decentraalItem = items.Single(item => Guid.Parse(item.Value<string>("id")!) == decentraalClassificationTypeId);
        var regelgevingItem = items.Single(item => Guid.Parse(item.Value<string>("id")!) == regelgevingDbClassificationTypeId);

        decentraalItem["permissions"]!.Value<bool>("canSelect").Should().BeTrue();
        regelgevingItem["permissions"]!.Value<bool>("canSelect").Should().BeFalse();
    }
}
