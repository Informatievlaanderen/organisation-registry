namespace OrganisationRegistry.Api.Infrastructure.Security;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

public class OrganisationRegistryAuthorizeAttribute : AuthorizeAttribute
{
    public OrganisationRegistryAuthorizeAttribute()
    {
        AuthenticationSchemes = string.Join(", ", JwtBearerDefaults.AuthenticationScheme);
    }
}
