namespace OrganisationRegistry.SqlServer;

using System;
using Microsoft.Data.SqlClient;
using System.Reflection;
using Autofac;
using Be.Vlaanderen.Basisregisters.DataDog.Tracing.Sql.EntityFrameworkCore;
using Configuration;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure;
using ProjectionState;
using OrganisationRegistry.Infrastructure.AppSpecific;
using OrganisationRegistry.Infrastructure.Events;

public class SqlServerModule : Autofac.Module
{
    private IConfiguration _configuration = null!;
    private IServiceCollection _services = null!;
    private ILoggerFactory _loggerFactory = null!;
    private bool _useSqlServer;

    public SqlServerModule(
        IConfiguration configuration,
        IServiceCollection services,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger<SqlServerModule>();
        var sqlConfiguration = configuration.GetSection(SqlServerConfiguration.Section).Get<SqlServerConfiguration>();
        var connectionString = sqlConfiguration.ConnectionString;

        _configuration = configuration;
        _services = services;
        _loggerFactory = loggerFactory;
        _useSqlServer = !string.IsNullOrWhiteSpace(connectionString);

        if (_useSqlServer)
            RunOnSqlServer(configuration, services, loggerFactory, connectionString);
        else
            RunInMemoryDb(services, loggerFactory, logger);

        services.Configure<SqlServerConfiguration>(configuration.GetSection(SqlServerConfiguration.Section));

        logger.LogInformation("Added {Context} to services:\n\tSchema: {Schema}\n\tTableName: {TableName}",
            nameof(OrganisationRegistryContext), WellknownSchemas.BackofficeSchema, MigrationTables.Default);
    }

    private static void RunOnSqlServer(
        IConfiguration configuration,
        IServiceCollection services,
        ILoggerFactory loggerFactory,
        string backofficeProjectionsConnectionString)
    {
        services
            .AddScoped(_ => new TraceDbConnection<OrganisationRegistryContext>(
                new SqlConnection(backofficeProjectionsConnectionString),
                configuration["DataDog:ServiceName"]))
            .AddDbContext<OrganisationRegistryContext>((provider, options) => options
                .UseLoggerFactory(loggerFactory)
                .UseSqlServer(provider.GetRequiredService<TraceDbConnection<OrganisationRegistryContext>>(), sqlServerOptions =>
                {
                    sqlServerOptions
                        .EnableRetryOnFailure()
                        .MigrationsAssembly("OrganisationRegistry.SqlServer")
                        .MigrationsHistoryTable(MigrationTables.Default, WellknownSchemas.BackofficeSchema);
                }));
    }

    private static void RunInMemoryDb(
        IServiceCollection services,
        ILoggerFactory loggerFactory,
        ILogger logger)
    {
        services
            .AddDbContext<OrganisationRegistryContext>(options => options
                .UseLoggerFactory(loggerFactory)
                .UseInMemoryDatabase(Guid.NewGuid().ToString(), _ => { }));

        logger.LogWarning("Running InMemory for {Context}!", nameof(OrganisationRegistryContext));
    }

    protected override void Load(ContainerBuilder builder)
    {
        // Register OrganisationRegistryContext natively in Autofac for Owned<T> support.
        // This is needed for ContextFactory to resolve Func<Owned<OrganisationRegistryContext>>.
        if (_useSqlServer)
        {
            var sqlConfiguration = _configuration.GetSection(SqlServerConfiguration.Section).Get<SqlServerConfiguration>();
            builder.Register<OrganisationRegistryContext>(c =>
            {
                var options = new DbContextOptionsBuilder<OrganisationRegistryContext>();
                options
                    .UseLoggerFactory(_loggerFactory)
                    .UseSqlServer(
                        new TraceDbConnection<OrganisationRegistryContext>(
                            new SqlConnection(sqlConfiguration.ConnectionString),
                            _configuration["DataDog:ServiceName"]),
                        sqlServerOptions =>
                        {
                            sqlServerOptions
                                .EnableRetryOnFailure()
                                .MigrationsAssembly("OrganisationRegistry.SqlServer")
                                .MigrationsHistoryTable(MigrationTables.Default, WellknownSchemas.BackofficeSchema);
                        });

                return new OrganisationRegistryContext(options.Options);
            })
            .InstancePerDependency();
        }
        else
        {
            builder.Register<OrganisationRegistryContext>(c =>
            {
                var options = new DbContextOptionsBuilder<OrganisationRegistryContext>();
                options
                    .UseLoggerFactory(_loggerFactory)
                    .UseInMemoryDatabase(Guid.NewGuid().ToString(), _ => { });

                return new OrganisationRegistryContext(options.Options);
            })
            .InstancePerDependency();
        }

        builder.RegisterType<ProjectionStates>()
            .As<IProjectionStates>()
            .SingleInstance();

        builder.RegisterType<MemoryCaches>()
            .As<MemoryCaches>()
            .As<IMemoryCaches>()
            .SingleInstance();

        builder.RegisterType<ContextFactory>()
            .As<IContextFactory>()
            .SingleInstance();

        builder.RegisterAssemblyTypes(typeof(OrganisationRegistrySqlServerAssemblyTokenClass).GetTypeInfo().Assembly)
            .AsClosedTypesOf(typeof(IEventHandler<>))
            .SingleInstance();

        builder.RegisterAssemblyTypes(typeof(OrganisationRegistrySqlServerAssemblyTokenClass).GetTypeInfo().Assembly)
            .AsClosedTypesOf(typeof(IReactionHandler<>))
            .SingleInstance();
    }
}
