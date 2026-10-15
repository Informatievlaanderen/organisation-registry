namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;

/// <summary>
/// Convenience factories for common OrganisationFormalFramework restrictions.
/// </summary>
public static class FormalFrameworkRestrictions
{
    /// <summary>
    /// Vlimpers grant for formal frameworks: the organisation must be under
    /// Vlimpers management <em>and</em> the touched formal framework id must be
    /// in the Vlimpers-allowed set. Both conditions live in a single grant
    /// (AND), mirroring <see cref="KeyRestrictions.VlimpersManaged"/> and
    /// <see cref="LabelRestrictions.VlimpersManaged"/>. Without the
    /// Vlimpers-management gate, a VlimpersBeheerder could manage a
    /// Vlimpers-owned formal framework on an organisation that has never been
    /// under Vlimpers management.
    /// </summary>
    public static IRestriction VlimpersManaged(IEnumerable<Guid> formalFrameworkIds)
        => new CompositeAndRestriction(
            RequireUnderVlimpersManagementRestriction.Instance,
            new AllowListRestriction<FormalFrameworkContext>(formalFrameworkIds));

    /// <summary>
    /// The formal framework id must be in the Regelgeving DB-owned set.
    /// </summary>
    public static IRestriction OwnedByRegelgevingDb(IEnumerable<Guid> formalFrameworkIds)
        => new AllowListRestriction<FormalFrameworkContext>(formalFrameworkIds);

    /// <summary>
    /// DecentraalBeheerder grant: the organisation must be in the user's
    /// Decentraal list and the formal framework id must not be Vlimpers-owned.
    /// </summary>
    public static IRestriction DecentraalOrganisationAndNotOwnedByVlimpers(IEnumerable<Guid> vlimpersFormalFrameworkIds)
        => new CompositeAndRestriction(
            DecentraalOrganisationRestriction.Instance,
            new NotAllowListRestriction<FormalFrameworkContext>(vlimpersFormalFrameworkIds));
}
