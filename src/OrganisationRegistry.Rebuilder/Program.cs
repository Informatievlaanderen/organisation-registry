namespace OrganisationRegistry.Rebuilder
{
    using System;
    using System.Data.Common;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using App.Metrics;
    using Autofac.Features.OwnedInstances;
    using Autofac.Util;
    using Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing.Sql.EntityFrameworkCore;
    using Configuration.Database;
    using Infrastructure;
    using Infrastructure.AppSpecific;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using OrganisationRegistry.Configuration.Database.Configuration;
    using Infrastructure.Authorization;
    using Infrastructure.Bus;
    using Infrastructure.Config;
    using OrganisationRegistry.Infrastructure.Configuration;
    using Infrastructure.Domain;
    using Infrastructure.Events;
    using Infrastructure.EventStore;
    using NodaTime;
    using Serilog;
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
                Log.Debug(eventArgs.Exception, "FirstChanceException event raised in {AppDomain}.", AppDomain.CurrentDomain.FriendlyName);

            AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
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
                        .Configure<InfrastructureConfiguration>(hostContext.Configuration.GetSection(InfrastructureConfiguration.Section))
                        .Configure<TogglesConfiguration>(hostContext.Configuration.GetSection(TogglesConfiguration.Section))
                        .Configure<OpenIdConnectConfiguration>(hostContext.Configuration.GetSection(OpenIdConnectConfiguration.Section));

                    builder.AddSingleton<IClock>(SystemClock.Instance);
                    builder.AddSingleton<MemoryCaches, MemoryCaches>();
                    builder.AddSingleton<IMemoryCaches, MemoryCaches>(provider => provider.GetRequiredService<MemoryCaches>());
                    builder.AddSingleton<IContextFactory, ContextFactory>();
                    builder.AddSingleton<IMemoryCachesMaintainer, MemoryCachesMaintainer>();

                    builder.AddSingleton<RebuildProcessor>();

                    var handlers = Assembly.GetAssembly(typeof(OrganisationRegistrySqlServerAssemblyTokenClass))
                        .GetTypes()
                        .Where(item => !item.IsInterface &&
                                       !item.IsAbstract &&
                                       item.GetInterfaces()
                                           .Where(i => i.IsGenericType)
                                           .Any(i => i.GetGenericTypeDefinition() == typeof(IEventHandler<>)))
                        .ToList();

                    handlers.ForEach(assignedType =>
                    {
                        builder.AddSingleton(assignedType);

                        assignedType.GetInterfaces().Where(i =>
                                i.IsGenericType &&
                                i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                            .ToList()
                            .ForEach(serviceType => builder.AddSingleton(serviceType, assignedType));
                    });

                    var types = Assembly.GetAssembly(typeof(OrganisationRegistrySqlServerAssemblyTokenClass))
                        .GetTypes()
                        .Where(item => !item.IsInterface &&
                                       !item.IsAbstract &&
                                       item.GetInterfaces()
                                           .Where(i => i.IsGenericType)
                                           .Any(i => i.GetGenericTypeDefinition() == typeof(IEventHandler<>)))
                        .ToList();

                    types.ForEach(assignedType =>
                    {
                        builder.AddSingleton(assignedType);

                        assignedType.GetInterfaces().Where(i =>
                                i.IsGenericType &&
                                i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                            .ToList()
                            .ForEach(serviceType => builder.AddSingleton(serviceType, assignedType));
                    });

                    builder.AddSingleton<InProcessBus, InProcessBus>();
                    builder.AddSingleton<IEventPublisher, InProcessBus>(provider => provider.GetRequiredService<InProcessBus>());
                    builder.AddSingleton<IHandlerRegistrar, InProcessBus>(provider => provider.GetRequiredService<InProcessBus>());
                    builder.AddSingleton<BusRegistrar>();

                    builder.Configure<SqlServerConfiguration>(hostContext.Configuration.GetSection(SqlServerConfiguration.Section));
                    builder.Configure<InfrastructureConfiguration>(hostContext.Configuration.GetSection(InfrastructureConfiguration.Section));

                    builder
                        .AddTransient<ISecurityService, NotImplementedSecurityService>()
                        .AddSingleton<IMetricsRoot>(new MetricsBuilder()
                            .Report.ToConsole().Build())

                        .AddSingleton<IDateTimeProvider, DateTimeProvider>()


                        .AddSingleton<IProjectionStates, ProjectionStates>()
                        .AddSingleton<IContextFactory, ContextFactory>()

                        .AddSingleton<IExternalIpFetcher, ExternalIpFetcher>()
                        .AddSingleton<IRepository, Repository>()
                        .AddSingleton<IEventStore, SqlServerEventStore>()
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
                                    .MigrationsHistoryTable(MigrationTables.Default, WellknownSchemas.OrganisationRegistrySchema);
                            }));
                        ;
                })
                .Build();

            var configuration = host.Services.GetRequiredService<IConfiguration>();
            var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            var registrar = host.Services.GetRequiredService<BusRegistrar>();

            var rebuildProcessor = host.Services.GetRequiredService<RebuildProcessor>();

            registrar.RegisterEventHandlers(typeof(MemoryCachesMaintainer));
            registrar.RegisterEventHandlersFromAssembly(typeof(OrganisationRegistrySqlServerAssemblyTokenClass));

            try
            {
                await DistributedLock<Program>.RunAsync(async () =>
                    {
                        await rebuildProcessor.Run().ConfigureAwait(false);
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
}
