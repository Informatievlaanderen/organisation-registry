namespace OrganisationRegistry.Api.Infrastructure.Configuration;

using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OrganisationRegistry.Infrastructure.Config;
using OrganisationRegistry.Infrastructure.Events;
using SqlServer;

public static class UseOrganisationRegistryEventSourcingExtension
{
    public static IApplicationBuilder UseOrganisationRegistryEventSourcing(this IApplicationBuilder app)
    {
        RegisterHandlers(app);

        InitialiseHandlers(app);

        return app;
    }

    private static void RegisterHandlers(IApplicationBuilder app)
    {
        var registrar = app.ApplicationServices.GetService<BusRegistrar>()!;

        registrar.RegisterCommandEnvelopeHandlers(typeof(BaseCommand));

        registrar.RegisterEventHandlersFromAssembly(typeof(OrganisationRegistrySqlServerAssemblyTokenClass));
        registrar.RegisterReactionHandlersFromAssembly(typeof(OrganisationRegistrySqlServerAssemblyTokenClass));
    }

    private static void InitialiseHandlers(IApplicationBuilder app)
    {
        var eventHandlers = typeof(OrganisationRegistrySqlServerAssemblyTokenClass)
            .GetTypeInfo()
            .Assembly
            .GetTypes()
            .Where(
                x => x
                    .GetInterfaces()
                    .Any(y => y.GetTypeInfo().IsGenericType && y.GetGenericTypeDefinition() == typeof(IEventHandler<>)));

        foreach (var eventHandler in eventHandlers)
            app.ApplicationServices.GetService(eventHandler);
    }
}