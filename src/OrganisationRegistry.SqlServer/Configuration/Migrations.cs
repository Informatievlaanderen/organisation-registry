namespace OrganisationRegistry.SqlServer.Configuration
{
    using Infrastructure;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure;

    public static class Migrations
    {
        public static void Run(SqlServerConfiguration sqlServerConfiguration, ILoggerFactory loggerFactory = null)
        {
            using var conn = new SqlConnection(sqlServerConfiguration.MigrationsConnectionString);
            using var cmd =
                new SqlCommand($"SELECT count(*) FROM sys.schemas WHERE name = '${WellknownSchemas.BackofficeSchema}'",
                    conn);
            conn.Open();

            var result = (int)cmd.ExecuteScalar();
            var schema = result == 0 ? WellknownSchemas.OrganisationRegistrySchema : WellknownSchemas.BackofficeSchema;
            var migratorOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
                .UseSqlServer(
                    sqlServerConfiguration.MigrationsConnectionString,
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", schema));

            if (loggerFactory != null)
                migratorOptions = migratorOptions.UseLoggerFactory(loggerFactory);

            using (var migrator = new OrganisationRegistryContext(migratorOptions.Options))
                migrator.Database.Migrate();
        }
    }
}
