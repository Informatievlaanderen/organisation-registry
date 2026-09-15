namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;

/// <summary>
/// Convenience factories for common OrganisationFormalFramework restrictions.
/// </summary>
public static class FormalFrameworkRestrictions
{
    /// <summary>
    /// The formal framework id must be in the Vlimpers-owned set.
    /// </summary>
    public static IRestriction OwnedByVlimpers(IEnumerable<Guid> formalFrameworkIds)
        => new AllowListRestriction<FormalFrameworkContext>(formalFrameworkIds);

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
