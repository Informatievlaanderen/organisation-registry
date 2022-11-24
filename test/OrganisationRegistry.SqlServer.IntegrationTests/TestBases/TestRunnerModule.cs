namespace OrganisationRegistry.SqlServer.IntegrationTests.TestBases;

using System;
using System.Reflection;
using Api;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Events;

public class TestRunnerModule<T> : Autofac.Module
{
    private readonly IConfiguration _configuration;
    private readonly IServiceCollection _services;
    private readonly ILoggerFactory _loggerFactory;
    private readonly DbContextOptions<OrganisationRegistryContext> _dbContextOptions;

    public TestRunnerModule(IConfiguration configuration,
        IServiceCollection services,
        ILoggerFactory loggerFactory,
        DbContextOptions<OrganisationRegistryContext> dbContextOptions)
    {
        _configuration = configuration;
        _services = services;
        _loggerFactory = loggerFactory;
        _dbContextOptions = dbContextOptions;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterModule(new ApiModule(_configuration, _services, _loggerFactory));
        builder.RegisterModule(new InfrastructureModule(_configuration, ProvideScopedServiceProvider, _services));
        builder.RegisterModule(new OrganisationRegistryModule());
        builder.RegisterModule(new SqlServerModule(_configuration, _services, _loggerFactory));

        builder.RegisterAssemblyTypes(typeof(T).GetTypeInfo().Assembly)
            .AsClosedTypesOf(typeof(IEventHandler<>))
            .SingleInstance();

        builder.RegisterInstance<IContextFactory>(
                new TestContextFactory(_dbContextOptions))
            .SingleInstance();

        builder.Populate(_services);
    }

    private static Func<IServiceProvider> ProvideScopedServiceProvider(IComponentContext context)
    {
        var defaultServiceProvider = context.Resolve<IServiceProvider>();
        return () => defaultServiceProvider.CreateScope().ServiceProvider;
    }
}
