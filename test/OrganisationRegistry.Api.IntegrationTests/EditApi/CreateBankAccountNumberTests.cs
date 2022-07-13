﻿namespace OrganisationRegistry.Api.IntegrationTests.EditApi;

using System;
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

    [EnvVarIgnoreTheory]
    [InlineData(ApiFixture.CJM.Client, ApiFixture.CJM.Scope)]
    [InlineData(ApiFixture.Test.Client, ApiFixture.Test.Scope)]
    public async Task CanCreateAndUpdateAs(string client, string scope)
    {
        await _fixture.CreateOrganisation(_organisationId, TestOrganisationForCreatebankaccountnumbers);

        var httpClient = await _fixture.CreateMachine2MachineClientFor(client, scope);

        var response = await CreateBankAccountNumber(httpClient, _organisationId, "BE86001197741650", "GEBABEBB");

        await ApiFixture.VerifyStatusCode(response, HttpStatusCode.Created);

        var organisationBankaccountId = ApiFixture.GetIdFrom(response.Headers);

        var updateResponse = await UpdateBankAccountNumber(httpClient, _organisationId, organisationBankaccountId, "BG72UNCR70001522734456");

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
            $"/v1/edit/organisations/{organisationId}/bankaccounts",
            new AddOrganisationBankAccountRequest
            {
                BankAccountNumber = bankAccountNumber,
                Bic = bic,
                ValidFrom = validFrom,
                ValidTo = validTo,
            });

    private static async Task<HttpResponseMessage> UpdateBankAccountNumber(HttpClient httpClient, Guid organisationId, Guid organisationBankAccountId, string bankAccountNumber, string bic = "", DateTime? validFrom = null, DateTime? validTo = null)
        => await ApiFixture.Put(
            httpClient,
            $"/v1/edit/organisations/{organisationId}/bankaccounts/{organisationBankAccountId}",
            new UpdateOrganisationBankAccountRequest
            {
                BankAccountNumber = bankAccountNumber,
                Bic = bic,
                ValidFrom = validFrom,
                ValidTo = validTo,
            });
}
