namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Keys.When_Updating_Keys;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Key;
using Xunit;

/// <summary>
/// Matrixrij <b>Sleutels</b> — UI-recht <c>canManageKeys</c> op de organisatieresponse.
/// Eén klasse per rij uit de rechtenmatrix (scherm/functionaliteit).
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_CanManageKeys
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_CanManageKeys(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Decentraalbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orgaanbeheerder)]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder)]
    public async Task Then_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        var organisationKeyId =  _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var keyTypeId = await _apiFixture.Create.KeyType();

        var response = await ApiFixture.Put(
            client,
            $"/v1/organisations/{organisationId}/keys/{organisationKeyId}",
            new UpdateOrganisationKeyRequest
            {
                OrganisationKeyId = organisationKeyId,
                KeyTypeId = keyTypeId,
                KeyValue = _apiFixture.Fixture.Create<string>(),
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
