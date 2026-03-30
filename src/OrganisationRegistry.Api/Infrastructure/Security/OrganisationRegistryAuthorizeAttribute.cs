namespace OrganisationRegistry.Api.Infrastructure.Security;

using System.Linq;
using Api.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using OrganisationRegistry.Infrastructure.Authorization;
using Schemes = OrganisationRegistry.Api.Infrastructure.Security.AuthenticationSchemes;

public class OrganisationRegistryAuthorizeAttribute : AuthorizeAttribute
{
    public OrganisationRegistryAuthorizeAttribute(params Role[] roles) : this()
    {
        Roles = string.Join(",", roles.Select(RoleMapping.Map));
    }

    public OrganisationRegistryAuthorizeAttribute()
    {
        AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}, {Schemes.EditApi}, {Schemes.BffApi}";
    }
}
