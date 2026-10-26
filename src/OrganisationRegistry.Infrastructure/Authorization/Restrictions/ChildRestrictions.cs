namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

/// <summary>
/// Convenience factories for the <see cref="Permission.CanManageChildren"/> and
/// <see cref="Permission.CanManageParent"/> restrictions, i.e. who may manage an
/// organisation's parent/child structure.
/// Keeps call sites readable, e.g.
/// <c>Permission.CanManageChildren.RestrictedTo(ChildRestrictions.UnderVlimpersManagement)</c>.
///
/// A VlimpersBeheerder may manage the parent/child structure of an organisation
/// that is under Vlimpers management. A DecentraalBeheerder's grants use
/// <see cref="DecentraalOrganisationRestriction"/> directly: their own organisation
/// (or one in their scope), regardless of Vlimpers management.
/// </summary>
public static class ChildRestrictions
{
    /// <summary>
    /// Vlimpers grant: the target organisation must be under Vlimpers management.
    /// </summary>
    public static IRestriction UnderVlimpersManagement
        => RequireUnderVlimpersManagementRestriction.Instance;
}
