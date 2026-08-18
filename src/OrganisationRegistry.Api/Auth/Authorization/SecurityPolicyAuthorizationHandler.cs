namespace OrganisationRegistry.Api.Auth.Authorization;

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Handling.Authorization;
using OrganisationRegistry.Infrastructure.AppSpecific;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;

public sealed record PolicyContext(
    IMemoryCaches MemoryCaches,
    IOrganisationRegistryConfiguration Configuration);

public sealed class LazySecurityPolicyRequirement(
    Func<PolicyContext, ISecurityPolicy> policyFactory)
    : IAuthorizationRequirement
{
    public Func<PolicyContext, ISecurityPolicy> PolicyFactory { get; } = policyFactory;
}

public sealed class LazySecurityPolicyAuthorizationHandler(
    ISecurityService securityService,
    PolicyContext policyContext,
    ILogger<LazySecurityPolicyAuthorizationHandler> logger)
    : AuthorizationHandler<LazySecurityPolicyRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, LazySecurityPolicyRequirement requirement)
    {
        logger.LogInformation("LazySecurityPolicyAuthorizationHandler");
        var user = await securityService.GetUser(context.User);
        var policy = requirement.PolicyFactory(policyContext); // built lazily, right here
        var result = policy.Check(user); // legacy ISecurityPolicy, unmodified

        if (result.IsSuccessful)
            context.Succeed(requirement);
        // else if (result.Exception is { } exception)
        //     context.Fail(new AuthorizationFailureReason(this, exception.Message));
    }
}

public static class AuthorizationServiceSecurityPolicyExtensions
{
    public static Task<Microsoft.AspNetCore.Authorization.AuthorizationResult> AuthorizeAsync(
        this IAuthorizationService authorizationService,
        ClaimsPrincipal user,
        Func<PolicyContext, ISecurityPolicy> policyFactory)
    {
        var requirement = new LazySecurityPolicyRequirement(policyFactory);

        return authorizationService.AuthorizeAsync(
            user,
            resource: null,
            requirements: [requirement]);
    }
}
