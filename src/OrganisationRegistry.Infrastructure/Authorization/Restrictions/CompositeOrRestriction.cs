namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

/// <summary>
/// Union of multiple restrictions for the same context. Used when a user
/// receives more than one restricted grant for the same permission from
/// different roles or scopes.
/// </summary>
public sealed class CompositeOrRestriction<TContext>
    : IRestriction<TContext>, IEquatable<CompositeOrRestriction<TContext>>
    where TContext : IRestrictionContext<TContext>
{
    private readonly ImmutableHashSet<IRestriction<TContext>> _restrictions;

    public CompositeOrRestriction(IEnumerable<IRestriction<TContext>>? restrictions)
    {
        _restrictions = restrictions is null
            ? ImmutableHashSet<IRestriction<TContext>>.Empty
            : ImmutableHashSet.CreateRange(restrictions);
    }

    public string Domain => TContext.Domain;

    public bool IsOkWith(TContext context)
        => _restrictions.Any(r => r.IsOkWith(context));

    public bool Equals(CompositeOrRestriction<TContext>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _restrictions.SetEquals(other._restrictions);
    }

    public override bool Equals(object? obj) => Equals(obj as CompositeOrRestriction<TContext>);

    public override int GetHashCode()
    {
        var hash = 0;
        foreach (var r in _restrictions)
            hash ^= r.GetHashCode();
        return hash;
    }

    public override string ToString()
        => $"CompositeOr<{typeof(TContext).Name}>[{_restrictions.Count}]";
}
