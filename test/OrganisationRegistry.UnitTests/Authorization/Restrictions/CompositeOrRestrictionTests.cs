namespace OrganisationRegistry.UnitTests.Authorization.Restrictions;

using System;
using System.Collections.Generic;
using FluentAssertions;
using OrganisationRegistry.Infrastructure.Authorization.Restrictions;
using Xunit;

public class CompositeOrRestrictionTests
{
    private static readonly Guid A = Guid.NewGuid();
    private static readonly Guid B = Guid.NewGuid();
    private static readonly Guid C = Guid.NewGuid();

    [Fact]
    public void Ok_when_any_member_restriction_is_ok()
    {
        var composite = new CompositeOrRestriction<KeyRestrictionContext>(
            new IRestriction<KeyRestrictionContext>[]
            {
                new AllowListRestriction<KeyRestrictionContext>(new[] { A }),
                new AllowListRestriction<KeyRestrictionContext>(new[] { B }),
            });

        composite.IsOkWith(new KeyRestrictionContext(A)).Should().BeTrue();
        composite.IsOkWith(new KeyRestrictionContext(B)).Should().BeTrue();
    }

    [Fact]
    public void Not_ok_when_no_member_restriction_is_ok()
    {
        var composite = new CompositeOrRestriction<KeyRestrictionContext>(
            new IRestriction<KeyRestrictionContext>[]
            {
                new AllowListRestriction<KeyRestrictionContext>(new[] { A }),
                new AllowListRestriction<KeyRestrictionContext>(new[] { B }),
            });

        composite.IsOkWith(new KeyRestrictionContext(C)).Should().BeFalse();
    }

    [Fact]
    public void Unrestricted_member_makes_composite_allow_everything()
    {
        var composite = new CompositeOrRestriction<KeyRestrictionContext>(
            new IRestriction<KeyRestrictionContext>[]
            {
                new AllowListRestriction<KeyRestrictionContext>(new[] { A }),
                UnrestrictedRestriction<KeyRestrictionContext>.Instance,
            });

        composite.IsOkWith(new KeyRestrictionContext(C)).Should().BeTrue();
    }

    [Fact]
    public void Empty_composite_denies()
    {
        var composite = new CompositeOrRestriction<KeyRestrictionContext>(
            Array.Empty<IRestriction<KeyRestrictionContext>>());

        composite.IsOkWith(new KeyRestrictionContext(A)).Should().BeFalse();
    }

    [Fact]
    public void Null_members_deny()
    {
        var composite = new CompositeOrRestriction<KeyRestrictionContext>(null);

        composite.IsOkWith(new KeyRestrictionContext(A)).Should().BeFalse();
    }

    [Fact]
    public void Domain_matches_context_domain()
    {
        var composite = new CompositeOrRestriction<KeyRestrictionContext>(
            new List<IRestriction<KeyRestrictionContext>>
            {
                new AllowListRestriction<KeyRestrictionContext>(new[] { A }),
            });

        composite.Domain.Should().Be(KeyRestrictionContext.Domain);
    }

    [Fact]
    public void Equality_is_by_member_contents_regardless_of_order()
    {
        var one = new AllowListRestriction<KeyRestrictionContext>(new[] { A });
        var two = new AllowListRestriction<KeyRestrictionContext>(new[] { B });

        var first = new CompositeOrRestriction<KeyRestrictionContext>(
            new IRestriction<KeyRestrictionContext>[] { one, two });
        var second = new CompositeOrRestriction<KeyRestrictionContext>(
            new IRestriction<KeyRestrictionContext>[] { two, one });

        first.Should().Be(second);
        first.GetHashCode().Should().Be(second.GetHashCode());
    }
}
