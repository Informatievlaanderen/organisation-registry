namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Update.When_Updating_Organisation_Limited_To_Vlimpers;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Detail;
using Xunit;

/// <summary>
/// Rollen zonder <c>CanManageOrganisationInfoLimitedToVlimpers</c> mogen de
/// vier Vlimpers-voorbehouden velden niet aanpassen en krijgen 403 terug.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_CanManageOrganisationInfoLimitedToVlimpers
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_CanManageOrganisationInfoLimitedToVlimpers(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Cjmbeheerder)]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orgaanbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orafinbeheerder)]
    public async Task Then_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        var response = await ApiFixture.Put(
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

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
