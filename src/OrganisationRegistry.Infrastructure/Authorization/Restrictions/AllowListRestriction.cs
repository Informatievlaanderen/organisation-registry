namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

/// <summary>
/// Allows the operation when every <see cref="IRestrictionContext{TSelf}.RelevantIds"/>
/// id is in the configured allow list. An empty relevant-id set yields vacuous truth,
/// mirroring "no ids to check means nothing to reject".
/// </summary>
public sealed class AllowListRestriction<TContext>
    : IRestriction<TContext>, IEquatable<AllowListRestriction<TContext>>
    where TContext : IRestrictionContext<TContext>
{
    private readonly ImmutableHashSet<Guid> _allowed;

    public AllowListRestriction(IEnumerable<Guid>? allowed)
    {
        _allowed = allowed is null
            ? ImmutableHashSet<Guid>.Empty
            : ImmutableHashSet.CreateRange(allowed);
    }

    public string Domain => TContext.Domain;

    public bool IsOkWith(TContext context)
    {
        if (context is null) return false;
        return context.RelevantIds.All(_allowed.Contains);
    }

    public bool Equals(AllowListRestriction<TContext>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _allowed.SetEquals(other._allowed);
    }

    public override bool Equals(object? obj) => Equals(obj as AllowListRestriction<TContext>);

    public override int GetHashCode()
    {
        var hash = 0;
        foreach (var id in _allowed)
            hash ^= id.GetHashCode();
        return hash;
    }

    public override string ToString()
        => $"AllowList<{typeof(TContext).Name}>[{_allowed.Count} ids]";
}
