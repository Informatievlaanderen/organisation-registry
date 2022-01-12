namespace OrganisationRegistry.VlaanderenBeNotifier.Schema
{
    using System.Data.Common;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;

    public sealed class VlaanderenBeNotifierTransactionalContext : VlaanderenBeNotifierContext
    {
        public VlaanderenBeNotifierTransactionalContext(DbConnection dbConnection, DbTransaction dbTransaction)
            : base(
                  new DbContextOptionsBuilder<VlaanderenBeNotifierContext>()
                    .UseSqlServer(
                        dbConnection,
                        x => x.MigrationsHistoryTable("__EFMigrationsHistory", WellknownSchemas.BackofficeSchema)).Options)
        {
            Database.UseTransaction(dbTransaction);
        }
    }
}
