namespace OrganisationRegistry.SqlServer.Configuration
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure;

    public static class Migrations
    {
        public static void Run(SqlServerConfiguration sqlServerConfiguration, ILoggerFactory loggerFactory = null)
        {
            var migratorOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
                .UseSqlServer(
                    sqlServerConfiguration.MigrationsConnectionString,
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", "OrganisationRegistry"));

            if (loggerFactory != null)
                migratorOptions = migratorOptions.UseLoggerFactory(loggerFactory);

            using (var migrator = new OrganisationRegistryContext(migratorOptions.Options))
                migrator.Database.Migrate();
        }
    }
}
