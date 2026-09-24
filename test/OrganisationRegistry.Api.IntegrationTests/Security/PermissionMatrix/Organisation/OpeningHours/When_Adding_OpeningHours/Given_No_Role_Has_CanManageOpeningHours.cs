namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.OpeningHours.When_Adding_OpeningHours;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.OpeningHour;
using Xunit;

/// <summary>
/// Matrixrij <b>Openingsuren</b> — gedreven door <see cref="Infrastructure.Authorization.Permission.CanManageOpeningHours" />.
/// Openingsuren worden in de toekomst verwijderd uit het register; de permissie bestaat
/// zodat de controller een expliciete beveiligingskeuze maakt (FR-006), maar is bewust aan
/// geen enkele rol toegekend in <c>RolePermissionMap</c>. Elke rol — inclusief AlgemeenBeheerder
/// en Developer — krijgt dus 403 bij het toevoegen van een openingsuur.
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

        var response = await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/openingHours",
            new AddOrganisationOpeningHourRequest
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
