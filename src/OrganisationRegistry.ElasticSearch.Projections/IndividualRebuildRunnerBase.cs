namespace OrganisationRegistry.ElasticSearch.Projections;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client;
using Infrastructure;
using Infrastructure.Change;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure.Events;
using Osc;
using SqlServer;
using SqlServer.Infrastructure;
using SqlServer.ProjectionState;

/// <summary>
/// Shared skeleton for the three IndividualRebuildRunner variants
/// (Organisations, Bodies, People). Subclasses only supply the type parameters
/// and a handful of small hooks (which DbSet, which event handlers, which
/// projection state key, how to obtain the aggregate id, how to remove the
/// pending-rebuild row).
/// </summary>
/// <typeparam name="TAggregate">Aggregate root type used to fetch envelopes from the event store.</typeparam>
/// <typeparam name="TDocument">Elastic document type produced by this projection.</typeparam>
/// <typeparam name="TToRebuild">EF entity type in the *ToRebuild table.</typeparam>
public abstract class IndividualRebuildRunnerBase<TAggregate, TDocument, TToRebuild>
    where TDocument : class, IDocument
    where TToRebuild : class
{
    public string ProjectionName => "IndividualRebuild";

    private readonly ILogger _logger;
    private readonly IEventStore _store;
    private readonly IContextFactory _contextFactory;
    private readonly IProjectionStates _projectionStates;
    private readonly ElasticBus _bus;
    private readonly Elastic _elastic;

    protected IndividualRebuildRunnerBase(
        ILogger logger,
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

        busRegistrar.RegisterEventHandlers(EventHandlers);
    }

    protected abstract string ProjectionStateKey { get; }
    protected abstract Type[] EventHandlers { get; }
    protected abstract DbSet<TToRebuild> DataToRebuildSet(OrganisationRegistryContext context);
    protected abstract Guid GetAggregateId(TToRebuild item);

    public async Task Run()
    {
        await using var context = _contextFactory.Create();

        var lastProcessedEventNumber =
            await _projectionStates.GetLastProcessedEventNumber(ProjectionStateKey);

        var toRebuildSet = DataToRebuildSet(context);
        var toRebuild = await toRebuildSet.ToListAsync();

        if (toRebuild.Count > 0)
            _logger.LogInformation(
                "[{ProjectionName}] Found {Count} aggregate(s) to rebuild",
                ProjectionName, toRebuild.Count);

        try
        {
            foreach (var item in toRebuild)
            {
                var aggregateId = GetAggregateId(item);

                var envelopes = _store
                    .GetEventEnvelopesUntil<TAggregate>(aggregateId, lastProcessedEventNumber)
                    .ToList();

                _logger.LogDebug(
                    "[{ProjectionName}] Found {NumberOfEnvelopes} envelopes (until #{MaxEventNumber}) to process for aggregate {AggregateId}",
                    ProjectionName, envelopes.Count, envelopes.Last().Number, aggregateId);

                var allChanges = new List<ElasticChanges>();
                foreach (var envelope in envelopes)
                {
                    var changes = await ProcessEnvelope(envelope);
                    allChanges.Add(changes);
                }

                var documentCache = new Dictionary<Guid, TDocument>();

                foreach (var changeSet in allChanges)
                {
                    foreach (var changeSetChange in changeSet.Changes)
                    {
                        await ProcessChange(changeSetChange, documentCache);
                    }
                }

                await FlushDocuments(documentCache);

                toRebuildSet.Remove(item);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(0, ex,
                "[{ProjectionName}] An exception occurred while handling envelopes", ProjectionName);
            throw;
        }
    }

    private async Task ProcessChange(IElasticChange? changeSetChange, Dictionary<Guid, TDocument> documentCache)
    {
        switch (changeSetChange)
        {
            case ElasticDocumentCreation<TDocument> elasticDocumentCreation:
            {
                foreach (var documentChange in elasticDocumentCreation.Changes)
                {
                    var document = documentChange.Value();
                    documentCache.Add(documentChange.Key, document);
                }
                break;
            }
            case ElasticPerDocumentChange<TDocument> perDocumentChange:
            {
                foreach (var documentChange in perDocumentChange.Changes)
                {
                    if (!documentCache.TryGetValue(documentChange.Key, out var document))
                    {
                        document = (await _elastic.TryGetAsync(async () =>
                                (await _elastic.WriteClient.GetAsync<TDocument>(documentChange.Key))
                                .ThrowOnFailure()))
                            .Source;

                        documentCache.Add(documentChange.Key, document);
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
                    (await _elastic.WriteClient.Indices.RefreshAsync(Indices.Index<TDocument>())).ThrowOnFailure());
                break;
            }
        }
    }

    private async Task FlushDocuments(Dictionary<Guid, TDocument> documentCache)
    {
        if (documentCache.Count == 0)
            return;

        if (documentCache.Any(x => x.Key == Guid.Empty || string.IsNullOrEmpty(x.Value.Name)))
            throw new Exception("Found document without key or name.");

        await _elastic.TryAsync(() =>
        {
            _elastic.WriteClient.BulkAll(documentCache.Values, b => b
                    .BackOffTime("30s")
                    .BackOffRetries(5)
                    .RefreshOnCompleted(false)
                    .MaxDegreeOfParallelism(Environment.ProcessorCount)
                    .Size(1000))
                .Wait(TimeSpan.FromMinutes(15),
                    next => _logger.LogInformation("Wrote page {PageNumber}", next.Page));

            return Task.CompletedTask;
        });

        documentCache.Clear();
    }

    private async Task<ElasticChanges> ProcessEnvelope(IEnvelope envelope)
    {
        var changes = await _bus.Publish(null, null, (dynamic)envelope);
        return new ElasticChanges(envelope.Number, changes);
    }
}
