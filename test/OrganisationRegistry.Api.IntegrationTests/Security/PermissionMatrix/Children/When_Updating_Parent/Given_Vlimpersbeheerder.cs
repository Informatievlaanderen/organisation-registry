namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Children.When_Updating_Parent;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Parent;
using OrganisationRegistry.Api.Backoffice.Vlimpers;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;

/// <summary>
/// Matrixrij <b>Onderliggende organisaties (kinderen)</b> voor de
/// <see cref="Role.VlimpersBeheerder" /> bij het aanpassen van een bovenliggende
/// organisatie. De vlimpersbeheerder mag dit enkel voor organisaties onder
/// Vlimpersbeheer.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Vlimpersbeheerder
{
    private readonly ApiFixture _apiFixture;

    public Given_Vlimpersbeheerder(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Vlimpersbeheerder_WithVlimpersManagedOrganisation_Then_Returns_Ok()
    {
        var childOrganisationId = await CreateVlimpersManagedOrganisation();
        var parentOrganisationId = await CreateVlimpersManagedOrganisation();
        var organisationParentId = await AddParentAsAlgemeenbeheerder(childOrganisationId, parentOrganisationId);

        var client = await _apiFixture.CreateBackofficeUserClientFor(ApiFixture.Backoffice.Vlimpersbeheerder);

        var response = await UpdateParent(client, childOrganisationId, organisationParentId, parentOrganisationId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Vlimpersbeheerder_WhenOrganisationNotUnderVlimpersManagement_Then_Returns_Forbidden()
    {
        var childOrganisationId = await CreateOrganisation();
        var parentOrganisationId = await CreateOrganisation();
        var organisationParentId = await AddParentAsAlgemeenbeheerder(childOrganisationId, parentOrganisationId);

        var client = await _apiFixture.CreateBackofficeUserClientFor(ApiFixture.Backoffice.Vlimpersbeheerder);

        var response = await UpdateParent(client, childOrganisationId, organisationParentId, parentOrganisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
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

        var algemeenbeheerderClient = await _apiFixture.CreateAlgemeenbeheerderClient();
        var response = await ApiFixture.Patch(
            algemeenbeheerderClient,
            $"/v1/organisations/{organisationId}/vlimpers",
            new VlimpersRequest { VlimpersManagement = true });
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        return organisationId;
    }

    private async Task<Guid> AddParentAsAlgemeenbeheerder(Guid childOrganisationId, Guid parentOrganisationId)
    {
        var organisationParentId = _apiFixture.Fixture.Create<Guid>();
        var algemeenbeheerderClient = await _apiFixture.CreateAlgemeenbeheerderClient();

        var response = await ApiFixture.Post(
            algemeenbeheerderClient,
            $"/v1/organisations/{childOrganisationId}/parents",
            new AddOrganisationParentRequest
            {
                OrganisationOrganisationParentId = organisationParentId,
                ParentOrganisationId = parentOrganisationId,
                ValidFrom = null,
                ValidTo = null,
            });
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        return organisationParentId;
    }

    private static async Task<HttpResponseMessage> UpdateParent(
        HttpClient client,
        Guid childOrganisationId,
        Guid organisationParentId,
        Guid parentOrganisationId)
        => await ApiFixture.Put(
            client,
            $"/v1/organisations/{childOrganisationId}/parents/{organisationParentId}",
            new UpdateOrganisationParentRequest
            {
                OrganisationOrganisationParentId = organisationParentId,
                ParentOrganisationId = parentOrganisationId,
                ValidFrom = null,
                ValidTo = null,
            });
}
