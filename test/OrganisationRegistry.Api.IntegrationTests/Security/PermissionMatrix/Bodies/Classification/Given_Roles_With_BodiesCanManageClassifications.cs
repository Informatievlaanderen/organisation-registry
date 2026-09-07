namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Bodies.Classification;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Body.BodyClassification;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_BodiesCanManageClassifications
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_BodiesCanManageClassifications(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();
        var bodyId = await BodyPermissionMatrix.CreateBareBody(_apiFixture);

        var response = await AddBodyClassification(client, bodyId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Orgaanbeheerder_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Orgaanbeheerder);
        var bodyId = await BodyPermissionMatrix.CreateBareBody(_apiFixture);

        var response = await AddBodyClassification(client, bodyId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOwnOrganisation_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);
        var bodyId = await BodyPermissionMatrix.CreateBodyForOrganisation(_apiFixture, _apiFixture.DecentraalbeheerderOrganisationId);

        var response = await BodyPermissionMatrix.WaitUntilAllowed(bodyId, () => AddBodyClassification(client, bodyId));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithChildOrganisationInScope_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);
        var bodyId = await BodyPermissionMatrix.CreateBodyForOrganisation(_apiFixture, _apiFixture.DecentraalbeheerderChildOrganisationId);

        var response = await BodyPermissionMatrix.WaitUntilAllowed(bodyId, () => AddBodyClassification(client, bodyId));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithBodyOutsideScope_Then_Returns_Forbidden()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);
        var bodyId = await BodyPermissionMatrix.CreateBareBody(_apiFixture);

        var response = await AddBodyClassification(client, bodyId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<HttpResponseMessage> AddBodyClassification(HttpClient client, Guid bodyId)
    {
        var classificationTypeId = await _apiFixture.Create.BodyClassificationType();
        var classificationId = await _apiFixture.Create.BodyClassification(classificationTypeId);

        return await ApiFixture.Post(
            client,
            $"/v1/bodies/{bodyId}/classifications",
            new AddBodyBodyClassificationRequest
            {
                BodyBodyClassificationId = _apiFixture.Fixture.Create<Guid>(),
                BodyClassificationTypeId = classificationTypeId,
                BodyClassificationId = classificationId,
                ValidFrom = null,
                ValidTo = null,
            });
    }
}
