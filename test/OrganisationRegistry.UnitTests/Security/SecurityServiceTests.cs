namespace OrganisationRegistry.UnitTests.Security;

using System.Security.Claims;
using System.Threading.Tasks;
using Api.Security;
using FluentAssertions;
using Moq;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Authorization.Cache;
using OrganisationRegistry.Infrastructure.Configuration;
using OrganisationRegistry.SqlServer;
using Xunit;

public class SecurityServiceTests
{
    [Fact]
    public async Task GetRequiredUser_MapsCjmClientIdToCjmWellknownUser()
    {
        var user = await CreateService().GetRequiredUser(
            Principal(new Claim("client_id", "cjmClient")));

        user.IsInAnyOf(Role.CjmBeheerder).Should().BeTrue();
    }

    [Fact]
    public async Task GetRequiredUser_MapsAzpToOrafinWellknownUser()
    {
        var user = await CreateService().GetRequiredUser(
            Principal(new Claim("azp", "orafinClient")));

        user.IsInAnyOf(Role.Orafin).Should().BeTrue();
    }

    [Fact]
    public async Task GetRequiredUser_MapsTestClientIdBeforeScopes()
    {
        var user = await CreateService().GetRequiredUser(
            Principal(
                new Claim("client_id", "testClient"),
                new Claim(
                    AcmIdmConstants.Claims.Scope,
                    $"{AcmIdmConstants.Scopes.CjmBeheerder} {AcmIdmConstants.Scopes.TestClient}")));

        user.IsInAnyOf(Role.AlgemeenBeheerder).Should().BeTrue();
        user.IsInAnyOf(Role.CjmBeheerder).Should().BeFalse();
    }

    [Fact]
    public async Task GetRequiredUser_MapsTestScopeBeforeCjmScope_WhenClientIdIsMissing()
    {
        var user = await CreateService().GetRequiredUser(
            Principal(new Claim(
                AcmIdmConstants.Claims.Scope,
                $"{AcmIdmConstants.Scopes.CjmBeheerder} {AcmIdmConstants.Scopes.TestClient}")));

        user.IsInAnyOf(Role.AlgemeenBeheerder).Should().BeTrue();
        user.IsInAnyOf(Role.CjmBeheerder).Should().BeFalse();
    }

    private static SecurityService CreateService()
        => new(
            Mock.Of<IContextFactory>(),
            Mock.Of<IOrganisationRegistryConfiguration>(),
            Mock.Of<ICache<OrganisationSecurityInformation>>());

    private static ClaimsPrincipal Principal(params Claim[] claims)
        => new(new ClaimsIdentity(claims, authenticationType: "test"));
}
