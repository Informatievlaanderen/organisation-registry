namespace OrganisationRegistry.ElasticSearch.Projections
{
    using System;
    using System.Data.Common;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon;
    using App.Metrics;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Autofac.Features.OwnedInstances;
    using Autofac.Util;
    using Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing.Sql.EntityFrameworkCore;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Body;
    using Cache;
    using Client;
    using Configuration;
    using Destructurama;
    using Infrastructure;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Nest;
    using Newtonsoft.Json;
    using NodaTime;
    using Serilog;
    using OrganisationRegistry.Configuration.Database;
    using OrganisationRegistry.Configuration.Database.Configuration;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.AppSpecific;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Bus;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Config;
    using OrganisationRegistry.Infrastructure.Configuration;
    using OrganisationRegistry.Infrastructure.Domain;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Infrastructure.EventStore;
    using OrganisationRegistry.Infrastructure.Infrastructure.Json;
    using Organisations;
    using People;
    using People.Handlers;
    using SqlServer;
    using SqlServer.Configuration;
    using SqlServer.Infrastructure;
    using SqlServer.ProjectionState;
    using IClock = NodaTime.IClock;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Starting RoadRegistry.Product.ProjectionHost");

            AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
                Log.Debug(eventArgs.Exception, "FirstChanceException event raised in {AppDomain}.", AppDomain.CurrentDomain.FriendlyName);

            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
                Log.Fatal((Exception)eventArgs.ExceptionObject, "Encountered a fatal exception, exiting program.");

            var host = new HostBuilder()
                .ConfigureHostConfiguration(builder =>
                {
                    builder
                        .AddEnvironmentVariables("DOTNET_")
                        .AddEnvironmentVariables("ASPNETCORE_");
                })
                .ConfigureAppConfiguration((hostContext, builder) =>
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                    if (hostContext.HostingEnvironment.IsProduction())
                    {
                        builder
                            .SetBasePath(Directory.GetCurrentDirectory());
                    }

                    builder
                        .AddJsonFile("appsettings.json", true, false)
                        .AddJsonFile(
                            $"appsettings.{hostContext.HostingEnvironment.EnvironmentName.ToLowerInvariant()}.json",
                            true, false)
                        .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", true, false)
                        .AddEnvironmentVariables()
                        .AddCommandLine(args);

                    var sqlConfiguration = builder.Build().GetSection(ConfigurationDatabaseConfiguration.Section).Get<ConfigurationDatabaseConfiguration>();
                    builder.AddEntityFramework(x =>
                        x.UseSqlServer(
                            sqlConfiguration.ConnectionString,
                            y => y.MigrationsHistoryTable("__EFMigrationsHistory", "OrganisationRegistry")));
                })
                .ConfigureLogging((hostContext, builder) =>
                {
                    Serilog.Debugging.SelfLog.Enable(Console.WriteLine);

                    var loggerConfiguration = new LoggerConfiguration()
                        .ReadFrom.Configuration(hostContext.Configuration)
                        .Enrich.FromLogContext()
                        .Enrich.WithMachineName()
                        .Enrich.WithThreadId()
                        .Enrich.WithEnvironmentUserName();

                    Log.Logger = loggerConfiguration.CreateLogger();

                    builder.AddSerilog(Log.Logger);
                })
                .ConfigureServices((hostContext, builder) =>
                {
                    builder
                        .Configure<InfrastructureConfiguration>(hostContext.Configuration.GetSection(InfrastructureConfiguration.Section))
                        .Configure<TogglesConfiguration>(hostContext.Configuration.GetSection(TogglesConfiguration.Section))
                        .Configure<OpenIdConnectConfiguration>(hostContext.Configuration.GetSection(OpenIdConnectConfiguration.Section));

                    builder
                        .AddSingleton<IClock>(SystemClock.Instance)
                        .AddSingleton<Scheduler>()
                        .AddHostedService<EventProcessor>();

                    var elasticTypes = Assembly.GetAssembly(typeof(OrganisationRegistryElasticSearchProjectionsAssemblyTokenClass))
                        .GetTypes()
                        .Where(item => item.GetInterfaces()
                            .Where(i => i.IsGenericType)
                            .Any(i => i.GetGenericTypeDefinition() == typeof(IElasticEventHandler<>)))
                        .ToList();

                    elasticTypes.ForEach(assignedType =>
                    {
                        builder.AddSingleton(assignedType);

                        assignedType.GetInterfaces().Where(i =>
                            i.GetGenericTypeDefinition() == typeof(IElasticEventHandler<>))
                            .ToList()
                            .ForEach(serviceType => builder.AddSingleton(serviceType, assignedType));
                    });

                    var types = Assembly.GetAssembly(typeof(OrganisationRegistryElasticSearchProjectionsAssemblyTokenClass))
                        .GetTypes()
                        .Where(item => item.GetInterfaces()
                            .Where(i => i.IsGenericType)
                            .Any(i => i.GetGenericTypeDefinition() == typeof(IEventHandler<>)))
                        .ToList();

                    types.ForEach(assignedType =>
                    {
                        builder.AddSingleton(assignedType);

                        assignedType.GetInterfaces().Where(i =>
                                i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                            .ToList()
                            .ForEach(serviceType => builder.AddSingleton(serviceType, assignedType));
                    });

                    builder.Configure<SqlServerConfiguration>(hostContext.Configuration.GetSection(SqlServerConfiguration.Section));
                    builder.Configure<ElasticSearchConfiguration>(hostContext.Configuration.GetSection(ElasticSearchConfiguration.Section));
                    builder.Configure<InfrastructureConfiguration>(hostContext.Configuration.GetSection(InfrastructureConfiguration.Section));

                    builder
                        .AddTransient<ISecurityService, NotImplementedSecurityService>()
                        .AddSingleton(provider =>
                        {
                            var inProcessBus = new InProcessBus(provider.GetRequiredService<ILogger<InProcessBus>>(),
                                provider.GetRequiredService<ISecurityService>());
                            return new CacheRunner(
                                provider.GetRequiredService<ILogger<CacheRunner>>(),
                                provider.GetRequiredService<IEventStore>(),
                                provider.GetRequiredService<IContextFactory>(),
                                provider.GetRequiredService<IProjectionStates>(),
                                inProcessBus,
                                new BusRegistrar(provider.GetRequiredService<ILogger<BusRegistrar>>(),
                                    inProcessBus,
                                    provider.GetRequiredService<Func<IServiceProvider>>())
                            );
                        })
                        .AddSingleton(provider =>
                        {
                            var inProcessBus = new InProcessBus(provider.GetRequiredService<ILogger<InProcessBus>>(),
                                provider.GetRequiredService<ISecurityService>());
                            return new IndividualRebuildRunner(
                                provider.GetRequiredService<ILogger<IndividualRebuildRunner>>(),
                                provider.GetRequiredService<IEventStore>(),
                                provider.GetRequiredService<IContextFactory>(),
                                provider.GetRequiredService<IProjectionStates>(),
                                inProcessBus,
                                new BusRegistrar(provider.GetRequiredService<ILogger<BusRegistrar>>(),
                                    inProcessBus,
                                    provider.GetRequiredService<Func<IServiceProvider>>())
                            );
                        })
                        .AddSingleton(provider =>
                        {
                            var bus = new ElasticBus(provider.GetRequiredService<ILogger<ElasticBus>>());
                            return new PeopleRunner(
                                provider.GetRequiredService<ILogger<PeopleRunner>>(),
                                provider.GetRequiredService<IOptions<ElasticSearchConfiguration>>(),
                                provider.GetRequiredService<IEventStore>(),
                                provider.GetRequiredService<IProjectionStates>(),
                                provider.GetRequiredService<Elastic>(),
                                bus,
                                provider.GetRequiredService<IMetricsRoot>(),
                                new ElasticBusRegistrar(provider.GetRequiredService<ILogger<ElasticBusRegistrar>>(),
                                    bus,
                                    provider.GetRequiredService<Func<IServiceProvider>>())
                            );
                        })
                        .AddSingleton(provider =>
                        {
                            var bus = new ElasticBus(provider.GetRequiredService<ILogger<ElasticBus>>());
                            return new OrganisationsRunner(
                                provider.GetRequiredService<ILogger<OrganisationsRunner>>(),
                                provider.GetRequiredService<IOptions<ElasticSearchConfiguration>>(),
                                provider.GetRequiredService<IEventStore>(),
                                provider.GetRequiredService<IProjectionStates>(),
                                provider.GetRequiredService<Elastic>(),
                                bus,
                                new ElasticBusRegistrar(provider.GetRequiredService<ILogger<ElasticBusRegistrar>>(),
                                    bus,
                                    provider.GetRequiredService<Func<IServiceProvider>>())
                            );
                        })
                        .AddSingleton(provider =>
                        {
                            var bus = new ElasticBus(provider.GetRequiredService<ILogger<ElasticBus>>());
                            return new BodyRunner(
                                provider.GetRequiredService<ILogger<BodyRunner>>(),
                                provider.GetRequiredService<IOptions<ElasticSearchConfiguration>>(),
                                provider.GetRequiredService<IEventStore>(),
                                provider.GetRequiredService<IProjectionStates>(),
                                provider.GetRequiredService<Elastic>(),
                                bus,
                                new ElasticBusRegistrar(provider.GetRequiredService<ILogger<ElasticBusRegistrar>>(),
                                    bus,
                                    provider.GetRequiredService<Func<IServiceProvider>>())
                            );
                        })
                        .AddSingleton<ElasticBus>()
                        .AddSingleton<IMetricsRoot>(new MetricsBuilder()
                            .Report.ToConsole().Build())

                        .AddSingleton<IDateTimeProvider, DateTimeProvider>()

                        .AddSingleton<Elastic>()

                        .AddSingleton<IProjectionStates, ProjectionStates>()
                        .AddSingleton<IContextFactory, ContextFactory>()

                        .AddSingleton<IExternalIpFetcher, ExternalIpFetcher>()
                        .AddSingleton<IRepository, Repository>()
                        .AddSingleton<IEventStore, SqlServerEventStore>()
                        .AddSingleton<IEventPublisher, NullPublisher>()
                        .AddTransient<ElasticBusRegistrar>()
                        .AddSingleton<Func<IServiceProvider>>(provider => () => provider)

                        .AddScoped<DbConnection>(s =>
                        {
                            var hostConfiguration = hostContext.Configuration;
                            return new TraceDbConnection<OrganisationRegistryContext>(
                                new SqlConnection(hostConfiguration.GetSection(SqlServerConfiguration.Section)
                                    .Get<SqlServerConfiguration>().ConnectionString),
                                hostConfiguration["DataDog:ServiceName"]);
                        })
                        .AddSingleton<Func<Owned<OrganisationRegistryContext>>>(
                            () =>
                            {
                                return new Owned<OrganisationRegistryContext>(
                                    new OrganisationRegistryContext(
                                        new DbContextOptionsBuilder<OrganisationRegistryContext>()
                                            .UseSqlServer(
                                                new TraceDbConnection<OrganisationRegistryContext>(
                                                    new SqlConnection(hostContext.Configuration.GetSection(SqlServerConfiguration.Section)
                                                        .Get<SqlServerConfiguration>().ConnectionString),
                                                    hostContext.Configuration["DataDog:ServiceName"]),
                                                options => options.EnableRetryOnFailure()
                                            ).Options), new Disposable());
                            })
                        .AddDbContext<OrganisationRegistryContext>((provider, options) => options
                            .UseLoggerFactory(provider.GetService<ILoggerFactory>())
                            .UseSqlServer(provider.GetRequiredService<TraceDbConnection<OrganisationRegistryContext>>(), sqlServerOptions =>
                            {
                                sqlServerOptions
                                    .EnableRetryOnFailure()
                                    .MigrationsAssembly("OrganisationRegistry.SqlServer")
                                    .MigrationsHistoryTable(MigrationTables.Default, Schema.Default);
                            }));
                        ;
                })
                .Build();

            var configuration = host.Services.GetRequiredService<IConfiguration>();
            var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            try
            {
                await DistributedLock<Program>.RunAsync(async () =>
                    {
                        await host.RunAsync().ConfigureAwait(false);
                    },
                    DistributedLockOptions.LoadFromConfiguration(configuration),
                    logger)
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                logger.LogCritical(e, "Encountered a fatal exception, exiting program.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }

    internal class Program2
    {
        public static async Task Main2(string[] args)
        {
            Console.WriteLine("Starting ElasticSearch Projections Runner");

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
                    y => y.MigrationsHistoryTable("__EFMigrationsHistory", "OrganisationRegistry")))
                .Build();

            // await RunCacheRunner(configuration);
            // await RunIndividualRunner(configuration);
            // await RunProgram<PeopleRunner>(configuration);
            // await RunProgram<OrganisationsRunner>(configuration);
            await RunBodyRunner(configuration);
        }

        private static async Task RunProgram<T, U>(IConfiguration configuration)
            where T : BaseRunner<U>
            where U: class, IDocument, new()
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
                    TerminateApplicationOnFailedRenew = true,
                    Enabled = elasticSearchOptions.LockEnabled,
                }, logger);

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
                    // UseOrganisationRegistryEventSourcing(app, runner);

                    await runner.Run();
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

        private static async Task RunCacheRunner(IConfiguration configuration)
        {
            var runnerName = nameof(CacheRunner);
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

            var distributedLock = new DistributedLock<CacheRunner>(
                new DistributedLockOptions
                {
                    Region = RegionEndpoint.GetBySystemName(elasticSearchOptions.LockRegionEndPoint),
                    AwsAccessKeyId = elasticSearchOptions.LockAccessKeyId,
                    AwsSecretAccessKey = elasticSearchOptions.LockAccessKeySecret,
                    TableName = elasticSearchOptions.LockTableName,
                    LeasePeriod = TimeSpan.FromMinutes(elasticSearchOptions.LockLeasePeriodInMinutes),
                    ThrowOnFailedRenew = true,
                    TerminateApplicationOnFailedRenew = true,
                    Enabled = elasticSearchOptions.LockEnabled,
                }, logger);

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
                    var runner = app.GetService<CacheRunner>();
                    UseOrganisationRegistryEventSourcing(app, runner);

                    await runner.Run();
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

        private static async Task RunBodyRunner(IConfiguration configuration)
        {
            var runnerName = nameof(BodyRunner);
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

            var distributedLock = new DistributedLock<CacheRunner>(
                new DistributedLockOptions
                {
                    Region = RegionEndpoint.GetBySystemName(elasticSearchOptions.LockRegionEndPoint),
                    AwsAccessKeyId = elasticSearchOptions.LockAccessKeyId,
                    AwsSecretAccessKey = elasticSearchOptions.LockAccessKeySecret,
                    TableName = elasticSearchOptions.LockTableName,
                    LeasePeriod = TimeSpan.FromMinutes(elasticSearchOptions.LockLeasePeriodInMinutes),
                    ThrowOnFailedRenew = true,
                    TerminateApplicationOnFailedRenew = true,
                    Enabled = elasticSearchOptions.LockEnabled,
                }, logger);

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
                    var runner = app.GetService<BodyRunner>();
                    UseOrganisationRegistryEventSourcing(app, runner);

                    await runner.Run();
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


        private static async Task RunIndividualRunner(IConfiguration configuration)
        {
            var runnerName = nameof(IndividualRebuildRunner);
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

            var distributedLock = new DistributedLock<IndividualRebuildRunner>(
                new DistributedLockOptions
                {
                    Region = RegionEndpoint.GetBySystemName(elasticSearchOptions.LockRegionEndPoint),
                    AwsAccessKeyId = elasticSearchOptions.LockAccessKeyId,
                    AwsSecretAccessKey = elasticSearchOptions.LockAccessKeySecret,
                    TableName = elasticSearchOptions.LockTableName,
                    LeasePeriod = TimeSpan.FromMinutes(elasticSearchOptions.LockLeasePeriodInMinutes),
                    ThrowOnFailedRenew = true,
                    TerminateApplicationOnFailedRenew = true,
                    Enabled = elasticSearchOptions.LockEnabled,
                }, logger);

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
                    var runner = app.GetService<IndividualRebuildRunner>();
                    UseOrganisationRegistryEventSourcing(app, runner);

                    await runner.Run();
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

            services.AddHostedService<EventProcessor>();

            var serviceProvider = services.BuildServiceProvider();

            var builder = new ContainerBuilder();
            builder.RegisterModule(new ElasticSearchProjectionsModule(configuration, services, serviceProvider.GetRequiredService<ILoggerFactory>()));
            return new AutofacServiceProvider(builder.Build());
        }

        // private static void UseOrganisationRegistryEventSourcing(IServiceProvider app, BaseRunner runner)
        // {
        //     var registrar = app.GetRequiredService<BusRegistrar>();
        //
        //     registrar.RegisterEventHandlers(runner.EventHandlers);
        // }

        private static void UseOrganisationRegistryEventSourcing(IServiceProvider app, IndividualRebuildRunner _)
        {
            var registrar = app.GetRequiredService<BusRegistrar>();

            registrar.RegisterEventHandlers(OrganisationsRunner.EventHandlers);
        }

        private static void UseOrganisationRegistryEventSourcing(IServiceProvider app, CacheRunner _)
        {
            var registrar = app.GetRequiredService<BusRegistrar>();

            registrar.RegisterEventHandlers(CacheRunner.EventHandlers);
        }

        private static void UseOrganisationRegistryEventSourcing(IServiceProvider app, BodyRunner _)
        {
            var registrar = app.GetRequiredService<ElasticBusRegistrar>();

            registrar.RegisterEventHandlers(BodyRunner.EventHandlers);
        }
    }
}
