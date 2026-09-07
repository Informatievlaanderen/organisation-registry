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
    public IEnumerable<Guid> RelevantIds => new[] { CapacityId };
}
