namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;

/// <summary>
/// Context for OrganisationCapacity operations. Carries the capacity ids a
/// command touches and whether the target organisation is under Vlimpers
/// management, so the CapacityPolicy can decide against the caller's allowed
/// capacity set and the Vlimpers-management gate in one pass. Mirrors
/// <see cref="LabelContext"/> and <see cref="KeyContext"/>.
/// </summary>
public sealed record CapacityContext(bool IsUnderVlimpersManagement, IReadOnlyCollection<Guid> CapacityIds)
    : IVlimpersManagedContext
{
    public CapacityContext(bool isUnderVlimpersManagement, Guid capacityId)
        : this(isUnderVlimpersManagement, new[] { capacityId }) { }

    public IEnumerable<Guid> RelevantIds => CapacityIds;
}
