namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;

/// <summary>
/// Convenience factories for common OrganisationKey restrictions. Keeps call
/// sites readable, e.g.
/// <c>Permission.CanManageKeys.RestrictedTo(KeyRestrictions.VlimpersManaged(ids))</c>.
/// </summary>
public static class KeyRestrictions
{
    /// <summary>
    /// The keytype ids the caller may touch must all be in <paramref name="keyTypeIds"/>.
    /// </summary>
    public static IRestriction AllowList(IEnumerable<Guid> keyTypeIds)
        => new AllowListRestriction<KeyContext>(keyTypeIds);

    /// <summary>
    /// Vlimpers grant for keys: the organisation must be under Vlimpers management
    /// <em>and</em> every touched keytype must be in the Vlimpers-allowed set. Both
    /// conditions live in a single grant (AND); other roles express their own key
    /// access as separate grants (OR).
    /// </summary>
    public static IRestriction VlimpersManaged(IEnumerable<Guid> keyTypeIds)
        => new CompositeAndRestriction(
            RequireUnderVlimpersManagementRestriction.Instance,
            new AllowListRestriction<KeyContext>(keyTypeIds));
}
