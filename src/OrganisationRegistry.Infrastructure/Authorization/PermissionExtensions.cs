namespace OrganisationRegistry.Infrastructure.Authorization;

using OrganisationRegistry.Infrastructure.Authorization.Restrictions;

/// <summary>
/// Fluent helpers for building restricted <see cref="PermissionEntry"/>
/// values at call sites such as <see cref="RolePermissionMap"/>.
/// </summary>
public static class PermissionExtensions
{
    /// <summary>
    /// Pairs a <see cref="Permission"/> with a typed restriction. The domain
    /// on the entry is taken from the context type via
    /// <see cref="IRestrictionContext{TSelf}.Domain"/> so it stays in sync
    /// with the restriction itself.
    /// </summary>
    public static PermissionEntry RestrictedTo<TContext>(
        this Permission permission,
        IRestriction<TContext> restriction)
        where TContext : IRestrictionContext<TContext>
        => new(permission, TContext.Domain, restriction);
}
