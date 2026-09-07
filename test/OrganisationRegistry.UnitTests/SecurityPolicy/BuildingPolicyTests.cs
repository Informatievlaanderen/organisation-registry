namespace OrganisationRegistry.UnitTests.SecurityPolicy;

using AutoFixture;
using FluentAssertions;
using Handling.Authorization;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Tests.Shared.Stubs;
using Xunit;

public class BuildingPolicyTests
{
    private readonly Fixture _fixture;
    private readonly IOrganisationRegistryConfiguration _configuration;

    public BuildingPolicyTests()
    {
        _fixture = new Fixture();
        _configuration = new OrganisationRegistryConfigurationStub();
    }

    private IUser UserWithRoles(params Role[] roles)
        => new UserBuilder()
            .AddRoles(roles)
            .WithPermissions(RolePermissionMap.For(roles, _configuration))
            .Build();

    private IUser DecentraalBeheerderFor(string ovoNumber)
        => new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisations(ovoNumber)
            .WithPermissions(
                RolePermissionMap.For(new[] { Role.DecentraalBeheerder }, _configuration))
            .Build();

    private static BuildingPolicy CreatePolicy(string ovoNumber)
        => new(ovoNumber);

    [Theory]
    [InlineData(Role.AlgemeenBeheerder)]
    [InlineData(Role.Developer)]
    public void UnrestrictedRolesAreAuthorizedForAnyOrganisation(Role role)
    {
        var user = UserWithRoles(role);

        CreatePolicy(_fixture.Create<string>())
            .Check(user)
            .Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void DecentraalBeheerderIsAuthorizedForTheirOwnOrganisation()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = DecentraalBeheerderFor(ovoNumber);

        CreatePolicy(ovoNumber)
            .Check(user)
            .Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void DecentraalBeheerderIsNotAuthorizedForOtherOrganisations()
    {
        var user = DecentraalBeheerderFor(_fixture.Create<string>());

        CreatePolicy(_fixture.Create<string>())
            .Check(user)
            .ShouldFailWith<InsufficientRights<BuildingPolicy>>();
    }

    [Theory]
    [InlineData(Role.RegelgevingBeheerder)]
    [InlineData(Role.CjmBeheerder)]
    [InlineData(Role.OrgaanBeheerder)]
    [InlineData(Role.Orafin)]
    public void RolesWithoutPermissionAreNotAuthorized(Role role)
    {
        var user = UserWithRoles(role);

        CreatePolicy(_fixture.Create<string>())
            .Check(user)
            .ShouldFailWith<InsufficientRights<BuildingPolicy>>();
    }
}
