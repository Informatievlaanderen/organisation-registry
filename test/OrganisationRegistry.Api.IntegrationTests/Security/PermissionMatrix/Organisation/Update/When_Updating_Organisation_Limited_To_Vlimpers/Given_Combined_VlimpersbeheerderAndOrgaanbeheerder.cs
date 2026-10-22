namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Update.When_Updating_Organisation_Limited_To_Vlimpers;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Body.Detail;
using OrganisationRegistry.Api.Backoffice.Organisation.Detail;
using OrganisationRegistry.Api.Backoffice.Vlimpers;
using Xunit;

/// <summary>
/// Verifies that a user holding both <c>vlimpersbeheerder</c> and
/// <c>orgaanbeheerder</c> roles simultaneously (e.g. the <c>vlimpersorgaan</c>
/// Keycloak test persona, see <see cref="ApiFixture.Backoffice.VlimpersbeheerderAndOrgaanbeheerder"/>)
/// keeps both permission sets: they can still edit the four Vlimpers-reserved
/// organisation-info fields on a Vlimpers-managed organisation (VlimpersBeheerder),
/// and can still register a new body (OrgaanBeheerder), regardless of which
/// organisation it is registered under. Roles are independent and additive
/// (<see cref="RolePermissionMap"/> unions all granted roles' permission sets),
/// so holding both should never reduce either role's individual rights.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Combined_VlimpersbeheerderAndOrgaanbeheerder
{
    private readonly ApiFixture _apiFixture;

    public Given_Combined_VlimpersbeheerderAndOrgaanbeheerder(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task CanStillEditVlimpersManagedOrganisation_Then_Returns_OK()
    {
        var organisationId = await CreateVlimpersManagedOrganisation();

        using var client = await _apiFixture.CreateDynamicClient(
            ApiFixture.Backoffice.VlimpersbeheerderAndOrgaanbeheerder);

        using var response = await UpdateOrganisation(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CanStillRegisterBody_Then_Returns_Created()
    {
        using var client = await _apiFixture.CreateDynamicClient(
            ApiFixture.Backoffice.VlimpersbeheerderAndOrgaanbeheerder);

        using var response = await RegisterBody(client);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    private async Task<Guid> CreateOrganisation()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        return organisationId;
    }

    private async Task<Guid> CreateVlimpersManagedOrganisation()
    {
        var organisationId = await CreateOrganisation();

        using var algemeenbeheerderClient = await _apiFixture.CreateAlgemeenbeheerderClient();
        using var response = await ApiFixture.Patch(
            algemeenbeheerderClient,
            $"/v1/organisations/{organisationId}/vlimpers",
            new VlimpersRequest { VlimpersManagement = true });
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        return organisationId;
    }

    private async Task<HttpResponseMessage> UpdateOrganisation(HttpClient client, Guid organisationId)
        => await ApiFixture.Put(
            client,
            $"/v1/organisations/{organisationId}/limitedtovlimpers",
            new UpdateOrganisationInfoLimitedToVlimpersRequest
            {
                Name = _apiFixture.Fixture.Create<string>(),
                ShortName = _apiFixture.Fixture.Create<string>(),
                Article = null,
                ValidFrom = null,
                ValidTo = null,
                OperationalValidFrom = null,
                OperationalValidTo = null,
            });

    private async Task<HttpResponseMessage> RegisterBody(HttpClient client)
    {
        await _apiFixture.Create.Body(_apiFixture.Fixture.Create<Guid>(), _apiFixture.Fixture.Create<string>());

        return await ApiFixture.Post(
            client,
            "/v1/bodies",
            new RegisterBodyRequest
            {
                Id = _apiFixture.Fixture.Create<Guid>(),
                Name = _apiFixture.Fixture.Create<string>(),
                OrganisationId = null,
            });
    }
}
