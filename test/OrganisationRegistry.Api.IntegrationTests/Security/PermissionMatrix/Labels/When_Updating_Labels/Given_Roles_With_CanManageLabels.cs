namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Labels.When_Updating_Labels;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Label;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_CanManageLabels
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_CanManageLabels(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_OK()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var labelTypeId = await _apiFixture.Create.LabelType();

        await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/labels",
            new AddOrganisationLabelRequest()
            {
                OrganisationLabelId = entityId,
                LabelTypeId = labelTypeId,
                LabelValue = _apiFixture.Fixture.Create<string>(),
                ValidFrom = null,
                ValidTo = null,
            });

        var response = await ApiFixture.Put(
            client,
            $"/v1/organisations/{organisationId}/labels/{entityId}",
            new UpdateOrganisationLabelRequest()
            {
                OrganisationLabelId = entityId,
                LabelTypeId = labelTypeId,
                LabelValue = _apiFixture.Fixture.Create<string>(),
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(Skip = "TODO: scoped role 'Decentraalbeheerder' is allowed by the permission matrix but the domain authorization policy requires the organisation (or entity) to be within the role's own scope (BeheerderForOrganisation / configured owned-ids). No fixture precedent exists for creating an organisation inside a scoped role's Keycloak OVO scope, so this positive cannot yet assert a 2xx. Enable once scoped-org test setup is available.")]
    public async Task For_Decentraalbeheerder_Then_Returns_OK()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var labelTypeId = await _apiFixture.Create.LabelType();

        await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/labels",
            new AddOrganisationLabelRequest()
            {
                OrganisationLabelId = entityId,
                LabelTypeId = labelTypeId,
                LabelValue = _apiFixture.Fixture.Create<string>(),
                ValidFrom = null,
                ValidTo = null,
            });

        var response = await ApiFixture.Put(
            client,
            $"/v1/organisations/{organisationId}/labels/{entityId}",
            new UpdateOrganisationLabelRequest()
            {
                OrganisationLabelId = entityId,
                LabelTypeId = labelTypeId,
                LabelValue = _apiFixture.Fixture.Create<string>(),
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
