namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.FormalFrameworks.When_Adding_FormalFrameworks;

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.FormalFramework;
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
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        var response = await AddFormalFramework(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOwnOrganisation_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var response = await AddFormalFramework(client, _apiFixture.DecentraalbeheerderOrganisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithChildOrganisationInScope_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var response = await AddFormalFramework(client, _apiFixture.DecentraalbeheerderChildOrganisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOrganisationOutsideScope_Then_Returns_Forbidden()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        var response = await AddFormalFramework(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task For_Regelgevingbeheerder_WithOwnedFormalFramework_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Regelgevingbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var categoryId = await _apiFixture.Create.FormalFrameworkCategory();
        var formalFrameworkId = await _apiFixture.Create.FormalFramework(
            _apiFixture.Configuration.Authorization.FormalFrameworkIdsOwnedByRegelgevingDbBeheerder.First(),
            categoryId);
        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());

        var response = await ApiFixture.Post(
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
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Regelgevingbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var categoryId = await _apiFixture.Create.FormalFrameworkCategory();
        var formalFrameworkId = await _apiFixture.Create.FormalFramework(categoryId);
        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());

        var response = await ApiFixture.Post(
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
