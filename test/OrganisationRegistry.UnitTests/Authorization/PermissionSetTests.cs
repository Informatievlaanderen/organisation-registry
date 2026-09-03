namespace OrganisationRegistry.UnitTests.Authorization;

using System.Linq;
using FluentAssertions;
using OrganisationRegistry.Infrastructure.Authorization;
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
        var a = PermissionSet.Of(Permission.CanManageBodies, Permission.CanManageBuildings);
        var b = PermissionSet.Of(Permission.CanManageBuildings, Permission.CanManageBodies);

        ((object)a).Should().Be(b);
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void Union_is_commutative_and_deduplicates()
    {
        var a = PermissionSet.Of(Permission.CanReadEvents, Permission.CanEditChildren);
        var b = PermissionSet.Of(Permission.CanEditChildren, Permission.CanManageBodies);

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
        set.ToList().Should().BeEquivalentTo(new[] { Permission.CanReadEvents, Permission.CanReadOrafin });
    }

    [Fact]
    public void Of_null_enumerable_returns_Empty()
    {
        PermissionSet.Of((System.Collections.Generic.IEnumerable<Permission>?)null!)
            .Should().BeSameAs(PermissionSet.Empty);
    }
}
