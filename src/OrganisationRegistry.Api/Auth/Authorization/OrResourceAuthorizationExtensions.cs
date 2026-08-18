namespace OrganisationRegistry.Api.Auth.Authorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using OrganisationRegistry.Infrastructure.AppSpecific;
using OrganisationRegistry.Infrastructure.Configuration;

public static class OrResourceAuthorizationExtensions
{

    public static IServiceCollection AddOrResourceAuthorization(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationHandler, RoleMatrixAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, SecurityServiceAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, LazySecurityPolicyAuthorizationHandler>();

        services.AddScoped<PolicyContext>(sp => new PolicyContext(
            sp.GetRequiredService<IMemoryCaches>(),
            sp.GetRequiredService<IOrganisationRegistryConfiguration>()));

        return services;
    }
}
