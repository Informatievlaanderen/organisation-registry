namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Functions.When_Deleting_Functions;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Function;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_CanManageFunctions
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_CanManageFunctions(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_NoContent()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = await AddFunction(client, organisationId);

        var response = await DeleteFunction(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOwnOrganisation_Then_Returns_NoContent()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.DecentraalbeheerderOrganisationId;
        var entityId = await AddFunction(client, organisationId);

        var response = await DeleteFunction(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithChildOrganisationInScope_Then_Returns_NoContent()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.DecentraalbeheerderChildOrganisationId;
        var entityId = await AddFunction(client, organisationId);

        var response = await DeleteFunction(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOrganisationOutsideScope_Then_Returns_Forbidden()
    {
        var privilegedClient = await _apiFixture.CreateAlgemeenbeheerderClient();
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = await AddFunction(privilegedClient, organisationId);

        var response = await DeleteFunction(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<Guid> AddFunction(HttpClient client, Guid organisationId)
    {
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var functionTypeId = await _apiFixture.Create.Function();
        var personId = await _apiFixture.Create.Person();

        await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/functions",
            new AddOrganisationFunctionRequest()
            {
                OrganisationFunctionId = entityId,
                FunctionId = functionTypeId,
                PersonId = personId,
                Contacts = null,
                ValidFrom = null,
                ValidTo = null,
            });

        return entityId;
    }


    private async Task<HttpResponseMessage> DeleteFunction(HttpClient client, Guid organisationId, Guid entityId)
    {
        return await ApiFixture.Delete(
            client,
            $"/v1/organisations/{organisationId}/functions/{entityId}");
    }
}
