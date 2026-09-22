namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Labels.When_Updating_Labels;

using System;
using System.Net;
using System.Net.Http;
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
        var entityId = await AddLabel(client, organisationId);

        var response = await UpdateLabel(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOwnOrganisation_Then_Returns_OK()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.DecentraalbeheerderOrganisationId;
        var entityId = await AddLabel(client, organisationId);

        var response = await UpdateLabel(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithChildOrganisationInScope_Then_Returns_OK()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.DecentraalbeheerderChildOrganisationId;
        var entityId = await AddLabel(client, organisationId);

        var response = await UpdateLabel(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOrganisationOutsideScope_Then_Returns_Forbidden()
    {
        var privilegedClient = await _apiFixture.CreateAlgemeenbeheerderClient();
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = await AddLabel(privilegedClient, organisationId);

        var response = await UpdateLabel(client, organisationId, entityId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<Guid> AddLabel(HttpClient client, Guid organisationId)
    {
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

        return entityId;
    }


    private async Task<HttpResponseMessage> UpdateLabel(HttpClient client, Guid organisationId, Guid entityId)
    {
        var labelTypeId = await _apiFixture.Create.LabelType();

        return await ApiFixture.Put(
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
    }
}
