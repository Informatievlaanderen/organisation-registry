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
        var restriction = new AllowListRestriction<KeyContext>(new[] { A, B });

        restriction.IsOkWith(new KeyContext(false, new[] { A, B })).Should().BeTrue();
        restriction.IsOkWith(new KeyContext(false, A)).Should().BeTrue();
    }

    [Fact]
    public void Not_ok_when_any_relevant_id_is_missing()
    {
        var restriction = new AllowListRestriction<KeyContext>(new[] { A, B });

        restriction.IsOkWith(new KeyContext(false, new[] { A, C })).Should().BeFalse();
        restriction.IsOkWith(new KeyContext(false, C)).Should().BeFalse();
    }

    [Fact]
    public void Empty_relevant_ids_yield_vacuous_truth()
    {
        var restriction = new AllowListRestriction<KeyContext>(new[] { A });

        restriction.IsOkWith(new KeyContext(false, Array.Empty<Guid>())).Should().BeTrue();
    }

    [Fact]
    public void Null_allowed_denies_any_non_empty_context()
    {
        var restriction = new AllowListRestriction<KeyContext>(null);

        restriction.IsOkWith(new KeyContext(false, A)).Should().BeFalse();
        restriction.IsOkWith(new KeyContext(false, Array.Empty<Guid>())).Should().BeTrue();
    }

    [Fact]
    public void Fails_closed_for_wrong_context_type()
    {
        var restriction = new AllowListRestriction<KeyContext>(new[] { A });

        restriction.IsOkWith(new OtherContext(A)).Should().BeFalse();
    }

    [Fact]
    public void Finds_matching_context_among_multiple_contexts()
    {
        var restriction = new AllowListRestriction<KeyContext>(new[] { A });

        restriction.IsOkWith(
                new OtherContext(C),
                new KeyContext(false, A))
            .Should().BeTrue();
    }

    [Fact]
    public void Fails_closed_when_matching_context_is_missing_among_multiple_contexts()
    {
        var restriction = new AllowListRestriction<KeyContext>(new[] { A });

        restriction.IsOkWith(
                new OtherContext(A),
                new OtherContext(C))
            .Should().BeFalse();
    }

    [Fact]
    public void Equality_is_by_allowed_contents_regardless_of_order()
    {
        var first = new AllowListRestriction<KeyContext>(new[] { A, B });
        var second = new AllowListRestriction<KeyContext>(new[] { B, A });

        first.Should().Be(second);
        first.GetHashCode().Should().Be(second.GetHashCode());
    }

    [Fact]
    public void Different_allowed_sets_are_not_equal()
    {
        var first = new AllowListRestriction<KeyContext>(new[] { A, B });
        var second = new AllowListRestriction<KeyContext>(new[] { A, C });

        first.Should().NotBe(second);
    }

    private sealed record OtherContext(Guid Id) : IRestrictionContext
    {
        public System.Collections.Generic.IEnumerable<Guid> RelevantIds => new[] { Id };
    }
}
