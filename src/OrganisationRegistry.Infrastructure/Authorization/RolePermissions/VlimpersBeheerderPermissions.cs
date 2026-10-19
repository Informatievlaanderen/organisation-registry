namespace OrganisationRegistry.Infrastructure.Authorization.RolePermissions;

using Configuration;
using Restrictions;

internal sealed class VlimpersBeheerderPermissions : IRolePermissionProvider
{
    // VlimpersBeheerder holds no unrestricted grant for CanImport — importing
    // is only permitted for organisations under Vlimpers management. The Personen
    // read screens (Functies/Hoedanigheden) are not organisation-scoped, so they
    // are granted statically here. Every other scoped permission is granted as a
    // restricted permission below.
    public PermissionSet Static { get; } = PermissionSet.Of(
        Permission.CanViewOrganisationFunctions,
        Permission.CanViewOrganisationCapacities,

        Permission.PeopleFunctionsRead,
        Permission.PeopleCapacitiesRead);

    public PermissionSet Restricted(IOrganisationRegistryConfiguration configuration)
        => PermissionSet.Of(
            // The four Vlimpers-reserved fields (formele naam, formele korte
            // naam, lidwoord, operationele geldigheid) are gated by a
            // dedicated permission, not CanManageOrganisation (which
            // VlimpersBeheerder no longer holds at all), so that
            // DecentraalBeheerder can never reach them.
            Permission.CanManageOrganisationInfoLimitedToVlimpers.RestrictedTo(
                ChildRestrictions.UnderVlimpersManagement),
            Permission.CanManageChildren.RestrictedTo(
                ChildRestrictions.UnderVlimpersManagement),
            Permission.CanManageParent.RestrictedTo(
                ChildRestrictions.UnderVlimpersManagement),
            Permission.CanImport.RestrictedTo(
                ChildRestrictions.UnderVlimpersManagement),
            Permission.CanManageKeys.RestrictedTo(
                KeyRestrictions.VlimpersManaged(
                    configuration.Authorization.KeyIdsAllowedForVlimpers)),
            Permission.CanManageFormalFrameworks.RestrictedTo(
                FormalFrameworkRestrictions.VlimpersManaged(
                    configuration.Authorization.FormalFrameworkIdsOwnedByVlimpers)),
            Permission.CanManageLabels.RestrictedTo(
                LabelRestrictions.VlimpersManaged(
                    configuration.Authorization.LabelIdsAllowedForVlimpers)));
}
