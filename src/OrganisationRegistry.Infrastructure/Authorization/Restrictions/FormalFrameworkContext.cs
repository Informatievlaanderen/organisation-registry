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
    private readonly IReadOnlyCollection<Guid>? _formalFrameworkIds;

    /// <summary>
    /// Multi-id / "any" overload used for organisation-level permission summaries:
    /// an empty collection yields an empty <see cref="RelevantIds"/> (vacuous truth).
    /// </summary>
    public FormalFrameworkContext(IReadOnlyCollection<Guid> formalFrameworkIds) : this(Guid.Empty)
        => _formalFrameworkIds = formalFrameworkIds;

    public IEnumerable<Guid> RelevantIds => _formalFrameworkIds ?? new[] { FormalFrameworkId };
}
