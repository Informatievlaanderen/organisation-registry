namespace OrganisationRegistry.Api.Auth.Authorization;

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Models;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Authorization;

public sealed class SecurityServiceAuthorizationHandler(
    ISecurityService securityService,
    ILogger<SecurityServiceAuthorizationHandler> logger)
    : AuthorizationHandler<GlobalResourceRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, GlobalResourceRequirement requirement)
    {
        logger.LogInformation("SecurityServiceAuthorizationHandler");
        var user = await securityService.GetUser(context.User);

        if (user == WellknownUsers.Nobody || !user.Roles.Any())
        {
            context.Fail();
            return;
        }

        var role = user.Roles.First();
        // TODO: replace with the real per-resource dispatch once method
        // signatures are confirmed, e.g.:
        //
        // var allowed = requirement.Resource switch
        // {
        //     ResourceDefinition.OrgOrganisations => securityService.CanAddOrganisation(user),
        //     ResourceDefinition.Delegations       => securityService.CanEditDelegation(user, /* ? */),
        //     _ => false,
        // };
        var allowed = false;

        if (allowed)
        {
            logger.LogInformation(
                "GlobalResource legacy-capability grant: user {UserId} on {Resource}:{Operations}",
                user.UserId, requirement.Resource, requirement.Operations);
            context.Succeed(requirement);
        }
    }
}
