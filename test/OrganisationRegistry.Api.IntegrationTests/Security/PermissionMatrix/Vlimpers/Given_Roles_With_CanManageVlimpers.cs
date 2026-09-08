namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Vlimpers;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using Backoffice.Vlimpers;
using FluentAssertions;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_CanManageVlimpers
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_CanManageVlimpers(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_Ok()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        var response = await ApiFixture.Patch(
            client,
            $"/v1/organisations/{organisationId}/vlimpers",
            new VlimpersRequest { VlimpersManagement = true });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
