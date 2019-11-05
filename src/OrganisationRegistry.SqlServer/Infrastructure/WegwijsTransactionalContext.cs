namespace OrganisationRegistry.SqlServer.Infrastructure
{
    using System.Data.Common;
    using Microsoft.EntityFrameworkCore;

    public sealed class OrganisationRegistryTransactionalContext : OrganisationRegistryContext
    {
        public OrganisationRegistryTransactionalContext(DbConnection dbConnection, DbTransaction dbTransaction)
            : base(
                  new DbContextOptionsBuilder<OrganisationRegistryContext>()
                    .UseSqlServer(
                        dbConnection,
                        x => x.MigrationsHistoryTable("__EFMigrationsHistory", "OrganisationRegistry")).Options)
        {
            Database.UseTransaction(dbTransaction);
        }
    }
}
