namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Bodies.LifecyclePhase;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Body.LifecyclePhase;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_BodiesCanManageLifecycles
{
    // A body auto-creates an active lifecycle phase covering [BodyValidFrom, +inf). By registering
    // the body with a fixed start we leave a gap before it, so an extra lifecycle phase ending
    // before that start does not overlap and can be added.
    private static readonly DateTime BodyValidFrom = new(2020, 1, 1);
    private static readonly DateTime PhaseValidTo = new(2019, 12, 31);

    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_BodiesCanManageLifecycles(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();
        var bodyId = await BodyPermissionMatrix.CreateBareBody(_apiFixture, BodyValidFrom);

        var response = await AddLifecyclePhase(client, bodyId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Orgaanbeheerder_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Orgaanbeheerder);
        var bodyId = await BodyPermissionMatrix.CreateBareBody(_apiFixture, BodyValidFrom);

        var response = await AddLifecyclePhase(client, bodyId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOwnOrganisation_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);
        var bodyId = await BodyPermissionMatrix.CreateBodyForOrganisation(_apiFixture, _apiFixture.DecentraalbeheerderOrganisationId, BodyValidFrom);

        var response = await BodyPermissionMatrix.WaitUntilAllowed(bodyId, () => AddLifecyclePhase(client, bodyId));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithChildOrganisationInScope_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);
        var bodyId = await BodyPermissionMatrix.CreateBodyForOrganisation(_apiFixture, _apiFixture.DecentraalbeheerderChildOrganisationId, BodyValidFrom);

        var response = await BodyPermissionMatrix.WaitUntilAllowed(bodyId, () => AddLifecyclePhase(client, bodyId));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithBodyOutsideScope_Then_Returns_Forbidden()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);
        var bodyId = await BodyPermissionMatrix.CreateBareBody(_apiFixture, BodyValidFrom);

        var response = await AddLifecyclePhase(client, bodyId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<HttpResponseMessage> AddLifecyclePhase(HttpClient client, Guid bodyId)
    {
        var lifecyclePhaseTypeId = await _apiFixture.Create.LifecyclePhaseType();

        return await ApiFixture.Post(
            client,
            $"/v1/bodies/{bodyId}/lifecyclephases",
            new AddBodyLifecyclePhaseRequest
            {
                BodyLifecyclePhaseId = _apiFixture.Fixture.Create<Guid>(),
                LifecyclePhaseTypeId = lifecyclePhaseTypeId,
                ValidFrom = null,
                ValidTo = PhaseValidTo,
            });
    }
}
