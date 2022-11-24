namespace OrganisationRegistry.SqlServer.IntegrationTests.TestBases;

using System;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OnProjections;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Authorization.Cache;
using OrganisationRegistry.Infrastructure.Bus;
using OrganisationRegistry.Infrastructure.Config;
using OrganisationRegistry.Infrastructure.Configuration;
using OrganisationRegistry.Infrastructure.Events;
using Tests.Shared;
using Tests.Shared.Stubs;

public abstract class ListViewTestBase
{
    private readonly InProcessBus _inProcessBus;

    public DbContextOptions<OrganisationRegistryContext> ContextOptions { get; }
    protected OrganisationRegistryContext Context => new(ContextOptions);

    protected ListViewTestBase()
    {
        Directory.SetCurrentDirectory(Directory.GetParent(typeof(SqlServerFixture).GetTypeInfo().Assembly.Location)!.Parent!.Parent!.Parent!.FullName);

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true)
            .Build();

        ContextOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
            .UseInMemoryDatabase(
                $"org-es-test-{Guid.NewGuid()}",
                _ => { }).Options;

        var services = new ServiceCollection();

        var app = ConfigureServices(services, configuration, ContextOptions);
        UseOrganisationRegistryEventSourcing(app);

        _inProcessBus = app.GetService<InProcessBus>()!;
    }

    private static IServiceProvider ConfigureServices(IServiceCollection services,
        IConfiguration configuration,
        DbContextOptions<OrganisationRegistryContext> dbContextOptions)
    {
        services.AddOptions();
        services.AddSingleton<ICache<OrganisationSecurityInformation>, OrganisationSecurityCache>()
            .AddSingleton<IOrganisationRegistryConfiguration>(
                new OrganisationRegistryConfigurationStub());

        var builder = new ContainerBuilder();
        builder.RegisterModule(new TestRunnerModule<OrganisationRegistrySqlServerAssemblyTokenClass>(configuration, services, new NullLoggerFactory(), dbContextOptions));

        return new AutofacServiceProvider(builder.Build());
    }

    private static void UseOrganisationRegistryEventSourcing(IServiceProvider app)
    {
        var registrar = app.GetService<BusRegistrar>()!;

        registrar.RegisterEventHandlers(typeof(MemoryCachesMaintainer));
        registrar.RegisterEventHandlersFromAssembly(typeof(OrganisationRegistrySqlServerAssemblyTokenClass));
    }


    protected async Task HandleEvents(params IEvent[] envelopes)
    {
        foreach (var envelope in envelopes)
        {
            await _inProcessBus.Publish(Mock.Of<DbConnection>(), Mock.Of<DbTransaction>(), (dynamic)envelope.ToEnvelope());
        }
    }
}
