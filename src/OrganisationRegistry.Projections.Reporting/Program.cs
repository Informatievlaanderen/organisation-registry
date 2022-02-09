namespace OrganisationRegistry.Projections.Reporting
{
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Destructurama;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Runners;
    using Serilog;
    using SqlServer.Infrastructure;
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon;
    using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Formatters.Json;
    using Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;
    using Configuration;
    using OrganisationRegistry.Configuration.Database;
    using OrganisationRegistry.Configuration.Database.Configuration;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.Config;
    using OrganisationRegistry.Infrastructure.Configuration;
    using OrganisationRegistry.Infrastructure.Infrastructure.Json;

    internal class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Starting Reporting Runner");

            JsonConvert.DefaultSettings =
                () => JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureForOrganisationRegistry();

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "development";
            var builder =
                new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.ToLowerInvariant()}.json", optional: true)
                    .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true)
                    .AddEnvironmentVariables();

            var sqlConfiguration = builder.Build().GetSection(ConfigurationDatabaseConfiguration.Section).Get<ConfigurationDatabaseConfiguration>();
            var configuration = builder
                .AddEntityFramework(x => x.UseSqlServer(
                    sqlConfiguration.ConnectionString,
                    y => y.MigrationsHistoryTable("__EFMigrationsHistory", WellknownSchemas.BackofficeSchema)))
                .Build();

            await RunProgram<GenderRatioRunner>(configuration);
        }

        private static async Task RunProgram<T>(IConfiguration configuration) where T : BaseRunner
        {
            var services = new ServiceCollection();
            services.AddLogging(loggingBuilder =>
            {
                var loggerConfiguration = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
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

            if (!app.GetService<IOptions<TogglesConfigurationSection>>().Value.ApplicationAvailable)
            {
                logger.LogInformation("Application offline, exiting program.");

                return;
            }

            var reportingRunnerOptions = app.GetService<IOptions<ReportingRunnerConfiguration>>().Value;

            var distributedLock = new DistributedLock<Program>(
                new DistributedLockOptions
                {
                    Region = RegionEndpoint.GetBySystemName(reportingRunnerOptions.LockRegionEndPoint),
                    AwsAccessKeyId = reportingRunnerOptions.LockAccessKeyId,
                    AwsSecretAccessKey = reportingRunnerOptions.LockAccessKeySecret,
                    TableName = reportingRunnerOptions.LockTableName,
                    LeasePeriod = TimeSpan.FromMinutes(reportingRunnerOptions.LockLeasePeriodInMinutes),
                    ThrowOnFailedRenew = true,
                    TerminateApplicationOnFailedRenew = true,
                    Enabled = reportingRunnerOptions.LockEnabled
                }, logger);

            bool acquiredLock = false;
            try
            {
                logger.LogInformation("Trying to acquire lock.");
                acquiredLock = distributedLock.AcquireLock();
                if (!acquiredLock)
                {
                    logger.LogInformation("Could not get lock, another instance is busy");
                    return;
                }

                if (app.GetService<IOptions<TogglesConfigurationSection>>().Value.ReportingRunnerAvailable)
                {
                    var runner = app.GetService<T>();

                    UseOrganisationRegistryEventSourcing(app, runner);

                    await ExecuteRunner(runner);

                    logger.LogInformation("Processing completed successfully, exiting program.");
                }
                else
                {
                    logger.LogInformation("Reporting Runner Toggle not enabled, exiting program.");
                }

                FlushLoggerAndTelemetry();
            }
            catch (Exception e)
            {
                // dotnet core only supports global exceptionhandler starting from 1.2
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

        private static async Task ExecuteRunner(BaseRunner runner)
        {
            await runner.Run();
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
            services.AddOptions();

            var serviceProvider = services.BuildServiceProvider();

            var builder = new ContainerBuilder();
            builder.RegisterModule(new ReportingRunnerModule(configuration, services, serviceProvider.GetService<ILoggerFactory>()));
            return new AutofacServiceProvider(builder.Build());
        }

        private static void UseOrganisationRegistryEventSourcing(IServiceProvider app, BaseRunner runner)
        {
            var registrar = app.GetService<BusRegistrar>();

            registrar.RegisterEventHandlers(typeof(MemoryCachesMaintainer));

            registrar.RegisterEventHandlers(runner.EventHandlers);
            registrar.RegisterReactionHandlers(runner.ReactionHandlers);
        }
    }
}
