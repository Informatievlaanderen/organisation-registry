namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.When_Creating_Child_Organisation;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Detail;
using OrganisationRegistry.Api.Backoffice.Vlimpers;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;

/// <summary>
/// Matrixrij <b>Organisatie</b> — dochterorganisatie registreren — voor de
/// <see cref="Role.VlimpersBeheerder" />. Een vlimpersbeheerder bezit
/// <see cref="Permission.CanManageChildren" /> enkel als restricted grant: hij mag
/// een dochterorganisatie enkel registreren onder een ouder die onder
/// Vlimpersbeheer valt.
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
    public async Task For_Vlimpersbeheerder_WithVlimpersManagedParent_Then_Returns_Created()
    {
        var parentOrganisationId = await CreateVlimpersManagedOrganisation();

        var client = await _apiFixture.CreateBackofficeUserClientFor(ApiFixture.Backoffice.Vlimpersbeheerder);

        var response = await CreateDaughterOrganisation(client, parentOrganisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Vlimpersbeheerder_WhenParentNotUnderVlimpersManagement_Then_Returns_Forbidden()
    {
        var parentOrganisationId = await CreateOrganisation();

        var client = await _apiFixture.CreateBackofficeUserClientFor(ApiFixture.Backoffice.Vlimpersbeheerder);

        var response = await CreateDaughterOrganisation(client, parentOrganisationId);

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

    private async Task<HttpResponseMessage> CreateDaughterOrganisation(HttpClient client, Guid parentOrganisationId)
        => await ApiFixture.Post(
            client,
            "/v1/organisations",
            new CreateOrganisationRequest
            {
                Id = _apiFixture.Fixture.Create<Guid>(),
                Name = _apiFixture.Fixture.Create<string>(),
                ParentOrganisationId = parentOrganisationId,
            });
}
