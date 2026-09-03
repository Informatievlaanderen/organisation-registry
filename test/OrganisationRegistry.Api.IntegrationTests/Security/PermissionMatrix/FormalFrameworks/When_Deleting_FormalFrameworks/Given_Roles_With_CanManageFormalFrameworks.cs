namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.FormalFrameworks.When_Deleting_FormalFrameworks;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.FormalFramework;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_CanManageFormalFrameworks
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_CanManageFormalFrameworks(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_NoContent()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = await AddFormalFramework(client, organisationId);

        var response = await DeleteFormalFramework(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOwnOrganisation_Then_Returns_NoContent()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.DecentraalbeheerderOrganisationId;
        var entityId = await AddFormalFramework(client, organisationId);

        var response = await DeleteFormalFramework(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithChildOrganisationInScope_Then_Returns_NoContent()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.DecentraalbeheerderChildOrganisationId;
        var entityId = await AddFormalFramework(client, organisationId);

        var response = await DeleteFormalFramework(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOrganisationOutsideScope_Then_Returns_Forbidden()
    {
        var privilegedClient = await _apiFixture.CreateAlgemeenbeheerderClient();
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = await AddFormalFramework(privilegedClient, organisationId);

        var response = await DeleteFormalFramework(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact(Skip = "TODO: scoped role 'Regelgevingbeheerder' is allowed by the permission matrix but the domain authorization policy requires the organisation (or entity) to be within the role's own scope (BeheerderForOrganisation / configured owned-ids). No fixture precedent exists for creating an organisation inside a scoped role's Keycloak OVO scope, so this positive cannot yet assert a 2xx. Enable once scoped-org test setup is available.")]
    public async Task For_Regelgevingbeheerder_Then_Returns_NoContent()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Regelgevingbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var categoryId = await _apiFixture.Create.FormalFrameworkCategory();
        var formalFrameworkId = await _apiFixture.Create.FormalFramework(categoryId);
        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());

        await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/formalframeworks",
            new AddOrganisationFormalFrameworkRequest()
            {
                OrganisationFormalFrameworkId = entityId,
                FormalFrameworkId = formalFrameworkId,
                ParentOrganisationId = parentOrganisationId,
                ValidFrom = null,
                ValidTo = null,
            });

        var response = await ApiFixture.Delete(
            client,
            $"/v1/organisations/{organisationId}/formalframeworks/{entityId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task<Guid> AddFormalFramework(HttpClient client, Guid organisationId)
    {
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var categoryId = await _apiFixture.Create.FormalFrameworkCategory();
        var formalFrameworkId = await _apiFixture.Create.FormalFramework(categoryId);
        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());

        await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/formalframeworks",
            new AddOrganisationFormalFrameworkRequest()
            {
                OrganisationFormalFrameworkId = entityId,
                FormalFrameworkId = formalFrameworkId,
                ParentOrganisationId = parentOrganisationId,
                ValidFrom = null,
                ValidTo = null,
            });

        return entityId;
    }


    private async Task<HttpResponseMessage> DeleteFormalFramework(HttpClient client, Guid organisationId, Guid entityId)
    {
        return await ApiFixture.Delete(
            client,
            $"/v1/organisations/{organisationId}/formalframeworks/{entityId}");
    }
}
