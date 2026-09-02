namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;

/// <summary>
/// Context for OrganisationKey operations. Carries the keytype ids a command
/// touches and whether the target organisation is under Vlimpers management, so
/// the KeyPolicy can decide against the caller's allowed keytype set and the
/// Vlimpers-management gate in one pass.
/// </summary>
public sealed record KeyContext(bool IsUnderVlimpersManagement, IReadOnlyCollection<Guid> KeyTypeIds)
    : IVlimpersManagedContext
{
    public KeyContext(bool isUnderVlimpersManagement, Guid keyTypeId)
        : this(isUnderVlimpersManagement, new[] { keyTypeId }) { }

    public IEnumerable<Guid> RelevantIds => KeyTypeIds;
}
