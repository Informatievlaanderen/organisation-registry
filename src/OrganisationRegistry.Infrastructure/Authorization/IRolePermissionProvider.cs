namespace OrganisationRegistry.Infrastructure.Authorization;

using Configuration;

/// <summary>
/// Provides the complete permission picture for a single role:
/// static (unrestricted) permissions and config-dependent restricted grants.
/// </summary>
public interface IRolePermissionProvider
{
    PermissionSet Static { get; }
    PermissionSet Restricted(IOrganisationRegistryConfiguration configuration);
}
