namespace OrganisationRegistry.Api.Infrastructure.Security;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using OrganisationRegistry.Infrastructure.Authorization;

public class OrganisationRegistryAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    [Obsolete("Role-based authorization is being replaced by permission-based authorization. Use RequiredPermissions instead. See feature 009-permission-based-authz.")]
    public OrganisationRegistryAuthorizeAttribute(params Role[] roles) : this()
    {
        Roles = string.Join(",", roles.Select(RoleMapping.Map));
    }

    public OrganisationRegistryAuthorizeAttribute()
    {
        Policy = PolicyNames.BackofficeUser;
    }

    /// <summary>
    /// One or more permissions required to access the endpoint. Any-of semantics: the
    /// identity is allowed if its <see cref="IUser.Permissions"/> contains at least one
    /// of these. When null or empty, no permission check is performed (policy-only authorization).
    /// </summary>
    public Permission[]? RequiredPermissions { get; set; }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (RequiredPermissions is null || RequiredPermissions.Length == 0)
            return;

        var securityService = context.HttpContext.RequestServices.GetService<ISecurityService>();
        if (securityService is null)
        {
            context.Result = new ForbidResult();
            return;
        }

        var user = await securityService.GetRequiredUser(context.HttpContext.User);

        if (user.HasAnyPermission(RequiredPermissions))
            return;

        context.Result = new ForbidResult();
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
