namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Carries the <see cref="IUser"/> into restriction evaluation so that
/// user-specific rules (e.g. DecentraalBeheerder organisation ownership) can be
/// expressed as restrictions rather than leaking back into policies.
/// </summary>
public sealed record UserContext(IUser User) : IRestrictionContext
{
    public IEnumerable<Guid> RelevantIds => Enumerable.Empty<Guid>();
}
