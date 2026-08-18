namespace OrganisationRegistry.Api.Auth.Authorization;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Models;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Authorization;

public sealed class RoleMatrixAuthorizationHandler(
    ISecurityService securityService,
    ILogger<RoleMatrixAuthorizationHandler> logger)
    : AuthorizationHandler<GlobalResourceRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, GlobalResourceRequirement requirement)
    {
        logger.LogInformation("RoleMatrixAuthorizationHandler");
        IUser user;
        try
        {
            user = await securityService.GetUser(context.User);
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "RoleMatrixAuthorizationHandler: GetUser threw, treating as no match");
            return;
        }

        if (user == WellknownUsers.Nobody || !user.Roles.Any())
        {
            context.Fail();
            return;
        }

        var role = user.Roles.First();

        if (RolePermissions.Validate(role, requirement.Resource, requirement.Operations, logger))
            context.Succeed(requirement);
    }
}
