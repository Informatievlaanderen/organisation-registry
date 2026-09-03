namespace OrganisationRegistry.Infrastructure.Authorization;

/// <summary>
/// Closed set of technical capability identifiers used throughout the
/// authorization layer. Permission ids are PascalCase C# enum members;
/// they are the sole language of authorization checks after the edge
/// translation from roles and scopes.
/// </summary>
public enum Permission
{
    CanEditChildren,
    CanEditVlimpers,
    CanEditDelegations,
    CanManageContacts,
    CanManageKeys,
    CanManageLabels,
    CanManageCapacities,
    CanManageFormalFrameworks,
    CanManageOrganisationClassifications,
    CanManageRegulations,
    CanManageFunctions,
    CanManageLocations,
    CanManageBuildings,
    CanManageRelations,
    CanManageBodies,
    CanImport,
    CanRunScheduledJobs,
    CanReadOrafin,

    /// <summary>Granted only to the <c>dv_organisatieregister_info</c> scope.</summary>
    CanReadInfoEndpoints,

    /// <summary>
    /// Read-only access to the configuration values endpoint
    /// (<c>ConfigurationController</c>). Granted to <see cref="Role.AlgemeenBeheerder"/>
    /// and <see cref="Role.Developer"/>, and to the <c>dv_organisatieregister_testclient</c>
    /// scope. Kept as a dedicated permission (rather than gating on
    /// <see cref="CanEditAll"/>) so future read-only roles can be granted
    /// visibility without also granting edit access.
    /// </summary>
    CanReadConfiguration,

    /// <summary>
    /// Add or update labels on an individual organisation
    /// (<c>OrganisationLabelCommandController</c>). Granted to
    /// <see cref="Role.AlgemeenBeheerder"/>, <see cref="Role.CjmBeheerder"/>,
    /// <see cref="Role.VlimpersBeheerder"/>, and <see cref="Role.DecentraalBeheerder"/>.
    /// Distinct from <see cref="CanManageLabels"/>, which gates master-data
    /// label-type administration and remains AB-only. Resource-level
    /// scope restrictions (e.g. Vlimpers-typed labels, own-organisation)
    /// are still enforced by <c>LabelPolicy</c> in the domain handler; this
    /// permission only opens the controller-level general check.
    /// </summary>
    CanEditOrganisationLabels,
    CanReadEvents,
    CanViewProjections
}
