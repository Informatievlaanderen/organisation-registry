namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;

/// <summary>
/// Convenience factories for common OrganisationCapacity restrictions.
/// </summary>
public static class CapacityRestrictions
{
    /// <summary>
    /// The capacity id must be in the Regelgeving DB-owned set.
    /// </summary>
    public static IRestriction OwnedByRegelgevingDb(IEnumerable<Guid> capacityIds)
        => new AllowListRestriction<CapacityContext>(capacityIds);

    /// <summary>
    /// DecentraalBeheerder grant: own organisation and the capacity id must not
    /// be Regelgeving DB-owned.
    /// </summary>
    public static IRestriction DecentraalOrganisationAndNotOwnedByRegelgevingDb(IEnumerable<Guid> regelgevingDbCapacityIds)
        => new CompositeAndRestriction(
            DecentraalOrganisationRestriction.Instance,
            new NotAllowListRestriction<CapacityContext>(regelgevingDbCapacityIds));
}
