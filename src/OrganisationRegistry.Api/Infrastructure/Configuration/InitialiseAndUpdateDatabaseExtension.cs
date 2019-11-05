namespace OrganisationRegistry.Api.Infrastructure.Configuration
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using SqlServer.Configuration;
    using OrganisationRegistry.Infrastructure.EventStore;

    public static class InitialiseAndUpdateDatabaseExtension
    {
        public static IApplicationBuilder InitialiseAndUpdateDatabase(this IApplicationBuilder app)
        {
            var loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();
            var sqlServerOptions = app.ApplicationServices.GetService<IOptions<SqlServerConfiguration>>();
            var sqlServerEventStore = app.ApplicationServices.GetService<SqlServerEventStore>();

            Migrations.Run(sqlServerOptions.Value, loggerFactory);

            sqlServerEventStore.InitaliseEventStore();

            return app;
        }
    }
}
