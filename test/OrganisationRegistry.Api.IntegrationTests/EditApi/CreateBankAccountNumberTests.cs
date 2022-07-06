namespace OrganisationRegistry.Api.IntegrationTests.EditApi;

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Edit.Organisation.BankAccount;
using FluentAssertions;
using Tests.Shared;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class CreateBankAccountNumberTests
{
    private const string TestOrganisationForCreatebankaccountnumbers = "test organisation for createBankAccountNumbers";
    private readonly ApiFixture _fixture;
    private readonly Guid _organisationId;

    public CreateBankAccountNumberTests(ApiFixture fixture)
    {
        _fixture = fixture;
        _organisationId = Guid.NewGuid();
    }

    [EnvVarIgnoreFact]
    public async Task WithoutBearer_ReturnsUnauthorized()
    {
        var organisationId = Guid.NewGuid();
        await _fixture.CreateOrganisation(_organisationId, TestOrganisationForCreatebankaccountnumbers);

        var response = await CreateBankAccountNumber(_fixture.HttpClient, organisationId, "BE86001197741650", "GEBABEBB");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [EnvVarIgnoreFact]
    public async Task AsCjmBeheerder_CanCreateAndUpdate()
    {
        await _fixture.CreateOrganisation(_organisationId, TestOrganisationForCreatebankaccountnumbers);

        var client = await _fixture.CreateMachine2MachineClientFor(ApiFixture.CJM.Client, ApiFixture.CJM.Scope);

        var response = await CreateBankAccountNumber(client, _organisationId, "BE86001197741650", "GEBABEBB");

        await ApiFixture.VerifyStatusCode(response, HttpStatusCode.Created);

        var organisationBankaccountId = GetFromLocation(response.Headers.Location!.ToString());

        var updateResponse = await UpdateBankAccountNumber(client, _organisationId, new Guid(organisationBankaccountId), "BG72UNCR70001522734456");

        await ApiFixture.VerifyStatusCode(updateResponse, HttpStatusCode.OK);
    }

    [EnvVarIgnoreFact]
    public async Task AsCjmBeheerder_CannotCreateWithInvalidBic()
    {
        await _fixture.CreateOrganisation(_organisationId, TestOrganisationForCreatebankaccountnumbers);

        var client = await _fixture.CreateMachine2MachineClientFor(ApiFixture.CJM.Client, ApiFixture.CJM.Scope);

        var response = await CreateBankAccountNumber(client, _organisationId, "BE86001197741650", "NOT_A_BIC");

        await ApiFixture.VerifyStatusCode(response, HttpStatusCode.BadRequest);
    }

    [EnvVarIgnoreFact]
    public async Task AsOrafinBeheerder_ReturnsForbidden()
    {
        await _fixture.CreateOrganisation(_organisationId, TestOrganisationForCreatebankaccountnumbers);

        var client = await _fixture.CreateMachine2MachineClientFor(ApiFixture.Orafin.Client, ApiFixture.Orafin.Scope);

        var response = await CreateBankAccountNumber(client, _organisationId, "BE86001197741650");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private static async Task<HttpResponseMessage> CreateBankAccountNumber(HttpClient httpClient, Guid organisationId, string bankAccountNumber, string bic = "", DateTime? validFrom = null, DateTime? validTo = null)
        => await ApiFixture.Post(
            httpClient,
            $"edit/organisations/{organisationId}/bankaccounts",
            new AddOrganisationBankAccountRequest()
            {
                BankAccountNumber = bankAccountNumber,
                Bic = bic,
                ValidFrom = validFrom,
                ValidTo = validTo,
            });

    private static async Task<HttpResponseMessage> UpdateBankAccountNumber(HttpClient httpClient, Guid organisationId, Guid organisationBankAccountId, string bankAccountNumber, string bic = "", DateTime? validFrom = null, DateTime? validTo = null)
        => await ApiFixture.Put(
            httpClient,
            $"edit/organisations/{organisationId}/bankaccounts/{organisationBankAccountId}",
            new AddOrganisationBankAccountRequest()
            {
                BankAccountNumber = bankAccountNumber,
                Bic = bic,
                ValidFrom = validFrom,
                ValidTo = validTo,
            });

    private static string GetFromLocation(string locationHeader)
        => locationHeader.Split('/').Last();
}
