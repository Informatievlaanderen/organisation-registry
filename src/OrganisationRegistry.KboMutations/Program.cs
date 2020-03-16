namespace OrganisationRegistry.KboMutations
{
    using System;
    using System.IO;
    using System.Threading;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Destructurama;
    using Infrastructure.Configuration;
    using Infrastructure.Infrastructure.Json;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using OrganisationRegistry.Configuration.Database;
    using OrganisationRegistry.Configuration.Database.Configuration;
    using Serilog;

    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting KboMutations");

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
                    y => y.MigrationsHistoryTable("__EFMigrationsHistory", "Wegwijs")))
                .Build();

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

            try
            {
                if (app.GetService<Runner>().Run())
                {
                    logger.LogInformation("Processing completed successfully, exiting program.");
                }
                else
                {
                    logger.LogInformation("KboMutations Toggle not enabled, exiting program.");
                }

                FlushLoggerAndTelemetry();
            }
            catch (Exception e)
            {
                logger.LogCritical(0, e, "Encountered a fatal exception, exiting program."); // dotnet core only supports global exceptionhandler starting from 1.2
                FlushLoggerAndTelemetry();
                throw;
            }
        }

        private static void FlushLoggerAndTelemetry()
        {
            Log.CloseAndFlush();

            // Allow some time for flushing before shutdown.
            Thread.Sleep(1000);
        }

        private static IServiceProvider ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();

            var serviceProvider = services.BuildServiceProvider();

            var builder = new ContainerBuilder();
            builder.RegisterModule(new KboMutationsRunnerModule(configuration, services, serviceProvider.GetService<ILoggerFactory>()));
            return new AutofacServiceProvider(builder.Build());
        }
    }
}
