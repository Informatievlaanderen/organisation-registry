namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Context that carries the id of the body an operation targets. Used by
/// restrictions that need to verify body-scoped access (e.g. a
/// DecentraalBeheerder managing a body that belongs to their own organisation
/// or a child organisation).
/// </summary>
public sealed record BodyContext(Guid BodyId) : IRestrictionContext
{
    public IEnumerable<Guid> RelevantIds => Enumerable.Empty<Guid>();
}
