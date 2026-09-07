namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Keys.When_Updating_Keys;

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Key;
using OrganisationRegistry.Api.Backoffice.Vlimpers;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;

/// <summary>
/// Matrixrij <b>Sleutels</b> — resource-restricties voor de
/// <see cref="Role.VlimpersBeheerder" />. Een vlimpersbeheerder bezit
/// <see cref="Permission.CanManageKeys" /> enkel als restricted grant: hij mag
/// uitsluitend <em>eigen</em> sleutels beheren, d.w.z. een Vlimpers-sleuteltype op
/// een organisatie die onder Vlimpersbeheer valt. Alle andere combinaties leveren
/// 403 op.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Vlimpersbeheerder
{
    private readonly ApiFixture _apiFixture;

    public Given_Vlimpersbeheerder(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    /// <summary>
    /// Positief: Vlimpers-sleuteltype op een organisatie onder Vlimpersbeheer mag
    /// door de vlimpersbeheerder aangepast worden.
    /// </summary>
    [Fact]
    public async Task For_Vlimpersbeheerder_WithVlimpersKeyOnVlimpersManagedOrganisation_Then_Returns_OK()
    {
        var organisationId = await CreateVlimpersManagedOrganisation();
        var vlimpersKeyTypeId = await CreateVlimpersKeyType();
        var organisationKeyId = await CreateKeyAsAlgemeenbeheerder(organisationId, vlimpersKeyTypeId);

        var client = await _apiFixture.CreateBackofficeUserClientFor(ApiFixture.Backoffice.Vlimpersbeheerder);

        var response = await UpdateKey(client, organisationId, organisationKeyId, vlimpersKeyTypeId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    /// <summary>
    /// Negatief: een niet-Vlimpers-sleuteltype mag de vlimpersbeheerder ook op een
    /// Vlimpers-beheerde organisatie niet aanpassen.
    /// </summary>
    [Fact]
    public async Task For_Vlimpersbeheerder_WithNonVlimpersKey_Then_Returns_Forbidden()
    {
        var organisationId = await CreateVlimpersManagedOrganisation();
        var otherKeyTypeId = await _apiFixture.Create.KeyType();
        var organisationKeyId = await CreateKeyAsAlgemeenbeheerder(organisationId, otherKeyTypeId);

        var client = await _apiFixture.CreateBackofficeUserClientFor(ApiFixture.Backoffice.Vlimpersbeheerder);

        var response = await UpdateKey(client, organisationId, organisationKeyId, otherKeyTypeId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Negatief: zelfs een Vlimpers-sleuteltype mag niet aangepast worden wanneer de
    /// organisatie niet onder Vlimpersbeheer valt.
    /// </summary>
    [Fact]
    public async Task For_Vlimpersbeheerder_WhenOrganisationNotUnderVlimpersManagement_Then_Returns_Forbidden()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var vlimpersKeyTypeId = await CreateVlimpersKeyType();
        var organisationKeyId = await CreateKeyAsAlgemeenbeheerder(organisationId, vlimpersKeyTypeId);

        var client = await _apiFixture.CreateBackofficeUserClientFor(ApiFixture.Backoffice.Vlimpersbeheerder);

        var response = await UpdateKey(client, organisationId, organisationKeyId, vlimpersKeyTypeId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<Guid> CreateVlimpersManagedOrganisation()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        var algemeenbeheerderClient = await _apiFixture.CreateAlgemeenbeheerderClient();
        var response = await ApiFixture.Patch(
            algemeenbeheerderClient,
            $"/v1/organisations/{organisationId}/vlimpers",
            new VlimpersRequest { VlimpersManagement = true });
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        return organisationId;
    }

    private async Task<Guid> CreateVlimpersKeyType()
        => await _apiFixture.Create.KeyType(
            _apiFixture.Configuration.Authorization.KeyIdsAllowedForVlimpers.First());

    private async Task<Guid> CreateKeyAsAlgemeenbeheerder(Guid organisationId, Guid keyTypeId)
    {
        var organisationKeyId = _apiFixture.Fixture.Create<Guid>();
        var algemeenbeheerderClient = await _apiFixture.CreateAlgemeenbeheerderClient();

        var response = await ApiFixture.Post(
            algemeenbeheerderClient,
            $"/v1/organisations/{organisationId}/keys",
            new AddOrganisationKeyRequest
            {
                OrganisationKeyId = organisationKeyId,
                KeyTypeId = keyTypeId,
                KeyValue = _apiFixture.Fixture.Create<string>(),
                ValidFrom = null,
                ValidTo = null,
            });
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        return organisationKeyId;
    }

    private async Task<HttpResponseMessage> UpdateKey(HttpClient client, Guid organisationId, Guid organisationKeyId, Guid keyTypeId)
        => await ApiFixture.Put(
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
}
