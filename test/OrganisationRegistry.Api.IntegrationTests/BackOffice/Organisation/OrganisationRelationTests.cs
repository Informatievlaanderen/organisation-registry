namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Organisation;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using Backoffice.Organisation.Relation;
using FluentAssertions;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class OrganisationRelationTests
{
    private readonly ApiFixture _apiFixture;
    private readonly OrganisationHelpers _helpers;

    public OrganisationRelationTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
        _helpers = new OrganisationHelpers(_apiFixture);
    }

    [Fact]
    public async Task TestOrganisationRelations()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        var route = $"/v1/organisations/{organisationId}/relations";
        await _helpers.CreateOrganisation(organisationId, _apiFixture.Fixture.Create<string>());

        var getResponse = await ApiFixture.Get(_apiFixture.HttpClient, $"{route}/{_apiFixture.Fixture.Create<Guid>()}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        // CREATE
        await _apiFixture.CreateWithInvalidDataAndVerifyBadRequest(route);

        var id = await CreateAndVerify(route);

        // UPDATE
        await UpdateAndVerify(route, id);

        await _apiFixture.UpdateWithInvalidDataAndVerifyBadRequest(route, id);

        // LIST
        await GetListAndVerify(route);
    }

    private async Task<Guid> CreateAndVerify(string baseRoute)
    {
        var relationTypeId = await _helpers.CreateOrganisationRelationType();

        var relatedOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _helpers.CreateOrganisation(relatedOrganisationId, _apiFixture.Fixture.Create<string>());

        var today = _apiFixture.Fixture.Create<DateTime>();

        var id = _apiFixture.Fixture.Create<Guid>();
        var body = new AddOrganisationRelationRequest
        {
            OrganisationRelationId = id,
            RelatedOrganisationId = relatedOrganisationId,
            RelationId = relationTypeId,
            ValidFrom = today.AddDays(-10),
            ValidTo = today.AddDays(10),
        };

        await _apiFixture.CreateAndVerify(baseRoute, body, VerifyResult);

        return id;
    }

    private async Task UpdateAndVerify(string baseRoute, Guid id)
    {
        var relationTypeId = await _helpers.CreateOrganisationRelationType();

        var relatedOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _helpers.CreateOrganisation(relatedOrganisationId, _apiFixture.Fixture.Create<string>());

        var today = _apiFixture.Fixture.Create<DateTime>();

        var body = new UpdateOrganisationRelationRequest
        {
            OrganisationRelationId = id,
            RelatedOrganisationId = relatedOrganisationId,
            RelationId = relationTypeId,
            ValidFrom = today.AddDays(-10),
            ValidTo = today.AddDays(10),
        };

        await _apiFixture.UpdateAndVerify(baseRoute, id, body, VerifyResult);
    }

    private static void VerifyResult(IReadOnlyDictionary<string, object> responseBody, dynamic body)
    {
        responseBody["relationId"].Should().Be(body.RelationId.ToString());
        responseBody["relatedOrganisationId"].Should().Be(body.RelatedOrganisationId.ToString());
        responseBody["validFrom"].Should().Be(body.ValidFrom?.Date);
        responseBody["validTo"].Should().Be(body.ValidTo?.Date);
    }

    private async Task GetListAndVerify(string route)
    {
        await CreateAndVerify(route);
        await CreateAndVerify(route);

        await _apiFixture.GetListAndVerify(route);
    }
}
