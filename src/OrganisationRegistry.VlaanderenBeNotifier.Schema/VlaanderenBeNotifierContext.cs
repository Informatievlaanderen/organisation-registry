namespace OrganisationRegistry.VlaanderenBeNotifier.Schema
{
    using System.Reflection;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;

    public class VlaanderenBeNotifierContext : DbContext
    {

        //Kbo Sync Queue
        public DbSet<OrganisationCacheItem> OrganisationCache { get; set; }

        // This needs to be DbContextOptions<T> for Autofac!
        public VlaanderenBeNotifierContext(DbContextOptions<VlaanderenBeNotifierContext> options)
            : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.ConfigureWarnings(
            //    x => x.Throw(RelationalEventId.QueryClientEvaluationWarning));

            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(
                    @"Server=localhost,21433;Database=OrganisationRegistry;User ID=sa;Password=E@syP@ssw0rd;",
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", WellknownSchemas.VlaanderenBeNotifierSchema));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddEntityConfigurationsFromAssembly(GetType().GetTypeInfo().Assembly);
        }
    }

    public class TemporaryDbContextFactory : IDesignTimeDbContextFactory<VlaanderenBeNotifierContext>
    {
        public VlaanderenBeNotifierContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<VlaanderenBeNotifierContext>();

            builder.UseSqlServer(
                @"Server=localhost,21433;Database=OrganisationRegistry;User ID=sa;Password=E@syP@ssw0rd;",
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", WellknownSchemas.VlaanderenBeNotifierSchema));

            return new VlaanderenBeNotifierContext(builder.Options);
        }
    }
}
