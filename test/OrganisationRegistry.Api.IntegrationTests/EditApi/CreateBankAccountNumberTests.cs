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

    [EnvVarIgnoreFact]
    public async Task WithoutBearer_ReturnsUnauthorized()
    {
        var organisationId = Guid.NewGuid();
        await _apiFixture.Create.Organisation(_organisationId, TestOrganisationForCreatebankaccountnumbers);

        var response = await CreateBankAccountNumber(_apiFixture.HttpClient, organisationId, "BE86001197741650", "GEBABEBB");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [EnvVarIgnoreTheory]
    [InlineData(ApiFixture.CJM.Client, ApiFixture.CJM.Scope)]
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

    [EnvVarIgnoreFact]
    public async Task AsCjmBeheerder_CannotCreateWithInvalidBic()
    {
        await _apiFixture.Create.Organisation(_organisationId, TestOrganisationForCreatebankaccountnumbers);

        var client = await _apiFixture.CreateMachine2MachineClientFor(ApiFixture.CJM.Client, ApiFixture.CJM.Scope);

        var response = await CreateBankAccountNumber(client, _organisationId, "BE86001197741650", "NOT_A_BIC");

        await ApiFixture.VerifyStatusCode(response, HttpStatusCode.BadRequest);
    }

    [EnvVarIgnoreFact]
    public async Task AsCjmBeheerder_CannotCreateWithInvalidFrom_InvalidTo()
    {
        await _apiFixture.Create.Organisation(_organisationId, TestOrganisationForCreatebankaccountnumbers);

        var response = await CreateBankAccountNumber(await _apiFixture.CreateCjmClient(), _organisationId, "BE86001197741650", "NOT_A_BIC", invalidValidFrom: "XX", invalidValidTo: "YY");

        await ApiFixture.VerifyStatusCode(response, HttpStatusCode.BadRequest);

        _testOutputHelper.WriteLine(await response.Content.ReadAsStringAsync());
    }

    [EnvVarIgnoreFact]
    public async Task AsCjmBeheerder_CannotCreateWithEmptyOrganisationId()
    {
        await _apiFixture.Create.Organisation(_organisationId, TestOrganisationForCreatebankaccountnumbers);

        var response = await CreateBankAccountNumber(await _apiFixture.CreateCjmClient(), Guid.Empty, "BE86001197741650", "NOT_A_BIC");

        await ApiFixture.VerifyStatusCode(response, HttpStatusCode.BadRequest);

        _testOutputHelper.WriteLine(await response.Content.ReadAsStringAsync());
    }

    [EnvVarIgnoreFact]
    public async Task AsCjmBeheerder_CannotCreateWithInvalidOrganisationId()
    {
        await _apiFixture.Create.Organisation(_organisationId, TestOrganisationForCreatebankaccountnumbers);

        var response = await CreateBankAccountNumber(await _apiFixture.CreateCjmClient(), Guid.Empty, "BE86001197741650", "NOT_A_BIC", invalidOrganisationId: "XX");

        await ApiFixture.VerifyStatusCode(response, HttpStatusCode.NotFound);

        _testOutputHelper.WriteLine(await response.Content.ReadAsStringAsync());
    }

    [EnvVarIgnoreFact]
    public async Task AsCjmBeheerder_CannotUpdateWithInvalidFrom_InvalidTo()
    {
        await _apiFixture.Create.Organisation(_organisationId, TestOrganisationForCreatebankaccountnumbers);

        var httpClient = await _apiFixture.CreateCjmClient();

        var response = await CreateBankAccountNumber(httpClient, _organisationId, "BE86001197741650", "GEBABEBB");
        var organisationBankaccountId = ApiFixture.GetIdFrom(response.Headers);

        var updateResponse = await UpdateBankAccountNumber(httpClient, _organisationId, organisationBankaccountId, "BG72UNCR70001522734456", invalidValidFrom: "XX", invalidValidTo: "YY");

        await ApiFixture.VerifyStatusCode(updateResponse, HttpStatusCode.BadRequest);

        _testOutputHelper.WriteLine(await updateResponse.Content.ReadAsStringAsync());
    }

    [EnvVarIgnoreFact]
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
