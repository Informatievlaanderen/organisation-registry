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
/// Covers <see cref="OrganisationPolicy"/> as used for the three
/// organisation-editing permissions:
///
/// <list type="bullet">
/// <item><see cref="Permission.CanManageOrganisation"/> (general endpoint,
/// all fields incl. the four Vlimpers-reserved ones): AlgemeenBeheerder/
/// Developer only, unrestricted. VlimpersBeheerder and DecentraalBeheerder
/// never hold it.</item>
/// <item><see cref="Permission.CanManageOrganisationInfoLimitedToVlimpers"/>
/// (the four Vlimpers-reserved fields): AlgemeenBeheerder/Developer
/// unrestricted, VlimpersBeheerder restricted to organisations under
/// Vlimpers management, DecentraalBeheerder never.</item>
/// <item><see cref="Permission.CanManageOrganisationInfoNotLimitedToVlimpers"/>
/// (every other field): AlgemeenBeheerder/Developer unrestricted,
/// DecentraalBeheerder restricted to their own organisation regardless of
/// Vlimpers-management status, VlimpersBeheerder never.</item>
/// </list>
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

    private static OrganisationPolicy CreatePolicy(
        Permission permission, string ovoNumber, bool isUnderVlimpersManagement)
        => new(permission, ovoNumber, isUnderVlimpersManagement);

    // --- CanManageOrganisation (general endpoint) --------------------------

    [Theory]
    [InlineData(Role.AlgemeenBeheerder)]
    public void CanManageOrganisation_UnrestrictedRolesAreAlwaysAuthorized(Role role)
    {
        var user = UserWithRoles(role);

        CreatePolicy(Permission.CanManageOrganisation, _fixture.Create<string>(), _fixture.Create<bool>())
            .Check(user)
            .Should().Be(AuthorizationResult.Success());
    }

    [Theory]
    [InlineData(Role.CjmBeheerder)]
    [InlineData(Role.RegelgevingBeheerder)]
    [InlineData(Role.OrgaanBeheerder)]
    [InlineData(Role.Orafin)]
    public void CanManageOrganisation_RolesWithoutPermissionAreNotAuthorized(Role role)
    {
        var user = UserWithRoles(role);

        CreatePolicy(Permission.CanManageOrganisation, _fixture.Create<string>(), isUnderVlimpersManagement: true)
            .Check(user)
            .ShouldFailWith<InsufficientRights<OrganisationPolicy>>();
    }

    [Fact]
    public void CanManageOrganisation_DecentraalBeheerderIsNeverAuthorized_EvenForOwnOrganisation()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = DecentraalBeheerderFor(ovoNumber);

        CreatePolicy(Permission.CanManageOrganisation, ovoNumber, isUnderVlimpersManagement: false)
            .Check(user)
            .ShouldFailWith<InsufficientRights<OrganisationPolicy>>();
    }

    [Fact]
    public void CanManageOrganisation_VlimpersBeheerderIsNeverAuthorized_EvenIfUnderVlimpersManagement()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = VlimpersBeheerderFor(ovoNumber);

        CreatePolicy(Permission.CanManageOrganisation, ovoNumber, isUnderVlimpersManagement: true)
            .Check(user)
            .ShouldFailWith<InsufficientRights<OrganisationPolicy>>();
    }

    // --- CanManageOrganisationInfoLimitedToVlimpers -------------------------

    [Theory]
    [InlineData(Role.AlgemeenBeheerder)]
    public void CanManageOrganisationInfoLimitedToVlimpers_UnrestrictedRolesAreAlwaysAuthorized(Role role)
    {
        var user = UserWithRoles(role);

        CreatePolicy(
                Permission.CanManageOrganisationInfoLimitedToVlimpers,
                _fixture.Create<string>(),
                _fixture.Create<bool>())
            .Check(user)
            .Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void CanManageOrganisationInfoLimitedToVlimpers_VlimpersBeheerderIsAuthorizedIfUnderVlimpersManagement()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = VlimpersBeheerderFor(ovoNumber);

        CreatePolicy(Permission.CanManageOrganisationInfoLimitedToVlimpers, ovoNumber, isUnderVlimpersManagement: true)
            .Check(user)
            .Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void CanManageOrganisationInfoLimitedToVlimpers_VlimpersBeheerderIsNotAuthorizedIfNotUnderVlimpersManagement()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = VlimpersBeheerderFor(ovoNumber);

        CreatePolicy(Permission.CanManageOrganisationInfoLimitedToVlimpers, ovoNumber, isUnderVlimpersManagement: false)
            .Check(user)
            .ShouldFailWith<InsufficientRights<OrganisationPolicy>>();
    }

    [Fact]
    public void CanManageOrganisationInfoLimitedToVlimpers_DecentraalBeheerderIsNeverAuthorized()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = DecentraalBeheerderFor(ovoNumber);

        CreatePolicy(Permission.CanManageOrganisationInfoLimitedToVlimpers, ovoNumber, isUnderVlimpersManagement: true)
            .Check(user)
            .ShouldFailWith<InsufficientRights<OrganisationPolicy>>();

        CreatePolicy(Permission.CanManageOrganisationInfoLimitedToVlimpers, ovoNumber, isUnderVlimpersManagement: false)
            .Check(user)
            .ShouldFailWith<InsufficientRights<OrganisationPolicy>>();
    }

    // --- CanManageOrganisationInfoNotLimitedToVlimpers ----------------------

    [Theory]
    [InlineData(Role.AlgemeenBeheerder)]
    public void CanManageOrganisationInfoNotLimitedToVlimpers_UnrestrictedRolesAreAlwaysAuthorized(Role role)
    {
        var user = UserWithRoles(role);

        CreatePolicy(
                Permission.CanManageOrganisationInfoNotLimitedToVlimpers,
                _fixture.Create<string>(),
                _fixture.Create<bool>())
            .Check(user)
            .Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void CanManageOrganisationInfoNotLimitedToVlimpers_DecentraalBeheerderIsAuthorizedForTheirOwnOrganisation_RegardlessOfVlimpersManagement()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = DecentraalBeheerderFor(ovoNumber);

        CreatePolicy(Permission.CanManageOrganisationInfoNotLimitedToVlimpers, ovoNumber, isUnderVlimpersManagement: false)
            .Check(user)
            .Should().Be(AuthorizationResult.Success());

        CreatePolicy(Permission.CanManageOrganisationInfoNotLimitedToVlimpers, ovoNumber, isUnderVlimpersManagement: true)
            .Check(user)
            .Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void CanManageOrganisationInfoNotLimitedToVlimpers_DecentraalBeheerderIsNotAuthorizedForOtherOrganisations()
    {
        var user = DecentraalBeheerderFor(_fixture.Create<string>());

        CreatePolicy(Permission.CanManageOrganisationInfoNotLimitedToVlimpers, _fixture.Create<string>(), isUnderVlimpersManagement: false)
            .Check(user)
            .ShouldFailWith<InsufficientRights<OrganisationPolicy>>();
    }

    [Fact]
    public void CanManageOrganisationInfoNotLimitedToVlimpers_VlimpersBeheerderIsNeverAuthorized()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = VlimpersBeheerderFor(ovoNumber);

        CreatePolicy(Permission.CanManageOrganisationInfoNotLimitedToVlimpers, ovoNumber, isUnderVlimpersManagement: true)
            .Check(user)
            .ShouldFailWith<InsufficientRights<OrganisationPolicy>>();
    }
}
