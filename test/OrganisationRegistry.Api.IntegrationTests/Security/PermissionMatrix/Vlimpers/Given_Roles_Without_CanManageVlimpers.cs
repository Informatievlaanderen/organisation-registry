namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Vlimpers;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using Backoffice.Vlimpers;
using FluentAssertions;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_CanManageVlimpers
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_CanManageVlimpers(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Vlimpersbeheerder)]
    [InlineData(ApiFixture.Backoffice.Decentraalbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orgaanbeheerder)]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder)]
    [InlineData(ApiFixture.Backoffice.Cjmbeheerder)]
    public async Task Then_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        var response = await ApiFixture.Patch(
            client,
            $"/v1/organisations/{organisationId}/vlimpers",
            new VlimpersRequest { VlimpersManagement = true });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
