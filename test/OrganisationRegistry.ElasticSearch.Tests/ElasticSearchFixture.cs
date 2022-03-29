namespace OrganisationRegistry.ElasticSearch.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Client;
    using Configuration;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Osc;
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
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true);

            var configurationRoot = builder.Build();

            var elasticSearchConfiguration =
                configurationRoot
                    .GetSection(ElasticSearchConfiguration.Section)
                    .Get<ElasticSearchConfiguration>();

            ElasticSearchOptions = new OptionsWrapper<ElasticSearchConfiguration>(elasticSearchConfiguration);
            var personIndex = Guid.NewGuid().ToString();
            ElasticSearchOptions.Value.PersonType = personIndex;
            ElasticSearchOptions.Value.PeopleReadIndex = personIndex;
            ElasticSearchOptions.Value.PeopleWriteIndex = personIndex;

            var organisationIndex = Guid.NewGuid().ToString();
            ElasticSearchOptions.Value.OrganisationType = organisationIndex;
            ElasticSearchOptions.Value.OrganisationsReadIndex = organisationIndex;
            ElasticSearchOptions.Value.OrganisationsWriteIndex = organisationIndex;

            var bodyIndex = Guid.NewGuid().ToString();
            ElasticSearchOptions.Value.BodyType = bodyIndex;
            ElasticSearchOptions.Value.BodyReadIndex = bodyIndex;
            ElasticSearchOptions.Value.BodyWriteIndex = bodyIndex;

            Elastic = new Elastic(LoggerFactory.CreateLogger<Elastic>(), ElasticSearchOptions);
        }

        public LoggerFactory LoggerFactory { get; set; }
        public IContextFactory ContextFactory { get; set; }
        public Elastic Elastic { get; }
        public IOptions<ElasticSearchConfiguration> ElasticSearchOptions { get; set; }

        public void Dispose()
        {
            var indices = new List<string>
            {
                ElasticSearchOptions.Value.PeopleWriteIndex,
                ElasticSearchOptions.Value.OrganisationsWriteIndex,
                ElasticSearchOptions.Value.BodyWriteIndex,
            };

            foreach (var index in indices)
            {
                if (Elastic.WriteClient.DoesIndexExist(index).GetAwaiter().GetResult())
                {
                    var deleteResult = Elastic.WriteClient.Indices.DeleteAsync(
                            new DeleteIndexRequest(Indices.Index(new List<IndexName> { index })))
                        .GetAwaiter().GetResult();

                    if (!deleteResult.IsValid)
                        throw new Exception($"Could not delete index '{index}'.");
                }
            }
        }
    }
}
