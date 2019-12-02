namespace OrganisationRegistry.SqlServer.IntegrationTests.OnEventStore
{
    using System;
    using System.IO;
    using System.Reflection;
    using Configuration;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class EventStoreSqlServerFixture
    {
        public IConfigurationRoot Config { get; }
        private DbContextOptions<OrganisationRegistryContext> ContextOptions { get; }
        private OrganisationRegistryContext Context { get; }

        public EventStoreSqlServerFixture()
        {
            Directory.SetCurrentDirectory(Directory.GetParent(typeof(EventStoreSqlServerFixture).GetTypeInfo().Assembly.Location).Parent.Parent.Parent.FullName);

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.eventstore.json", optional: false)
                .AddJsonFile($"appsettings.eventstore.{Environment.MachineName}.json", optional: true);

            Config = builder.Build();

            var migrationsConnectionString =
                Config.GetSection(SqlServerConfiguration.Section)
                    .Get<SqlServerConfiguration>()
                    .MigrationsConnectionString;

            ContextOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
                .UseSqlServer(
                    migrationsConnectionString,
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", "OrganisationRegistry"))
                .Options;

            Context = new OrganisationRegistryContext(ContextOptions);
            Context.Database.EnsureDeleted();

            using (var migrator = Context)
                migrator.Database.Migrate();
        }
    }
}
