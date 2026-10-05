namespace OrganisationRegistry.UnitTests.Backoffice.Body;

using System;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Body;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Authorization.Restrictions;
using OrganisationRegistry.Tests.Shared;
using Xunit;

/// <summary>
/// <see cref="BodyPermissions.CanDelete"/> is derived from the same
/// <see cref="Permission.CanManageBodies"/> check as <see cref="BodyPermissions.CanEdit"/>
/// (a body has no dedicated delete permission). This was previously hardcoded to
/// <c>false</c> for every role, hiding the delete action in the UI even for roles
/// that hold <see cref="Permission.CanManageBodies"/>. These tests pin
/// <c>CanDelete</c> (and <c>CanEdit</c>, which must always agree with it) to the
/// same "who may manage this body" matrix:
///
/// | Role                                  | CanManageBodies grant                 | CanEdit/CanDelete |
/// |----------------------------------------|----------------------------------------|--------------------|
/// | AlgemeenBeheerder                      | unrestricted                           | true (any body)    |
/// | OrgaanBeheerder                        | unrestricted                           | true (any body)    |
/// | CjmBeheerder                           | unrestricted                           | true (any body)    |
/// | DecentraalBeheerder, own/child body     | restricted (DecentraalBodyRestriction) | true               |
/// | DecentraalBeheerder, unrelated body    | restricted (DecentraalBodyRestriction) | false              |
/// | VlimpersBeheerder                      | no grant                               | false              |
/// | RegelgevingBeheerder                   | no grant                               | false              |
/// | VoMedewerker                           | no grant                               | false              |
///
/// DecentraalBeheerder's <c>CanManageBodies</c> grant is config-dependent (added
/// by <c>RolePermissionMap.RestrictedGrantsFor</c>, which needs
/// <c>IOrganisationRegistryConfiguration</c>) and is therefore not present on a
/// role-only <see cref="TestUser.DecentraalBeheerder"/>; those two cases build
/// the user explicitly via <c>WithPermissions</c> to attach the real
/// <see cref="DecentraalBodyRestriction"/>-restricted grant instead.
/// </summary>
public class BodyPermissionsCanDeleteAcrossRolesTests
{
    private static readonly Guid BodyId = Guid.NewGuid();

    [Fact]
    public void AlgemeenBeheerder_can_edit_and_delete_any_body()
    {
        var permissions = BodyPermissions.For(TestUser.AlgemeenBeheerder, BodyId);

        permissions.CanEdit.Should().BeTrue();
        permissions.CanDelete.Should().BeTrue();
    }

    [Fact]
    public void OrgaanBeheerder_can_edit_and_delete_any_body()
    {
        var permissions = BodyPermissions.For(TestUser.OrgaanBeheerder, BodyId);

        permissions.CanEdit.Should().BeTrue();
        permissions.CanDelete.Should().BeTrue();
    }

    [Fact]
    public void CjmBeheerder_can_edit_and_delete_any_body()
    {
        var permissions = BodyPermissions.For(TestUser.CjmBeheerder, BodyId);

        permissions.CanEdit.Should().BeTrue();
        permissions.CanDelete.Should().BeTrue();
    }

    [Fact]
    public void DecentraalBeheerder_can_edit_and_delete_a_body_in_their_own_or_child_organisation()
    {
        var user = new UserBuilder()
            .AsDecentraalBeheerder()
            .AddBodies(BodyId)
            .WithPermissions(
                PermissionSet.Of(
                    Permission.CanManageBodies.RestrictedTo(
                        DecentraalBodyRestriction.Instance)))
            .Build();

        var permissions = BodyPermissions.For(user, BodyId);

        permissions.CanEdit.Should().BeTrue();
        permissions.CanDelete.Should().BeTrue();
    }

    [Fact]
    public void DecentraalBeheerder_cannot_edit_or_delete_a_body_outside_their_own_or_child_organisation()
    {
        var user = new UserBuilder()
            .AsDecentraalBeheerder()
            .WithPermissions(
                PermissionSet.Of(
                    Permission.CanManageBodies.RestrictedTo(
                        DecentraalBodyRestriction.Instance)))
            .Build();

        var permissions = BodyPermissions.For(user, BodyId);

        permissions.CanEdit.Should().BeFalse();
        permissions.CanDelete.Should().BeFalse();
    }

    [Fact]
    public void VlimpersBeheerder_cannot_edit_or_delete_any_body()
    {
        var permissions = BodyPermissions.For(TestUser.VlimpersBeheerder, BodyId);

        permissions.CanEdit.Should().BeFalse();
        permissions.CanDelete.Should().BeFalse();
    }

    [Fact]
    public void RegelgevingBeheerder_cannot_edit_or_delete_any_body()
    {
        var permissions = BodyPermissions.For(TestUser.RegelgevingBeheerder, BodyId);

        permissions.CanEdit.Should().BeFalse();
        permissions.CanDelete.Should().BeFalse();
    }

    [Fact]
    public void VoMedewerker_cannot_edit_or_delete_any_body()
    {
        var permissions = BodyPermissions.For(TestUser.VoMedewerker, BodyId);

        permissions.CanEdit.Should().BeFalse();
        permissions.CanDelete.Should().BeFalse();
    }
}
