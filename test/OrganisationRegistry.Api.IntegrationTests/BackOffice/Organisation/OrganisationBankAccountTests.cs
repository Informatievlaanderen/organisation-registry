namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Organisation;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class OrganisationBankAccountTests
{
    private readonly ApiFixture _apiFixture;

    public OrganisationBankAccountTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task TestOrganisationBankAccounts()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        var route = $"/v1/organisations/{organisationId}/bankaccounts";
        await _apiFixture.CreateOrganisation(organisationId, _apiFixture.Fixture.Create<string>());

        var getResponse = await ApiFixture.Get(_apiFixture.HttpClient, $"{route}/{_apiFixture.Fixture.Create<Guid>()}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        // CREATE
        await CreateWithInvalidDataAndVerifyBadRequest(route);

        var id = await CreateAndVerify(route);

        // UPDATE
        await UpdateAndVerify(route, id);

        await UpdateWithInvalidDataAndVerifyBadRequest(route, id);

        // LIST
        await GetListAndVerify(route);

        //REMOVE
        await RemoveAndVerify(route, id);
    }

    private async Task CreateWithInvalidDataAndVerifyBadRequest(string route)
    {
        var createResponse = await ApiFixture.Post(_apiFixture.HttpClient, $"{route}", "prut");
        await ApiFixture.VerifyStatusCode(createResponse, HttpStatusCode.BadRequest);
    }

    private async Task<Guid> CreateAndVerify(
        string baseRoute)
    {
        var creationId = _apiFixture.Fixture.Create<Guid>();
        var body = new
        {
            OrganisationBankAccountId = creationId,
            BankAccountNumber = _apiFixture.Fixture.Create<string>(),
            IsIban = false,
            Bic = _apiFixture.Fixture.Create<string>(),
            IsBic = false,
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
        responseBody["bankAccountNumber"].Should().Be(body.BankAccountNumber);
        responseBody["isIban"].Should().Be(body.IsIban);
        responseBody["bic"].Should().Be(body.Bic);
        responseBody["isBic"].Should().Be(body.IsBic);

        return creationId;
    }

    private async Task UpdateAndVerify(
        string baseRoute,
        Guid id
    )
    {
        var body = new
        {
            OrganisationBankAccountId = id,
            BankAccountNumber = _apiFixture.Fixture.Create<string>(),
            IsIban = false,
            Bic = _apiFixture.Fixture.Create<string>(),
            IsBic = false,
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
        responseBody["bankAccountNumber"].Should().Be(body.BankAccountNumber);
        responseBody["isIban"].Should().Be(body.IsIban);
        responseBody["bic"].Should().Be(body.Bic);
        responseBody["isBic"].Should().Be(body.IsBic);
    }

    private async Task UpdateWithInvalidDataAndVerifyBadRequest(string baseRoute, Guid id)
    {
        // update
        var updateResponse = await ApiFixture.Put(_apiFixture.HttpClient, $"{baseRoute}/{id}", "prut");
        await ApiFixture.VerifyStatusCode(updateResponse, HttpStatusCode.BadRequest);
    }

    private async Task GetListAndVerify(string route)
    {
        await CreateAndVerify(route);
        await CreateAndVerify(route);

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
