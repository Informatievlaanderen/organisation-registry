namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Kbo;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_CanManageKbo
{
    // KBO number backed by the WireMock GeefOnderneming mock (valid, non-terminated
    // organisation). 0563634435 is already used by CreateFromKboNumberTests, so this
    // matrix test uses the second mocked number to avoid a KBO-uniqueness clash.
    private const string MockedKboNumber = "0563634434";

    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_CanManageKbo(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_Ok()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        var response = await ApiFixture.Put(
            client,
            $"/v1/organisations/{organisationId}/kbo/number/{MockedKboNumber}",
            new { });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
