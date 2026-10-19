namespace OrganisationRegistry.Api.IntegrationTests.EditApi;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

[Collection(ApiTestsCollection.Name)]
public class CreateBankAccountNumberTests
{
    private const string TestOrganisationForCreatebankaccountnumbers = "test organisation for createBankAccountNumbers";
    private readonly ApiFixture _apiFixture;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Guid _organisationId;

    public CreateBankAccountNumberTests(ApiFixture apiFixture, ITestOutputHelper testOutputHelper)
    {
        _apiFixture = apiFixture;
        _testOutputHelper = testOutputHelper;
        _organisationId = Guid.NewGuid();
    }

    [SkipBankAccounts]
    public async Task WithoutBearer_ReturnsUnauthorized()
    {
        var organisationId = Guid.NewGuid();
        await _apiFixture.Create.Organisation(_organisationId, TestOrganisationForCreatebankaccountnumbers);

        var response = await CreateBankAccountNumber(_apiFixture.HttpClient, organisationId, "BE86001197741650", "GEBABEBB");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory(Skip = "Skip Bankaccounts")]
    [InlineData(ApiFixture.Test.Client, ApiFixture.Test.Scope)]
    public async Task CanCreateAndUpdateAs(string client, string scope)
    {
        await _apiFixture.Create.Organisation(_organisationId, TestOrganisationForCreatebankaccountnumbers);

        var httpClient = await _apiFixture.CreateMachine2MachineClientFor(client, scope);

        var response = await CreateBankAccountNumber(httpClient, _organisationId, "BE86001197741650", "GEBABEBB");

        await ApiFixture.VerifyStatusCode(response, HttpStatusCode.Created);

        var organisationBankaccountId = ApiFixture.GetIdFrom(response.Headers);

        var updateResponse = await UpdateBankAccountNumber(httpClient, _organisationId, organisationBankaccountId, "BG72UNCR70001522734456");

        await ApiFixture.VerifyStatusCode(updateResponse, HttpStatusCode.OK);
    }






    [SkipBankAccounts]
    public async Task AsOrafinBeheerder_ReturnsForbidden()
    {
        await _apiFixture.Create.Organisation(_organisationId, TestOrganisationForCreatebankaccountnumbers);

        var client = await _apiFixture.CreateMachine2MachineClientFor(ApiFixture.Orafin.Client, ApiFixture.Orafin.Scope);

        var response = await CreateBankAccountNumber(client, _organisationId, "BE86001197741650");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private static async Task<HttpResponseMessage> CreateBankAccountNumber(HttpClient httpClient, Guid organisationId, string bankAccountNumber, string bic = "", DateTime? validFrom = null, DateTime? validTo = null, string? invalidValidFrom = null, string? invalidValidTo = null, string? invalidOrganisationId = null)
        => await ApiFixture.Post(
            httpClient,
            $"/v1/edit/organisations/{invalidOrganisationId ?? organisationId.ToString()}/bankaccounts",
            new
            {
                BankAccountNumber = bankAccountNumber,
                Bic = bic,
                ValidFrom = invalidValidFrom ?? validFrom?.ToString("O"),
                ValidTo = invalidValidTo ?? validTo?.ToString("O"),
            });

    private static async Task<HttpResponseMessage> UpdateBankAccountNumber(HttpClient httpClient, Guid organisationId, Guid organisationBankAccountId, string bankAccountNumber, string bic = "", DateTime? validFrom = null, DateTime? validTo = null, string? invalidValidFrom = null, string? invalidValidTo = null, string? invalidOrganisationId = null)
        => await ApiFixture.Put(
            httpClient,
            $"/v1/edit/organisations/{invalidOrganisationId ?? organisationId.ToString()}/bankaccounts/{organisationBankAccountId}",
            new
            {
                BankAccountNumber = bankAccountNumber,
                Bic = bic,
                ValidFrom = invalidValidFrom ?? validFrom?.ToString("O"),
                ValidTo = invalidValidTo ?? validTo?.ToString("O"),
            });
}
