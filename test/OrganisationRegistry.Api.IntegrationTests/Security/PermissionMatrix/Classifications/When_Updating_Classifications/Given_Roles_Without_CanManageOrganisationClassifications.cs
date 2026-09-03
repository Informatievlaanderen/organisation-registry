namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Classifications.When_Updating_Classifications;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.OrganisationClassification;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_CanManageOrganisationClassifications
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_CanManageOrganisationClassifications(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Orgaanbeheerder)]
    public async Task Then_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var classificationTypeId = await _apiFixture.Create.CreateOrganisationClassificationType(false);
        var classificationId = await _apiFixture.Create.OrganisationClassification(classificationTypeId);

        var response = await ApiFixture.Put(
            client,
            $"/v1/organisations/{organisationId}/classifications/{entityId}",
            new UpdateOrganisationOrganisationClassificationRequest()
            {
                OrganisationOrganisationClassificationId = entityId,
                OrganisationClassificationTypeId = classificationTypeId,
                OrganisationClassificationId = classificationId,
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
