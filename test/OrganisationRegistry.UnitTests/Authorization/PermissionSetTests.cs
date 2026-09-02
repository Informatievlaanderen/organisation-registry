namespace OrganisationRegistry.UnitTests.Authorization;

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Authorization.Restrictions;
using Xunit;


// all permissions here picked at random, no meaning
public class PermissionSetTests
{
    [Fact]
    public void Empty_has_no_permissions()
    {
        PermissionSet.Empty.Count.Should().Be(0);
    }

    [Fact]
    public void Empty_is_singleton_reference()
    {
        PermissionSet.Of().Should().BeSameAs(PermissionSet.Empty);
        PermissionSet.Of(System.Array.Empty<Permission>()).Should().BeSameAs(PermissionSet.Empty);
    }

    [Fact]
    public void Of_deduplicates_permissions()
    {
        var set = PermissionSet.Of(
            Permission.CanReadEvents,
            Permission.CanReadEvents,
            Permission.CanEditChildren);

        set.Count.Should().Be(2);
        set.Contains(Permission.CanReadEvents).Should().BeTrue();
        set.Contains(Permission.CanEditChildren).Should().BeTrue();
    }

    [Fact]
    public void Equality_is_by_contents_not_reference()
    {
        var a = PermissionSet.Of(Permission.CanAddBodies, Permission.CanEditBodies);
        var b = PermissionSet.Of(Permission.CanEditBodies, Permission.CanAddBodies);

        ((object)a).Should().Be(b);
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void Union_is_commutative_and_deduplicates()
    {
        var a = PermissionSet.Of(Permission.CanReadEvents, Permission.CanEditChildren);
        var b = PermissionSet.Of(Permission.CanEditChildren, Permission.CanAddBodies);

        var ab = a.Union(b);
        var ba = b.Union(a);

        ((object)ab).Should().Be(ba);
        ab.Count.Should().Be(3);
    }

    [Fact]
    public void Union_with_Empty_returns_original_instance()
    {
        var a = PermissionSet.Of(Permission.CanReadEvents);
        a.Union(PermissionSet.Empty).Should().BeSameAs(a);
        PermissionSet.Empty.Union(a).Should().BeSameAs(a);
    }

    [Fact]
    public void Enumeration_yields_all_permissions()
    {
        var set = PermissionSet.Of(Permission.CanReadEvents, Permission.CanReadOrafin);
        set.Select(e => e.Permission).Should()
            .BeEquivalentTo(new[] { Permission.CanReadEvents, Permission.CanReadOrafin });
    }

    [Fact]
    public void Of_null_enumerable_returns_Empty()
    {
        PermissionSet.Of((System.Collections.Generic.IEnumerable<Permission>?)null!)
            .Should().BeSameAs(PermissionSet.Empty);
    }

    [Fact]
    public void IsSatisfiedFor_returns_false_on_empty_set()
    {
        PermissionSet.Empty
            .IsSatisfiedFor(Permission.CanManageKeys, StubContext.Empty)
            .Should().BeFalse();
    }

    [Fact]
    public void IsSatisfiedFor_returns_false_when_permission_is_missing()
    {
        var set = PermissionSet.Of(Permission.CanReadEvents);

        set.IsSatisfiedFor(Permission.CanManageKeys, StubContext.Empty).Should().BeFalse();
    }

    [Fact]
    public void Unrestricted_grant_satisfies_any_context()
    {
        var set = PermissionSet.Of(Permission.CanManageKeys);

        set.IsSatisfiedFor(Permission.CanManageKeys, StubContext.Empty).Should().BeTrue();
        set.IsSatisfiedFor(Permission.CanManageKeys, StubContext.AlwaysDenied).Should().BeTrue();
    }

    [Fact]
    public void Restricted_grant_defers_to_restriction()
    {
        var okSet = PermissionSet.Of(
            Permission.CanManageKeys.RestrictedTo(AlwaysOk.Instance));
        var denySet = PermissionSet.Of(
            Permission.CanManageKeys.RestrictedTo(AlwaysDeny.Instance));

        okSet.IsSatisfiedFor(Permission.CanManageKeys, StubContext.Empty).Should().BeTrue();
        denySet.IsSatisfiedFor(Permission.CanManageKeys, StubContext.Empty).Should().BeFalse();
    }

    [Fact]
    public void Unrestricted_grant_absorbs_restricted_grant_for_same_permission()
    {
        var set = PermissionSet.Of(
            (PermissionEntry)Permission.CanManageKeys,
            Permission.CanManageKeys.RestrictedTo(AlwaysDeny.Instance));

        set.IsSatisfiedFor(Permission.CanManageKeys, StubContext.Empty).Should().BeTrue();
    }

    [Fact]
    public void Multiple_restricted_grants_for_same_permission_or_together()
    {
        var set = PermissionSet.Of(
            Permission.CanManageKeys.RestrictedTo(AlwaysDeny.Instance),
            Permission.CanManageKeys.RestrictedTo(AlwaysOk.Instance));

        set.IsSatisfiedFor(Permission.CanManageKeys, StubContext.Empty).Should().BeTrue();
    }

    private sealed class AlwaysOk : IRestriction
    {
        public static readonly AlwaysOk Instance = new();
        public bool IsOkWith(IRestrictionContext context) => true;
    }

    private sealed class AlwaysDeny : IRestriction
    {
        public static readonly AlwaysDeny Instance = new();
        public bool IsOkWith(IRestrictionContext context) => false;
    }

    private sealed class StubContext : IRestrictionContext
    {
        public static readonly StubContext Empty = new();
        public static readonly StubContext AlwaysDenied = new();
        public IEnumerable<Guid> RelevantIds => Array.Empty<Guid>();
    }
}
