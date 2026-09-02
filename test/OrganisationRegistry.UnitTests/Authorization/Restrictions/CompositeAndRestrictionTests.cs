namespace OrganisationRegistry.UnitTests.Authorization.Restrictions;

using System;
using System.Collections.Generic;
using FluentAssertions;
using OrganisationRegistry.Infrastructure.Authorization.Restrictions;
using Xunit;

public class CompositeAndRestrictionTests
{
    private static readonly KeyContext AnyContext = new(false, Array.Empty<Guid>());

    [Fact]
    public void Ok_when_all_components_pass()
    {
        var composite = new CompositeAndRestriction(AlwaysOk.Instance, AlwaysOk.Instance);

        composite.IsOkWith(AnyContext).Should().BeTrue();
    }

    [Fact]
    public void Not_ok_when_any_component_fails()
    {
        var composite = new CompositeAndRestriction(AlwaysOk.Instance, AlwaysDeny.Instance);

        composite.IsOkWith(AnyContext).Should().BeFalse();
    }

    [Fact]
    public void Empty_composite_yields_vacuous_truth()
    {
        var composite = new CompositeAndRestriction();

        composite.IsOkWith(AnyContext).Should().BeTrue();
    }

    [Fact]
    public void Null_component_enumerable_yields_vacuous_truth()
    {
        var composite = new CompositeAndRestriction((IEnumerable<IRestriction>)null!);

        composite.IsOkWith(AnyContext).Should().BeTrue();
    }

    [Fact]
    public void Equality_is_by_component_set_regardless_of_order()
    {
        var a = new StubTagged("a");
        var b = new StubTagged("b");

        var first = new CompositeAndRestriction(a, b);
        var second = new CompositeAndRestriction(b, a);

        first.Should().Be(second);
        first.GetHashCode().Should().Be(second.GetHashCode());
    }

    [Fact]
    public void Different_component_sets_are_not_equal()
    {
        var a = new StubTagged("a");
        var b = new StubTagged("b");
        var c = new StubTagged("c");

        var first = new CompositeAndRestriction(a, b);
        var second = new CompositeAndRestriction(a, c);

        first.Should().NotBe(second);
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

    private sealed record StubTagged(string Tag) : IRestriction
    {
        public bool IsOkWith(IRestrictionContext context) => true;
    }
}
