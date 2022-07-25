namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Organisation;

using System;
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

    public OrganisationFormalFrameworkTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task TestOrganisationFormalFrameworks()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        var route = $"/v1/organisations/{organisationId}/formalframeworks";
        await _apiFixture.CreateOrganisation(organisationId, _apiFixture.Fixture.Create<string>());
        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.CreateOrganisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());

        var getResponse = await ApiFixture.Get(_apiFixture.HttpClient, $"{route}/{_apiFixture.Fixture.Create<Guid>()}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        // CREATE
        await CreateWithInvalidDataAndVerifyBadRequest(route);

        var (id, formalFrameworkId) = await CreateAndVerify(route, parentOrganisationId);

        // UPDATE
        await UpdateAndVerify(route, id, formalFrameworkId, parentOrganisationId);

        await UpdateWithInvalidDataAndVerifyBadRequest(route, id);

        // LIST
        await GetListAndVerify(route, parentOrganisationId);

        //REMOVE
        await RemoveAndVerify(route, id);
    }

    private async Task CreateWithInvalidDataAndVerifyBadRequest(string route)
    {
        var createResponse = await ApiFixture.Post(_apiFixture.HttpClient, $"{route}", "prut");
        await ApiFixture.VerifyStatusCode(createResponse, HttpStatusCode.BadRequest);
    }

    private async Task<(Guid creationId, Guid formalFrameworkId)> CreateAndVerify(string baseRoute, Guid parentOrganisationId)
    {
        var formalFrameworkCategoryId = await _apiFixture.CreateFormalFrameworkCategory();
        var formalFrameworkId = await _apiFixture.CreateFormalFramework(formalFrameworkCategoryId);

        var creationId = _apiFixture.Fixture.Create<Guid>();
        var body = new AddOrganisationFormalFrameworkRequest
        {
            OrganisationFormalFrameworkId = creationId,
            FormalFrameworkId = formalFrameworkId,
            ParentOrganisationId = parentOrganisationId,
            ValidFrom = null,
            ValidTo = null,
        };
        var createResponse = await ApiFixture.Post(
            _apiFixture.HttpClient,
            baseRoute,
            body);
        await ApiFixture.VerifyStatusCode(createResponse, HttpStatusCode.Created);

        // get
        var getResponse = await ApiFixture.Get(_apiFixture.HttpClient, createResponse.Headers.Location!.ToString());
        await ApiFixture.VerifyStatusCode(getResponse, HttpStatusCode.OK);

        var responseBody = await ApiFixture.Deserialize(getResponse);
        responseBody["formalFrameworkId"].Should().Be(body.FormalFrameworkId.ToString());
        responseBody["parentOrganisationId"].Should().Be(body.ParentOrganisationId.ToString());

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

        // update
        var updateResponse = await ApiFixture.Put(
            _apiFixture.HttpClient,
            $"{baseRoute}/{id}",
            body);
        await ApiFixture.VerifyStatusCode(updateResponse, HttpStatusCode.OK);

        // get
        var getResponse = await ApiFixture.Get(_apiFixture.HttpClient, $"{baseRoute}/{id}");
        await ApiFixture.VerifyStatusCode(getResponse, HttpStatusCode.OK);

        var responseBody = await ApiFixture.Deserialize(getResponse);
        responseBody["formalFrameworkId"].Should().Be(body.FormalFrameworkId.ToString());
        responseBody["parentOrganisationId"].Should().Be(body.ParentOrganisationId.ToString());
        responseBody["validFrom"].Should().Be(body.ValidFrom.Value.Date);
        responseBody["validTo"].Should().Be(body.ValidTo.Value.Date);
    }

    private async Task UpdateWithInvalidDataAndVerifyBadRequest(string baseRoute, Guid id)
    {
        // update
        var updateResponse = await ApiFixture.Put(_apiFixture.HttpClient, $"{baseRoute}/{id}", "prut");
        await ApiFixture.VerifyStatusCode(updateResponse, HttpStatusCode.BadRequest);
    }

    private async Task GetListAndVerify(string route, Guid parentOrganisationId)
    {
        await CreateAndVerify(route, parentOrganisationId);
        await CreateAndVerify(route, parentOrganisationId);

        var getResponse = await ApiFixture.Get(_apiFixture.HttpClient, $"{route}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var deserializedResponse = await ApiFixture.DeserializeAsList(getResponse);
        deserializedResponse.Should().NotBeNull();

        deserializedResponse.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    private async Task RemoveAndVerify(string baseRoute, Guid id)
    {
        // remove
        var deleteResponse = await ApiFixture.Delete(_apiFixture.HttpClient, $"{baseRoute}/{id}");
        await ApiFixture.VerifyStatusCode(deleteResponse, HttpStatusCode.NoContent);

        // get
        var getResponse = await ApiFixture.Get(_apiFixture.HttpClient, $"{baseRoute}/{id}");
        await ApiFixture.VerifyStatusCode(getResponse, HttpStatusCode.NotFound);
    }
}
