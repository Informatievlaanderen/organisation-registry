namespace OrganisationRegistry.Infrastructure.Authorization.RolePermissions;

using Configuration;

internal sealed class VoDecentraalBeheerderPermissions : IRolePermissionProvider
{
    // VoDecentraalBeheerder: same static read-only view permissions as
    // DecentraalBeheerder (Functies, Hoedanigheden on both organisation
    // and people screens). The restricted CRUD grants are identical to
    // DecentraalBeheerder's too; neither role is granted CanManageContacts
    // (only AlgemeenBeheerder and Developer hold it).
    public PermissionSet Static { get; } = PermissionSet.Of(
        Permission.CanViewOrganisationCapacities,
        Permission.CanViewOrganisationFunctions,
        Permission.PeopleFunctionsRead,
        Permission.PeopleCapacitiesRead);

    public PermissionSet Restricted(IOrganisationRegistryConfiguration configuration)
        => DecentraalRestrictedGrants.For(configuration);
}
