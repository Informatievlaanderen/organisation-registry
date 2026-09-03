namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Relations.When_Updating_Relations;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Relation;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_CanManageRelations
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_CanManageRelations(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_OK()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = await AddRelation(client, organisationId);

        var response = await UpdateRelation(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOwnOrganisation_Then_Returns_OK()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.DecentraalbeheerderOrganisationId;
        var entityId = await AddRelation(client, organisationId);

        var response = await UpdateRelation(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithChildOrganisationInScope_Then_Returns_OK()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.DecentraalbeheerderChildOrganisationId;
        var entityId = await AddRelation(client, organisationId);

        var response = await UpdateRelation(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOrganisationOutsideScope_Then_Returns_Forbidden()
    {
        var privilegedClient = await _apiFixture.CreateAlgemeenbeheerderClient();
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = await AddRelation(privilegedClient, organisationId);

        var response = await UpdateRelation(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<Guid> AddRelation(HttpClient client, Guid organisationId)
    {
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var relationTypeId = await _apiFixture.Create.OrganisationRelationType();
        var relatedOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(relatedOrganisationId, _apiFixture.Fixture.Create<string>());

        await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/relations",
            new AddOrganisationRelationRequest()
            {
                OrganisationRelationId = entityId,
                RelationId = relationTypeId,
                RelatedOrganisationId = relatedOrganisationId,
                ValidFrom = null,
                ValidTo = null,
            });

        return entityId;
    }


    private async Task<HttpResponseMessage> UpdateRelation(HttpClient client, Guid organisationId, Guid entityId)
    {
        var relationTypeId = await _apiFixture.Create.OrganisationRelationType();
        var relatedOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(relatedOrganisationId, _apiFixture.Fixture.Create<string>());

        return await ApiFixture.Put(
            client,
            $"/v1/organisations/{organisationId}/relations/{entityId}",
            new UpdateOrganisationRelationRequest()
            {
                OrganisationRelationId = entityId,
                RelationId = relationTypeId,
                RelatedOrganisationId = relatedOrganisationId,
                ValidFrom = null,
                ValidTo = null,
            });
    }
}
