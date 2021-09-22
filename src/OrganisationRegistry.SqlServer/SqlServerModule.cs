namespace OrganisationRegistry.SqlServer
{
    using System;
    using System.Data.Common;
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
        public SqlServerModule(
            IConfiguration configuration,
            IServiceCollection services,
            ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<SqlServerModule>();
            var sqlConfiguration = configuration.GetSection(SqlServerConfiguration.Section).Get<SqlServerConfiguration>();
            var connectionString = sqlConfiguration.ConnectionString;

            var hasConnectionString = !string.IsNullOrWhiteSpace(connectionString);
            if (hasConnectionString)
                RunOnSqlServer(configuration, services, loggerFactory, connectionString);
            else
                RunInMemoryDb(services, loggerFactory, logger);

            services.Configure<SqlServerConfiguration>(configuration.GetSection(SqlServerConfiguration.Section));

            logger.LogInformation(
                "Added {Context} to services:" +
                Environment.NewLine +
                "\tSchema: {Schema}" +
                Environment.NewLine +
                "\tTableName: {TableName}",
                nameof(OrganisationRegistryContext), Schema.Default, MigrationTables.Default);
        }

        private static void RunOnSqlServer(
            IConfiguration configuration,
            IServiceCollection services,
            ILoggerFactory loggerFactory,
            string backofficeProjectionsConnectionString)
        {
            services
                .AddScoped(s => new TraceDbConnection<OrganisationRegistryContext>(
                    new SqlConnection(backofficeProjectionsConnectionString),
                    configuration["DataDog:ServiceName"]))
                .AddDbContext<OrganisationRegistryContext>((provider, options) => options
                    .UseLoggerFactory(loggerFactory)
                    .UseSqlServer(provider.GetRequiredService<TraceDbConnection<OrganisationRegistryContext>>(), sqlServerOptions =>
                    {
                        sqlServerOptions
                            .EnableRetryOnFailure()
                            .MigrationsAssembly("OrganisationRegistry.SqlServer")
                            .MigrationsHistoryTable(MigrationTables.Default, Schema.Default);
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
                    .UseInMemoryDatabase(Guid.NewGuid().ToString(), sqlServerOptions => { }));

            logger.LogWarning("Running InMemory for {Context}!", nameof(OrganisationRegistryContext));
        }

        protected override void Load(ContainerBuilder builder)
        {
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
}
