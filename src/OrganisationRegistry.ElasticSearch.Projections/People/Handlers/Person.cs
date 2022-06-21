namespace OrganisationRegistry.ElasticSearch.Projections.People.Handlers;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Cache;
using Client;
using Configuration;
using ElasticSearch.People;
using Infrastructure;
using Infrastructure.Change;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Person.Events;
using Osc;
using SqlServer;

public class Person : BaseProjection<Person>,
    IElasticEventHandler<InitialiseProjection>,
    IElasticEventHandler<PersonCreated>,
    IElasticEventHandler<PersonUpdated>
{
    private readonly IPersonHandlerCache _cache;
    private readonly IContextFactory _contextFactory;
    private readonly Elastic _elastic;
    private readonly ElasticSearchConfiguration _elasticSearchOptions;

    public Person(
        ILogger<Person> logger,
        Elastic elastic,
        IContextFactory contextFactory,
        IOptions<ElasticSearchConfiguration> elasticSearchOptions,
        IPersonHandlerCache cache) : base(logger)
    {
        _elastic = elastic;
        _contextFactory = contextFactory;
        _cache = cache;
        _elasticSearchOptions = elasticSearchOptions.Value;
    }

    public async Task<IElasticChange> Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<InitialiseProjection> message)
    {
        if (message.Body.ProjectionName != typeof(Person).FullName)
            return new ElasticNoChange();

        Logger.LogInformation("Rebuilding index for {ProjectionName}", message.Body.ProjectionName);
        await PrepareIndex(_elastic.WriteClient, true);

        await using var context = _contextFactory.Create();
        await _cache.ClearCache(context);

        return new ElasticNoChange();
    }

    public async Task<IElasticChange> Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<PersonCreated> message)
        => await new ElasticDocumentCreation<PersonDocument>(
            message.Body.PersonId,
            () => new PersonDocument
            {
                ChangeId = message.Number,
                ChangeTime = message.Timestamp,
                Id = message.Body.PersonId,
                FirstName = message.Body.FirstName,
                Name = message.Body.Name,
            }).ToAsyncResult();


    public async Task<IElasticChange> Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<PersonUpdated> message)
        => await new ElasticPerDocumentChange<PersonDocument>(
            message.Body.PersonId,
            document =>
            {
                document.ChangeId = message.Number;
                document.ChangeTime = message.Timestamp;

                document.FirstName = message.Body.FirstName;
                document.Name = message.Body.Name;
            }
        ).ToAsyncResult();

    private async Task PrepareIndex(IOpenSearchClient client, bool deleteIndex)
    {
        var indexName = _elasticSearchOptions.PeopleWriteIndex;

        if (deleteIndex && await client.DoesIndexExist(indexName))
        {
            var deleteResult = await client.Indices.DeleteAsync(
                new DeleteIndexRequest(Indices.Index(new List<IndexName> { indexName })));

            if (!deleteResult.IsValid)
                throw new Exception($"Could not delete people index '{indexName}'.");
        }

        if (!await client.DoesIndexExist(indexName))
        {
            var indexResult = await client.Indices.CreateAsync(
                indexName,
                index => index
                    .Map<PersonDocument>(PersonDocument.Mapping)
                    .Settings(
                        descriptor => descriptor
                            .NumberOfShards(_elasticSearchOptions.NumberOfShards)
                            .NumberOfReplicas(_elasticSearchOptions.NumberOfReplicas)));

            if (!indexResult.IsValid)
                throw new Exception($"Could not create people index '{indexName}'.");
        }
    }
}
