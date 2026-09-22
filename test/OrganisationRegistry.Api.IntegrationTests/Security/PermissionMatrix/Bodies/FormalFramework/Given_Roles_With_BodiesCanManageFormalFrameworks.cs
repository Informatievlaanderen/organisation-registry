namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Bodies.FormalFramework;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Body.FormalFramework;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_BodiesCanManageFormalFrameworks
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_BodiesCanManageFormalFrameworks(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_AddingThenUpdating_Then_Succeeds()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();
        var bodyId = await BodyPermissionMatrix.CreateBareBody(_apiFixture);

        var (formalFrameworkId, bodyFormalFrameworkId) = await CreateFormalFramework();

        var addResponse = await AddBodyFormalFramework(client, bodyId, bodyFormalFrameworkId, formalFrameworkId);
        addResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var updateResponse = await UpdateBodyFormalFramework(client, bodyId, bodyFormalFrameworkId, formalFrameworkId);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Orgaanbeheerder_AddingThenUpdating_Then_Succeeds()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Orgaanbeheerder);
        var bodyId = await BodyPermissionMatrix.CreateBareBody(_apiFixture);

        var (formalFrameworkId, bodyFormalFrameworkId) = await CreateFormalFramework();

        var addResponse = await AddBodyFormalFramework(client, bodyId, bodyFormalFrameworkId, formalFrameworkId);
        addResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var updateResponse = await UpdateBodyFormalFramework(client, bodyId, bodyFormalFrameworkId, formalFrameworkId);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOwnOrganisation_AddingThenUpdating_Then_Succeeds()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);
        var bodyId = await BodyPermissionMatrix.CreateBodyForOrganisation(_apiFixture, _apiFixture.DecentraalbeheerderOrganisationId);

        var (formalFrameworkId, bodyFormalFrameworkId) = await CreateFormalFramework();

        var addResponse = await BodyPermissionMatrix.WaitUntilAllowed(
            bodyId,
            () => AddBodyFormalFramework(client, bodyId, bodyFormalFrameworkId, formalFrameworkId));
        addResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var updateResponse = await UpdateBodyFormalFramework(client, bodyId, bodyFormalFrameworkId, formalFrameworkId);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithChildOrganisationInScope_AddingThenUpdating_Then_Succeeds()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);
        var bodyId = await BodyPermissionMatrix.CreateBodyForOrganisation(_apiFixture, _apiFixture.DecentraalbeheerderChildOrganisationId);

        var (formalFrameworkId, bodyFormalFrameworkId) = await CreateFormalFramework();

        var addResponse = await BodyPermissionMatrix.WaitUntilAllowed(
            bodyId,
            () => AddBodyFormalFramework(client, bodyId, bodyFormalFrameworkId, formalFrameworkId));
        addResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var updateResponse = await UpdateBodyFormalFramework(client, bodyId, bodyFormalFrameworkId, formalFrameworkId);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithBodyOutsideScope_Adding_Then_Returns_Forbidden()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);
        var bodyId = await BodyPermissionMatrix.CreateBareBody(_apiFixture);

        var (formalFrameworkId, bodyFormalFrameworkId) = await CreateFormalFramework();

        var addResponse = await AddBodyFormalFramework(client, bodyId, bodyFormalFrameworkId, formalFrameworkId);
        addResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithBodyOutsideScope_Updating_Then_Returns_Forbidden()
    {
        var privilegedClient = await _apiFixture.CreateAlgemeenbeheerderClient();
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);
        var bodyId = await BodyPermissionMatrix.CreateBareBody(_apiFixture);

        var (formalFrameworkId, bodyFormalFrameworkId) = await CreateFormalFramework();
        await AddBodyFormalFramework(privilegedClient, bodyId, bodyFormalFrameworkId, formalFrameworkId);

        var updateResponse = await UpdateBodyFormalFramework(client, bodyId, bodyFormalFrameworkId, formalFrameworkId);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<(Guid FormalFrameworkId, Guid BodyFormalFrameworkId)> CreateFormalFramework()
    {
        var categoryId = await _apiFixture.Create.FormalFrameworkCategory();
        var formalFrameworkId = await _apiFixture.Create.FormalFramework(categoryId);
        return (formalFrameworkId, _apiFixture.Fixture.Create<Guid>());
    }

    private static async Task<HttpResponseMessage> AddBodyFormalFramework(
        HttpClient client,
        Guid bodyId,
        Guid bodyFormalFrameworkId,
        Guid formalFrameworkId)
        => await ApiFixture.Post(
            client,
            $"/v1/bodies/{bodyId}/formalframeworks",
            new AddBodyFormalFrameworkRequest
            {
                BodyFormalFrameworkId = bodyFormalFrameworkId,
                FormalFrameworkId = formalFrameworkId,
                ValidFrom = null,
                ValidTo = null,
            });

    private static async Task<HttpResponseMessage> UpdateBodyFormalFramework(
        HttpClient client,
        Guid bodyId,
        Guid bodyFormalFrameworkId,
        Guid formalFrameworkId)
        => await ApiFixture.Put(
            client,
            $"/v1/bodies/{bodyId}/formalframeworks/{bodyFormalFrameworkId}",
            new UpdateBodyFormalFrameworkRequest
            {
                BodyFormalFrameworkId = bodyFormalFrameworkId,
                FormalFrameworkId = formalFrameworkId,
                ValidFrom = null,
                ValidTo = null,
            });
}
