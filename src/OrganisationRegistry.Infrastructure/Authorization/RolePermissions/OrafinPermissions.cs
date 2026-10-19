namespace OrganisationRegistry.Infrastructure.Authorization.RolePermissions;

using Configuration;

internal sealed class OrafinPermissions : IRolePermissionProvider
{
    public PermissionSet Static { get; } = PermissionSet.Of(
        Permission.CanReadOrafin);

    public PermissionSet Restricted(IOrganisationRegistryConfiguration configuration)
        => PermissionSet.Empty;
}
