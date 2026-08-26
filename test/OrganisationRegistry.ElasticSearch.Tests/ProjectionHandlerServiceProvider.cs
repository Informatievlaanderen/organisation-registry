namespace OrganisationRegistry.ElasticSearch.Tests;

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Client;
using Configuration;
using OrganisationRegistry.Infrastructure.Configuration;
using OrganisationRegistry.Infrastructure.Events;
using Projections.People.Cache;
using SqlServer;
using SqlServer.Infrastructure;
using OrganisationRegistry.Tests.Shared.Stubs;

/// <summary>
/// Registers a runner's event handlers the way Program.cs does -- hand the types to the container and
/// let it do the constructor injection -- but against test infrastructure. A handler that grows a new
/// dependency needs one registration here, and the container names the type it cannot resolve.
/// </summary>
public static class ProjectionHandlerServiceProvider
{
    public static IServiceProvider Build(
        IContextFactory contextFactory,
        ElasticSearchFixture fixture,
        Type[] eventHandlers)
    {
        var services = new ServiceCollection()
            .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))
            .AddSingleton(fixture.Elastic)
            .AddSingleton(fixture.ElasticSearchOptions)
            .AddSingleton(contextFactory)
            .AddSingleton<IEventStore>(new InMemoryEventStore())
            .AddSingleton<IOrganisationManagementConfiguration>(new OrganisationManagementConfigurationStub())
            .AddSingleton<IPersonHandlerCache>(new PersonHandlerCacheStub())
            .AddSingleton(new MemoryCachesMaintainer(new MemoryCaches(contextFactory), contextFactory));

        foreach (var handlerType in eventHandlers)
            services.AddSingleton(handlerType);

        return services.BuildServiceProvider();
    }
}
