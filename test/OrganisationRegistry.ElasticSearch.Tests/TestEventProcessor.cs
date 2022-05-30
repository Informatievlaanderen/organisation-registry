namespace OrganisationRegistry.ElasticSearch.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client;
using Infrastructure.Events;
using Osc;
using Projections.Infrastructure;
using Projections.Infrastructure.Change;

public class TestEventProcessor
{
    private readonly ElasticBus _bus;
    private readonly ElasticSearchFixture _elasticSearchFixture;

    public TestEventProcessor(ElasticBus bus, ElasticSearchFixture elasticSearchFixture)
    {
        _bus = bus;
        _elasticSearchFixture = elasticSearchFixture;
    }

    public async Task Handle<T>(List<IEnvelope> envelopes)
        where T : class, IDocument, new()
    {
        var allChanges = new List<ElasticChanges>();
        foreach (var envelope in envelopes)
        {
            var changes = await ProcessEnvelope(envelope);
            allChanges.Add(changes);
        }

        var documentCache = new Dictionary<Guid, T>();

        foreach (var changeSet in allChanges)
        {
            foreach (var changeSetChange in changeSet.Changes)
            {
                await ProcessChange(_elasticSearchFixture.Elastic, changeSetChange, documentCache);
            }
        }

        await FlushDocuments(documentCache, _elasticSearchFixture.Elastic);
    }

    private async Task<ElasticChanges> ProcessEnvelope(IEnvelope envelope)
    {
        var changes = await _bus.Publish(null, null, (dynamic)envelope);
        return new ElasticChanges(envelope.Number, changes);
    }

    private static async Task ProcessChange<T>(Elastic elastic, dynamic change, Dictionary<Guid, T> documentCache) where T : class, IDocument, new()
    {
        switch (change)
        {
            case ElasticDocumentCreation<T> elasticDocumentCreation:
            {
                foreach (var documentChange in elasticDocumentCreation.Changes)
                {
                    var document = documentChange.Value();
                    documentCache.Add(documentChange.Key, document);
                }

                break;
            }
            case ElasticPerDocumentChange<T> perDocumentChange:
            {
                foreach (var documentChange in perDocumentChange.Changes)
                {
                    T? document;

                    if (!documentCache.ContainsKey(documentChange.Key))
                    {
                        document = (await elastic.TryGetAsync(
                                async () =>
                                    (await elastic.WriteClient.GetAsync<T>(documentChange.Key))
                                    .ThrowOnFailure()))
                            .Source;

                        documentCache.Add(documentChange.Key, document);
                    }
                    else
                    {
                        document = documentCache[documentChange.Key];
                    }

                    await documentChange.Value(document);
                }

                break;
            }
            case ElasticMassChange massChange:
            {
                await FlushDocuments(documentCache, elastic);
                await massChange.Change(elastic);
                await elastic.TryGetAsync(
                    async () =>
                        (await elastic.WriteClient.Indices.RefreshAsync(Indices.Index<T>())).ThrowOnFailure());
                break;
            }
        }
    }

    private static async Task FlushDocuments<T>(Dictionary<Guid, T> documentCache, Elastic elastic)
        where T : class, IDocument, new()
    {
        if (documentCache.Any())
        {
            if (documentCache.Any(x => x.Key == Guid.Empty || string.IsNullOrEmpty(x.Value.Name)))
            {
                throw new Exception("Found document without key or name.");
            }

            await elastic.TryAsync(
                () =>
                {
                    elastic.WriteClient.BulkAll(
                        documentCache.Values,
                        b => b
                            .BackOffTime("30s")
                            .BackOffRetries(5)
                            .RefreshOnCompleted(false)
                            .MaxDegreeOfParallelism(Environment.ProcessorCount)
                            .Size(1000)
                    ).Wait(
                        TimeSpan.FromMinutes(15),
                        _ =>
                        {
                            //_logger.LogInformation("[{ProjectionName}] Flushed documents, page {PageNumber}", ProjectionName, next.Page);
                        });

                    return Task.CompletedTask;
                });
            documentCache.Clear();
        }
    }
}