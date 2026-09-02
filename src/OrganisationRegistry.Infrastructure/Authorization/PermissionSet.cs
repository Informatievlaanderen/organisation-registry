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
/// paired with an <see cref="IRestriction"/> for a specific domain). Union
/// semantics across roles/scopes: an unrestricted grant for a permission
/// absorbs any restricted grants for the same permission when queried through
/// <see cref="IsRestrictedTo{TContext}"/> and <see cref="GetRestriction{TContext}"/>.
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
    /// True when the caller has at least one restricted grant for the given
    /// context's domain <em>and</em> no unrestricted grant that would absorb
    /// those restrictions. Handler policies use this to skip restriction
    /// enforcement when the user is allowed to do anything.
    /// </summary>
    public bool IsRestrictedTo<TContext>()
        where TContext : IRestrictionContext<TContext>
    {
        var domain = TContext.Domain;
        var hasRestricted = false;
        var restrictedPermissions = new HashSet<Permission>();

        foreach (var entry in _entries)
        {
            if (entry.RestrictionDomain != domain)
                continue;
            hasRestricted = true;
            restrictedPermissions.Add(entry.Permission);
        }

        if (!hasRestricted)
            return false;

        // If any of the restricted permissions is also granted unrestricted
        // (anywhere in the set), the unrestricted grant absorbs the restriction.
        foreach (var entry in _entries)
        {
            if (entry.RestrictionDomain is null &&
                restrictedPermissions.Contains(entry.Permission))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Resolves the effective restriction for a context domain. Fail-closed:
    /// missing entries return <see cref="DenyAllRestriction{TContext}"/>.
    /// If any unrestricted grant exists for a restricted permission in this
    /// domain, returns <see cref="UnrestrictedRestriction{TContext}"/>. A
    /// single typed restriction is returned directly; multiple are combined
    /// via <see cref="CompositeOrRestriction{TContext}"/>.
    /// </summary>
    public IRestriction<TContext> GetRestriction<TContext>()
        where TContext : IRestrictionContext<TContext>
    {
        var domain = TContext.Domain;
        var typed = new List<IRestriction<TContext>>();
        var restrictedPermissions = new HashSet<Permission>();

        foreach (var entry in _entries)
        {
            if (entry.RestrictionDomain != domain)
                continue;
            restrictedPermissions.Add(entry.Permission);
            if (entry.Restriction is IRestriction<TContext> t)
                typed.Add(t);
        }

        if (typed.Count == 0)
            return DenyAllRestriction<TContext>.Instance;

        // Unrestricted absorbs restricted for the same permission.
        foreach (var entry in _entries)
        {
            if (entry.RestrictionDomain is null &&
                restrictedPermissions.Contains(entry.Permission))
                return UnrestrictedRestriction<TContext>.Instance;
        }

        return typed.Count == 1
            ? typed[0]
            : new CompositeOrRestriction<TContext>(typed);
    }

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
