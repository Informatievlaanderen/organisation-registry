namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Update.When_Updating_Organisation_Not_Limited_To_Vlimpers;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Detail;
using Xunit;

/// <summary>
/// Rollen zonder <c>CanManageOrganisationInfoNotLimitedToVlimpers</c> mogen
/// de niet-Vlimpers-voorbehouden velden niet aanpassen en krijgen 403 terug.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_CanManageOrganisationInfoNotLimitedToVlimpers
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_CanManageOrganisationInfoNotLimitedToVlimpers(ApiFixture apiFixture)
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
            $"/v1/organisations/{organisationId}/notlimitedtovlimpers",
            new UpdateOrganisationInfoNotLimitedToVlimpersRequest
            {
                Description = _apiFixture.Fixture.Create<string>(),
                ShowOnVlaamseOverheidSites = false,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
