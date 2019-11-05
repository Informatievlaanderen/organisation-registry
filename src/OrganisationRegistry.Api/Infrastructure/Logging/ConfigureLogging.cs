namespace OrganisationRegistry.Api.Infrastructure.Logging
{
    using System;
    using Destructurama;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Serilog;

    public static class ConfigureLoggingExtension
    {
        public static IApplicationBuilder ConfigureLogging(this IApplicationBuilder app)
        {
            var configuration = app.ApplicationServices.GetService<IConfiguration>();
            var loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();

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

            return app;
        }
    }
}
