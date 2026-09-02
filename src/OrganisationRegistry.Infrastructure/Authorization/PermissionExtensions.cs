namespace OrganisationRegistry.Infrastructure.Authorization;

using OrganisationRegistry.Infrastructure.Authorization.Restrictions;

/// <summary>
/// Fluent helpers for building restricted <see cref="PermissionEntry"/>
/// values at call sites such as <see cref="RolePermissionMap"/>.
/// </summary>
public static class PermissionExtensions
{
    /// <summary>
    /// Pairs a <see cref="Permission"/> with a restriction that the operation's
    /// <see cref="IRestrictionContext"/> must satisfy for the grant to apply.
    /// </summary>
    public static PermissionEntry RestrictedTo(
        this Permission permission,
        IRestriction restriction)
        => new(permission, restriction);
}
