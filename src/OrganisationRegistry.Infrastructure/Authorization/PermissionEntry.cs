namespace OrganisationRegistry.Infrastructure.Authorization;

using OrganisationRegistry.Infrastructure.Authorization.Restrictions;

/// <summary>
/// A single grant inside a <see cref="PermissionSet"/>. A grant is either
/// unrestricted (<see cref="Restriction"/> is <c>null</c>) or restricted
/// (paired with an <see cref="IRestriction"/> that the operation's context must
/// satisfy).
///
/// The record's structural equality lets <see cref="PermissionSet"/> use an
/// <see cref="System.Collections.Immutable.ImmutableHashSet{T}"/> for
/// deduplication and value-equality without extra plumbing.
/// </summary>
public sealed record PermissionEntry(
    Permission Permission,
    IRestriction? Restriction)
{
    /// <summary>
    /// Convenience projection so call sites and factories can pass a bare
    /// <see cref="Authorization.Permission"/> where a
    /// <see cref="PermissionEntry"/> is expected. The result is unrestricted.
    /// </summary>
    public static implicit operator PermissionEntry(Permission permission)
        => new(permission, null);

    public bool IsRestricted => Restriction is not null;
}
