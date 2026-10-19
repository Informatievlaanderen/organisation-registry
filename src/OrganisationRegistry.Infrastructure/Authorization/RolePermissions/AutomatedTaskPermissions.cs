namespace OrganisationRegistry.Infrastructure.Authorization.RolePermissions;

using Configuration;

internal sealed class AutomatedTaskPermissions : IRolePermissionProvider
{
    // Transitional: AutomatedTask keeps CanRunScheduledJobs until the
    // scheduled-job / sync services migrate to Client Credentials
    // (see WellknownUsers.ScheduledCommandsService / KboSyncService / Magda).
    // T036 will remove or [Obsolete] this once migration completes.
    public PermissionSet Static { get; } = PermissionSet.Of(
        Permission.CanViewOrganisationCapacities,
        Permission.CanManageCapacities,
        Permission.CanRunScheduledJobs,
        Permission.CanManageKeys,
        Permission.CanManageKbo);

    public PermissionSet Restricted(IOrganisationRegistryConfiguration configuration)
        => PermissionSet.Empty;
}
