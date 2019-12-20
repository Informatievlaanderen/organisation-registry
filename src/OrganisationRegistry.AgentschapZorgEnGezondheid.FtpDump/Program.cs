namespace OrganisationRegistry.AgentschapZorgEnGezondheid.FtpDump
{
    using System;
    using System.IO;
    using System.Threading;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Formatters.Json;
    using Destructurama;
    using Infrastructure.Configuration;
    using Infrastructure.Infrastructure.Json;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Serilog;
    using OrganisationRegistry.Configuration.Database;
    using OrganisationRegistry.Configuration.Database.Configuration;

    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting Agentschap Zorg en Gezondheid FTP Dump");

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

            var services = new ServiceCollection();
            var app = ConfigureServices(services, configuration);

            ConfigureLogging(app);

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
                    //telemetryClient.TrackEvent("AgentschapZorgEnGezondheidFtpDump::Ran");
                    logger.LogInformation("Processing completed successfully, exiting program.");
                }
                else
                {
                    logger.LogInformation("AgentschapZorgEnGezondheidFtpDump Toggle not enabled, exiting program.");
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

        private static IServiceProvider ConfigureServices(
            IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOptions();

            var builder = new ContainerBuilder();
            builder.RegisterModule(new AgentschapZorgEnGezondheidFtpDumpModule(configuration, services, null));
            return new AutofacServiceProvider(builder.Build());
        }

        private static void ConfigureLogging(IServiceProvider app)
        {
            var configuration = app.GetService<IConfiguration>();
            var loggerFactory = app.GetService<ILoggerFactory>();

            Serilog.Debugging.SelfLog.Enable(Console.WriteLine);

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.LiterateConsole()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithEnvironmentUserName()
                .Destructure.JsonNetTypes();

            Log.Logger = logger.CreateLogger();

            loggerFactory.AddSerilog();
        }
    }
}
