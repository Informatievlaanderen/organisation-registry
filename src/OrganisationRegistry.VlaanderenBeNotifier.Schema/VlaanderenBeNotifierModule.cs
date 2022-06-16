namespace OrganisationRegistry.VlaanderenBeNotifier.Schema;

using System;
using System.Reflection;
using Autofac;
using Be.Vlaanderen.Basisregisters.DataDog.Tracing.Sql.EntityFrameworkCore;
using Infrastructure;
using Infrastructure.Events;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class VlaanderenBeNotifierModule : Autofac.Module
{
    public VlaanderenBeNotifierModule(ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger<VlaanderenBeNotifierModule>();

        logger.LogInformation("Added {Context} to services:\n\tSchema: {Schema}\n\tTableName: {TableName}",
            nameof(VlaanderenBeNotifierContext), WellknownSchemas.BackofficeSchema, MigrationTables.Default);
    }

    private static void RunOnSqlServer(
        IConfiguration configuration,
        IServiceCollection services,
        ILoggerFactory loggerFactory,
        string backofficeProjectionsConnectionString)
    {
        services
            .AddScoped(_ => new TraceDbConnection<VlaanderenBeNotifierContext>(
                new SqlConnection(backofficeProjectionsConnectionString),
                configuration["DataDog:ServiceName"]))
            .AddDbContext<VlaanderenBeNotifierContext>((provider, options) => options
                .UseLoggerFactory(loggerFactory)
                .UseSqlServer(provider.GetRequiredService<TraceDbConnection<VlaanderenBeNotifierContext>>(), sqlServerOptions =>
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
            .AddDbContext<VlaanderenBeNotifierContext>(options => options
                .UseLoggerFactory(loggerFactory)
                .UseInMemoryDatabase(Guid.NewGuid().ToString(), _ => { }));

        logger.LogWarning("Running InMemory for {Context}!", nameof(VlaanderenBeNotifierContext));
    }

    protected override void Load(ContainerBuilder builder)
    {
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
