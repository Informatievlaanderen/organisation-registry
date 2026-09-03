namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Regulations.When_Adding_Regulations;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Regulation;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_CanManageRegulations
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_CanManageRegulations(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Decentraalbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orgaanbeheerder)]
    public async Task Then_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var themeId = await _apiFixture.Create.RegulationTheme();
        var subThemeId = await _apiFixture.Create.RegulationSubTheme(themeId);

        var response = await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/regulations",
            new AddOrganisationRegulationRequest()
            {
                OrganisationRegulationId = entityId,
                RegulationThemeId = themeId,
                RegulationSubThemeId = subThemeId,
                Date = null,
                Name = _apiFixture.Fixture.Create<string>(),
                Url = null,
                WorkRulesUrl = null,
                Description = null,
                DescriptionRendered = null,
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
