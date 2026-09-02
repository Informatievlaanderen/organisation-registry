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
    /// Shorthand for <c>Permissions.IsSatisfiedFor(permission, context)</c>.
    /// Core authorization decision used by handler policies: true when a grant
    /// for <paramref name="permission"/> applies to <paramref name="context"/>.
    /// Fail-closed when no grant applies.
    /// </summary>
    bool IsSatisfiedFor(Permission permission, IRestrictionContext context);
}
