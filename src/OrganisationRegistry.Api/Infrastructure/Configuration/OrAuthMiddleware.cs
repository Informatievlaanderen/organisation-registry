namespace OrganisationRegistry.Api.Infrastructure.Configuration;

using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Auth.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;

public  class OrAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ISecurityService _securityService;

    public OrAuthMiddleware(RequestDelegate next,
        ISecurityService securityService)
    {
        _next = next;
        _securityService = securityService;
    }

    public async Task Invoke(HttpContext context, IOptions<TogglesConfigurationSection> configuration)
    {
        var attribute = context.GetEndpoint()?.Metadata.GetMetadata<GlobalResourceAttribute>();
        if (attribute is null)
        {
            await _next.Invoke(context);
            return;
        }

        IUser? user;

        try
        {
            user = await _securityService.GetUser(context.User);
            if (user == WellknownUsers.Nobody)
                throw new Exception("De gebruiker beschikt niet over een geldige Wegwijs-rol. - 403"); // TODO
        }
        catch (Exception e)
        {
            user = null;
        }

        if (user is null)
        {
            throw new Exception();
        }

        var requiredResourceName = attribute.ResourceDefinition; // bv. org.organisations
        var requiredAttributeOperation = attribute.CrudOperations; // bv. [create, read]

        var role = user.Roles.First(); // bv. decentraalbeheerder

        //var userPermissions = RolePermissions.Resolve(role); // bv. [ "org.organisations:create", "body.info:create"]


        var requiredCrudOperations = RolePermissions.HasPermission(userPermissions); // bv.



        var requiredPermission = $"{requiredResourceName}:{requiredAttributeOperation}"; // bv. org.organisations:create

    }

}
