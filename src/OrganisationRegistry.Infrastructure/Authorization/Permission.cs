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

    /// <summary>
    /// Create or update a <c>Persoon</c> (<c>PersonDetailCommandController</c>).
    /// Granted only to AlgemeenBeheerder (and Developer). There is no delete
    /// permission: people can never be deleted via the API. Reading a person
    /// (<c>PersonDetailController</c>/<c>PersonListController</c>) requires no
    /// permission — it is open to any caller, including unauthenticated ones.
    /// </summary>
    PeopleWrite,

    /// <summary>
    /// Read a person's functions (<c>PersonFunctionController</c>). Granted to
    /// every backoffice role; unauthenticated ("Publiek") callers are rejected.
    /// There is no corresponding write permission: functions are only ever
    /// managed from the organisation side (<see cref="CanManageFunctions"/>).
    /// </summary>
    PeopleFunctionsRead,

    /// <summary>
    /// Read a person's capacities (<c>PersonCapacityController</c>). Granted to
    /// every backoffice role; unauthenticated ("Publiek") callers are rejected.
    /// There is no corresponding write permission: capacities are only ever
    /// managed from the organisation side (<see cref="CanManageCapacities"/>).
    /// </summary>
    PeopleCapacitiesRead,

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

    /// <summary>
    /// Single, coarse-grained "Systeem" permission gating the Statistieken,
    /// Events (<c>EventsController</c>) and Stopgezet-in-KBO
    /// (<c>OrganisationKboController.Get</c>, <c>kbo/terminated</c>) admin
    /// screens. Deliberately not split per screen (see
    /// <c>ui-permission-matrix.md</c>): all three are read-only, operational/
    /// diagnostic views intended for <see cref="Role.AlgemeenBeheerder"/> (and
    /// <see cref="Role.Developer"/>) only. Every other role — including
    /// CjmBeheerder/Orafin, which previously had role-based access to
    /// <c>kbo/terminated</c> — gets 403.
    /// </summary>
    System,
    CanViewProjections,

    // Fine-grained parameter (master-data) permissions. Each parameter screen
    // has its own write (and optionally delete) permission so access can be
    // granted per parameter type in the future. Today only Role.AlgemeenBeheerder
    // holds them (see RolePermissionMap). Reading these master-data lists is not
    // gated by a dedicated permission: the list endpoints only require the
    // default BackofficeUser policy (any authenticated backoffice user), and
    // some also accept the corresponding CanManage* permission so a scoped
    // manager can still populate its own dropdowns (see the relevant *Controller).
    ParametersLocationsWrite,
    ParametersBuildingsWrite,
    ParametersInformationSystemsWrite,
    ParametersInformationSystemsDelete,
    ParametersOrganisationClassificationsWrite,
    ParametersOrganisationClassificationTypesWrite,
    ParametersBodyClassificationsWrite,
    ParametersBodyClassificationTypesWrite,
    ParametersOrganisationRelationTypesWrite,
    ParametersFormalFrameworksWrite,
    ParametersFormalFrameworkCategoriesWrite,
    ParametersLifecyclePhaseTypesWrite,
    ParametersCapacitiesWrite,
    ParametersCapacitiesDelete,
    ParametersFunctionTypesWrite,
    ParametersContactTypesWrite,
    ParametersLabelTypesWrite,
    ParametersPurposesWrite,
    ParametersSeatTypesWrite,
    ParametersMandateRoleTypesWrite,
    ParametersLocationTypesWrite,
    ParametersRegulationThemesWrite,
    ParametersRegulationSubThemesWrite,
}
