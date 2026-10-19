namespace OrganisationRegistry.Infrastructure.Authorization.RolePermissions;

using Configuration;
using Restrictions;

internal sealed class CjmBeheerderPermissions : IRolePermissionProvider
{
    public PermissionSet Static { get; } = PermissionSet.Of(
        Permission.CanManageBodies,
        Permission.BodiesCanManageContacts,
        Permission.BodiesCanManageSeats,
        Permission.BodiesCanManageMandates,
        Permission.BodiesCanManageLifecycles,
        Permission.BodiesCanManageOrganisations,
        Permission.BodiesCanManageClassifications,
        Permission.BodiesCanManageFormalFrameworks,
        Permission.BodiesCanManageMep);

    public PermissionSet Restricted(IOrganisationRegistryConfiguration configuration)
        => PermissionSet.Of(
            Permission.CanManageOrganisationClassifications.RestrictedTo(
                ClassificationTypeRestrictions.OwnedByCjm(
                    configuration.Authorization.OrganisationClassificationTypeIdsOwnedByCjm)));
}
