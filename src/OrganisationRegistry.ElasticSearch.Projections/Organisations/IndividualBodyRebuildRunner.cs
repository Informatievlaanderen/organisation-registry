namespace OrganisationRegistry.ElasticSearch.Projections.Bodies;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Body;
using Client;
using ElasticSearch.Bodies;
using Infrastructure;
using Infrastructure.Change;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenSearch.Client;
using OrganisationRegistry.Infrastructure.Events;
using SqlServer;
using SqlServer.ProjectionState;

public class IndividualBodyRebuildRunner
{
    public string ProjectionName => "IndividualRebuild";

    private readonly ILogger<IndividualBodyRebuildRunner> _logger;
    private readonly IEventStore _store;
    private readonly IContextFactory _contextFactory;
    private readonly IProjectionStates _projectionStates;
    private readonly ElasticBus _bus;
    private readonly Elastic _elastic;

    public IndividualBodyRebuildRunner(
        ILogger<IndividualBodyRebuildRunner> logger,
        IEventStore store,
        IContextFactory contextFactory,
        IProjectionStates projectionStates,
        ElasticBus bus,
        Elastic elastic,
        ElasticBusRegistrar busRegistrar)
    {
        _logger = logger;
        _store = store;
        _contextFactory = contextFactory;
        _projectionStates = projectionStates;
        _bus = bus;
        _elastic = elastic;

        busRegistrar.RegisterEventHandlers(BodyRunner.EventHandlers);
    }

    public async Task Run()
    {
        await using var context = _contextFactory.Create();

        var lastProcessedEventNumber =
            await _projectionStates.GetLastProcessedEventNumber(BodyRunner.ElasticSearchProjectionsProjectionName);

        var bodyToRebuilds = await context.BodiesToRebuild.ToListAsync();

        if (bodyToRebuilds.Count > 0)
            _logger.LogInformation("[{ProjectionName}] Found {NumberOfBodies} bodies to rebuild", ProjectionName, bodyToRebuilds.Count);

        try
        {
            foreach (var body in bodyToRebuilds)
            {
                var envelopes = _store
                    .GetEventEnvelopesUntil<OrganisationRegistry.Body.Body>(
                        body.BodyId,
                        lastProcessedEventNumber)
                    .ToList();

                _logger.LogDebug("[{ProjectionName}] Found {NumberOfEnvelopes} envelopes (until #{MaxEventNumber}) to process for Body {BodyId}",
                    ProjectionName, envelopes.Count, envelopes.Last().Number, body.BodyId);

                var allChanges = new List<ElasticChanges>();
                foreach (var envelope in envelopes)
                {
                    var changes = await ProcessEnvelope(envelope);
                    allChanges.Add(changes);
                }

                var documentCache = new Dictionary<Guid, BodyDocument>();

                foreach (var changeSet in allChanges)
                {
                    foreach (var changeSetChange in changeSet.Changes)
                    {
                        await ProcessChange(changeSetChange, documentCache);
                    }
                }
                await FlushDocuments(documentCache);

                context.BodiesToRebuild.Remove(body);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(0, ex, "[{ProjectionName}] An exception occurred while handling envelopes", ProjectionName);
            throw;
        }
    }

    private async Task ProcessChange(IElasticChange? changeSetChange, Dictionary<Guid, BodyDocument> documentCache)
    {
        switch (changeSetChange)
        {
            case ElasticDocumentCreation<BodyDocument> elasticDocumentCreation:
            {
                foreach (var documentChange in elasticDocumentCreation.Changes)
                {
                    var document = documentChange.Value();
                    documentCache.Add(documentChange.Key, document);
                }
                break;
            }
            case ElasticPerDocumentChange<BodyDocument> perDocumentChange:
            {
                foreach (var documentChange in perDocumentChange.Changes)
                {
                    BodyDocument? document;

                    if (!documentCache.ContainsKey(documentChange.Key))
                    {
                        document = (await _elastic.TryGetAsync(async () =>
                                (await _elastic.WriteClient.GetAsync<BodyDocument>(documentChange.Key))
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
                await FlushDocuments(documentCache);
                await massChange.Change(_elastic);
                await _elastic.TryGetAsync(async () =>
                    (await _elastic.WriteClient.Indices.RefreshAsync(Indices.Index<BodyDocument>())).ThrowOnFailure());
                break;
            }
        }
    }


    private async Task FlushDocuments(Dictionary<Guid, BodyDocument> documentCache)
    {
        if (documentCache.Any())
        {
            if (documentCache.Any(x => x.Key == Guid.Empty || string.IsNullOrEmpty(x.Value.Name)))
            {
                throw new Exception("Found document without key or name.");
            }

            await _elastic.TryAsync(async () =>
            {
                _elastic.WriteClient.BulkAll(documentCache.Values, b => b
                        .BackOffTime("30s")
                        .BackOffRetries(5)
                        .RefreshOnCompleted(false)
                        .MaxDegreeOfParallelism(Environment.ProcessorCount)
                        .Size(1000)
                    )
                    .Wait(TimeSpan.FromMinutes(15), next =>
                    {
                        _logger.LogInformation("Wrote page {PageNumber}", next.Page);
                    });

                await Task.CompletedTask;
            });
            documentCache.Clear();
        }
    }


    private async Task<ElasticChanges> ProcessEnvelope(IEnvelope envelope)
    {
        var changes = await _bus.Publish(null, null, (dynamic) envelope);
        return new ElasticChanges(envelope.Number, changes);
    }
}
