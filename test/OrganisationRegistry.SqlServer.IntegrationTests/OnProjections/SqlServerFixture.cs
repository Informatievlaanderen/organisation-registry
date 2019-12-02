namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections
{
    using System;
    using System.IO;
    using System.Reflection;
    using Api.Infrastructure;
    using Configuration;
    using Infrastructure;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using OrganisationRegistry.Infrastructure.Events;

    public class SqlServerFixture
    {
        public IServiceProvider ServiceProvider;
        public DbContextOptions<OrganisationRegistryContext> ContextOptions { get; }
        public IEventPublisher Publisher { get; }
        public string ConnectionString { get; }

        public SqlServerFixture()
        {
            Directory.SetCurrentDirectory(Directory.GetParent(typeof(SqlServerFixture).GetTypeInfo().Assembly.Location).Parent.Parent.Parent.FullName);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true)
                .Build();

            var migrationsConnectionString =
                configuration.GetSection(SqlServerConfiguration.Section)
                    .Get<SqlServerConfiguration>()
                    .MigrationsConnectionString;

            ContextOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
                .UseSqlServer(
                    migrationsConnectionString,
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", "OrganisationRegistry"))
                .Options;

            var context = new OrganisationRegistryContext(ContextOptions);
            context.Database.EnsureDeleted();

            using (var migrator = context)
                migrator.Database.Migrate();

            IWebHostBuilder hostBuilder = new WebHostBuilder();
            hostBuilder = hostBuilder.UseKestrel(server => server.AddServerHeader = false);

            var webHost = new TestServer(hostBuilder
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(configuration)
                .UseStartup<Startup>()).Host;

            Publisher = (IEventPublisher)webHost.Services.GetService(typeof(IEventPublisher));
            ServiceProvider = webHost.Services;

            var sqlServerConfig = (IOptions<SqlServerConfiguration>)webHost.Services.GetService(typeof(IOptions<SqlServerConfiguration>));

            ConnectionString = sqlServerConfig.Value.MigrationsConnectionString;
            ContextOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
                .UseSqlServer(
                    ConnectionString,
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", "OrganisationRegistry"))
                .Options;
        }
    }
}
