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
    /// Vlimpers grant for capacities: the organisation must be under Vlimpers
    /// management <em>and</em> every touched capacity must be in the
    /// Vlimpers-allowed set (LA/Leidinggevend topkader and HRBP). Mirrors
    /// <see cref="LabelRestrictions.VlimpersManaged"/>.
    /// </summary>
    public static IRestriction VlimpersManaged(IEnumerable<Guid> capacityIds)
        => new CompositeAndRestriction(
            RequireUnderVlimpersManagementRestriction.Instance,
            new AllowListRestriction<CapacityContext>(capacityIds));

    /// <summary>
    /// DecentraalBeheerder grant: own organisation and the capacity id must not
    /// be Regelgeving DB-owned.
    /// </summary>
    public static IRestriction DecentraalOrganisationAndNotOwnedByRegelgevingDb(IEnumerable<Guid> regelgevingDbCapacityIds)
        => new CompositeAndRestriction(
            DecentraalOrganisationRestriction.Instance,
            new NotAllowListRestriction<CapacityContext>(regelgevingDbCapacityIds));
}
