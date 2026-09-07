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
                Permission.CanManageChildren,
                Permission.CanManageContacts,
                Permission.CanManageFunctions,
                Permission.CanManageCapacities,
                Permission.CanManageLocations,
                Permission.CanManageBuildings,
                Permission.CanManageLabels,
                Permission.CanManageOrganisationClassifications,
                Permission.CanManageFormalFrameworks,
                Permission.CanManageKeys,
                Permission.CanManageRegulations,
                Permission.CanManageBodies,
                Permission.BodiesCanManageContacts,
                Permission.BodiesCanManageSeats,
                Permission.BodiesCanManageMandates,
                Permission.BodiesCanManageLifecycles,
                Permission.BodiesCanManageOrganisations,
                Permission.BodiesCanManageClassifications,
                Permission.BodiesCanManageFormalFrameworks,
                Permission.BodiesCanManageMep,
                Permission.CanManageRelations,
                Permission.CanManageKbo,
                Permission.CanManageVlimpers,

                Permission.CanImport,
                Permission.CanReadConfiguration),

            [Role.OrgaanBeheerder] = PermissionSet.Of(
                Permission.CanManageBodies,
                Permission.BodiesCanManageContacts,
                Permission.BodiesCanManageSeats,
                Permission.BodiesCanManageMandates,
                Permission.BodiesCanManageLifecycles,
                Permission.BodiesCanManageOrganisations,
                Permission.BodiesCanManageClassifications,
                Permission.BodiesCanManageFormalFrameworks,
                Permission.BodiesCanManageMep),

            [Role.VlimpersBeheerder] = PermissionSet.Of(
                Permission.CanManageChildren),

            // DecentraalBeheerder holds no unrestricted grants: every permission it
            // has (organisation-scoped edits and body management) is granted as a
            // data-driven restricted grant (own / child organisation or body) via
            // the config-aware overload (RestrictedGrantsFor).
            [Role.DecentraalBeheerder] = PermissionSet.Empty,

            [Role.RegelgevingBeheerder] = PermissionSet.Of(
                Permission.CanManageRegulations),

            [Role.CjmBeheerder] = PermissionSet.Of(
                Permission.CanManageRegulations,
                Permission.CanManageCapacities,
                Permission.CanManageLabels,
                Permission.CanManageBodies,
                Permission.BodiesCanManageContacts,
                Permission.BodiesCanManageSeats,
                Permission.BodiesCanManageMandates,
                Permission.BodiesCanManageLifecycles,
                Permission.BodiesCanManageOrganisations,
                Permission.BodiesCanManageClassifications,
                Permission.BodiesCanManageFormalFrameworks,
                Permission.BodiesCanManageMep),


            [Role.Orafin] = PermissionSet.Of(
                Permission.CanReadOrafin),

            [Role.Developer] = PermissionSet.Of(
                Permission.CanManageChildren,
                Permission.CanManageContacts,
                Permission.CanManageFunctions,
                Permission.CanManageCapacities,
                Permission.CanManageLocations,
                Permission.CanManageBuildings,
                Permission.CanManageLabels,
                Permission.CanManageOrganisationClassifications,
                Permission.CanManageFormalFrameworks,
                Permission.CanManageKeys,
                Permission.CanManageRegulations,
                Permission.CanManageBodies,
                Permission.BodiesCanManageContacts,
                Permission.BodiesCanManageSeats,
                Permission.BodiesCanManageMandates,
                Permission.BodiesCanManageLifecycles,
                Permission.BodiesCanManageOrganisations,
                Permission.BodiesCanManageClassifications,
                Permission.BodiesCanManageFormalFrameworks,
                Permission.BodiesCanManageMep,
                Permission.CanManageRelations,
                Permission.CanManageKbo,
                Permission.CanManageVlimpers,

                Permission.CanImport,
                Permission.CanRunScheduledJobs,
                Permission.CanReadConfiguration),

            // Transitional: AutomatedTask keeps CanRunScheduledJobs until the
            // scheduled-job / sync services migrate to Client Credentials
            // (see WellknownUsers.ScheduledCommandsService / KboSyncService / Magda).
            // T036 will remove or [Obsolete] this once migration completes.
            [Role.AutomatedTask] = PermissionSet.Of(
                Permission.CanManageCapacities,
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
                        configuration.Authorization.FormalFrameworkIdsOwnedByVlimpers)),
                Permission.CanManageLabels.RestrictedTo(
                    LabelRestrictions.VlimpersManaged(
                        configuration.Authorization.LabelIdsAllowedForVlimpers))),
            Role.DecentraalBeheerder => PermissionSet.Of(
                Permission.CanManageFunctions.RestrictedTo(
                    DecentraalOrganisationRestriction.Instance),
                Permission.CanManageLocations.RestrictedTo(
                    DecentraalOrganisationRestriction.Instance),
                Permission.CanManageBuildings.RestrictedTo(
                    DecentraalOrganisationRestriction.Instance),
                Permission.CanManageRelations.RestrictedTo(
                    DecentraalOrganisationRestriction.Instance),
                Permission.CanManageFormalFrameworks.RestrictedTo(
                    FormalFrameworkRestrictions.DecentraalOrganisationAndNotOwnedByVlimpers(
                        configuration.Authorization.FormalFrameworkIdsOwnedByVlimpers)),
                Permission.CanManageCapacities.RestrictedTo(
                    CapacityRestrictions.DecentraalOrganisationAndNotOwnedByRegelgevingDb(
                        configuration.Authorization.CapacityIdsOwnedByRegelgevingDbBeheerder)),
                Permission.CanManageOrganisationClassifications.RestrictedTo(
                    ClassificationTypeRestrictions.DecentraalOrganisationAndNotOwned(
                        configuration.Authorization.OrganisationClassificationTypeIdsOwnedByRegelgevingDbBeheerder,
                        configuration.Authorization.OrganisationClassificationTypeIdsOwnedByCjm)),
                // Own organisation labels are allowed, except Vlimpers-owned
                // labeltypes which are reserved for the VlimpersBeheerder.
                Permission.CanManageLabels.RestrictedTo(
                    LabelRestrictions.DecentraalOrganisationAndNotOwnedByVlimpers(
                        configuration.Authorization.LabelIdsAllowedForVlimpers)),
                // Body management: DecentraalBeheerder may only manage a body that
                // belongs to their own organisation or a child organisation
                // (i.e. one present in the user's Bodies). MEP-decreet
                // (BodiesCanManageMep) is intentionally never granted.
                Permission.CanManageBodies.RestrictedTo(
                    DecentraalBodyRestriction.Instance),
                Permission.BodiesCanManageContacts.RestrictedTo(
                    DecentraalBodyRestriction.Instance),
                Permission.BodiesCanManageSeats.RestrictedTo(
                    DecentraalBodyRestriction.Instance),
                Permission.BodiesCanManageMandates.RestrictedTo(
                    DecentraalBodyRestriction.Instance),
                Permission.BodiesCanManageLifecycles.RestrictedTo(
                    DecentraalBodyRestriction.Instance),
                Permission.BodiesCanManageOrganisations.RestrictedTo(
                    DecentraalBodyRestriction.Instance),
                Permission.BodiesCanManageClassifications.RestrictedTo(
                    DecentraalBodyRestriction.Instance),
                Permission.BodiesCanManageFormalFrameworks.RestrictedTo(
                    DecentraalBodyRestriction.Instance)),
            Role.RegelgevingBeheerder => PermissionSet.Of(
                Permission.CanManageFormalFrameworks.RestrictedTo(
                    FormalFrameworkRestrictions.OwnedByRegelgevingDb(
                        configuration.Authorization.FormalFrameworkIdsOwnedByRegelgevingDbBeheerder)),
                Permission.CanManageCapacities.RestrictedTo(
                    CapacityRestrictions.OwnedByRegelgevingDb(
                        configuration.Authorization.CapacityIdsOwnedByRegelgevingDbBeheerder)),
                Permission.CanManageOrganisationClassifications.RestrictedTo(
                    ClassificationTypeRestrictions.OwnedByRegelgevingDb(
                        configuration.Authorization.OrganisationClassificationTypeIdsOwnedByRegelgevingDbBeheerder))),
            Role.CjmBeheerder => PermissionSet.Of(
                Permission.CanManageOrganisationClassifications.RestrictedTo(
                    ClassificationTypeRestrictions.OwnedByCjm(
                        configuration.Authorization.OrganisationClassificationTypeIdsOwnedByCjm))),
            _ => PermissionSet.Empty,
        };

    /// <summary>Test-only: clears the unknown-role throttle memory.</summary>
    internal static void ResetThrottleState() => LoggedUnknownRoles.Clear();
}
