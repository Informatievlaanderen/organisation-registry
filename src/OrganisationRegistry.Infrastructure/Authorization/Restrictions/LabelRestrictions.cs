namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;

/// <summary>
/// Convenience factories for common OrganisationLabel restrictions. Mirrors
/// <see cref="KeyRestrictions"/> and <see cref="FormalFrameworkRestrictions"/>:
/// a DecentraalBeheerder may edit labels on their own organisation, but never a
/// Vlimpers-owned labeltype.
/// </summary>
public static class LabelRestrictions
{
    /// <summary>
    /// Vlimpers grant for labels: the organisation must be under Vlimpers
    /// management <em>and</em> every touched labeltype must be in the
    /// Vlimpers-allowed set.
    /// </summary>
    public static IRestriction VlimpersManaged(IEnumerable<Guid> labelTypeIds)
        => new CompositeAndRestriction(
            RequireUnderVlimpersManagementRestriction.Instance,
            new AllowListRestriction<LabelContext>(labelTypeIds));

    /// <summary>
    /// DecentraalBeheerder grant: own organisation and none of the touched
    /// labeltypes is Vlimpers-owned. Vlimpers-owned labels are reserved for the
    /// VlimpersBeheerder and can never be edited by a DecentraalBeheerder,
    /// regardless of the organisation's Vlimpers-management status. Mirrors
    /// <see cref="FormalFrameworkRestrictions.DecentraalOrganisationAndNotOwnedByVlimpers"/>.
    /// </summary>
    public static IRestriction DecentraalOrganisationAndNotOwnedByVlimpers(IEnumerable<Guid> vlimpersLabelTypeIds)
        => new CompositeAndRestriction(
            DecentraalOrganisationRestriction.Instance,
            new NotAllowListRestriction<LabelContext>(vlimpersLabelTypeIds));
}
