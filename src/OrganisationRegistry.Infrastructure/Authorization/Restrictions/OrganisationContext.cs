namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Context that carries the OVO number of the organisation an operation targets.
/// Used by restrictions that need to verify organisation-scoped access (e.g.
/// DecentraalBeheerder rights).
/// </summary>
public sealed record OrganisationContext(string OvoNumber) : IRestrictionContext
{
    public IEnumerable<Guid> RelevantIds => Enumerable.Empty<Guid>();
}
