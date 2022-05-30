namespace OrganisationRegistry.SqlServer.Infrastructure;

using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure;

public sealed class OrganisationRegistryTransactionalContext : OrganisationRegistryContext
{
    public OrganisationRegistryTransactionalContext(DbConnection dbConnection, DbTransaction dbTransaction)
        : base(
            new DbContextOptionsBuilder<OrganisationRegistryContext>()
                .UseSqlServer(
                    dbConnection,
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", WellknownSchemas.BackofficeSchema)).Options)
    {
        Database.UseTransaction(dbTransaction);
    }
}