namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

/// <summary>
/// Conjunction of restrictions within a single grant: the context must satisfy
/// <em>every</em> component restriction. This is the AND half of the restriction
/// algebra — OR is expressed as separate grants (entries) in a
/// <see cref="PermissionSet"/>, which are unioned at evaluation time.
///
/// Order-insensitive value equality lets two grants built from the same
/// components compare equal regardless of the order they were supplied.
/// </summary>
public sealed class CompositeAndRestriction
    : IRestriction, IEquatable<CompositeAndRestriction>
{
    private readonly ImmutableHashSet<IRestriction> _restrictions;

    public CompositeAndRestriction(params IRestriction[] restrictions)
        : this((IEnumerable<IRestriction>)restrictions) { }

    public CompositeAndRestriction(IEnumerable<IRestriction> restrictions)
    {
        _restrictions = restrictions is null
            ? ImmutableHashSet<IRestriction>.Empty
            : ImmutableHashSet.CreateRange(restrictions);
    }

    public bool IsOkWith(IRestrictionContext context)
        => _restrictions.All(r => r.IsOkWith(context));

    public bool Equals(CompositeAndRestriction? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _restrictions.SetEquals(other._restrictions);
    }

    public override bool Equals(object? obj) => Equals(obj as CompositeAndRestriction);

    public override int GetHashCode()
    {
        var hash = 0;
        foreach (var restriction in _restrictions)
            hash ^= restriction.GetHashCode();
        return hash;
    }

    public override string ToString()
        => $"CompositeAnd[{string.Join(" AND ", _restrictions.Select(r => r.ToString()))}]";
}
