namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;

/// <summary>
/// Context for OrganisationFormalFramework operations. Carries the formal
/// framework id a command touches so the FormalFrameworkPolicy can decide
/// against the caller's allowed formal-framework set.
/// </summary>
public sealed record FormalFrameworkContext(Guid FormalFrameworkId) : IRestrictionContext
{
    public IEnumerable<Guid> RelevantIds => new[] { FormalFrameworkId };
}
