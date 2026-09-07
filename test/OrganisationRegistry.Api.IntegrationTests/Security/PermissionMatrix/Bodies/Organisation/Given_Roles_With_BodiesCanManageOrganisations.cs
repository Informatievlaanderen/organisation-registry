namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Bodies.Organisation;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Body.Organisation;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_BodiesCanManageOrganisations
{
    // Registering a body for an organisation synchronously adds a body-organisation link
    // whose id equals the body id. That link covers (-inf, +inf), so a second link cannot be
    // added without overlap; therefore the representative verb for this controller is the update (PUT).
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_BodiesCanManageOrganisations(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_Ok()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();
        var organisationId = await CreateOrganisation();
        var bodyId = await BodyPermissionMatrix.CreateBodyForOrganisation(_apiFixture, organisationId);

        var response = await UpdateBodyOrganisation(client, bodyId, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Orgaanbeheerder_Then_Returns_Ok()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Orgaanbeheerder);
        var organisationId = await CreateOrganisation();
        var bodyId = await BodyPermissionMatrix.CreateBodyForOrganisation(_apiFixture, organisationId);

        var response = await UpdateBodyOrganisation(client, bodyId, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOwnOrganisation_Then_Returns_Ok()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);
        var organisationId = _apiFixture.DecentraalbeheerderOrganisationId;
        var bodyId = await BodyPermissionMatrix.CreateBodyForOrganisation(_apiFixture, organisationId);

        var response = await BodyPermissionMatrix.WaitUntilAllowed(bodyId, () => UpdateBodyOrganisation(client, bodyId, organisationId));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithChildOrganisationInScope_Then_Returns_Ok()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);
        var organisationId = _apiFixture.DecentraalbeheerderChildOrganisationId;
        var bodyId = await BodyPermissionMatrix.CreateBodyForOrganisation(_apiFixture, organisationId);

        var response = await BodyPermissionMatrix.WaitUntilAllowed(bodyId, () => UpdateBodyOrganisation(client, bodyId, organisationId));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithBodyOutsideScope_Then_Returns_Forbidden()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);
        var organisationId = await CreateOrganisation();
        var bodyId = await BodyPermissionMatrix.CreateBodyForOrganisation(_apiFixture, organisationId);

        var response = await UpdateBodyOrganisation(client, bodyId, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<Guid> CreateOrganisation()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        return organisationId;
    }

    private async Task<HttpResponseMessage> UpdateBodyOrganisation(HttpClient client, Guid bodyId, Guid organisationId)
        => await ApiFixture.Put(
            client,
            $"/v1/bodies/{bodyId}/organisations/{bodyId}",
            new UpdateBodyOrganisationRequest
            {
                BodyOrganisationId = bodyId,
                OrganisationId = organisationId,
                ValidFrom = null,
                ValidTo = null,
            });
}
