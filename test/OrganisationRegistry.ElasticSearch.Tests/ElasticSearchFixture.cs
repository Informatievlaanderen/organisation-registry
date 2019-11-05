namespace OrganisationRegistry.ElasticSearch.Tests
{
    using System;
    using System.IO;
    using System.Reflection;
    using Autofac.Features.OwnedInstances;
    using Client;
    using Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using SqlServer.Infrastructure;

    // ReSharper disable once ClassNeverInstantiated.Global
    // Do not make abstract!
    public class ElasticSearchFixture : IDisposable
    {
        public ElasticSearchFixture()
        {
            LoggerFactory = new LoggerFactory();
            ContextFactory = () => new Owned<OrganisationRegistryContext>(null, null);
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
        public Func<Owned<OrganisationRegistryContext>> ContextFactory { get; set; }
        public Elastic Elastic { get; }
        public IOptions<ElasticSearchConfiguration> ElasticSearchOptions { get; set; }

        public void Dispose()
        {
        }
    }
}
