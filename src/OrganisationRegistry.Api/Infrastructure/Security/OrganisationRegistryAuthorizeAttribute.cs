namespace OrganisationRegistry.Api.Infrastructure.Security;

using System.Collections.Generic;
using System.Linq;
using Api.Security;
using Microsoft.AspNetCore.Authorization;
using OrganisationRegistry.Infrastructure.Authorization;

public class OrganisationRegistryAuthorizeAttribute : AuthorizeAttribute
{
    public OrganisationRegistryAuthorizeAttribute(params Role[] roles) : this()
    {
        Roles = string.Join(",", roles.Select(RoleMapping.Map));
    }

    public OrganisationRegistryAuthorizeAttribute()
    {
        Policy = PolicyNames.BackofficeUser;
    }
}

// !Refactor this
public class OrProtectedAttribute : AuthorizeAttribute
{
    public OrProtectedAttribute()
    {
        List<Role> roles = [Role.AlgemeenBeheerder, Role.VlimpersBeheerder, Role.DecentraalBeheerder, Role.OrgaanBeheerder, Role.RegelgevingBeheerder, Role.Orafin, Role.CjmBeheerder, Role.Developer, Role.AutomatedTask];
        Roles = string.Join(",", roles.Select(RoleMapping.Map));
        Policy = PolicyNames.BackofficeUser;
    }
}
