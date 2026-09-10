namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;

/// <summary>
/// Context for OrganisationCapacity operations. Carries the capacity id a command
/// touches so the CapacityPolicy can decide against the caller's allowed capacity
/// set (e.g. the Regelgeving-owned capacities).
/// </summary>
public sealed record CapacityContext(Guid CapacityId) : IRestrictionContext
{
    private readonly IReadOnlyCollection<Guid>? _capacityIds;

    /// <summary>
    /// Multi-id / "any" overload used for organisation-level permission summaries:
    /// an empty collection yields an empty <see cref="RelevantIds"/> (vacuous truth).
    /// </summary>
    public CapacityContext(IReadOnlyCollection<Guid> capacityIds) : this(Guid.Empty)
        => _capacityIds = capacityIds;

    public IEnumerable<Guid> RelevantIds => _capacityIds ?? new[] { CapacityId };
}
