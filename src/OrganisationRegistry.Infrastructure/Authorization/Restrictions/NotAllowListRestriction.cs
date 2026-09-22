namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

/// <summary>
/// Rejects the operation when any relevant id in the supplied
/// <typeparamref name="TContext"/> is in the configured deny list. The opposite
/// of <see cref="AllowListRestriction{TContext}"/>: it expresses "everywhere
/// except these ids". An empty relevant-id set yields vacuous truth.
/// </summary>
public sealed class NotAllowListRestriction<TContext>
    : IRestriction, IEquatable<NotAllowListRestriction<TContext>>
    where TContext : IRestrictionContext
{
    private readonly ImmutableHashSet<Guid> _denied;

    public NotAllowListRestriction(IEnumerable<Guid>? denied)
    {
        _denied = denied is null
            ? ImmutableHashSet<Guid>.Empty
            : ImmutableHashSet.CreateRange(denied);
    }

    public bool IsOkWith(params IRestrictionContext[] contexts)
        => contexts.OfType<TContext>().FirstOrDefault() is { } typed &&
           !typed.RelevantIds.Any(_denied.Contains);

    public bool Equals(NotAllowListRestriction<TContext>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _denied.SetEquals(other._denied);
    }

    public override bool Equals(object? obj) => Equals(obj as NotAllowListRestriction<TContext>);

    public override int GetHashCode()
    {
        var hash = 0;
        foreach (var id in _denied)
            hash ^= id.GetHashCode();
        return hash;
    }

    public override string ToString()
        => $"NotAllowList<{typeof(TContext).Name}>[{_denied.Count} ids]";
}
