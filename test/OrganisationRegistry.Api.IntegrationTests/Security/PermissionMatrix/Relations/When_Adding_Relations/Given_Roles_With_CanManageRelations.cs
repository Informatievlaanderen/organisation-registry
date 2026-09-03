namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Relations.When_Adding_Relations;

using System;
using System.Net;
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
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var relationTypeId = await _apiFixture.Create.OrganisationRelationType();
        var relatedOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(relatedOrganisationId, _apiFixture.Fixture.Create<string>());

        var response = await ApiFixture.Post(
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

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact(Skip = "TODO: scoped role 'Decentraalbeheerder' is allowed by the permission matrix but the domain authorization policy requires the organisation (or entity) to be within the role's own scope (BeheerderForOrganisation / configured owned-ids). No fixture precedent exists for creating an organisation inside a scoped role's Keycloak OVO scope, so this positive cannot yet assert a 2xx. Enable once scoped-org test setup is available.")]
    public async Task For_Decentraalbeheerder_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var relationTypeId = await _apiFixture.Create.OrganisationRelationType();
        var relatedOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(relatedOrganisationId, _apiFixture.Fixture.Create<string>());

        var response = await ApiFixture.Post(
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

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
