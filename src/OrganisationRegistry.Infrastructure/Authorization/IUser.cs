namespace OrganisationRegistry.Infrastructure.Authorization;

using System;
using System.Collections.Generic;
using OrganisationRegistry.Infrastructure.Authorization.Restrictions;

public interface IUser
{
    string FirstName { get; set; }
    string LastName { get; set; }
    string UserId { get; set; }
    string Ip { get; set; }
    Role[] Roles { get; set; }
    PermissionSet Permissions { get; }
    bool IsAuthorizedForVlimpersOrganisations { get; }
    List<string> Organisations { get; }
    bool IsInAnyOf(params Role[] roles);
    bool HasPermission(Permission permission);
    bool HasAnyPermission(params Permission[] permissions);
    bool IsDecentraalBeheerderForOrganisation(string ovoNumber);
    bool IsDecentraalBeheerderForOrganisation(Guid organisationId);
    bool IsDecentraalBeheerderForBody(Guid bodyId);

    /// <summary>
    /// Shorthand for <c>Permissions.IsRestrictedTo&lt;TContext&gt;()</c>.
    /// True when the caller carries at least one restricted grant for the
    /// context's domain and no unrestricted grant absorbs it.
    /// </summary>
    bool IsRestrictedTo<TContext>()
        where TContext : IRestrictionContext<TContext>;

    /// <summary>
    /// Shorthand for <c>Permissions.GetRestriction&lt;TContext&gt;()</c>.
    /// Never returns null; missing entries yield
    /// <see cref="DenyAllRestriction{TContext}"/> (fail-closed).
    /// </summary>
    IRestriction<TContext> GetRestriction<TContext>()
        where TContext : IRestrictionContext<TContext>;
}
