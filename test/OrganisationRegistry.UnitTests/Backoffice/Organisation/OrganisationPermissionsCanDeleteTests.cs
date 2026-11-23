namespace OrganisationRegistry.UnitTests.Backoffice.Organisation;

using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Authorization.Restrictions;
using OrganisationRegistry.Tests.Shared;
using Xunit;

/// <summary>
/// <see cref="OrganisationPermissions.CanDelete"/> follows
/// <see cref="Permission.CanTerminateOrganisation"/>: unrestricted for
/// AlgemeenBeheerder, restricted to Vlimpers-managed organisations for
/// VlimpersBeheerder, never for the read-only VO medewerker.
/// </summary>
public class OrganisationPermissionsCanDeleteTests
{
    private const string OvoNumber = "OVO000001";

    private static IUser VlimpersUser
        => new UserBuilder()
            .AddRoles(Role.VlimpersBeheerder)
            .WithPermissions(
                PermissionSet.Of(
                    Permission.CanTerminateOrganisation.RestrictedTo(
                        ChildRestrictions.UnderVlimpersManagement)))
            .Build();

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AlgemeenBeheerder_can_delete_regardless_of_vlimpers_management(bool isUnderVlimpersManagement)
        => OrganisationPermissions.For(TestUser.AlgemeenBeheerder, OvoNumber, isUnderVlimpersManagement)
            .CanDelete.Should().BeTrue();

    [Fact]
    public void VlimpersBeheerder_can_delete_an_organisation_under_vlimpers_management()
        => OrganisationPermissions.For(VlimpersUser, OvoNumber, isUnderVlimpersManagement: true)
            .CanDelete.Should().BeTrue();

    [Fact]
    public void VlimpersBeheerder_cannot_delete_an_organisation_not_under_vlimpers_management()
        => OrganisationPermissions.For(VlimpersUser, OvoNumber, isUnderVlimpersManagement: false)
            .CanDelete.Should().BeFalse();

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void VoMedewerker_cannot_delete(bool isUnderVlimpersManagement)
        => OrganisationPermissions.For(TestUser.VoMedewerker, OvoNumber, isUnderVlimpersManagement)
            .CanDelete.Should().BeFalse();
}
