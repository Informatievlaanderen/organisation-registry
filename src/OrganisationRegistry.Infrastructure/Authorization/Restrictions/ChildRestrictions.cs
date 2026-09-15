namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

/// <summary>
/// Convenience factories for the <see cref="Permission.CanManageChildren"/>
/// restrictions, i.e. who may manage an organisation's parent/child structure.
/// Keeps call sites readable, e.g.
/// <c>Permission.CanManageChildren.RestrictedTo(ChildRestrictions.UnderVlimpersManagement)</c>.
///
/// Mirrors the historical role-based <c>VlimpersPolicy</c>: a VlimpersBeheerder may
/// manage children of an organisation that is under Vlimpers management, whereas a
/// DecentraalBeheerder may manage children of their own organisation as long as it
/// is <em>not</em> under Vlimpers management.
/// </summary>
public static class ChildRestrictions
{
    /// <summary>
    /// Vlimpers grant: the target organisation must be under Vlimpers management.
    /// </summary>
    public static IRestriction UnderVlimpersManagement
        => RequireUnderVlimpersManagementRestriction.Instance;

    /// <summary>
    /// Decentraal grant: the caller must be a DecentraalBeheerder for the target
    /// organisation <em>and</em> that organisation must not be under Vlimpers
    /// management (Vlimpers-managed organisations are reserved for the
    /// VlimpersBeheerder). Both conditions live in a single grant (AND).
    /// </summary>
    public static IRestriction DecentraalAndNotUnderVlimpersManagement
        => new CompositeAndRestriction(
            DecentraalOrganisationRestriction.Instance,
            RequireNotUnderVlimpersManagementRestriction.Instance);
}
