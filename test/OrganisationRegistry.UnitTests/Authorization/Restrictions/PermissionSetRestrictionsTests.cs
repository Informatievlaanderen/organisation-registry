namespace OrganisationRegistry.UnitTests.Authorization.Restrictions;

using System;
using FluentAssertions;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Authorization.Restrictions;
using Xunit;

public class PermissionSetRestrictionsTests
{
    private static readonly Guid A = Guid.NewGuid();
    private static readonly Guid B = Guid.NewGuid();
    private static readonly Guid C = Guid.NewGuid();

    [Fact]
    public void No_entries_for_domain_is_not_restricted_and_denies_all()
    {
        var set = PermissionSet.Of(Permission.CanManageKeys, Permission.CanReadEvents);

        set.IsRestrictedTo<KeyRestrictionContext>().Should().BeFalse();
        set.GetRestriction<KeyRestrictionContext>()
            .Should().BeSameAs(DenyAllRestriction<KeyRestrictionContext>.Instance);
    }

    [Fact]
    public void RestrictedTo_extension_stamps_the_context_domain()
    {
        var entry = Permission.CanManageKeys.RestrictedTo(KeyRestrictions.AllowList(new[] { A }));

        entry.Permission.Should().Be(Permission.CanManageKeys);
        entry.RestrictionDomain.Should().Be(KeyRestrictionContext.Domain);
        entry.IsRestricted.Should().BeTrue();
    }

    [Fact]
    public void Single_restricted_grant_is_returned_directly()
    {
        var allowList = KeyRestrictions.AllowList(new[] { A, B });
        var set = PermissionSet.Of(Permission.CanManageKeys.RestrictedTo(allowList));

        set.IsRestrictedTo<KeyRestrictionContext>().Should().BeTrue();

        var restriction = set.GetRestriction<KeyRestrictionContext>();
        restriction.Should().Be(allowList);
        restriction.IsOkWith(new KeyRestrictionContext(A)).Should().BeTrue();
        restriction.IsOkWith(new KeyRestrictionContext(C)).Should().BeFalse();
    }

    [Fact]
    public void Multiple_restricted_grants_combine_into_composite_or()
    {
        var set = PermissionSet.Of(
            Permission.CanManageKeys.RestrictedTo(KeyRestrictions.AllowList(new[] { A })),
            Permission.CanManageKeys.RestrictedTo(KeyRestrictions.AllowList(new[] { B })));

        set.IsRestrictedTo<KeyRestrictionContext>().Should().BeTrue();

        var restriction = set.GetRestriction<KeyRestrictionContext>();
        restriction.Should().BeOfType<CompositeOrRestriction<KeyRestrictionContext>>();
        restriction.IsOkWith(new KeyRestrictionContext(A)).Should().BeTrue();
        restriction.IsOkWith(new KeyRestrictionContext(B)).Should().BeTrue();
        restriction.IsOkWith(new KeyRestrictionContext(C)).Should().BeFalse();
    }

    [Fact]
    public void Unrestricted_grant_absorbs_restricted_grant_for_same_permission()
    {
        var set = PermissionSet.Of(
            Permission.CanManageKeys,
            Permission.CanManageKeys.RestrictedTo(KeyRestrictions.AllowList(new[] { A })));

        set.IsRestrictedTo<KeyRestrictionContext>().Should().BeFalse();
        set.GetRestriction<KeyRestrictionContext>()
            .Should().BeSameAs(UnrestrictedRestriction<KeyRestrictionContext>.Instance);
    }

    [Fact]
    public void Unrestricted_grant_for_other_permission_does_not_absorb()
    {
        var allowList = KeyRestrictions.AllowList(new[] { A });
        var set = PermissionSet.Of(
            Permission.CanManageLabels,
            Permission.CanManageKeys.RestrictedTo(allowList));

        set.IsRestrictedTo<KeyRestrictionContext>().Should().BeTrue();
        set.GetRestriction<KeyRestrictionContext>().Should().Be(allowList);
    }
}
