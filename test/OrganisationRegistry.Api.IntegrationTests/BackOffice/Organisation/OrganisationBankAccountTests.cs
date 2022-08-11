namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Organisation;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class OrganisationBankAccountTests
{
    private readonly ApiFixture _apiFixture;
    private readonly OrganisationHelpers _helpers;

    public OrganisationBankAccountTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
        _helpers = new OrganisationHelpers(_apiFixture);
    }

    [Fact]
    public async Task TestOrganisationBankAccounts()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        var route = $"/v1/organisations/{organisationId}/bankaccounts";
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

        //REMOVE
        await _apiFixture.RemoveAndVerify(route, id);
    }

    private async Task<Guid> CreateAndVerify(string baseRoute)
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

        await _apiFixture.CreateAndVerify(baseRoute, body, VerifyResult);

        return creationId;
    }

    private async Task UpdateAndVerify(string baseRoute, Guid id)
    {
        var body = new
        {
            OrganisationBankAccountId = id,
            BankAccountNumber = _apiFixture.Fixture.Create<string>(),
            IsIban = false,
            Bic = _apiFixture.Fixture.Create<string>(),
            IsBic = false,
        };

        await _apiFixture.UpdateAndVerify(baseRoute, id, body, VerifyResult);
    }

    private static void VerifyResult(IReadOnlyDictionary<string, object> responseBody, dynamic body)
    {
        responseBody["bankAccountNumber"].Should().Be(body.BankAccountNumber);
        responseBody["isIban"].Should().Be(body.IsIban);
        responseBody["bic"].Should().Be(body.Bic);
        responseBody["isBic"].Should().Be(body.IsBic);
    }

    private async Task GetListAndVerify(string route)
    {
        await CreateAndVerify(route);
        await CreateAndVerify(route);

        await _apiFixture.GetListAndVerify(route);
    }
}
