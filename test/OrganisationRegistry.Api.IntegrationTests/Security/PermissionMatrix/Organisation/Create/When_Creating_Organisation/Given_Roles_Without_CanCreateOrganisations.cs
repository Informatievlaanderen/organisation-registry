namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Create.When_Creating_Organisation;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Detail;
using Xunit;

/// <summary>
/// Enkel AlgemeenBeheerder bezit <c>CanCreateOrganisations</c>. Alle andere
/// rollen — inclusief CjmBeheerder, VlimpersBeheerder en DecentraalBeheerder,
/// die elders wel organisatiegebonden rechten hebben — krijgen 403 terug bij
/// het registreren van een top-level organisatie.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_CanCreateOrganisations
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_CanCreateOrganisations(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Cjmbeheerder)]
    [InlineData(ApiFixture.Backoffice.Vlimpersbeheerder)]
    [InlineData(ApiFixture.Backoffice.Decentraalbeheerder)]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orgaanbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orafinbeheerder)]
    public async Task Then_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var response = await ApiFixture.Post(
            client,
            "/v1/organisations",
            new CreateOrganisationRequest
            {
                Id = _apiFixture.Fixture.Create<Guid>(),
                Name = _apiFixture.Fixture.Create<string>(),
                ParentOrganisationId = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
