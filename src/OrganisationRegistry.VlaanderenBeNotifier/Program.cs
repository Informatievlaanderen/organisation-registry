namespace OrganisationRegistry.VlaanderenBeNotifier
{
    using System;
    using System.IO;
    using System.Threading;
    using Amazon;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;
    using Configuration;
    using Destructurama;
    using Infrastructure.Config;
    using Infrastructure.Configuration;
    using Infrastructure.Infrastructure.Json;
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

    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting VlaanderenBeNotifier");

            JsonConvert.DefaultSettings =
                () => JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureForOrganisationRegistry();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true)
                .AddEnvironmentVariables();

            var sqlConfiguration = builder.Build().GetSection(ConfigurationDatabaseConfiguration.Section).Get<ConfigurationDatabaseConfiguration>();
            var configuration = builder
                .AddEntityFramework(x => x.UseSqlServer(
                    sqlConfiguration.ConnectionString,
                    y => y.MigrationsHistoryTable("__EFMigrationsHistory", "OrganisationRegistry")))
                .Build();

            RunProgram<Runner>(configuration);
        }

        private static void RunProgram<T>(IConfiguration configuration) where T : Runner
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

            if (!app.GetService<IOptions<TogglesConfiguration>>().Value.ApplicationAvailable)
            {
                logger.LogInformation("Application offline, exiting program.");
                return;
            }

            var options = app.GetService<IOptions<VlaanderenBeNotifierConfiguration>>().Value;

            var distributedLock = new DistributedLock<T>(
                new DistributedLockOptions
                {
                    Region = RegionEndpoint.GetBySystemName(options.LockRegionEndPoint),
                    AwsAccessKeyId = options.LockAccessKeyId,
                    AwsSecretAccessKey = options.LockAccessKeySecret,
                    TableName = options.LockTableName,
                    LeasePeriod = TimeSpan.FromMinutes(options.LockLeasePeriodInMinutes),
                    ThrowOnFailedRenew = true,
                    TerminateApplicationOnFailedRenew = true
                }, logger);

            var acquiredLock = false;
            try
            {
                logger.LogInformation("Trying to acquire lock.");
                acquiredLock = distributedLock.AcquireLock();

                if (!acquiredLock)
                {
                    logger.LogInformation("Could not get lock, another instance is busy");
                    return;
                }

                if (app.GetService<IOptions<TogglesConfiguration>>().Value.VlaanderenBeNotifierAvailable)
                {
                    var runner = app.GetService<T>();
                    UseOrganisationRegistryEventSourcing(app);

                    ExecuteRunner(runner);
                    logger.LogInformation("Processing completed successfully, exiting program.");
                }
                else
                {
                    logger.LogInformation("VlaanderenBeNotifier Toggle not enabled, exiting program.");
                }

                FlushLoggerAndTelemetry();
            }
            catch (Exception e)
            {
                logger.LogCritical(0, e, "Encountered a fatal exception, exiting program."); // dotnet core only supports global exceptionhandler starting from 1.2
                FlushLoggerAndTelemetry();
                throw;
            }
            finally
            {
                if (acquiredLock)
                    distributedLock.ReleaseLock();
            }
        }

        private static void ExecuteRunner(Runner runner) => runner.Run();

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
            builder.RegisterModule(new VlaanderenBeNotifierRunnerModule(configuration, services, serviceProvider.GetService<ILoggerFactory>()));
            return new AutofacServiceProvider(builder.Build());
        }

        private static void UseOrganisationRegistryEventSourcing(IServiceProvider app)
        {
            var registrar = app.GetService<BusRegistrar>();

            registrar.RegisterEventHandlers(typeof(MemoryCachesMaintainer));
            registrar.RegisterEventHandlersFromAssembly(typeof(OrganisationRegistryVlaanderenBeNotifierAssemblyTokenClass));
        }
    }
}
