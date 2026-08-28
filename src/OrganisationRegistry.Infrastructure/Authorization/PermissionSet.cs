namespace OrganisationRegistry.Infrastructure.Authorization;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

/// <summary>
/// Immutable set of <see cref="Permission"/> values. Value-equality by contents.
/// The internal language of the authorization layer after edge translation
/// from roles (edit-api, token-exchange) and scopes (client credentials).
/// </summary>
public sealed class PermissionSet : IReadOnlyCollection<Permission>, IEquatable<PermissionSet>
{
    private readonly ImmutableHashSet<Permission> _permissions;

    public static readonly PermissionSet Empty = new(ImmutableHashSet<Permission>.Empty);

    private PermissionSet(ImmutableHashSet<Permission> permissions)
    {
        _permissions = permissions;
    }

    public static PermissionSet Of(params Permission[] permissions)
        => permissions is null || permissions.Length == 0
            ? Empty
            : new PermissionSet(ImmutableHashSet.CreateRange(permissions));

    public static PermissionSet Of(IEnumerable<Permission> permissions)
    {
        if (permissions is null)
            return Empty;

        var set = ImmutableHashSet.CreateRange(permissions);
        return set.IsEmpty ? Empty : new PermissionSet(set);
    }

    public int Count => _permissions.Count;

    public bool Contains(Permission permission) => _permissions.Contains(permission);

    public PermissionSet Union(PermissionSet other)
    {
        if (other is null || other._permissions.IsEmpty)
            return this;
        if (_permissions.IsEmpty)
            return other;

        var union = _permissions.Union(other._permissions);
        return new PermissionSet(union);
    }

    public IEnumerator<Permission> GetEnumerator() => _permissions.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Equals(PermissionSet? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _permissions.SetEquals(other._permissions);
    }

    public override bool Equals(object? obj) => Equals(obj as PermissionSet);

    public override int GetHashCode()
    {
        var hash = 0;
        foreach (var permission in _permissions)
            hash ^= permission.GetHashCode();
        return hash;
    }

    public override string ToString()
        => _permissions.IsEmpty
            ? "PermissionSet[]"
            : $"PermissionSet[{string.Join(", ", _permissions.OrderBy(p => p.ToString()))}]";
}
