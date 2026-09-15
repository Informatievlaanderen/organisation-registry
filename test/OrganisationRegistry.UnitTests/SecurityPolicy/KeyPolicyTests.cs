namespace OrganisationRegistry.UnitTests.SecurityPolicy;

using System;
using AutoFixture;
using FluentAssertions;
using Handling.Authorization;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Authorization.Restrictions;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Xunit;

/// <summary>
/// KeyPolicy is role-independent: access is driven entirely by the
/// <see cref="Permission.CanManageKeys"/> grant and its (optional) restriction,
/// evaluated against a two-axis <see cref="KeyContext"/> — the organisation's
/// Vlimpers-management status and the keytype ids involved.
/// </summary>
public class KeyPolicyTests
{
    private readonly Fixture _fixture = new();

    private static User UserWith(PermissionSet permissions)
        => new UserBuilder().WithPermissions(permissions).Build();

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void UnrestrictedCanManageKeysPassesForAnyContext(bool isUnderVlimpersManagement)
    {
        var user = UserWith(PermissionSet.Of(Permission.CanManageKeys));

        var authorizationResult =
            new KeyPolicy(isUnderVlimpersManagement, _fixture.Create<Guid>())
                .Check(user);

        authorizationResult.Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void VlimpersManagedGrantPassesWhenUnderManagementAndKeyTypeAllowed()
    {
        var allowedKeyTypeId = _fixture.Create<Guid>();
        var user = UserWith(
            PermissionSet.Of(
                Permission.CanManageKeys.RestrictedTo(
                    KeyRestrictions.VlimpersManaged(new[] { allowedKeyTypeId }))));

        var authorizationResult =
            new KeyPolicy(isUnderVlimpersManagement: true, allowedKeyTypeId)
                .Check(user);

        authorizationResult.Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void VlimpersManagedGrantFailsWhenNotUnderManagement()
    {
        var allowedKeyTypeId = _fixture.Create<Guid>();
        var user = UserWith(
            PermissionSet.Of(
                Permission.CanManageKeys.RestrictedTo(
                    KeyRestrictions.VlimpersManaged(new[] { allowedKeyTypeId }))));

        var authorizationResult =
            new KeyPolicy(isUnderVlimpersManagement: false, allowedKeyTypeId)
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights<KeyPolicy>>();
    }

    [Fact]
    public void VlimpersManagedGrantFailsWhenKeyTypeNotAllowed()
    {
        var allowedKeyTypeId = _fixture.Create<Guid>();
        var otherKeyTypeId = _fixture.Create<Guid>();
        var user = UserWith(
            PermissionSet.Of(
                Permission.CanManageKeys.RestrictedTo(
                    KeyRestrictions.VlimpersManaged(new[] { allowedKeyTypeId }))));

        var authorizationResult =
            new KeyPolicy(isUnderVlimpersManagement: true, otherKeyTypeId)
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights<KeyPolicy>>();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WithoutCanManageKeysFails(bool isUnderVlimpersManagement)
    {
        var user = UserWith(PermissionSet.Empty);

        var authorizationResult =
            new KeyPolicy(isUnderVlimpersManagement, _fixture.Create<Guid>())
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights<KeyPolicy>>();
    }
}
