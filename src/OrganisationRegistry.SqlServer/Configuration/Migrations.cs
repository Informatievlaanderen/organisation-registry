namespace OrganisationRegistry.SqlServer.Configuration
{
    using System;
    using Infrastructure;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure;

    public static class Migrations
    {
        public static void Run(SqlServerConfiguration sqlServerConfiguration, ILoggerFactory loggerFactory = null)
        {
            EnsureMigrationsInSchema(sqlServerConfiguration.MigrationsConnectionString, WellknownSchemas.BackofficeSchema);

            var migratorOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
                .UseSqlServer(
                    sqlServerConfiguration.MigrationsConnectionString,
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", WellknownSchemas.BackofficeSchema));


            if (loggerFactory != null)
                migratorOptions = migratorOptions.UseLoggerFactory(loggerFactory);

            using (var migrator = new OrganisationRegistryContext(migratorOptions.Options))
                migrator.Database.Migrate();
        }

        private static void EnsureMigrationsInSchema(string connectionString, string schema)
        {
            try
            {
                using var conn = new SqlConnection(connectionString);
                conn.Open();

                using var ensureSchema = new SqlCommand(
                    $"IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = '{schema}')\n" +
                    $"BEGIN EXEC('CREATE SCHEMA {schema}')\n" +
                    "END");
                ensureSchema.ExecuteNonQuery();

                using var moveSchema = new SqlCommand(
                    "IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'OrganisationRegistry'" +
                    "AND TABLE_NAME = '__EFMigrationsHistory'))\n" +
                    $"ALTER SCHEMA {schema} TRANSFER OrganisationRegistry.__EFMigrationsHistory",
                    conn);
                moveSchema.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not alter Migrations schema: {0}", ex.Message);
            }
        }
    }
}
