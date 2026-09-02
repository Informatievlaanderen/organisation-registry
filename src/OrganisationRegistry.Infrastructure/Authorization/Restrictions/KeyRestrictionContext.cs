namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;

/// <summary>
/// Context for OrganisationKey operations. Carries the keytype ids that a
/// command touches so the KeyPolicy can decide against the caller's allowed
/// keytype set.
/// </summary>
public sealed record KeyRestrictionContext(IReadOnlyCollection<Guid> KeyTypeIds)
    : IRestrictionContext<KeyRestrictionContext>
{
    public KeyRestrictionContext(Guid keyTypeId) : this(new[] { keyTypeId }) { }

    public static string Domain => "OrganisationKeys";

    public IEnumerable<Guid> RelevantIds => KeyTypeIds;
}
