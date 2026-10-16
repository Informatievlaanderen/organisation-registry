namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.FormalFrameworks.When_Adding_FormalFrameworks;

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.FormalFramework;
using OrganisationRegistry.Api.Backoffice.Vlimpers;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_CanManageFormalFrameworks
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_CanManageFormalFrameworks(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_Created()
    {
        using var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        using var response = await AddFormalFramework(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOwnOrganisation_Then_Returns_Created()
    {
        using var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        using var response = await AddFormalFramework(client, _apiFixture.DecentraalbeheerderOrganisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithChildOrganisationInScope_Then_Returns_Created()
    {
        using var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        using var response = await AddFormalFramework(client, _apiFixture.DecentraalbeheerderChildOrganisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOrganisationOutsideScope_Then_Returns_Forbidden()
    {
        using var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        using var response = await AddFormalFramework(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task For_Regelgevingbeheerder_WithOwnedFormalFramework_Then_Returns_Created()
    {
        using var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Regelgevingbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var categoryId = await _apiFixture.Create.FormalFrameworkCategory();
        var formalFrameworkId = await _apiFixture.Create.FormalFramework(
            _apiFixture.Configuration.Authorization.FormalFrameworkIdsOwnedByRegelgevingDbBeheerder.First(),
            categoryId);
        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());

        using var response = await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/formalframeworks",
            new AddOrganisationFormalFrameworkRequest()
            {
                OrganisationFormalFrameworkId = entityId,
                FormalFrameworkId = formalFrameworkId,
                ParentOrganisationId = parentOrganisationId,
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Regelgevingbeheerder_WithNonOwnedFormalFramework_Then_Returns_Forbidden()
    {
        using var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Regelgevingbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var categoryId = await _apiFixture.Create.FormalFrameworkCategory();
        var formalFrameworkId = await _apiFixture.Create.FormalFramework(categoryId);
        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());

        using var response = await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/formalframeworks",
            new AddOrganisationFormalFrameworkRequest()
            {
                OrganisationFormalFrameworkId = entityId,
                FormalFrameworkId = formalFrameworkId,
                ParentOrganisationId = parentOrganisationId,
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task For_Vlimpersbeheerder_WithOrgOwnedByVlimpers_WithOwnedFormalFramework_Then_Returns_Created()
    {
        using var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Vlimpersbeheerder);

        var organisationId = await CreateVlimpersManagedOrganisation();
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var categoryId = await _apiFixture.Create.FormalFrameworkCategory();
        var formalFrameworkId = await _apiFixture.Create.FormalFramework(
            _apiFixture.Configuration.Authorization.FormalFrameworkIdsOwnedByVlimpers.First(),
            categoryId);
        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());

        using var response = await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/formalframeworks",
            new AddOrganisationFormalFrameworkRequest()
            {
                OrganisationFormalFrameworkId = entityId,
                FormalFrameworkId = formalFrameworkId,
                ParentOrganisationId = parentOrganisationId,
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    /// <summary>
    /// Regressietest: zelfs een Vlimpers-owned formeel kader mag de
    /// vlimpersbeheerder niet beheren wanneer de organisatie niet (meer) onder
    /// Vlimpersbeheer valt. Zonder de Vlimpers-managementgate op
    /// <see cref="OrganisationRegistry.Infrastructure.Authorization.Restrictions.FormalFrameworkRestrictions.VlimpersManaged"/>
    /// zou de allow-list-check alleen al volstaan.
    /// </summary>
    [Fact]
    public async Task For_Vlimpersbeheerder_WithOwnedFormalFramework_OnOrganisationNotUnderVlimpersManagement_Then_Returns_Forbidden()
    {
        using var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Vlimpersbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var categoryId = await _apiFixture.Create.FormalFrameworkCategory();
        var formalFrameworkId = await _apiFixture.Create.FormalFramework(
            _apiFixture.Configuration.Authorization.FormalFrameworkIdsOwnedByVlimpers.First(),
            categoryId);
        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());

        using var response = await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/formalframeworks",
            new AddOrganisationFormalFrameworkRequest()
            {
                OrganisationFormalFrameworkId = entityId,
                FormalFrameworkId = formalFrameworkId,
                ParentOrganisationId = parentOrganisationId,
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<Guid> CreateVlimpersManagedOrganisation()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        using var algemeenbeheerderClient = await _apiFixture.CreateAlgemeenbeheerderClient();
        using var response = await ApiFixture.Patch(
            algemeenbeheerderClient,
            $"/v1/organisations/{organisationId}/vlimpers",
            new VlimpersRequest { VlimpersManagement = true });
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        return organisationId;
    }

    [Fact]
    public async Task For_Vlimpersbeheerder_WithNonOwnedFormalFramework_Then_Returns_Forbidden()
    {
        using var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Vlimpersbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var categoryId = await _apiFixture.Create.FormalFrameworkCategory();
        var formalFrameworkId = await _apiFixture.Create.FormalFramework(categoryId);
        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());

        using var response = await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/formalframeworks",
            new AddOrganisationFormalFrameworkRequest()
            {
                OrganisationFormalFrameworkId = entityId,
                FormalFrameworkId = formalFrameworkId,
                ParentOrganisationId = parentOrganisationId,
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithVlimpersOwnedFormalFramework_Then_Returns_Forbidden()
    {
        using var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.DecentraalbeheerderOrganisationId;
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var categoryId = await _apiFixture.Create.FormalFrameworkCategory();
        var formalFrameworkId = await _apiFixture.Create.FormalFramework(
            _apiFixture.Configuration.Authorization.FormalFrameworkIdsOwnedByVlimpers.First(),
            categoryId);
        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());

        using var response = await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/formalframeworks",
            new AddOrganisationFormalFrameworkRequest()
            {
                OrganisationFormalFrameworkId = entityId,
                FormalFrameworkId = formalFrameworkId,
                ParentOrganisationId = parentOrganisationId,
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<HttpResponseMessage> AddFormalFramework(HttpClient client, Guid organisationId)
    {
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var categoryId = await _apiFixture.Create.FormalFrameworkCategory();
        var formalFrameworkId = await _apiFixture.Create.FormalFramework(categoryId);
        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());

        return await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/formalframeworks",
            new AddOrganisationFormalFrameworkRequest()
            {
                OrganisationFormalFrameworkId = entityId,
                FormalFrameworkId = formalFrameworkId,
                ParentOrganisationId = parentOrganisationId,
                ValidFrom = null,
                ValidTo = null,
            });
    }
}
