namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;

/// <summary>
/// Convenience factory for common OrganisationKey restrictions. Keeps call
/// sites readable: <c>Permission.CanManageKeys.RestrictedTo(KeyRestrictions.AllowList(ids))</c>.
/// </summary>
public static class KeyRestrictions
{
    public static AllowListRestriction<KeyRestrictionContext> AllowList(IEnumerable<Guid> keyTypeIds)
        => new(keyTypeIds);
}
