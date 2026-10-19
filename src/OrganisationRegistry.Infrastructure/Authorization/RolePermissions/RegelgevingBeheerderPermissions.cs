namespace OrganisationRegistry.Infrastructure.Authorization.RolePermissions;

using Configuration;
using Restrictions;

internal sealed class RegelgevingBeheerderPermissions : IRolePermissionProvider
{
    public PermissionSet Static { get; } = PermissionSet.Of(
        Permission.CanManageRegulations,

        Permission.CanViewOrganisationCapacities,
        Permission.CanViewOrganisationFunctions,
        Permission.CanViewOrganisationKbo,
        Permission.CanViewOrganisationVlimpers,

        Permission.PeopleFunctionsRead,
        Permission.PeopleCapacitiesRead);

    public PermissionSet Restricted(IOrganisationRegistryConfiguration configuration)
        => PermissionSet.Of(
            Permission.CanManageFormalFrameworks.RestrictedTo(
                FormalFrameworkRestrictions.OwnedByRegelgevingDb(
                    configuration.Authorization.FormalFrameworkIdsOwnedByRegelgevingDbBeheerder)),
            Permission.CanManageCapacities.RestrictedTo(
                CapacityRestrictions.OwnedByRegelgevingDb(
                    configuration.Authorization.CapacityIdsOwnedByRegelgevingDbBeheerder)),
            Permission.CanManageOrganisationClassifications.RestrictedTo(
                ClassificationTypeRestrictions.OwnedByRegelgevingDb(
                    configuration.Authorization.OrganisationClassificationTypeIdsOwnedByRegelgevingDbBeheerder)));
}
