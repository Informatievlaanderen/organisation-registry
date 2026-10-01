namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Context that carries the OVO number of the organisation an operation targets.
/// Used by restrictions that need to verify organisation-scoped access (e.g.
/// DecentraalBeheerder rights).
/// </summary>

public abstract record OrganisationContext : IRestrictionContext
{
    private OrganisationContext() { }

    public sealed record ByOvoNumber(string? OvoNumber) : OrganisationContext;

    public sealed record ById(Guid? OrganisationId) : OrganisationContext;

    public IEnumerable<Guid> RelevantIds => Enumerable.Empty<Guid>();
}
