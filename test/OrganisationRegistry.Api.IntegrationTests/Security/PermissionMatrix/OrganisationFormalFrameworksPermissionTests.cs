namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix;

using System;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Backoffice.Organisation.FormalFramework;
using FluentAssertions;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;

/// <summary>
/// Matrixrij <b>Toepassingsgebieden</b> — UI-recht <c>canManageFormalFrameworks</c>
/// op de organisatieresponse.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class OrganisationFormalFrameworksPermissionTests
{
    private readonly ApiFixture _apiFixture;

    public OrganisationFormalFrameworksPermissionTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    /// <summary>
    /// Positieve gate: een algemeenbeheerder bezit
    /// <see cref="Permission.CanManageFormalFrameworks" /> en mag dus een
    /// toepassingsgebied aan een organisatie toewijzen.
    /// </summary>
    [Fact]
    public async Task CanManageFormalFrameworks_Gates_OrganisationFormalFrameworkCommandController()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());
        var formalFrameworkCategoryId = await _apiFixture.Create.FormalFrameworkCategory();
        var formalFrameworkId = await _apiFixture.Create.FormalFramework(formalFrameworkCategoryId);

        var response = await PostAddFormalFramework(
            client,
            organisationId,
            parentOrganisationId,
            formalFrameworkId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    /// <summary>
    /// Negatieve gate: een orgaanbeheerder mist
    /// <see cref="Permission.CanManageFormalFrameworks" />, dus weigert het
    /// autorisatiefilter de request met 403 nog voor de domeinpolicy draait.
    /// </summary>
    [Fact]
    public async Task WithoutCanManageFormalFrameworks_OrganisationFormalFrameworkCommandController_Returns403()
    {
        var client = await _apiFixture.CreateBackofficeUserClientFor(ApiFixture.Backoffice.Orgaanbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());

        var formalFrameworkCategoryId = await _apiFixture.Create.FormalFrameworkCategory();
        var formalFrameworkId = await _apiFixture.Create.FormalFramework(formalFrameworkCategoryId);

        var response = await PostAddFormalFramework(
            client,
            organisationId,
            parentOrganisationId,
            formalFrameworkId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Vlimpersbeheerder mag enkel toepassingsgebieden beheren die eigendom zijn
    /// van Vlimpers.
    /// </summary>
    [Fact]
    public async Task Vlimpersbeheerder_CanUseVlimpersOwnedFormalFramework()
    {
        var client = await _apiFixture.CreateBackofficeUserClientFor(ApiFixture.Backoffice.Vlimpersbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());

        var formalFrameworkCategoryId = await _apiFixture.Create.FormalFrameworkCategory();
        var vlimpersFormalFrameworkId = _apiFixture.Configuration.Authorization.FormalFrameworkIdsOwnedByVlimpers.First();
        await _apiFixture.Create.FormalFramework(vlimpersFormalFrameworkId, formalFrameworkCategoryId);

        var response = await PostAddFormalFramework(
            client,
            organisationId,
            parentOrganisationId,
            vlimpersFormalFrameworkId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    /// <summary>
    /// Vlimpersbeheerder mag geen toepassingsgebieden beheren die niet eigendom
    /// zijn van Vlimpers.
    /// </summary>
    [Fact]
    public async Task Vlimpersbeheerder_CannotUseNonVlimpersFormalFramework()
    {
        var client = await _apiFixture.CreateBackofficeUserClientFor(ApiFixture.Backoffice.Vlimpersbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());

        var formalFrameworkCategoryId = await _apiFixture.Create.FormalFrameworkCategory();
        var formalFrameworkId = await _apiFixture.Create.FormalFramework(formalFrameworkCategoryId);

        var response = await PostAddFormalFramework(
            client,
            organisationId,
            parentOrganisationId,
            formalFrameworkId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Regelgevingbeheerder mag enkel toepassingsgebieden beheren die eigendom
    /// zijn van de Regelgeving-databank.
    /// </summary>
    [Fact]
    public async Task Regelgevingbeheerder_CanUseRegelgevingOwnedFormalFramework()
    {
        var client = await _apiFixture.CreateBackofficeUserClientFor(ApiFixture.Backoffice.Regelgevingbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());

        var formalFrameworkCategoryId = await _apiFixture.Create.FormalFrameworkCategory();
        var regelgevingFormalFrameworkId = _apiFixture.Configuration.Authorization.FormalFrameworkIdsOwnedByRegelgevingDbBeheerder.First();
        await _apiFixture.Create.FormalFramework(regelgevingFormalFrameworkId, formalFrameworkCategoryId);

        var response = await PostAddFormalFramework(
            client,
            organisationId,
            parentOrganisationId,
            regelgevingFormalFrameworkId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    /// <summary>
    /// Regelgevingbeheerder mag geen Vlimpers-toepassingsgebieden beheren.
    /// </summary>
    [Fact]
    public async Task Regelgevingbeheerder_CannotUseVlimpersOwnedFormalFramework()
    {
        var client = await _apiFixture.CreateBackofficeUserClientFor(ApiFixture.Backoffice.Regelgevingbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());

        var formalFrameworkCategoryId = await _apiFixture.Create.FormalFrameworkCategory();
        var vlimpersFormalFrameworkId = _apiFixture.Configuration.Authorization.FormalFrameworkIdsOwnedByVlimpers.First();
        await _apiFixture.Create.FormalFramework(vlimpersFormalFrameworkId, formalFrameworkCategoryId);

        var response = await PostAddFormalFramework(
            client,
            organisationId,
            parentOrganisationId,
            vlimpersFormalFrameworkId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Decentraalbeheerder mag een niet-Vlimpers-toepassingsgebied beheren voor
    /// een organisatie waarvoor hij beheerder is.
    /// </summary>
    [Fact]
    public async Task Decentraalbeheerder_CanUseNonVlimpersFormalFrameworkForOwnOrganisation()
    {
        var client = await _apiFixture.CreateBackofficeUserClientFor(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = await _apiFixture.GetOrCreateOrganisationWithOvoNumber("OVO000002");
        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());

        var formalFrameworkCategoryId = await _apiFixture.Create.FormalFrameworkCategory();
        var formalFrameworkId = await _apiFixture.Create.FormalFramework(formalFrameworkCategoryId);

        var response = await PostAddFormalFramework(
            client,
            organisationId,
            parentOrganisationId,
            formalFrameworkId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    /// <summary>
    /// Decentraalbeheerder mag geen toepassingsgebied beheren voor een organisatie
    /// waarvoor hij geen beheerder is.
    /// </summary>
    [Fact]
    public async Task Decentraalbeheerder_CannotUseFormalFrameworkForOtherOrganisation()
    {
        var client = await _apiFixture.CreateBackofficeUserClientFor(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());

        var formalFrameworkCategoryId = await _apiFixture.Create.FormalFrameworkCategory();
        var formalFrameworkId = await _apiFixture.Create.FormalFramework(formalFrameworkCategoryId);

        var response = await PostAddFormalFramework(
            client,
            organisationId,
            parentOrganisationId,
            formalFrameworkId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Decentraalbeheerder mag zelfs voor zijn eigen organisatie geen
    /// Vlimpers-toepassingsgebied beheren.
    /// </summary>
    [Fact]
    public async Task Decentraalbeheerder_CannotUseVlimpersOwnedFormalFrameworkForOwnOrganisation()
    {
        var client = await _apiFixture.CreateBackofficeUserClientFor(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = await _apiFixture.GetOrCreateOrganisationWithOvoNumber("OVO000002");
        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());

        var formalFrameworkCategoryId = await _apiFixture.Create.FormalFrameworkCategory();
        var vlimpersFormalFrameworkId = _apiFixture.Configuration.Authorization.FormalFrameworkIdsOwnedByVlimpers.First();
        await _apiFixture.Create.FormalFramework(vlimpersFormalFrameworkId, formalFrameworkCategoryId);

        var response = await PostAddFormalFramework(
            client,
            organisationId,
            parentOrganisationId,
            vlimpersFormalFrameworkId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Cjmbeheerder beschikt niet over <see cref="Permission.CanManageFormalFrameworks" />.
    /// </summary>
    [Fact]
    public async Task Cjmbeheerder_CannotManageFormalFrameworks()
    {
        var client = await _apiFixture.CreateBackofficeUserClientFor(ApiFixture.Backoffice.Cjmbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());

        var formalFrameworkCategoryId = await _apiFixture.Create.FormalFrameworkCategory();
        var formalFrameworkId = await _apiFixture.Create.FormalFramework(formalFrameworkCategoryId);

        var response = await PostAddFormalFramework(
            client,
            organisationId,
            parentOrganisationId,
            formalFrameworkId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<HttpResponseMessage> PostAddFormalFramework(
        HttpClient client,
        Guid organisationId,
        Guid parentOrganisationId,
        Guid formalFrameworkId)
        => await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/formalframeworks",
            new AddOrganisationFormalFrameworkRequest
            {
                OrganisationFormalFrameworkId = _apiFixture.Fixture.Create<Guid>(),
                FormalFrameworkId = formalFrameworkId,
                ParentOrganisationId = parentOrganisationId,
                ValidFrom = null,
                ValidTo = null,
            });
}
