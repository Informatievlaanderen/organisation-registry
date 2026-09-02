namespace OrganisationRegistry.Infrastructure.Authorization;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Configuration;
using Restrictions;
using Microsoft.Extensions.Logging;

/// <summary>
/// Static translation from <see cref="Role"/> claims (edit-api and token-exchange
/// entry points) to the internal <see cref="PermissionSet"/> language.
///
/// Unknown / unmapped roles fail closed (return <see cref="PermissionSet.Empty"/>)
/// and emit a Serilog warning throttled to once per role per process.
/// </summary>
public static class RolePermissionMap
{
    private static readonly IReadOnlyDictionary<Role, PermissionSet> Map =
        new Dictionary<Role, PermissionSet>
        {
            [Role.AlgemeenBeheerder] = PermissionSet.Of(
                Permission.CanEditChildren,
                Permission.CanAddBodies,
                Permission.CanEditBodies,
                Permission.CanRegisterBodies,
                Permission.CanAddLocations,
                Permission.CanManageKeys,
                Permission.CanManageLabels,
                Permission.CanManageCapacities,
                Permission.CanManageFormalFrameworks,
                Permission.CanManageOrganisationClassifications,
                Permission.CanManageRegulations,
                Permission.CanImport,
                Permission.CanEditVlimpers,
                Permission.CanEditDelegations,
                Permission.CanReadConfiguration,
                Permission.CanEditOrganisationLabels),

            [Role.VlimpersBeheerder] = PermissionSet.Of(
                Permission.CanEditVlimpers,
                Permission.CanEditChildren,
                Permission.CanEditOrganisationLabels),

            [Role.DecentraalBeheerder] = PermissionSet.Of(
                Permission.CanEditChildren,
                Permission.CanAddLocations,
                Permission.CanAddBodies,
                Permission.CanEditBodies,
                Permission.CanEditDelegations,
                Permission.CanEditOrganisationLabels),

            [Role.OrgaanBeheerder] = PermissionSet.Of(
                Permission.CanAddBodies,
                Permission.CanEditBodies,
                Permission.CanRegisterBodies),

            [Role.RegelgevingBeheerder] = PermissionSet.Of(
                Permission.CanManageRegulations),

            [Role.CjmBeheerder] = PermissionSet.Of(
                Permission.CanAddBodies,
                Permission.CanEditBodies,
                Permission.CanEditOrganisationLabels),

            [Role.Orafin] = PermissionSet.Of(
                Permission.CanReadOrafin),

            [Role.Developer] = PermissionSet.Of(
                Permission.CanEditChildren,
                Permission.CanAddBodies,
                Permission.CanEditBodies,
                Permission.CanRegisterBodies,
                Permission.CanAddLocations,
                Permission.CanManageLabels,
                Permission.CanManageCapacities,
                Permission.CanManageFormalFrameworks,
                Permission.CanManageOrganisationClassifications,
                Permission.CanManageRegulations,
                Permission.CanImport,
                Permission.CanEditVlimpers,
                Permission.CanEditDelegations,
                Permission.CanReadConfiguration,
                Permission.CanEditOrganisationLabels,
                Permission.CanRunScheduledJobs),

            // Transitional: AutomatedTask keeps CanRunScheduledJobs until the
            // scheduled-job / sync services migrate to Client Credentials
            // (see WellknownUsers.ScheduledCommandsService / KboSyncService / Magda).
            // T036 will remove or [Obsolete] this once migration completes.
            [Role.AutomatedTask] = PermissionSet.Of(
                Permission.CanRunScheduledJobs),
        };

    private static readonly ConcurrentDictionary<Role, byte> LoggedUnknownRoles = new();

    public static PermissionSet For(Role role, ILogger? logger = null)
    {
        if (Map.TryGetValue(role, out var permissions))
            return permissions;

        if (LoggedUnknownRoles.TryAdd(role, 0))
            logger?.LogWarning(
                "Unknown Role {Role} encountered during permission translation; returning empty permission set (fail-closed).",
                role);

        return PermissionSet.Empty;
    }

    public static PermissionSet For(IEnumerable<Role>? roles, ILogger? logger = null)
    {
        if (roles is null)
            return PermissionSet.Empty;

        var union = PermissionSet.Empty;
        foreach (var role in roles)
            union = union.Union(For(role, logger));

        return union;
    }

    /// <summary>
    /// Config-aware translation. Layers data-driven restricted grants (whose
    /// restrictions depend on runtime configuration, e.g. the Vlimpers-allowed
    /// keytype ids) on top of the static role → permission mapping. Use this
    /// overload wherever a caller needs restriction enforcement (SecurityService);
    /// the config-less overload is fine where only permission presence matters.
    /// </summary>
    public static PermissionSet For(
        IEnumerable<Role>? roles,
        IOrganisationRegistryConfiguration configuration,
        ILogger? logger = null)
    {
        if (roles is null)
            return PermissionSet.Empty;

        var roleList = roles as IReadOnlyCollection<Role> ?? roles.ToList();
        var union = For(roleList, logger);

        foreach (var role in roleList)
            union = union.Union(RestrictedGrantsFor(role, configuration));

        return union;
    }

    /// <summary>
    /// Config-dependent restricted grants for a single role. Empty for roles
    /// whose access is fully expressed by the static map.
    /// </summary>
    private static PermissionSet RestrictedGrantsFor(
        Role role,
        IOrganisationRegistryConfiguration configuration)
        => role switch
        {
            Role.VlimpersBeheerder => PermissionSet.Of(
                Permission.CanManageKeys.RestrictedTo(
                    KeyRestrictions.VlimpersManaged(
                        configuration.Authorization.KeyIdsAllowedForVlimpers)),
                Permission.CanManageFormalFrameworks.RestrictedTo(
                    FormalFrameworkRestrictions.OwnedByVlimpers(
                        configuration.Authorization.FormalFrameworkIdsOwnedByVlimpers))),
            Role.DecentraalBeheerder => PermissionSet.Of(
                Permission.CanManageFormalFrameworks.RestrictedTo(
                    FormalFrameworkRestrictions.DecentraalOrganisationAndNotOwnedByVlimpers(
                        configuration.Authorization.FormalFrameworkIdsOwnedByVlimpers))),
            Role.RegelgevingBeheerder => PermissionSet.Of(
                Permission.CanManageFormalFrameworks.RestrictedTo(
                    FormalFrameworkRestrictions.OwnedByRegelgevingDb(
                        configuration.Authorization.FormalFrameworkIdsOwnedByRegelgevingDbBeheerder))),
            _ => PermissionSet.Empty,
        };

    /// <summary>Test-only: clears the unknown-role throttle memory.</summary>
    internal static void ResetThrottleState() => LoggedUnknownRoles.Clear();
}
