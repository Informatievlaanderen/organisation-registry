namespace OrganisationRegistry.Infrastructure.Authorization.RolePermissions;

using Configuration;

internal sealed class DecentraalBeheerderPermissions : IRolePermissionProvider
{
    // DecentraalBeheerder holds no unrestricted grants for organisation-scoped
    // edits and body management (every such permission is a data-driven
    // restricted grant). The Personen read screens (Functies/Hoedanigheden)
    // are not organisation-scoped, so they are granted statically here.
    public PermissionSet Static { get; } = PermissionSet.Of(
        Permission.CanViewOrganisationCapacities,
        Permission.CanViewOrganisationFunctions,
        Permission.PeopleFunctionsRead,
        Permission.PeopleCapacitiesRead);

    public PermissionSet Restricted(IOrganisationRegistryConfiguration configuration)
        => DecentraalRestrictedGrants.For(configuration);
}
