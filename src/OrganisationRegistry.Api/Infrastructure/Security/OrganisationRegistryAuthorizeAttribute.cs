namespace OrganisationRegistry.Api.Infrastructure.Security;

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
