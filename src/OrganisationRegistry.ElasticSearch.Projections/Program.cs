namespace OrganisationRegistry.ElasticSearch.Projections
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
            Console.WriteLine("Starting ElasticSearch Projections Runner");

            JsonConvert.DefaultSettings =
                () => JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureForOrganisationRegistry();

            var builder =
                new ConfigurationBuilder()
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

            RunProgram<PeopleRunner>(configuration);
            RunProgram<OrganisationsRunner>(configuration);
            RunProgram<BodyRunner>(configuration);
        }

        private static void RunProgram<T>(IConfiguration configuration) where T : BaseRunner
        {
            var runnerName = typeof(T).Name;
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
                logger.LogInformation("[{RunnerName}] Application offline, exiting program.", runnerName);
                return;
            }

            var elasticSearchOptions = app.GetService<IOptions<ElasticSearchConfiguration>>().Value;

            var distributedLock = new DistributedLock<T>(
                new DistributedLockOptions
                {
                    Region = RegionEndpoint.GetBySystemName(elasticSearchOptions.LockRegionEndPoint),
                    AwsAccessKeyId = elasticSearchOptions.LockAccessKeyId,
                    AwsSecretAccessKey = elasticSearchOptions.LockAccessKeySecret,
                    TableName = elasticSearchOptions.LockTableName,
                    LeasePeriod = TimeSpan.FromMinutes(elasticSearchOptions.LockLeasePeriodInMinutes),
                    ThrowOnFailedRenew = true,
                    TerminateApplicationOnFailedRenew = true
                });

            var acquiredLock = false;
            try
            {
                logger.LogInformation("[{RunnerName}] Trying to acquire lock.", runnerName);
                acquiredLock = distributedLock.AcquireLock();

                if (!acquiredLock)
                {
                    logger.LogInformation("[{RunnerName}] Could not get lock, another instance is busy", runnerName);
                    return;
                }

                if (app.GetService<IOptions<TogglesConfiguration>>().Value.ElasticSearchProjectionsAvailable)
                {
                    var runner = app.GetService<T>();
                    UseOrganisationRegistryEventSourcing(app, runner);

                    ExecuteRunner(runner);
                    logger.LogInformation("[{RunnerName}] Processing completed successfully, exiting program.", runnerName);
                }
                else
                {
                    logger.LogInformation("[{RunnerName}] ElasticSearch Projections Toggle not enabled, exiting program.", runnerName);
                }

                FlushLoggerAndTelemetry();
            }
            catch (Exception e)
            {
                logger.LogCritical(0, e, "[{RunnerName}] Encountered a fatal exception, exiting program."); // dotnet core only supports global exceptionhandler starting from 1.2
                FlushLoggerAndTelemetry();
                throw;
            }
            finally
            {
                if (acquiredLock)
                    distributedLock.ReleaseLock();
            }
        }

        private static void ExecuteRunner(BaseRunner runner) => runner.Run();

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
            builder.RegisterModule(new ElasticSearchProjectionsModule(configuration, services, serviceProvider.GetService<ILoggerFactory>()));
            return new AutofacServiceProvider(builder.Build());
        }

        private static void UseOrganisationRegistryEventSourcing(IServiceProvider app, BaseRunner runner)
        {
            var registrar = app.GetService<BusRegistrar>();

            registrar.RegisterEventHandlers(typeof(MemoryCachesMaintainer));

            registrar.RegisterEventHandlers(runner.EventHandlers);
            registrar.RegisterReactionHandlers(runner.ReactionHandlers);

            //registrar.RegisterEventHandlersFromAssembly(typeof(OrganisationRegistryElasticSearchProjectionsAssemblyTokenClass));
            //registrar.RegisterReactionHandlersFromAssembly(typeof(OrganisationRegistryElasticSearchProjectionsAssemblyTokenClass));
        }
    }
}
