namespace OrganisationRegistry.Infrastructure.Authorization;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using OrganisationRegistry.Infrastructure.Authorization.Restrictions;

/// <summary>
/// Immutable set of <see cref="PermissionEntry"/> grants. Value-equality by
/// contents. The internal language of the authorization layer after edge
/// translation from roles (edit-api, token-exchange) and scopes (client
/// credentials).
///
/// A grant is either unrestricted (bare permission) or restricted (permission
/// paired with an <see cref="IRestriction"/>). Evaluation is OR across grants:
/// a permission is satisfied for a context when <em>any</em> grant for that
/// permission applies. An unrestricted grant always applies, so it naturally
/// absorbs restricted grants for the same permission. AND within a single grant
/// is expressed with <see cref="CompositeAndRestriction"/>.
/// </summary>
public sealed class PermissionSet : IReadOnlyCollection<PermissionEntry>, IEquatable<PermissionSet>
{
    private readonly ImmutableHashSet<PermissionEntry> _entries;

    public static readonly PermissionSet Empty = new(ImmutableHashSet<PermissionEntry>.Empty);

    private PermissionSet(ImmutableHashSet<PermissionEntry> entries)
    {
        _entries = entries;
    }

    public static PermissionSet Of() => Empty;

    public static PermissionSet Of(params Permission[] permissions)
    {
        if (permissions is null || permissions.Length == 0)
            return Empty;

        var builder = ImmutableHashSet.CreateBuilder<PermissionEntry>();
        foreach (var p in permissions)
            builder.Add(p);
        return builder.Count == 0 ? Empty : new PermissionSet(builder.ToImmutable());
    }

    public static PermissionSet Of(IEnumerable<Permission>? permissions)
    {
        if (permissions is null)
            return Empty;

        var builder = ImmutableHashSet.CreateBuilder<PermissionEntry>();
        foreach (var p in permissions)
            builder.Add(p);
        return builder.Count == 0 ? Empty : new PermissionSet(builder.ToImmutable());
    }

    public static PermissionSet Of(params PermissionEntry[] entries)
    {
        if (entries is null || entries.Length == 0)
            return Empty;

        var set = ImmutableHashSet.CreateRange(entries);
        return set.IsEmpty ? Empty : new PermissionSet(set);
    }

    public static PermissionSet Of(IEnumerable<PermissionEntry>? entries)
    {
        if (entries is null)
            return Empty;

        var set = ImmutableHashSet.CreateRange(entries);
        return set.IsEmpty ? Empty : new PermissionSet(set);
    }

    public int Count => _entries.Count;

    /// <summary>
    /// True when any grant (restricted or unrestricted) is present for the
    /// permission. Controller-level gates use this to decide whether to let
    /// the request through; restriction enforcement happens further in.
    /// </summary>
    public bool Contains(Permission permission)
        => _entries.Any(e => e.Permission == permission);

    /// <summary>
    /// Core authorization decision. True when there is at least one grant for
    /// <paramref name="permission"/> that applies to <paramref name="context"/>.
    /// Evaluation is OR across grants: an unrestricted grant always applies (and
    /// so absorbs any restricted grant for the same permission); a restricted
    /// grant applies only when its <see cref="IRestriction"/> is satisfied by the
    /// context. An empty set — or a set without any grant for the permission —
    /// yields <c>false</c> (fail-closed).
    /// </summary>
    public bool IsSatisfiedFor(Permission permission, IRestrictionContext context)
        => _entries.Any(e =>
            e.Permission == permission &&
            (e.Restriction is null || e.Restriction.IsOkWith(context)));

    public PermissionSet Union(PermissionSet other)
    {
        if (other is null || other._entries.IsEmpty)
            return this;
        if (_entries.IsEmpty)
            return other;

        var union = _entries.Union(other._entries);
        return new PermissionSet(union);
    }

    public IEnumerator<PermissionEntry> GetEnumerator() => _entries.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Equals(PermissionSet? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _entries.SetEquals(other._entries);
    }

    public override bool Equals(object? obj) => Equals(obj as PermissionSet);

    public override int GetHashCode()
    {
        var hash = 0;
        foreach (var entry in _entries)
            hash ^= entry.GetHashCode();
        return hash;
    }

    public override string ToString()
        => _entries.IsEmpty
            ? "PermissionSet[]"
            : $"PermissionSet[{string.Join(", ", _entries.Select(e => e.ToString()).OrderBy(s => s))}]";
}
