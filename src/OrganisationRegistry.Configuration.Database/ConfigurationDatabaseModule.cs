namespace OrganisationRegistry.Configuration.Database;

using System;
using Microsoft.Data.SqlClient;
using Autofac;
using Be.Vlaanderen.Basisregisters.DataDog.Tracing.Sql.EntityFrameworkCore;
using Configuration;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class ConfigurationDatabaseModule : Module
{
    public ConfigurationDatabaseModule(
        IConfiguration configuration,
        IServiceCollection services,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger<ConfigurationDatabaseModule>();
        var sqlConfiguration = configuration.GetSection(ConfigurationDatabaseConfiguration.Section).Get<ConfigurationDatabaseConfiguration>();
        var connectionString = sqlConfiguration.ConnectionString;

        var hasConnectionString = !string.IsNullOrWhiteSpace(connectionString);
        if (hasConnectionString)
            RunOnSqlServer(configuration, services, loggerFactory, connectionString);
        else
            RunInMemoryDb(services, loggerFactory, logger);

        services.Configure<ConfigurationDatabaseConfiguration>(configuration.GetSection(ConfigurationDatabaseConfiguration.Section));

        logger.LogInformation("Added {Context} to services:\n\tSchema: {Schema}\n\tTableName: {TableName}",
            nameof(ConfigurationContext), WellknownSchemas.OrganisationRegistrySchema, MigrationTables.Default);
    }

    private static void RunOnSqlServer(
        IConfiguration configuration,
        IServiceCollection services,
        ILoggerFactory loggerFactory,
        string backofficeProjectionsConnectionString)
    {
        services
            .AddScoped(_ => new TraceDbConnection<ConfigurationContext>(
                new SqlConnection(backofficeProjectionsConnectionString),
                configuration["DataDog:ServiceName"]))
            .AddDbContext<ConfigurationContext>((provider, options) => options
                .UseLoggerFactory(loggerFactory)
                .UseSqlServer(provider.GetRequiredService<TraceDbConnection<ConfigurationContext>>(), sqlServerOptions =>
                {
                    sqlServerOptions
                        .EnableRetryOnFailure()
                        .MigrationsHistoryTable(MigrationTables.Default, WellknownSchemas.BackofficeSchema);
                }));
    }

    private static void RunInMemoryDb(
        IServiceCollection services,
        ILoggerFactory loggerFactory,
        ILogger logger)
    {
        services
            .AddDbContext<ConfigurationContext>(options => options
                .UseLoggerFactory(loggerFactory)
                .UseInMemoryDatabase(Guid.NewGuid().ToString(), _ => { }));

        logger.LogWarning("Running InMemory for {Context}!", nameof(ConfigurationContext));
    }

    protected override void Load(ContainerBuilder builder) { }
}
