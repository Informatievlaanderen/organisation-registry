namespace OrganisationRegistry.Infrastructure.Authorization.RolePermissions;

using Configuration;

internal sealed class DeveloperPermissions : IRolePermissionProvider
{
    public PermissionSet Static { get; } = PermissionSet.Of(
        Permission.CanViewOrganisationFunctions,
        Permission.CanViewOrganisationCapacities,
        Permission.CanManageChildren,
        Permission.CanManageParent,
        Permission.CanManageOrganisation,
        Permission.CanManageOrganisationInfoLimitedToVlimpers,
        Permission.CanManageOrganisationInfoNotLimitedToVlimpers,
        Permission.CanTerminateOrganisation,
        Permission.CanCreateOrganisations,
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
        Permission.CanViewOrganisationKbo,
        Permission.CanViewOrganisationVlimpers,

        Permission.CanImport,
        Permission.CanRunScheduledJobs,
        Permission.CanReadConfiguration,
        Permission.System,

        Permission.PeopleWrite,
        Permission.PeopleFunctionsRead,
        Permission.PeopleCapacitiesRead,

        Permission.ParametersLocationsWrite,
        Permission.ParametersBuildingsWrite,
        Permission.ParametersInformationSystemsWrite,
        Permission.ParametersInformationSystemsDelete,
        Permission.ParametersOrganisationClassificationsWrite,
        Permission.ParametersOrganisationClassificationTypesWrite,
        Permission.ParametersBodyClassificationsWrite,
        Permission.ParametersBodyClassificationTypesWrite,
        Permission.ParametersOrganisationRelationTypesWrite,
        Permission.ParametersFormalFrameworksWrite,
        Permission.ParametersFormalFrameworkCategoriesWrite,
        Permission.ParametersLifecyclePhaseTypesWrite,
        Permission.ParametersCapacitiesWrite,
        Permission.ParametersCapacitiesDelete,
        Permission.ParametersFunctionTypesWrite,
        Permission.ParametersContactTypesWrite,
        Permission.ParametersLabelTypesWrite,
        Permission.ParametersPurposesWrite,
        Permission.ParametersSeatTypesWrite,
        Permission.ParametersMandateRoleTypesWrite,
        Permission.ParametersLocationTypesWrite,
        Permission.ParametersRegulationThemesWrite,
        Permission.ParametersRegulationSubThemesWrite,

        Permission.DelegationsRead,
        Permission.DelegationsWrite,
        Permission.DelegationsDelete,
        Permission.DelegationsCreate,
        Permission.CanAssignManualIdentifiers);

    public PermissionSet Restricted(IOrganisationRegistryConfiguration configuration)
        => PermissionSet.Empty;
}
