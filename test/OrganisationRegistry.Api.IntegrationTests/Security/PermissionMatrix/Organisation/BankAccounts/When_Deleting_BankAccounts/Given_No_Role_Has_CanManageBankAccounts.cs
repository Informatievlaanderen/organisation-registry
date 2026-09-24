namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.BankAccounts.When_Deleting_BankAccounts;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Xunit;

/// <summary>
/// Matrixrij <b>Bankrekeningen</b> — gedreven door <see cref="Infrastructure.Authorization.Permission.CanManageBankAccounts" />.
/// Zie <see cref="When_Adding_BankAccounts.Given_No_Role_Has_CanManageBankAccounts" /> voor de
/// rationale: de permissie is bewust aan geen enkele rol toegekend, dus elke rol krijgt 403.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_No_Role_Has_CanManageBankAccounts
{
    private readonly ApiFixture _apiFixture;

    public Given_No_Role_Has_CanManageBankAccounts(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Algemeenbeheerder)]
    [InlineData(ApiFixture.Backoffice.Vlimpersbeheerder)]
    [InlineData(ApiFixture.Backoffice.Decentraalbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orgaanbeheerder)]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder)]
    [InlineData(ApiFixture.Backoffice.Cjmbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orafinbeheerder)]
    public async Task Then_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        var response = await ApiFixture.Delete(
            client,
            $"/v1/organisations/{organisationId}/bankAccounts/{_apiFixture.Fixture.Create<Guid>()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
