namespace OrganisationRegistry.Infrastructure.Authorization.RolePermissions;

using Configuration;

internal sealed class OrgaanBeheerderPermissions : IRolePermissionProvider
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
        Permission.BodiesCanManageMep,

        Permission.CanViewOrganisationFunctions,
        Permission.CanViewOrganisationCapacities,

        Permission.PeopleFunctionsRead,
        Permission.PeopleCapacitiesRead);

    public PermissionSet Restricted(IOrganisationRegistryConfiguration configuration)
        => PermissionSet.Empty;
}
