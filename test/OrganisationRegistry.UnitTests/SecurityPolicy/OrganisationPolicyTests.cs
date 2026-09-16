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

/// <summary>
/// Covers <see cref="OrganisationPolicy"/> as used for
/// <see cref="Permission.CanManageOrganisation"/>: AlgemeenBeheerder
/// holds an unrestricted grant, VlimpersBeheerder is restricted to
/// organisations under Vlimpers management, and DecentraalBeheerder is
/// restricted to their own organisation as long as it is not under Vlimpers
/// management. CjmBeheerder no longer holds this permission.
/// </summary>
public class OrganisationPolicyTests
{
    private readonly Fixture _fixture;
    private readonly IOrganisationRegistryConfiguration _configuration;

    public OrganisationPolicyTests()
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

    private IUser VlimpersBeheerderFor(string ovoNumber)
        => new UserBuilder()
            .AddRoles(Role.VlimpersBeheerder)
            .AddOrganisations(ovoNumber)
            .WithPermissions(
                RolePermissionMap.For(new[] { Role.VlimpersBeheerder }, _configuration))
            .Build();

    private static OrganisationPolicy CreatePolicy(string ovoNumber, bool isUnderVlimpersManagement)
        => new(Permission.CanManageOrganisation, ovoNumber, isUnderVlimpersManagement);

    [Theory]
    [InlineData(Role.AlgemeenBeheerder)]
    public void UnrestrictedRolesAreAlwaysAuthorized(Role role)
    {
        var user = UserWithRoles(role);

        CreatePolicy(_fixture.Create<string>(), _fixture.Create<bool>())
            .Check(user)
            .Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void CjmBeheerderIsNotAuthorized()
    {
        var user = UserWithRoles(Role.CjmBeheerder);

        CreatePolicy(_fixture.Create<string>(), _fixture.Create<bool>())
            .Check(user)
            .Should().NotBe(AuthorizationResult.Success());
    }

    [Fact]
    public void DecentraalBeheerderIsAuthorizedForTheirOwnOrganisationIfNotUnderVlimpersManagement()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = DecentraalBeheerderFor(ovoNumber);

        CreatePolicy(ovoNumber, isUnderVlimpersManagement: false)
            .Check(user)
            .Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void DecentraalBeheerderIsNotAuthorizedIfUnderVlimpersManagement()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = DecentraalBeheerderFor(ovoNumber);

        CreatePolicy(ovoNumber, isUnderVlimpersManagement: true)
            .Check(user)
            .ShouldFailWith<InsufficientRights<OrganisationPolicy>>();
    }

    [Fact]
    public void DecentraalBeheerderIsNotAuthorizedForOtherOrganisations()
    {
        var user = DecentraalBeheerderFor(_fixture.Create<string>());

        CreatePolicy(_fixture.Create<string>(), isUnderVlimpersManagement: false)
            .Check(user)
            .ShouldFailWith<InsufficientRights<OrganisationPolicy>>();
    }

    [Fact]
    public void VlimpersBeheerderIsAuthorizedIfUnderVlimpersManagement()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = VlimpersBeheerderFor(ovoNumber);

        CreatePolicy(ovoNumber, isUnderVlimpersManagement: true)
            .Check(user)
            .Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void VlimpersBeheerderIsNotAuthorizedIfNotUnderVlimpersManagement()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = VlimpersBeheerderFor(ovoNumber);

        CreatePolicy(ovoNumber, isUnderVlimpersManagement: false)
            .Check(user)
            .ShouldFailWith<InsufficientRights<OrganisationPolicy>>();
    }

    [Theory]
    [InlineData(Role.RegelgevingBeheerder)]
    [InlineData(Role.OrgaanBeheerder)]
    [InlineData(Role.Orafin)]
    public void RolesWithoutPermissionAreNotAuthorizedEvenIfUnderVlimpersManagement(Role role)
    {
        var user = UserWithRoles(role);

        CreatePolicy(_fixture.Create<string>(), isUnderVlimpersManagement: true)
            .Check(user)
            .ShouldFailWith<InsufficientRights<OrganisationPolicy>>();
    }
}
