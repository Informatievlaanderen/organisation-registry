namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections
{
    using System;
    using System.IO;
    using System.Reflection;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using OrganisationRegistry.Infrastructure.Events;

    public class SqlServerFixture
    {
        public IServiceProvider ServiceProvider;
        public IConfigurationRoot Configuration { get; }
        public DbContextOptions<OrganisationRegistryContext> ContextOptions { get; }
        public IEventPublisher Publisher { get; }
        public string ConnectionString { get; }

        public SqlServerFixture()
        {
            Directory.SetCurrentDirectory(Directory.GetParent(typeof(SqlServerFixture).GetTypeInfo().Assembly.Location).Parent.Parent.Parent.FullName);

            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true)
                .Build();
        }
    }
}
