namespace OrganisationRegistry.Infrastructure.Authorization.RolePermissions;

using Configuration;

internal sealed class VoMedewerkerPermissions : IRolePermissionProvider
{
    // VoMedewerker: read-only role, no scoping. Conceptually "Publiek +
    // read access to Functies and Hoedanigheden" (both on organisations
    // and on people). Holds no CanManage*/CanCreate*/CanTerminate*/
    // Parameters*Write/Delete/Delegations*/System/CanImport permission —
    // every screen not covered below is read-only via the (currently
    // ungated) list/detail GET endpoints open to any authenticated
    // backoffice user, same as every other role. See
    // ui-permission-matrix.md for the full rights table.
    public PermissionSet Static { get; } = PermissionSet.Of(
        Permission.PeopleFunctionsRead,
        Permission.PeopleCapacitiesRead,
        Permission.CanViewOrganisationFunctions,
        Permission.CanViewOrganisationCapacities);

    public PermissionSet Restricted(IOrganisationRegistryConfiguration configuration)
        => PermissionSet.Empty;
}
