namespace OrganisationRegistry.UnitTests.Backoffice.Organisation;

using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Authorization.Restrictions;
using OrganisationRegistry.Tests.Shared;
using Xunit;

/// <summary>
/// <see cref="OrganisationPermissions.CanManageBodies"/> drives tab
/// visibility and the "register a new body" action on an organisation's
/// detail screen. It mirrors <see cref="OrganisationRegistry.Handling.Authorization.RegisterBodyPolicy"/>:
/// an unrestricted <see cref="Permission.CanManageBodies"/> holder
/// (AlgemeenBeheerder, OrgaanBeheerder) always sees it; a DecentraalBeheerder
/// only sees it for their own (or a child) organisation; roles without any
/// <c>CanManageBodies</c> grant (VlimpersBeheerder, RegelgevingBeheerder, VO
/// medewerker) never see it.
/// </summary>
public class OrganisationPermissionsCanManageBodiesTests
{
    private const string OwnOvoNumber = "OVO000003";
    private const string OtherOvoNumber = "OVO000004";

    [Fact]
    public void AlgemeenBeheerder_can_manage_bodies_for_any_organisation()
    {
        var permissions = OrganisationPermissions.For(
            TestUser.AlgemeenBeheerder, OtherOvoNumber, isUnderVlimpersManagement: false);

        permissions.CanManageBodies.Should().BeTrue();
    }

    [Fact]
    public void OrgaanBeheerder_can_manage_bodies_for_any_organisation()
    {
        var permissions = OrganisationPermissions.For(
            TestUser.OrgaanBeheerder, OtherOvoNumber, isUnderVlimpersManagement: false);

        permissions.CanManageBodies.Should().BeTrue();
    }

    [Fact]
    public void DecentraalBeheerder_can_manage_bodies_for_their_own_organisation()
    {
        var user = new UserBuilder()
            .AsDecentraalBeheerder()
            .AddOrganisations(OwnOvoNumber)
            .WithPermissions(
                PermissionSet.Of(
                    Permission.CanManageBodies.RestrictedTo(DecentraalOrganisationRestriction.Instance)))
            .Build();

        var permissions = OrganisationPermissions.For(
            user, OwnOvoNumber, isUnderVlimpersManagement: false);

        permissions.CanManageBodies.Should().BeTrue();
    }

    [Fact]
    public void DecentraalBeheerder_cannot_manage_bodies_for_another_organisation()
    {
        var user = new UserBuilder()
            .AsDecentraalBeheerder()
            .AddOrganisations(OwnOvoNumber)
            .WithPermissions(
                PermissionSet.Of(
                    Permission.CanManageBodies.RestrictedTo(DecentraalOrganisationRestriction.Instance)))
            .Build();

        var permissions = OrganisationPermissions.For(
            user, OtherOvoNumber, isUnderVlimpersManagement: false);

        permissions.CanManageBodies.Should().BeFalse();
    }

    [Fact]
    public void VlimpersBeheerder_cannot_manage_bodies()
    {
        var permissions = OrganisationPermissions.For(
            TestUser.VlimpersBeheerder, OwnOvoNumber, isUnderVlimpersManagement: true);

        permissions.CanManageBodies.Should().BeFalse();
    }

    [Fact]
    public void RegelgevingBeheerder_cannot_manage_bodies()
    {
        var permissions = OrganisationPermissions.For(
            TestUser.RegelgevingBeheerder, OwnOvoNumber, isUnderVlimpersManagement: false);

        permissions.CanManageBodies.Should().BeFalse();
    }

    [Fact]
    public void VoMedewerker_cannot_manage_bodies()
    {
        var permissions = OrganisationPermissions.For(
            TestUser.VoMedewerker, OwnOvoNumber, isUnderVlimpersManagement: false);

        permissions.CanManageBodies.Should().BeFalse();
    }
}
