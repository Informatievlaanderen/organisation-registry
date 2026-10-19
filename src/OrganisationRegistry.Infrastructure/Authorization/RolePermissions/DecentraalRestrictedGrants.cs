namespace OrganisationRegistry.Infrastructure.Authorization.RolePermissions;

using Configuration;
using Restrictions;

internal static class DecentraalRestrictedGrants
{
    public static PermissionSet For(IOrganisationRegistryConfiguration configuration)
        => PermissionSet.Of(
            // DecentraalBeheerder may edit every organisation-info field
            // except the four Vlimpers-reserved ones (gated by the
            // dedicated CanManageOrganisationInfoLimitedToVlimpers
            // permission, which DecentraalBeheerder never holds), for
            // their own organisation, regardless of Vlimpers-management
            // status — these fields are never Vlimpers-reserved.
            Permission.CanManageOrganisationInfoNotLimitedToVlimpers.RestrictedTo(
                DecentraalOrganisationRestriction.Instance),
            Permission.CanManageChildren.RestrictedTo(
                ChildRestrictions.DecentraalAndNotUnderVlimpersManagement),
            Permission.CanManageParent.RestrictedTo(
                ChildRestrictions.DecentraalAndNotUnderVlimpersManagement),
            Permission.CanManageFunctions.RestrictedTo(
                DecentraalOrganisationRestriction.Instance),
            Permission.CanManageLocations.RestrictedTo(
                DecentraalOrganisationRestriction.Instance),
            Permission.CanManageBuildings.RestrictedTo(
                DecentraalOrganisationRestriction.Instance),
            Permission.CanManageRelations.RestrictedTo(
                DecentraalOrganisationRestriction.Instance),
            Permission.CanManageFormalFrameworks.RestrictedTo(
                FormalFrameworkRestrictions.DecentraalOrganisationAndNotOwnedByVlimpers(
                    configuration.Authorization.FormalFrameworkIdsOwnedByVlimpers)),
            Permission.CanManageCapacities.RestrictedTo(
                CapacityRestrictions.DecentraalOrganisationAndNotOwnedByRegelgevingDb(
                    configuration.Authorization.CapacityIdsOwnedByRegelgevingDbBeheerder)),
            Permission.CanManageOrganisationClassifications.RestrictedTo(
                ClassificationTypeRestrictions.DecentraalOrganisationAndNotOwned(
                    configuration.Authorization.OrganisationClassificationTypeIdsOwnedByRegelgevingDbBeheerder)),
            // Own organisation labels are allowed, except Vlimpers-owned
            // labeltypes which are reserved for the VlimpersBeheerder.
            Permission.CanManageLabels.RestrictedTo(
                LabelRestrictions.DecentraalOrganisationAndNotOwnedByVlimpers(
                    configuration.Authorization.LabelIdsAllowedForVlimpers)),
            // Body management: DecentraalBeheerder may only manage a body that
            // belongs to their own organisation or a child organisation
            // (i.e. one present in the user's Bodies). MEP-decreet
            // (BodiesCanManageMep) is intentionally never granted.
            Permission.CanManageBodies.RestrictedTo(
                DecentraalBodyRestriction.Instance),
            // Registering a new body targets an organisation (the body
            // doesn't exist yet, so there is no BodyContext to check
            // against). This second grant for the same permission lets
            // RegisterBodyPolicy's OrganisationContext check succeed for
            // the user's own organisation or a child organisation in scope.
            Permission.CanManageBodies.RestrictedTo(
                DecentraalOrganisationRestriction.Instance),
            Permission.BodiesCanManageContacts.RestrictedTo(
                DecentraalBodyRestriction.Instance),
            Permission.BodiesCanManageSeats.RestrictedTo(
                DecentraalBodyRestriction.Instance),
            Permission.BodiesCanManageMandates.RestrictedTo(
                DecentraalBodyRestriction.Instance),
            Permission.BodiesCanManageLifecycles.RestrictedTo(
                DecentraalBodyRestriction.Instance),
            Permission.BodiesCanManageOrganisations.RestrictedTo(
                DecentraalBodyRestriction.Instance),
            Permission.BodiesCanManageClassifications.RestrictedTo(
                DecentraalBodyRestriction.Instance),
            Permission.BodiesCanManageFormalFrameworks.RestrictedTo(
                DecentraalBodyRestriction.Instance));
}
