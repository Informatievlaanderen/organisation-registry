namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.OpeningHours.When_Updating_OpeningHours;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.OpeningHour;
using Xunit;

/// <summary>
/// Matrixrij <b>Openingsuren</b> — gedreven door <see cref="Infrastructure.Authorization.Permission.CanManageOpeningHours" />.
/// Zie <see cref="When_Adding_OpeningHours.Given_No_Role_Has_CanManageOpeningHours" /> voor de
/// rationale: de permissie is bewust aan geen enkele rol toegekend, dus elke rol krijgt 403.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_No_Role_Has_CanManageOpeningHours
{
    private readonly ApiFixture _apiFixture;

    public Given_No_Role_Has_CanManageOpeningHours(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Algemeenbeheerder)]
    [InlineData(ApiFixture.Backoffice.Vlimpersbeheerder)]
    [InlineData(ApiFixture.Backoffice.Decentraalbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orgaanbeheerder)]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder)]
    [InlineData(ApiFixture.Backoffice.Cjmbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orafinbeheerder)]
    public async Task Then_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        var response = await ApiFixture.Put(
            client,
            $"/v1/organisations/{organisationId}/openingHours/{_apiFixture.Fixture.Create<Guid>()}",
            new UpdateOrganisationOpeningHourRequest
            {
                OrganisationOpeningHourId = _apiFixture.Fixture.Create<Guid>(),
                Opens = new TimeSpan(9, 0, 0),
                Closes = new TimeSpan(17, 0, 0),
                DayOfWeek = DayOfWeek.Monday,
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
