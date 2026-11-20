namespace OrganisationRegistry.UnitTests.Backoffice.Body;

using System;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Body;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Tests.Shared;
using OrganisationRegistry.Tests.Shared.Stubs;
using Xunit;

public class BodyPermissionsCanManageContactsAcrossRolesTests
{
    private static readonly Guid BodyId = Guid.NewGuid();

    private static IUser UserWith(Role role)
        => new UserBuilder()
            .AddRoles(role)
            .AddBodies(BodyId)
            .WithPermissions(
                RolePermissionMap.For(new[] { role }, new OrganisationRegistryConfigurationStub()))
            .Build();

    [Fact]
    public void VoDecentraalBeheerder_cannot_manage_contacts_of_a_body_in_their_own_organisation()
    {
        var permissions = BodyPermissions.For(UserWith(Role.VoDecentraalBeheerder), BodyId);

        permissions.CanManageContacts.Should().BeFalse();

        // everything else stays identical to DecentraalBeheerder
        permissions.CanEdit.Should().BeTrue();
        permissions.CanDelete.Should().BeTrue();
        permissions.CanManageSeats.Should().BeTrue();
        permissions.CanManageMandates.Should().BeTrue();
        permissions.CanManageLifecycle.Should().BeTrue();
        permissions.CanManageOrganisations.Should().BeTrue();
        permissions.CanManageFormalFrameworks.Should().BeTrue();
        permissions.CanManageClassifications.Should().BeTrue();
        permissions.CanManageMep.Should().BeFalse();
    }

    [Fact]
    public void DecentraalBeheerder_can_manage_contacts_of_a_body_in_their_own_organisation()
        => BodyPermissions.For(UserWith(Role.DecentraalBeheerder), BodyId)
            .CanManageContacts.Should().BeTrue();

    [Fact]
    public void DecentraalBeheerder_cannot_manage_contacts_of_a_body_outside_their_own_organisation()
        => BodyPermissions.For(UserWith(Role.DecentraalBeheerder), Guid.NewGuid())
            .CanManageContacts.Should().BeFalse();

    [Theory]
    [InlineData(Role.AlgemeenBeheerder)]
    [InlineData(Role.OrgaanBeheerder)]
    public void Unrestricted_roles_can_manage_contacts_of_any_body(Role role)
        => BodyPermissions.For(UserWith(role), Guid.NewGuid())
            .CanManageContacts.Should().BeTrue();
}

