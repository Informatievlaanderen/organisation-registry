namespace OrganisationRegistry.ElasticSearch.Projections
{
    using System;
    using System.Data.Common;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Api.Configuration;
    using App.Metrics;
    using App.Metrics.Scheduling;
    using Autofac.Features.OwnedInstances;
    using Autofac.Util;
    using Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing.Sql.EntityFrameworkCore;
    using Body;
    using Cache;
    using Client;
    using Configuration;
    using Infrastructure;
    using Metrics;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using NodaTime;
    using OrganisationRegistry.Configuration;
    using Serilog;
    using OrganisationRegistry.Configuration.Database;
    using OrganisationRegistry.Configuration.Database.Configuration;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Bus;
    using OrganisationRegistry.Infrastructure.Config;
    using OrganisationRegistry.Infrastructure.Configuration;
    using OrganisationRegistry.Infrastructure.Domain;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Infrastructure.EventStore;
    using Organisations;
    using People;
    using Serilog.Events;
    using SqlServer;
    using SqlServer.Configuration;
    using SqlServer.Infrastructure;
    using SqlServer.ProjectionState;
    using IClock = NodaTime.IClock;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Starting OrganisationRegistry ElasticSearch Projections.");

            AppDomain.CurrentDomain.FirstChanceException += (_, eventArgs) =>
                Log.Debug(eventArgs.Exception, "FirstChanceException event raised in {AppDomain}",
                    AppDomain.CurrentDomain.FriendlyName);

            AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
                Log.Fatal((Exception)eventArgs.ExceptionObject, "Encountered a fatal exception, exiting program");

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

                    var sqlConfiguration = builder.Build().GetSection(ConfigurationDatabaseConfiguration.Section)
                        .Get<ConfigurationDatabaseConfiguration>();
                    builder.AddEntityFramework(x =>
                        x.UseSqlServer(
                            sqlConfiguration.ConnectionString,
                            y => y.MigrationsHistoryTable("__EFMigrationsHistory", WellknownSchemas.BackofficeSchema)));
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
                        .Configure<InfrastructureConfigurationSection>(
                            hostContext.Configuration.GetSection(InfrastructureConfigurationSection.Name))
                        .Configure<TogglesConfigurationSection>(
                            hostContext.Configuration.GetSection(TogglesConfigurationSection.Name))
                        .Configure<OpenIdConnectConfigurationSection>(
                            hostContext.Configuration.GetSection(OpenIdConnectConfigurationSection.Name));

                    builder
                        .AddSingleton<IOrganisationManagementConfiguration>(
                            _ =>
                                new OrganisationManagementConfiguration(
                                    hostContext.Configuration
                                        .GetSection(OrganisationManagementConfigurationSection.Name)
                                        .Get<OrganisationManagementConfigurationSection>()));

                    builder
                        .AddSingleton<IClock>(SystemClock.Instance)
                        .AddSingleton<Scheduler>()
                        .AddHostedService<EventProcessor>();

                    var elasticTypes =
                        Assembly.GetAssembly(typeof(OrganisationRegistryElasticSearchProjectionsAssemblyTokenClass))!
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

                    var types = Assembly.GetAssembly(
                            typeof(OrganisationRegistryElasticSearchProjectionsAssemblyTokenClass))!
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

                    builder.Configure<SqlServerConfiguration>(
                        hostContext.Configuration.GetSection(SqlServerConfiguration.Section));
                    builder.Configure<ElasticSearchConfiguration>(
                        hostContext.Configuration.GetSection(ElasticSearchConfiguration.Section));
                    builder.Configure<InfrastructureConfigurationSection>(
                        hostContext.Configuration.GetSection(InfrastructureConfigurationSection.Name));

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
                                    provider.GetRequiredService<Func<IServiceProvider>>()),
                                provider.GetRequiredService<IMetricsRoot>()
                            );
                        })
                        .AddSingleton(provider =>
                        {
                            var bus = new ElasticBus(provider.GetRequiredService<ILogger<ElasticBus>>());
                            return new IndividualRebuildRunner(
                                provider.GetRequiredService<ILogger<IndividualRebuildRunner>>(),
                                provider.GetRequiredService<IEventStore>(),
                                provider.GetRequiredService<IContextFactory>(),
                                provider.GetRequiredService<IProjectionStates>(),
                                bus,
                                provider.GetRequiredService<Elastic>(),
                                new ElasticBusRegistrar(provider.GetRequiredService<ILogger<ElasticBusRegistrar>>(),
                                    bus,
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
                                    provider.GetRequiredService<Func<IServiceProvider>>()),
                                provider.GetRequiredService<IMetricsRoot>()
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
                                    provider.GetRequiredService<Func<IServiceProvider>>()),
                                provider.GetRequiredService<IMetricsRoot>()
                            );
                        })
                        .AddSingleton<ElasticBus>()
                        .AddSingleton(new MetricsBuilder()
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
                        .AddScoped<DbConnection>(_ =>
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
                                                    new SqlConnection(hostContext.Configuration
                                                        .GetSection(SqlServerConfiguration.Section)
                                                        .Get<SqlServerConfiguration>().ConnectionString),
                                                    hostContext.Configuration["DataDog:ServiceName"]),
                                                options => options.EnableRetryOnFailure()
                                            ).Options), new Disposable());
                            })
                        .AddDbContext<OrganisationRegistryContext>((provider, options) => options
                            .UseLoggerFactory(provider.GetService<ILoggerFactory>())
                            .UseSqlServer(provider.GetRequiredService<TraceDbConnection<OrganisationRegistryContext>>(),
                                sqlServerOptions =>
                                {
                                    sqlServerOptions
                                        .EnableRetryOnFailure()
                                        .MigrationsAssembly("OrganisationRegistry.SqlServer")
                                        .MigrationsHistoryTable(MigrationTables.Default,
                                            WellknownSchemas.OrganisationRegistrySchema);
                                }));

                    builder.Configure<AppMetricsConfiguration>
                        (hostContext.Configuration.GetSection(AppMetricsConfiguration.Section));

                    builder.AddSingleton<IMetricsRoot>(_ => new MetricsBuilder()
                        .Report.ToSerilog(LogEventLevel.Information)
                        .Build());
                })
                .Build();

            var configuration = host.Services.GetRequiredService<IConfiguration>();

            ConfigureAppMetrics(host, configuration);

            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            try
            {
                await DistributedLock<Program>.RunAsync(async () => { await host.RunAsync().ConfigureAwait(false); },
                        DistributedLockOptions.LoadFromConfiguration(configuration),
                        logger)
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                logger.LogCritical(e, "Encountered a fatal exception, exiting program");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void ConfigureAppMetrics(IHost host, IConfiguration configuration)
        {
            var appMetricsConfiguration = configuration
                .GetSection("AppMetrics").Get<AppMetricsConfiguration>();

            var metrics = host.Services.GetRequiredService<IMetricsRoot>();

            var scheduler = new AppMetricsTaskScheduler(
                TimeSpan.FromSeconds(appMetricsConfiguration.FlushInterval),
                () => Task.WhenAll(metrics.ReportRunner.RunAllAsync()));
            scheduler.Start();
        }
    }
}
