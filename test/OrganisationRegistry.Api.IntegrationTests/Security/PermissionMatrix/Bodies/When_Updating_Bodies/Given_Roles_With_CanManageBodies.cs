namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Bodies.When_Updating_Bodies;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Body.Organisation;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_CanManageBodies
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_CanManageBodies(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_OK()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var bodyId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Body(bodyId, _apiFixture.Fixture.Create<string>());
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = _apiFixture.Fixture.Create<Guid>();

        await ApiFixture.Post(
            client,
            $"/v1/bodies/{bodyId}/organisations",
            new AddBodyOrganisationRequest
            {
                BodyOrganisationId = entityId,
                OrganisationId = organisationId,
                ValidFrom = null,
                ValidTo = null,
            });

        var response = await ApiFixture.Put(
            client,
            $"/v1/bodies/{bodyId}/organisations/{entityId}",
            new UpdateBodyOrganisationRequest
            {
                BodyOrganisationId = entityId,
                OrganisationId = organisationId,
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(Skip = "TODO: scoped role 'Decentraalbeheerder' is allowed by the permission matrix but EditBodyPolicy only grants a DecentraalBeheerder access when it is beheerder for the body (IsDecentraalBeheerderForBody). No fixture precedent exists for creating a body inside a scoped role's Keycloak scope, so this positive cannot yet assert a 2xx. Enable once scoped-body test setup is available.")]
    public async Task For_Decentraalbeheerder_Then_Returns_OK()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var bodyId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Body(bodyId, _apiFixture.Fixture.Create<string>());
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = _apiFixture.Fixture.Create<Guid>();

        await ApiFixture.Post(
            client,
            $"/v1/bodies/{bodyId}/organisations",
            new AddBodyOrganisationRequest
            {
                BodyOrganisationId = entityId,
                OrganisationId = organisationId,
                ValidFrom = null,
                ValidTo = null,
            });

        var response = await ApiFixture.Put(
            client,
            $"/v1/bodies/{bodyId}/organisations/{entityId}",
            new UpdateBodyOrganisationRequest
            {
                BodyOrganisationId = entityId,
                OrganisationId = organisationId,
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
