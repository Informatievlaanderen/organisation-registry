namespace OrganisationRegistry.UnitTests.Authorization.Restrictions;

using System;
using FluentAssertions;
using OrganisationRegistry.Infrastructure.Authorization.Restrictions;
using Xunit;

public class AllowListRestrictionTests
{
    private static readonly Guid A = Guid.NewGuid();
    private static readonly Guid B = Guid.NewGuid();
    private static readonly Guid C = Guid.NewGuid();

    [Fact]
    public void Ok_when_all_relevant_ids_are_allowed()
    {
        var restriction = new AllowListRestriction<KeyRestrictionContext>(new[] { A, B });

        restriction.IsOkWith(new KeyRestrictionContext(new[] { A, B })).Should().BeTrue();
        restriction.IsOkWith(new KeyRestrictionContext(A)).Should().BeTrue();
    }

    [Fact]
    public void Not_ok_when_any_relevant_id_is_missing()
    {
        var restriction = new AllowListRestriction<KeyRestrictionContext>(new[] { A, B });

        restriction.IsOkWith(new KeyRestrictionContext(new[] { A, C })).Should().BeFalse();
        restriction.IsOkWith(new KeyRestrictionContext(C)).Should().BeFalse();
    }

    [Fact]
    public void Empty_relevant_ids_yield_vacuous_truth()
    {
        var restriction = new AllowListRestriction<KeyRestrictionContext>(new[] { A });

        restriction.IsOkWith(new KeyRestrictionContext(Array.Empty<Guid>())).Should().BeTrue();
    }

    [Fact]
    public void Null_allowed_denies_any_non_empty_context()
    {
        var restriction = new AllowListRestriction<KeyRestrictionContext>(null);

        restriction.IsOkWith(new KeyRestrictionContext(A)).Should().BeFalse();
        restriction.IsOkWith(new KeyRestrictionContext(Array.Empty<Guid>())).Should().BeTrue();
    }

    [Fact]
    public void Domain_matches_context_domain()
    {
        var restriction = new AllowListRestriction<KeyRestrictionContext>(new[] { A });

        restriction.Domain.Should().Be(KeyRestrictionContext.Domain);
    }

    [Fact]
    public void Equality_is_by_allowed_contents_regardless_of_order()
    {
        var first = new AllowListRestriction<KeyRestrictionContext>(new[] { A, B });
        var second = new AllowListRestriction<KeyRestrictionContext>(new[] { B, A });

        first.Should().Be(second);
        first.GetHashCode().Should().Be(second.GetHashCode());
    }

    [Fact]
    public void Different_allowed_sets_are_not_equal()
    {
        var first = new AllowListRestriction<KeyRestrictionContext>(new[] { A, B });
        var second = new AllowListRestriction<KeyRestrictionContext>(new[] { A, C });

        first.Should().NotBe(second);
    }
}
