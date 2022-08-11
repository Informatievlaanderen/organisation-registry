namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Organisation;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using Backoffice.Organisation.FormalFramework;
using FluentAssertions;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class OrganisationFormalFrameworkTests
{
    private readonly ApiFixture _apiFixture;
    private readonly OrganisationHelpers _helpers;

    public OrganisationFormalFrameworkTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
        _helpers = new OrganisationHelpers(_apiFixture);
    }

    [Fact]
    public async Task TestOrganisationFormalFrameworks()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        var route = $"/v1/organisations/{organisationId}/formalframeworks";
        await _helpers.CreateOrganisation(organisationId, _apiFixture.Fixture.Create<string>());
        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _helpers.CreateOrganisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());

        var getResponse = await ApiFixture.Get(_apiFixture.HttpClient, $"{route}/{_apiFixture.Fixture.Create<Guid>()}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        // CREATE
        await _apiFixture.CreateWithInvalidDataAndVerifyBadRequest(route);

        var (id, formalFrameworkId) = await CreateAndVerify(route, parentOrganisationId);

        // UPDATE
        await UpdateAndVerify(route, id, formalFrameworkId, parentOrganisationId);

        await _apiFixture.UpdateWithInvalidDataAndVerifyBadRequest(route, id);

        // LIST
        await GetListAndVerify(route, parentOrganisationId);

        //REMOVE
        await _apiFixture.RemoveAndVerify(route, id);
    }

    private async Task<(Guid creationId, Guid formalFrameworkId)> CreateAndVerify(string baseRoute, Guid parentOrganisationId)
    {
        var formalFrameworkCategoryId = await _helpers.CreateFormalFrameworkCategory();
        var formalFrameworkId = await _helpers.CreateFormalFramework(formalFrameworkCategoryId);

        var creationId = _apiFixture.Fixture.Create<Guid>();
        var body = new AddOrganisationFormalFrameworkRequest
        {
            OrganisationFormalFrameworkId = creationId,
            FormalFrameworkId = formalFrameworkId,
            ParentOrganisationId = parentOrganisationId,
            ValidFrom = null,
            ValidTo = null,
        };

        await _apiFixture.CreateAndVerify(baseRoute, body, VerifyResult);

        return (creationId, formalFrameworkId);
    }

    private async Task UpdateAndVerify(
        string baseRoute,
        Guid id,
        Guid formalFrameworkId,
        Guid parentOrganisationId)
    {
        var today = _apiFixture.Fixture.Create<DateTime>();
        var body = new UpdateOrganisationFormalFrameworkRequest
        {
            OrganisationFormalFrameworkId = id,
            FormalFrameworkId = formalFrameworkId,
            ParentOrganisationId = parentOrganisationId,
            ValidFrom = today.AddDays(-10),
            ValidTo = today.AddDays(10),
        };

        await _apiFixture.UpdateAndVerify(baseRoute, id, body, VerifyResult);
    }

    private static void VerifyResult(IReadOnlyDictionary<string, object> responseBody, dynamic body)
    {
        responseBody["formalFrameworkId"].Should().Be(body.FormalFrameworkId.ToString());
        responseBody["parentOrganisationId"].Should().Be(body.ParentOrganisationId.ToString());
        responseBody["validFrom"].Should().Be(body.ValidFrom?.Date);
        responseBody["validTo"].Should().Be(body.ValidTo?.Date);
    }

    private async Task GetListAndVerify(string route, Guid parentOrganisationId)
    {
        await CreateAndVerify(route, parentOrganisationId);
        await CreateAndVerify(route, parentOrganisationId);

        await _apiFixture.GetListAndVerify(route);
    }
}
