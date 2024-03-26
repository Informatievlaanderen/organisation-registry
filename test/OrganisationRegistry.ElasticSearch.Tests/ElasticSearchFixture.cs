namespace OrganisationRegistry.ElasticSearch.Tests;

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
using OpenSearch.Client;
using OrganisationRegistry.Tests.Shared;
using SqlServer;
using SqlServer.Infrastructure;

// ReSharper disable once ClassNeverInstantiated.Global
// Do not make abstract!
public class ElasticSearchFixture : IDisposable
{
    public ElasticSearchFixture()
    {
        ContextOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
            .UseInMemoryDatabase(
                $"org-es-test-{Guid.NewGuid()}",
                _ => { }).Options;

        LoggerFactory = new LoggerFactory();
        ContextFactory = new TestContextFactory(ContextOptions);
        var maybeConfigurationBasePath = Directory.GetParent(GetType().GetTypeInfo().Assembly.Location)?.FullName;
        if (maybeConfigurationBasePath is not { } configurationBasePath)
            throw new NullReferenceException("Configuration base path cannot be null");

        var builder = new ConfigurationBuilder()
            .SetBasePath(configurationBasePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true);

        Configuration = builder.Build();

        var elasticSearchConfiguration =
            builder.Build()
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
    public IConfiguration Configuration { get; }
    public DbContextOptions<OrganisationRegistryContext> ContextOptions { get; }

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
