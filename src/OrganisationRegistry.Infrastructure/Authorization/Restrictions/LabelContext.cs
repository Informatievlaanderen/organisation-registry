namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;

/// <summary>
/// Context for OrganisationLabel operations. Carries the labeltype ids a command
/// touches and whether the target organisation is under Vlimpers management, so
/// the LabelPolicy can decide against the caller's allowed labeltype set and the
/// Vlimpers-management gate in one pass. Mirrors <see cref="KeyContext"/>.
/// </summary>
public sealed record LabelContext(bool IsUnderVlimpersManagement, IReadOnlyCollection<Guid> LabelTypeIds)
    : IVlimpersManagedContext
{
    public LabelContext(bool isUnderVlimpersManagement, Guid labelTypeId)
        : this(isUnderVlimpersManagement, new[] { labelTypeId }) { }

    public IEnumerable<Guid> RelevantIds => LabelTypeIds;
}
