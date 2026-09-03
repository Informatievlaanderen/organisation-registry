namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Relations.When_Adding_Relations;

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
    public async Task For_Algemeenbeheerder_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        var response = await AddRelation(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOwnOrganisation_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var response = await AddRelation(client, _apiFixture.DecentraalbeheerderOrganisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithChildOrganisationInScope_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var response = await AddRelation(client, _apiFixture.DecentraalbeheerderChildOrganisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOrganisationOutsideScope_Then_Returns_Forbidden()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        var response = await AddRelation(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<HttpResponseMessage> AddRelation(HttpClient client, Guid organisationId)
    {
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var relationTypeId = await _apiFixture.Create.OrganisationRelationType();
        var relatedOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(relatedOrganisationId, _apiFixture.Fixture.Create<string>());

        return await ApiFixture.Post(
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
    }
}
