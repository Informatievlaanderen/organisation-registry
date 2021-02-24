namespace OrganisationRegistry.ElasticSearch.Tests
{
    using System;
    using System.IO;
    using System.Reflection;
    using Autofac.Features.OwnedInstances;
    using Client;
    using Configuration;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using SqlServer;
    using SqlServer.Infrastructure;

    // ReSharper disable once ClassNeverInstantiated.Global
    // Do not make abstract!
    public class ElasticSearchFixture : IDisposable
    {
        public ElasticSearchFixture()
        {
            var dbContextOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
                .UseInMemoryDatabase(
                    $"org-es-test-{Guid.NewGuid()}",
                    builder => { }).Options;
            var context = new OrganisationRegistryContext(
                dbContextOptions);

            LoggerFactory = new LoggerFactory();
            ContextFactory = new TestContextFactory(dbContextOptions);
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(GetType().GetTypeInfo().Assembly.Location).FullName)
                .AddJsonFile("appsettings.json", optional: false);

            var configurationRoot = builder.Build();

            var elasticSearchConfiguration =
                configurationRoot
                    .GetSection(ElasticSearchConfiguration.Section)
                    .Get<ElasticSearchConfiguration>();

            ElasticSearchOptions = new OptionsWrapper<ElasticSearchConfiguration>(elasticSearchConfiguration);
            Elastic = new Elastic(LoggerFactory.CreateLogger<Elastic>(), ElasticSearchOptions);
        }

        public LoggerFactory LoggerFactory { get; set; }
        public IContextFactory ContextFactory { get; set; }
        public Elastic Elastic { get; }
        public IOptions<ElasticSearchConfiguration> ElasticSearchOptions { get; set; }

        public void Dispose()
        {
        }
    }
}
