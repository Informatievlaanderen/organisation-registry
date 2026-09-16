namespace OrganisationRegistry.Infrastructure.Authorization;

/// <summary>
/// Closed set of technical capability identifiers used throughout the
/// authorization layer. Permission ids are PascalCase C# enum members;
/// they are the sole language of authorization checks after the edge
/// translation from roles and scopes.
/// </summary>
public enum Permission
{
    CanManageChildren,
    CanManageOrganisation,

    /// <summary>
    /// Create a new <em>top-level</em> organisation (one with no parent).
    /// Unrestricted-only: unlike <see cref="CanManageChildren"/> (which gates
    /// adding a daughter organisation under an existing parent, and is held
    /// as a restricted grant by VlimpersBeheerder/DecentraalBeheerder), there
    /// is no parent organisation to scope a restriction against, so this
    /// permission is granted only to AlgemeenBeheerder (and Developer).
    /// </summary>
    CanCreateOrganisations,
    CanManageContacts,
    CanManageFunctions,
    CanManageCapacities,
    CanManageLocations,
    CanManageBuildings,
    CanManageLabels,
    CanManageOrganisationClassifications,
    CanManageFormalFrameworks,
    CanManageKeys,
    CanManageRegulations,
    CanManageBodies,
    BodiesCanManageContacts,
    BodiesCanManageSeats,
    BodiesCanManageMandates,
    BodiesCanManageLifecycles,
    BodiesCanManageOrganisations,
    BodiesCanManageClassifications,
    BodiesCanManageFormalFrameworks,
    BodiesCanManageMep,
    CanManageRelations,
    CanManageKbo,
    CanManageVlimpers,

    // TODO check below
    CanImport,
    CanRunScheduledJobs,
    CanReadOrafin,

    /// <summary>Granted only to the <c>dv_organisatieregister_info</c> scope.</summary>
    CanReadInfoEndpoints,

    /// <summary>
    /// Read-only access to the configuration values endpoint
    /// (<c>ConfigurationController</c>). Granted to <see cref="Role.AlgemeenBeheerder"/>
    /// and <see cref="Role.Developer"/>, and to the <c>dv_organisatieregister_testclient</c>
    /// scope. Kept as a dedicated permission so future read-only roles can be
    /// granted visibility without also granting edit access.
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
    //CanEditOrganisationLabels,
    CanReadEvents,
    CanViewProjections,

    // Fine-grained parameter (master-data) permissions. Each parameter screen
    // has its own read/write (and optionally delete) permission so access can be
    // granted per parameter type in the future. Today only Role.AlgemeenBeheerder
    // holds them (see RolePermissionMap).
    ParametersLocationsRead,
    ParametersLocationsWrite,
    ParametersBuildingsRead,
    ParametersBuildingsWrite,
    ParametersInformationSystemsRead,
    ParametersInformationSystemsWrite,
    ParametersInformationSystemsDelete,
    ParametersOrganisationClassificationsRead,
    ParametersOrganisationClassificationsWrite,
    ParametersOrganisationClassificationTypesRead,
    ParametersOrganisationClassificationTypesWrite,
    ParametersBodyClassificationsRead,
    ParametersBodyClassificationsWrite,
    ParametersBodyClassificationTypesRead,
    ParametersBodyClassificationTypesWrite,
    ParametersOrganisationRelationTypesRead,
    ParametersOrganisationRelationTypesWrite,
    ParametersFormalFrameworksRead,
    ParametersFormalFrameworksWrite,
    ParametersFormalFrameworkCategoriesRead,
    ParametersFormalFrameworkCategoriesWrite,
    ParametersLifecyclePhaseTypesRead,
    ParametersLifecyclePhaseTypesWrite,
    ParametersCapacitiesRead,
    ParametersCapacitiesWrite,
    ParametersCapacitiesDelete,
    ParametersFunctionTypesRead,
    ParametersFunctionTypesWrite,
    ParametersContactTypesRead,
    ParametersContactTypesWrite,
    ParametersLabelTypesRead,
    ParametersLabelTypesWrite,
    ParametersPurposesRead,
    ParametersPurposesWrite,
    ParametersSeatTypesRead,
    ParametersSeatTypesWrite,
    ParametersMandateRoleTypesRead,
    ParametersMandateRoleTypesWrite,
    ParametersLocationTypesRead,
    ParametersLocationTypesWrite,
    ParametersRegulationThemesRead,
    ParametersRegulationThemesWrite,
    ParametersRegulationSubThemesRead,
    ParametersRegulationSubThemesWrite,
}
