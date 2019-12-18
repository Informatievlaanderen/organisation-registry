namespace OrganisationRegistry.Projections.Delegations
{
    using System;
    using System.IO;
    using System.Threading;
    using Amazon;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Formatters.Json;
    using Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;
    using Configuration;
    using Destructurama;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Serilog;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.Configuration.Database;
    using OrganisationRegistry.Configuration.Database.Configuration;
    using OrganisationRegistry.Infrastructure.Config;
    using OrganisationRegistry.Infrastructure.Configuration;
    using OrganisationRegistry.Infrastructure.Infrastructure.Json;

    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting Delegations Runner");

            JsonConvert.DefaultSettings =
                () => JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureDefaultForApi();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true)
                .AddEnvironmentVariables();

            var sqlConfiguration = builder.Build().GetSection(ConfigurationDatabaseConfiguration.Section)
                .Get<ConfigurationDatabaseConfiguration>();
            var configuration = builder
                .AddEntityFramework(x => x.UseSqlServer(
                    sqlConfiguration.ConnectionString,
                    y => y.MigrationsHistoryTable("__EFMigrationsHistory", "OrganisationRegistry")))
                .Build();

            var services = new ServiceCollection();
            services.AddLogging(loggingBuilder =>
            {
                var loggerConfiguration = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .WriteTo.LiterateConsole()
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithThreadId()
                    .Enrich.WithEnvironmentUserName()
                    .Destructure.JsonNetTypes();

                Serilog.Debugging.SelfLog.Enable(Console.WriteLine);

                Log.Logger = loggerConfiguration.CreateLogger();

                loggingBuilder.AddSerilog();
            });

            var app = ConfigureServices(services, configuration);

            var logger = app.GetService<ILogger<Program>>();

            var delegationsRunnerOptions = app.GetService<IOptions<DelegationsRunnerConfiguration>>().Value;

            if (!app.GetService<IOptions<TogglesConfiguration>>().Value.ApplicationAvailable)
            {
                logger.LogInformation("Application offline, exiting program.");
                return;
            }

            UseOrganisationRegistryEventSourcing(app);

            var distributedLock = new DistributedLock<Program>(
                new DistributedLockOptions
                {
                    Region = RegionEndpoint.GetBySystemName(delegationsRunnerOptions.LockRegionEndPoint),
                    AwsAccessKeyId = delegationsRunnerOptions.LockAccessKeyId,
                    AwsSecretAccessKey = delegationsRunnerOptions.LockAccessKeySecret,
                    TableName = delegationsRunnerOptions.LockTableName,
                    LeasePeriod = TimeSpan.FromMinutes(delegationsRunnerOptions.LockLeasePeriodInMinutes),
                    ThrowOnFailedRenew = true,
                    TerminateApplicationOnFailedRenew = true
                });

            bool acquiredLock = false;
            try
            {
                acquiredLock = distributedLock.AcquireLock();
                if (!acquiredLock)
                {
                    logger.LogInformation("Could not get lock, another instance is busy");
                    return;
                }

                if (app.GetService<Runner>().Run())
                {
                    logger.LogInformation("Processing completed successfully, exiting program.");
                }
                else
                {
                    logger.LogInformation("DelegationsRunner Toggle not enabled, exiting program.");
                }

                FlushLoggerAndTelemetry();
            }
            catch (Exception e)
            {
                // dotnet core only supports global exceptionhandler starting from 1.2 (TODO)
                logger.LogCritical(0, e, "Encountered a fatal exception, exiting program.");
                FlushLoggerAndTelemetry();
                throw;
            }
            finally
            {
                if (acquiredLock)
                {
                    distributedLock.ReleaseLock();
                }
            }
        }

        private static void FlushLoggerAndTelemetry()
        {
            Log.CloseAndFlush();

            // Allow some time for flushing before shutdown.
            Thread.Sleep(1000);
        }

        private static IServiceProvider ConfigureServices(
            IServiceCollection services,
            IConfiguration configuration)
        {
            var serviceProvider = services.BuildServiceProvider();

            var builder = new ContainerBuilder();
            builder.RegisterModule(new DelegationsRunnerModule(configuration, services, serviceProvider.GetService<ILoggerFactory>()));
            return new AutofacServiceProvider(builder.Build());
        }

        private static void UseOrganisationRegistryEventSourcing(IServiceProvider app)
        {
            var registrar = app.GetService<BusRegistrar>();

            registrar.RegisterEventHandlers(typeof(MemoryCachesMaintainer));
            registrar.RegisterEventHandlersFromAssembly(typeof(OrganisationRegistryDelegationsRunnerAssemblyTokenClass));
        }
    }
}
