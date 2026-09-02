namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Keys.When_Updating_Keys;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Key;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;

/// <summary>
/// Matrixrij <b>Sleutels</b> — UI-recht <c>canManageKeys</c> op de organisatieresponse.
/// Eén klasse per rij uit de rechtenmatrix (scherm/functionaliteit).
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_CanManageKeys
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_CanManageKeys(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    /// <summary>
    /// Positieve gate: een algemeenbeheerder — geauthenticeerd met een echt Keycloak-token
    /// uit de direct access grant, niet met de zelfgemunte JWT van de fixture — bezit
    /// <see cref="Permission.CanManageKeys" /> en mag dus een organisatiesleutel aanmaken.
    /// </summary>
    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        var organisationKeyId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var keyTypeId = await _apiFixture.Create.KeyType();

        await CreateKey(client, organisationId, organisationKeyId, keyTypeId);

        var response = await UpdateKey(client, organisationId, organisationKeyId, keyTypeId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<HttpResponseMessage> UpdateKey(HttpClient client, Guid organisationId, Guid organisationKeyId, Guid keyTypeId)
    {
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
        return response;
    }

    private async Task CreateKey(HttpClient client, Guid organisationId, Guid organisationKeyId, Guid keyTypeId)
    {
        await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/keys",
            new AddOrganisationKeyRequest
            {
                OrganisationKeyId = organisationKeyId,
                KeyTypeId = keyTypeId,
                KeyValue = _apiFixture.Fixture.Create<string>(),
                ValidFrom = null,
                ValidTo = null,
            });
    }
}
